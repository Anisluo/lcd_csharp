# 国显 (Guoxian) 版本

国显客户部署版。来源：`lcd_csharp.zip`。

## 特点
- **现代重构架构**，与 `main` 几乎一致（同一血脉），仅约 16 个源文件差异：
  - LCD: App.xaml.cs / Ctrl/ProcessCtrl.cs / Data/Project.cs / MainWindow.xaml.cs /
    View/CamView.xaml.cs / dataBase/Database.cs / LCD.csproj
  - LCD.Core: Data/DisplayFormat.cs
  - LCD.Drv: BM7A.cs / Motion/MovCtrl.cs / LCD.Drv.csproj + 新增 OmniDriverStub.cs
    （用桩替代专有 NETOmniDriver SDK）
  - 新增 shims/ 项目（Microsoft.DwayneNeed 垫片）
- 国显源码在 main 之上多两笔提交的效果已并入本分支：
  "harden startup against missing hardware DLLs" 与 "auto-create SQLite schema (修复点击测试闪退)"。

## 说明
仅提交源码，遵循 `.gitignore`（不含 bin/obj/packages）。
