$ErrorActionPreference = 'Stop'

$exe    = 'f:\LCD\LCD\LCD_V2\bin\Debug\LCD_V2.exe'
$outDir = 'f:\LCD\LCD\docs\screenshots'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

Add-Type @'
using System;
using System.Runtime.InteropServices;
public class Win32 {
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left, Top, Right, Bottom;
    }
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int cmd);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")] public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
    public const uint MOUSEEVENTF_LEFTDOWN = 0x02;
    public const uint MOUSEEVENTF_LEFTUP   = 0x04;
}
'@

# Launch the app
$proc = Start-Process -FilePath $exe -PassThru
$proc.WaitForInputIdle(5000) | Out-Null
Start-Sleep -Seconds 3

# Locate the main window via UIA
$root = [System.Windows.Automation.AutomationElement]::RootElement
$win  = $null
for ($i = 0; $i -lt 40; $i++) {
    $win = $root.FindFirst(
        [System.Windows.Automation.TreeScope]::Children,
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::NameProperty,
            'LCD 光学测试系统 V2')))
    if ($win) { break }
    Start-Sleep -Milliseconds 400
}
if (-not $win) {
    Write-Error 'Main window not found via UIA'
    $proc.Kill()
    exit 1
}

$hwnd = [IntPtr]$win.Current.NativeWindowHandle
[Win32]::ShowWindow($hwnd, 3) | Out-Null   # SW_MAXIMIZE
Start-Sleep -Milliseconds 600
[Win32]::SetForegroundWindow($hwnd) | Out-Null
Start-Sleep -Milliseconds 400

$pages = @(
    @{ Name = '仪表板';   File = '01-dashboard.png' },
    @{ Name = '模板管理'; File = '02-templates.png' },
    @{ Name = '仪器指标'; File = '03-metrics.png'   },
    @{ Name = '运动平台'; File = '04-motion.png'    },
    @{ Name = '测试结果'; File = '05-results.png'   },
    @{ Name = '设备状态'; File = '06-devices.png'   },
    @{ Name = '运行日志'; File = '07-logs.png'      },
    @{ Name = '系统设置'; File = '08-settings.png'  }
)

foreach ($p in $pages) {
    $nav = $win.FindFirst(
        [System.Windows.Automation.TreeScope]::Descendants,
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::NameProperty, $p.Name)))
    if (-not $nav) {
        Write-Host "Nav item not found: $($p.Name) — skipping"
        continue
    }
    # Click at the element's bounding rectangle centre — works whether the UIA
    # element is a ListBoxItem or just the TextBlock inside it, because the
    # click hits whichever container sits at that screen position.
    $br = $nav.Current.BoundingRectangle
    if ($br.IsEmpty) {
        Write-Host "Empty bounding rect for nav item: $($p.Name) — skipping"
        continue
    }
    $cx = [int]($br.Left + $br.Width / 2)
    $cy = [int]($br.Top  + $br.Height / 2)
    [Win32]::SetCursorPos($cx, $cy) | Out-Null
    Start-Sleep -Milliseconds 80
    [Win32]::mouse_event([Win32]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
    Start-Sleep -Milliseconds 40
    [Win32]::mouse_event([Win32]::MOUSEEVENTF_LEFTUP,   0, 0, 0, 0)

    # Motion page has a 3D viewport that needs time to render
    $waitMs = if ($p.Name -eq '运动平台') { 2500 } else { 1200 }
    Start-Sleep -Milliseconds $waitMs

    # Capture window rectangle
    $rect = New-Object 'Win32+RECT'
    [Win32]::GetWindowRect($hwnd, [ref]$rect) | Out-Null
    $w = $rect.Right  - $rect.Left
    $h = $rect.Bottom - $rect.Top
    if ($w -le 0 -or $h -le 0) {
        Write-Host "Window rect invalid for $($p.Name), skipping"
        continue
    }

    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen($rect.Left, $rect.Top, 0, 0, $bmp.Size)
    $dest = Join-Path $outDir $p.File
    $bmp.Save($dest, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose()
    $bmp.Dispose()
    Write-Host "Saved $dest ($($w)x$($h))"
}

Start-Sleep -Milliseconds 400
$proc.Kill()
Write-Host 'Done.'
