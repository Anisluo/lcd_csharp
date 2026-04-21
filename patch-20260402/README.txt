patch-20260402

This folder contains the customer-side update package for the "仪器固定，翻转式" feature.

Files:
- flip_update.patch
- apply_flip_update.ps1
- customer_run_flip_update.bat

Recommended usage:
1. Copy the whole folder to the customer's project root.
2. Open the folder.
3. Double-click customer_run_flip_update.bat

Changes applied by this patch:
- LCD\Data\Project.cs       : Add IsFlipped field to Config class
- LCD\View\DeviceView.xaml  : Add "仪器固定，翻转式" CheckBox to DeviceView bottom bar
- LCD\Ctrl\MovCtrl.cs       : MoveXAxisDown / MoveYAxiesDown reverse direction when IsFlipped is true

Notes:
- The PowerShell script auto-detects the project root by searching upward.
- The .patch file is for manual review or hand-applied updates.
- The script is idempotent: safe to run multiple times.
