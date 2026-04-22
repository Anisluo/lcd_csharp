using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace LCD_V2.Views
{
    /// <summary>
    /// Schematic 3D preview of a 5-axis gantry — base plate + X rail + Y carriage +
    /// Y rail + Z column + instrument head. The pose is static ("representative mid-
    /// stroke"); axis colours dim when the axis is disabled in the AxisConfig list.
    /// No real-motion animation yet — call SetAxes(...) to re-colour after edits.
    /// </summary>
    public partial class Motion3DView : UserControl
    {
        // ── camera orbit state ──
        private double _azimuthDeg   = 35.0;
        private double _elevationDeg = 22.0;
        private double _distance     = 58.0;
        private const double DistMin =  25.0;
        private const double DistMax = 140.0;
        private const double ElevMin = -82.0;
        private const double ElevMax =  82.0;

        private Point _lastDragPoint;
        private bool  _dragging;

        private readonly PerspectiveCamera _camera = new PerspectiveCamera
        {
            FieldOfView = 45,
            UpDirection = new Vector3D(0, 1, 0),
        };

        private List<AxisConfig> _axes = new List<AxisConfig>();

        // ── simulation state ──
        // Current gantry pose in world units. RebuildScene reads these; the sim tick
        // writes them. Start at the old hardcoded "representative mid-stroke" values
        // so the static view before simulation looks the same as before.
        private double _simX = 5.0;  // world-X position of the Y-carriage
        private double _simY = 3.0;  // world-Z position of the Z-carriage (our "Y axis")
        private double _simU = 0.0;  // U-axis turntable angle (degrees, 0..360)
        private double _simV = 0.0;  // V-axis tilt angle applied to both instrument and disc
        private double _simVPhase = 0.0;  // phase accumulator for V oscillation during sim
        private const double VTiltAmplitude = 30.0;   // degrees — ±amplitude of V swing
        private const double VStaticAngle   = 22.0;   // degrees — static tilt shown when V enabled + not simulating
        private readonly DispatcherTimer _simTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        private readonly List<Point> _simPoints = new List<Point>();   // (X_mm, Y_mm) demo grid
        private int _simIdx;
        private DateTime _lastTickUtc;

        // ── fixed geometry constants that simulation / scene build both reference ──
        // Disc / panel sits at world origin in X-Z so it centres both the X gantry rail
        // span (-18..+18) and the Y / bridge span (symmetric around 0 in dual-drive mode).
        private const double DiscCenterZ = 0.0;  // world-Z of the turntable centre
        private const double DiscTrackR  = 5.0;  // radius at which X/Y follow the rotating reference point

        // Instrument is mounted on the Z carriage via a bracket and offset this
        // much along the bracket direction (gantry-local -X) from the Z-carriage
        // centre. X/Y tracking adds this to _simX so the lens — not the carriage —
        // lands on the rotating red marker.
        private const double InstrumentLensOffset = 2.2;

        // ── palette ──
        private static readonly Brush BaseBrush       = new SolidColorBrush(Color.FromRgb(0xC8, 0xCE, 0xD6));
        private static readonly Brush PanelBrush      = new SolidColorBrush(Color.FromRgb(0xF3, 0xF5, 0xF8));
        private static readonly Brush XBrush          = new SolidColorBrush(Color.FromRgb(0x24, 0x7C, 0xE2));
        private static readonly Brush YBrush          = new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B));
        private static readonly Brush ZBrush          = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81));
        private static readonly Brush InstrumentBrush = new SolidColorBrush(Color.FromRgb(0x9F, 0x12, 0x39));
        private static readonly Brush LensBrush       = new SolidColorBrush(Color.FromRgb(0x37, 0x41, 0x51));
        private static readonly Brush DisabledBrush   = new SolidColorBrush(Color.FromRgb(0xDB, 0xDF, 0xE4));

        public Motion3DView()
        {
            InitializeComponent();

            Viewport.Camera = _camera;
            RebuildScene();
            UpdateCamera();

            DragSurface.MouseLeftButtonDown += DragSurface_MouseLeftButtonDown;
            DragSurface.MouseLeftButtonUp   += DragSurface_MouseLeftButtonUp;
            DragSurface.MouseMove           += DragSurface_MouseMove;
            DragSurface.MouseWheel          += DragSurface_MouseWheel;

            _simTimer.Tick += SimTick;
            Unloaded += (s, e) => StopSimulation(); // don't keep ticking after nav-away
        }

        /// <summary>Re-colour the scene based on which axes are enabled.</summary>
        public void SetAxes(IEnumerable<AxisConfig> axes)
        {
            _axes = axes?.ToList() ?? new List<AxisConfig>();
            RebuildScene();
            UpdateAxisStateLabel();
        }

        // ────────── camera ──────────

        private void UpdateCamera()
        {
            double az = _azimuthDeg   * Math.PI / 180.0;
            double el = _elevationDeg * Math.PI / 180.0;
            double cosEl = Math.Cos(el);
            // orbit around the centre-of-interest (roughly the Y carriage at y≈8)
            var look = new Point3D(0, 8, 0);
            var pos = new Point3D(
                look.X + _distance * cosEl * Math.Sin(az),
                look.Y + _distance * Math.Sin(el),
                look.Z + _distance * cosEl * Math.Cos(az));
            _camera.Position      = pos;
            _camera.LookDirection = new Vector3D(look.X - pos.X, look.Y - pos.Y, look.Z - pos.Z);
        }

        private void DragSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastDragPoint = e.GetPosition(DragSurface);
            _dragging = true;
            DragSurface.CaptureMouse();
            DragSurface.Cursor = Cursors.SizeAll;
        }

        private void DragSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragging = false;
            DragSurface.ReleaseMouseCapture();
            DragSurface.Cursor = Cursors.Arrow;
        }

        private void DragSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            var p = e.GetPosition(DragSurface);
            double dx = p.X - _lastDragPoint.X;
            double dy = p.Y - _lastDragPoint.Y;
            _lastDragPoint = p;

            _azimuthDeg   -= dx * 0.4;
            _elevationDeg += dy * 0.3;
            if (_elevationDeg < ElevMin) _elevationDeg = ElevMin;
            if (_elevationDeg > ElevMax) _elevationDeg = ElevMax;
            UpdateCamera();
        }

        private void DragSurface_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _distance *= e.Delta > 0 ? 0.90 : 1.10;
            if (_distance < DistMin) _distance = DistMin;
            if (_distance > DistMax) _distance = DistMax;
            UpdateCamera();
        }

        private void BtnResetView_Click(object sender, RoutedEventArgs e)
        {
            _azimuthDeg = 35.0; _elevationDeg = 22.0; _distance = 58.0;
            UpdateCamera();
        }

        // ────────── scene build ──────────

        private bool IsAxisEnabled(string axisName)
        {
            if (_axes == null || _axes.Count == 0) return true;  // no config yet → show in full colour
            var matches = _axes.Where(x =>
                string.Equals(x.AxisName, axisName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 0) return false;                // axis unassigned → dim it
            return matches.Any(m => m.Enabled);                  // enabled if any driving channel is enabled
        }

        private bool IsAxisInterpolated(string axisName)
        {
            if (_axes == null) return false;
            return _axes.Any(x =>
                string.Equals(x.AxisName, axisName, StringComparison.OrdinalIgnoreCase)
                && x.Interpolate);
        }

        private void RebuildScene()
        {
            Viewport.Children.Clear();

            var visual = new ModelVisual3D();
            var group = new Model3DGroup();

            // lighting — one key + one fill + ambient
            group.Children.Add(new AmbientLight(Color.FromRgb(0x55, 0x55, 0x58)));
            group.Children.Add(new DirectionalLight(Colors.White,        new Vector3D(-1.0, -2.0, -1.2)));
            group.Children.Add(new DirectionalLight(Color.FromRgb(0x70, 0x74, 0x80), new Vector3D(1.2, -0.5, 1.0)));

            bool xE = IsAxisEnabled("X");
            bool yE = IsAxisEnabled("Y");
            bool zE = IsAxisEnabled("Z");
            bool uE = IsAxisEnabled("U");
            bool vE = IsAxisEnabled("V");

            // ── static geometry ──
            // base plate (40 × 0.8 × 30, top at y≈0)
            group.Children.Add(Cuboid(new Point3D(0, -0.4, 0), 40, 0.8, 30, BaseBrush));

            // panel / product stage on top of the base.
            //   U disabled → plain rectangular jig (static)
            //   U enabled  → circular turntable + cross-spokes + target marker, grouped
            //                so a single RotateTransform3D around world-Y spins everything
            if (uE)
            {
                const double discR = 9.0;
                const double discH = 0.30;
                const double discY = 0.15;
                var discGroup = new Model3DGroup();
                discGroup.Children.Add(Cylinder(new Point3D(0, discY, DiscCenterZ), discR, discH, 56, PanelBrush));

                // rotation cue — two perpendicular radial lines sitting on the disc top
                Brush spoke = new SolidColorBrush(Color.FromRgb(0xB0, 0xB6, 0xC0));
                double spokeY = discY + discH / 2 + 0.03;
                discGroup.Children.Add(Cuboid(new Point3D(0, spokeY, DiscCenterZ), discR * 2, 0.04, 0.18, spoke));
                discGroup.Children.Add(Cuboid(new Point3D(0, spokeY, DiscCenterZ), 0.18, 0.04, discR * 2, spoke));

                // target reference point the instrument is locked onto during U rotation —
                // a small red square at disc-local (DiscTrackR, 0). Because it's part of
                // discGroup, it rotates with the disc and the X/Y carriage tracks its
                // world position each tick.
                discGroup.Children.Add(Cuboid(
                    new Point3D(DiscTrackR, spokeY + 0.03, DiscCenterZ), 0.7, 0.08, 0.7,
                    new SolidColorBrush(Color.FromRgb(0xE1, 0x1D, 0x48))));

                // Compose U spin (world-Y) and V tilt (world-X) through the disc centre.
                // Transform3DGroup applies children[0] first, so U is applied in the
                // untilted frame and V then tilts the whole spinning disc — physically
                // the V platform carries the U-spinning turntable.
                var discTr = new Transform3DGroup();
                discTr.Children.Add(new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(0, 1, 0), _simU),
                    new Point3D(0, 0, DiscCenterZ)));
                double vDiscAngle = vE ? (IsSimulating ? _simV : VStaticAngle) : 0.0;
                discTr.Children.Add(new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(1, 0, 0), vDiscAngle),
                    new Point3D(0, 0, DiscCenterZ)));
                discGroup.Transform = discTr;
                group.Children.Add(discGroup);
            }
            else
            {
                group.Children.Add(Cuboid(new Point3D(0, 0.06, DiscCenterZ), 22, 0.1, 13, PanelBrush));
            }

            // origin corner indicator — small cube at world origin for orientation sense
            group.Children.Add(Cuboid(new Point3D(-19, 0.25, 14), 0.4, 0.4, 0.4, InstrumentBrush));

            // ── Gantry subgroup ──
            // All gantry geometry (rails, carriages, column, bracket, instrument) is
            // authored in an un-rotated "local" frame that matches the original layout;
            // a single RotateTransform3D spins the whole assembly 90° around world-Y
            // so the X rail runs along world Z instead of world X. The disc / base /
            // lights stay in the parent group, unaffected.
            var gantry = new Model3DGroup();

            // ── X axis: single rail, or dual-drive gantry pair when Interpolate=true ──
            bool xDual = IsAxisInterpolated("X");
            double yCarX = _simX;        // gantry X-axis position (mutated by sim tick)
            Brush xBr = xE ? XBrush : DisabledBrush;
            Brush yBr = yE ? YBrush : DisabledBrush;

            gantry.Children.Add(Cuboid(new Point3D(0, 0.6, -12), 36, 1.2, 2.5, xBr));
            gantry.Children.Add(Cuboid(new Point3D(yCarX, 2.2, -12), 3.2, 2.6, 3.2, xBr));

            if (xDual)
            {
                gantry.Children.Add(Cuboid(new Point3D(0, 0.6, +12), 36, 1.2, 2.5, xBr));
                gantry.Children.Add(Cuboid(new Point3D(yCarX, 2.2, +12), 3.2, 2.6, 3.2, xBr));
                gantry.Children.Add(Cuboid(new Point3D(yCarX, 3.0, 0), 2.0, 1.4, 26, yBr));
            }
            else
            {
                gantry.Children.Add(Cuboid(new Point3D(yCarX, 3.0, -1), 2.0, 1.4, 22, yBr));
            }

            // Z carriage, Z column, sub-carriage (slides on column)
            double zCarZ = _simY;
            gantry.Children.Add(Cuboid(new Point3D(yCarX, 3.8, zCarZ), 3.2, 2.0, 3.2, yE ? YBrush : DisabledBrush));

            double zColH = 13.0;
            double zColBottomY = 4.8;
            double zColCenterY = zColBottomY + zColH / 2;
            gantry.Children.Add(Cuboid(new Point3D(yCarX, zColCenterY, zCarZ), 1.8, zColH, 1.8, zE ? ZBrush : DisabledBrush));

            double instY = zColBottomY + zColH * 0.60;
            gantry.Children.Add(Cuboid(new Point3D(yCarX, instY, zCarZ), 2.6, 1.6, 2.6, zE ? ZBrush : DisabledBrush));

            // ── Side-mounted instrument (V axis) ──
            // Bracket + instrument mount on the Z carriage's -X face (rotated 90° from
            // the original -Z face layout). This swings the lens out of the Y rail /
            // bridge's line of sight — previously the lens looked straight down through
            // the bridge centre at world Z=0 after the outer gantry rotation.
            // V pivot rod runs along gantry-local Z, so V rotation is around (0,0,1).
            double bracketX = yCarX - 1.5;
            gantry.Children.Add(Cuboid(
                new Point3D(bracketX, instY, zCarZ), 0.4, 1.1, 0.6, InstrumentBrush));

            Point3D vPivot = new Point3D(bracketX - 0.2, instY, zCarZ);

            var instAssembly = new Model3DGroup();
            Point3D bodyCenter = new Point3D(vPivot.X - 1.6, vPivot.Y - 1.0, vPivot.Z);
            instAssembly.Children.Add(Cuboid(bodyCenter, 3.2, 2.0, 2.8, InstrumentBrush));
            instAssembly.Children.Add(Cuboid(
                new Point3D(bodyCenter.X + 1.1, bodyCenter.Y - 1.5, bodyCenter.Z),
                1.0, 1.0, 1.0, LensBrush));

            if (vE)
            {
                // V pivot rod runs along gantry-local X (perpendicular to the XY plane
                // formed by the Z column axis and the bracket-offset direction), so the
                // instrument swings in the YZ plane — a vertical tilt toward / away
                // from the disc rather than a sideways swing.
                // During sim _simV oscillates ±VTiltAmplitude; otherwise it shows the
                // static VStaticAngle indication.
                double vAngle = IsSimulating ? _simV : VStaticAngle;
                instAssembly.Transform = new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(1, 0, 0), vAngle), vPivot);
            }
            gantry.Children.Add(instAssembly);

            // No outer rotation — X rail runs along world X, Y rail along world Z.
            // (The previous iteration rotated the whole gantry 90° Y; undoing that 90°
            // for the X/Y rails brings them back to their natural orientation. The
            // Z-axis bracket-on-(-X)-face tweak is preserved inside the gantry.)
            group.Children.Add(gantry);

            visual.Content = group;
            Viewport.Children.Add(visual);
        }

        // ────────── geometry helpers ──────────

        private static GeometryModel3D Cuboid(Point3D c, double sx, double sy, double sz, Brush fill)
        {
            var mesh = new MeshGeometry3D();
            double hx = sx / 2, hy = sy / 2, hz = sz / 2;
            // 8 corners (right-handed, +Y up, +Z toward viewer)
            Point3D v0 = new Point3D(c.X - hx, c.Y - hy, c.Z - hz);
            Point3D v1 = new Point3D(c.X + hx, c.Y - hy, c.Z - hz);
            Point3D v2 = new Point3D(c.X + hx, c.Y + hy, c.Z - hz);
            Point3D v3 = new Point3D(c.X - hx, c.Y + hy, c.Z - hz);
            Point3D v4 = new Point3D(c.X - hx, c.Y - hy, c.Z + hz);
            Point3D v5 = new Point3D(c.X + hx, c.Y - hy, c.Z + hz);
            Point3D v6 = new Point3D(c.X + hx, c.Y + hy, c.Z + hz);
            Point3D v7 = new Point3D(c.X - hx, c.Y + hy, c.Z + hz);

            mesh.Positions.Add(v0); mesh.Positions.Add(v1); mesh.Positions.Add(v2); mesh.Positions.Add(v3);
            mesh.Positions.Add(v4); mesh.Positions.Add(v5); mesh.Positions.Add(v6); mesh.Positions.Add(v7);

            // 6 faces — CCW winding when viewed from outside the cuboid
            AddFace(mesh, 0, 3, 2, 0, 2, 1); // -Z
            AddFace(mesh, 4, 5, 6, 4, 6, 7); // +Z
            AddFace(mesh, 0, 4, 7, 0, 7, 3); // -X
            AddFace(mesh, 1, 2, 6, 1, 6, 5); // +X
            AddFace(mesh, 0, 1, 5, 0, 5, 4); // -Y
            AddFace(mesh, 3, 7, 6, 3, 6, 2); // +Y

            var mat = new MaterialGroup();
            mat.Children.Add(new DiffuseMaterial(fill));
            mat.Children.Add(new SpecularMaterial(new SolidColorBrush(Color.FromArgb(0x60, 0xFF, 0xFF, 0xFF)), 35));
            return new GeometryModel3D(mesh, mat) { BackMaterial = mat };
        }

        private static void AddFace(MeshGeometry3D m,
            int a, int b, int c, int d, int e, int f)
        {
            m.TriangleIndices.Add(a); m.TriangleIndices.Add(b); m.TriangleIndices.Add(c);
            m.TriangleIndices.Add(d); m.TriangleIndices.Add(e); m.TriangleIndices.Add(f);
        }

        private static GeometryModel3D Cylinder(Point3D center, double radius, double height, int sides, Brush fill)
        {
            var mesh = new MeshGeometry3D();
            double y0 = center.Y - height / 2;
            double y1 = center.Y + height / 2;
            int N = Math.Max(8, sides);

            // bottom ring 0..N-1, top ring N..2N-1, then 2 caps centres
            for (int i = 0; i < N; i++)
            {
                double a = 2 * Math.PI * i / N;
                mesh.Positions.Add(new Point3D(
                    center.X + radius * Math.Cos(a), y0, center.Z + radius * Math.Sin(a)));
            }
            for (int i = 0; i < N; i++)
            {
                double a = 2 * Math.PI * i / N;
                mesh.Positions.Add(new Point3D(
                    center.X + radius * Math.Cos(a), y1, center.Z + radius * Math.Sin(a)));
            }
            int topC = mesh.Positions.Count; mesh.Positions.Add(new Point3D(center.X, y1, center.Z));
            int botC = mesh.Positions.Count; mesh.Positions.Add(new Point3D(center.X, y0, center.Z));

            for (int i = 0; i < N; i++)
            {
                int j = (i + 1) % N;
                // side quad (outward-facing CCW)
                mesh.TriangleIndices.Add(i);     mesh.TriangleIndices.Add(j);     mesh.TriangleIndices.Add(N + j);
                mesh.TriangleIndices.Add(i);     mesh.TriangleIndices.Add(N + j); mesh.TriangleIndices.Add(N + i);
                // top cap (+Y normal, CCW viewed from above)
                mesh.TriangleIndices.Add(topC);  mesh.TriangleIndices.Add(N + j); mesh.TriangleIndices.Add(N + i);
                // bottom cap (-Y normal, CCW viewed from below)
                mesh.TriangleIndices.Add(botC);  mesh.TriangleIndices.Add(i);     mesh.TriangleIndices.Add(j);
            }

            var mat = new MaterialGroup();
            mat.Children.Add(new DiffuseMaterial(fill));
            mat.Children.Add(new SpecularMaterial(new SolidColorBrush(Color.FromArgb(0x60, 0xFF, 0xFF, 0xFF)), 35));
            return new GeometryModel3D(mesh, mat) { BackMaterial = mat };
        }

        // ────────── simulation ──────────

        /// <summary>True while the demo gantry animation is running.</summary>
        public bool IsSimulating => _simTimer.IsEnabled;

        /// <summary>Fires after each simulation state change (start, stop, point advance).</summary>
        public event EventHandler SimulationStateChanged;

        /// <summary>Starts the demo. Two modes, chosen by whether U is enabled in the config:
        ///   • U disabled → loop a 9-point XY grid at MidSpeed mm/s (previous behaviour)
        ///   • U enabled  → spin the turntable at U.MidSpeed °/s; X/Y track a point 5 mm off
        ///                  the disc centre so the instrument stays locked on the same
        ///                  physical spot as it rotates (Type-A-style following).
        /// Disabled axes don't move.</summary>
        public void StartSimulation()
        {
            if (_simPoints.Count == 0) BuildDemoPoints();
            _simIdx = 0;
            _simU = 0.0;
            _simV = 0.0;
            _simVPhase = 0.0;
            // Snap X/Y to the U=0 tracking target so the sim doesn't begin with a visible jump.
            // No outer gantry rotation, so lens world == lens gantry-local =
            //   (_simX - offset, *, _simY). Marker at θ=0 is world (DiscTrackR, *, 0):
            //     _simX - offset = DiscTrackR   → _simX = DiscTrackR + offset
            //     _simY          = 0
            if (IsAxisEnabled("U"))
            {
                _simX = DiscTrackR + InstrumentLensOffset;
                _simY = 0;
            }
            _lastTickUtc = DateTime.UtcNow;
            _simTimer.Start();
            RebuildScene();
            UpdateAxisStateLabel();
            SimulationStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void StopSimulation()
        {
            if (!_simTimer.IsEnabled) return;
            _simTimer.Stop();
            UpdateAxisStateLabel();
            SimulationStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BuildDemoPoints()
        {
            // 3×3 grid inside the panel-under-test footprint.
            // X ∈ {-10, 0, +10}, Y(world-Z) ∈ {-3, 2, 7}.
            _simPoints.Clear();
            double[] xs = { -10, 0, +10 };
            double[] ys = { -3, 2, 7 };
            foreach (var y in ys)
                foreach (var x in xs)
                    _simPoints.Add(new Point(x, y));
        }

        private void SimTick(object sender, EventArgs e)
        {
            var now = DateTime.UtcNow;
            double dt = (now - _lastTickUtc).TotalSeconds;
            _lastTickUtc = now;
            if (dt > 0.2) dt = 0.2; // clamp so a paused tab doesn't teleport on resume

            // V-axis oscillation runs in parallel with whichever XY mode is active —
            // V.MidSpeed °/s advances the phase; _simV = ±VTiltAmplitude · sin(phase).
            // Same _simV value drives both the instrument tilt AND the disc tilt,
            // so the two move together during sim.
            if (IsAxisEnabled("V"))
            {
                double vSpeed = GetAxisSpeed("V");
                _simVPhase = (_simVPhase + vSpeed * dt) % 360.0;
                _simV = VTiltAmplitude * Math.Sin(_simVPhase * Math.PI / 180.0);
            }

            if (IsAxisEnabled("U"))
            {
                // ── U rotation mode ──
                // Turntable spins at U.MidSpeed °/s. A reference point glued to the disc
                // at local (DiscTrackR, 0) traces a circle in world XZ; the X/Y gantry
                // snaps to it each tick (if those axes are enabled) so the instrument
                // stays locked on the same spot on the rotating product.
                double uSpeed = GetAxisSpeed("U");          // °/s (MidSpeed of U row)
                _simU = (_simU + uSpeed * dt) % 360.0;

                // No outer gantry rotation: lens world == lens gantry-local.
                // Marker world at angle θ: (DiscTrackR cos θ, *, −DiscTrackR sin θ).
                // Lens gantry-local: (_simX − offset, *, _simY). Solve:
                //   _simX − offset =  DiscTrackR cos θ   →  _simX = DiscTrackR cos θ + offset
                //   _simY          = −DiscTrackR sin θ
                double theta = _simU * Math.PI / 180.0;
                double targetX = DiscTrackR * Math.Cos(theta) + InstrumentLensOffset;
                double targetY = -DiscTrackR * Math.Sin(theta);
                if (IsAxisEnabled("X")) _simX = targetX;
                if (IsAxisEnabled("Y")) _simY = targetY;

                RebuildScene();
                UpdateAxisStateLabel();
                return;
            }

            // ── grid-point traversal mode (U disabled) ──
            if (_simPoints.Count == 0) { _simTimer.Stop(); return; }
            var tgt = _simPoints[_simIdx];

            double vx = IsAxisEnabled("X") ? GetAxisSpeed("X") : 0.0;
            double vy = IsAxisEnabled("Y") ? GetAxisSpeed("Y") : 0.0;

            double dx = tgt.X - _simX;
            double dy = tgt.Y - _simY;
            if (vx > 0) _simX += Math.Sign(dx) * Math.Min(Math.Abs(dx), vx * dt);
            if (vy > 0) _simY += Math.Sign(dy) * Math.Min(Math.Abs(dy), vy * dt);

            RebuildScene();

            if (Math.Abs(tgt.X - _simX) < 0.05 && Math.Abs(tgt.Y - _simY) < 0.05)
            {
                _simIdx = (_simIdx + 1) % _simPoints.Count;
                UpdateAxisStateLabel();
                SimulationStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private double GetAxisSpeed(string axisName)
        {
            // Use MidSpeed as the demo pace — feels right for a schematic (not max-rapid).
            // Fallback 40 mm/s covers the case where no axis config has been supplied yet.
            var a = _axes?.FirstOrDefault(x =>
                string.Equals(x.AxisName, axisName, StringComparison.OrdinalIgnoreCase));
            if (a == null) return 40.0;
            return Math.Max(1.0, a.MidSpeed);
        }

        public int SimPointIndex => _simIdx;
        public int SimPointCount => _simPoints.Count;

        private void UpdateAxisStateLabel()
        {
            // During simulation the overlay shows progress — either U angle (tracking mode)
            // or the current grid point (traversal mode).
            if (IsSimulating)
            {
                if (IsAxisEnabled("U"))
                    TxtAxisState.Text = $"仿真 · U {_simU:0}°";
                else if (_simPoints.Count > 0)
                    TxtAxisState.Text = $"仿真 · 点 {_simIdx + 1} / {_simPoints.Count}";
                else
                    TxtAxisState.Text = "仿真";
                return;
            }

            if (_axes == null || _axes.Count == 0)
            {
                TxtAxisState.Text = "未配置";
                return;
            }
            // Show logical-axis state. If a logical axis is driven by >1 channel,
            // append the count — e.g. "X✓×2" for a gantry dual-drive X.
            var parts = new List<string>(MotionCatalog.AxisNames.Length);
            foreach (var ax in MotionCatalog.AxisNames)
            {
                var matches = _axes.Where(m =>
                    string.Equals(m.AxisName, ax, StringComparison.OrdinalIgnoreCase)).ToList();
                if (matches.Count == 0) { parts.Add(ax + "—"); continue; }
                bool on = matches.Any(m => m.Enabled);
                string suffix = matches.Count > 1 ? "×" + matches.Count : "";
                parts.Add(ax + (on ? "✓" : "—") + suffix);
            }
            TxtAxisState.Text = string.Join("  ", parts);
        }
    }
}
