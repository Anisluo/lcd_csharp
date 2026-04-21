# Code Map

## Main structure

- `LCD/Data/Project.cs`
  Stores global config, device config, template persistence, result containers, and `ENUMMESSTYLE`.
- `LCD/View/CustomView.xaml` + `LCD/View/CustomView.xaml.cs`
  Main template management UI. Creates template types, lists templates, opens coordinate editor, saves template groups.
- `LCD/View/CustomTemplate.xaml` + `LCD/View/CustomTemplate.xaml.cs`
  Point-like template editor and auto-generation UI.
- `LCD/View/DataTemplateView.xaml.cs`
  Displays the selected template table in the main UI.
- `LCD/Ctrl/ProcessCtrl.cs`
  Executes template groups, routes by measurement type, runs movement and measurement.
- `LCD/Ctrl/PointF.cs`
  Mechanism-dependent coordinate compensation core.
- `LCD/View/MPCView.xaml` + `LCD/View/MPCView.xaml.cs`
  Equipment mechanism and axis parameter editor. This is where centers, axis polarity, limits, interpolation, compensation direction, and `EQType` are configured.

## Template persistence model

Persisted object: `InfoData`

Defined in:

- `LCD/View/CustomView.xaml.cs`

Key fields:

- `id`
- `height`
- `IsSelected`
- `MESTYPE`
- `Name`
- `lstdata`
- product geometry fields such as `productLength`, `productWidth`
- criteria fields such as `IsLchk`, `Isxchk`, `Isychk`, `IsBalancechk`

`lstdata` stores a CSV-like table:

- first row is header
- remaining rows are data

## Where new template types are added

### Enum

- `LCD/Data/Project.cs`
  `ENUMMESSTYLE`

### Combo box options

- `LCD/View/CustomView.xaml`

### UI selection -> enum

- `LCD/View/CustomView.xaml.cs`
  `comboBoxType_SelectionChanged()`

### Add template button

- `LCD/View/CustomView.xaml.cs`
  `OnBnClickedAddTemplate()`

This currently infers `MESTYPE` from the combo text like `01`, `02`, `03`, `05`, `07`.

### Type display text

- `LCD/View/CustomView.xaml.cs`
  `update_type()`

### Table display

- `LCD/View/DataTemplateView.xaml.cs`
  `Init(...)`

### Runtime execution

- `LCD/Ctrl/ProcessCtrl.cs`
  `Run()`

## Where coordinate editor opens

Entry:

- `LCD/View/CustomView.xaml.cs`
  `OnBnClickedAutoCreate()`

Current behavior:

- load or create `InfoData`
- open `CustomTemplate`
- return edited `DataTable`
- copy threshold/product parameters back into `InfoData`

This is the best place to branch TCO to a dedicated editor if needed.

## Where coordinate compensation happens

Runtime path:

1. `ProcessCtrl.Run()`
2. `ProcessCtrl.ProcessPointTemplateXYZ()`
3. read row `X/Y/Z/U/V/Ball`
4. branch on `Project.cfg.EQType`
5. if not `Type_D`, call `PointF.UpdateByAlgorithm1(height, Project.PtCenter)`
6. execute `OnMove2Point(...)`

## Equipment mechanism data used by compensation

Configured mainly through:

- `Project.cfg.EQType`
- `Project.cfg.XCenter/YCenter/ZCenter/UCenter/VCenter/BallCenter`
- `Project.cfg.ax_*`
- `Project.PtCenter`
- `Project.Xorg/Yorg/Zorg/Uorg/Vorg/Ballorg`

Edited in:

- `LCD/View/MPCView.xaml.cs`

## Why TCO must be linked with equipment mechanism

Because TCO requires angle-dependent measurements around the display center, while actual movement is not a pure angle command:

- the machine may rotate around different physical centers
- the compensation formula differs by `EquipmentType`
- thickness and axis direction affect final target coordinates

So TCO generation must remain compatible with the same compensation assumptions already used by `ProcessPointTemplateXYZ()` and `PointF`.

