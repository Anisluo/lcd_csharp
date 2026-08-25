# 更新记录 (Changelog)

> **约定**：每次改动都在此记录——**日期 / 分支 / 改了什么 / 为什么**，并随提交一起推送。
> 客户分支（guoxian、suqian）各自维护自己的 `CHANGELOG.md`；本文件记录 `main` 及仓库级
> （跨分支、结构性）的变更。

## 2026-08-25

### main
- **合入国显的两笔通用修复**（从 guoxian 血脉 cherry-pick）：
  - `harden startup against missing optional hardware DLLs and incomplete Config`
    （a04b0a6）—— 加固缺硬件 DLL / 配置不全时的启动；新增 `LCD.Drv/OmniDriverStub.cs`
    （桩替代专有 NETOmniDriver SDK）与 `shims/`（Microsoft.DwayneNeed 垫片），改动
    App.xaml.cs / Data/Project.cs / MainWindow.xaml.cs / View/CamView.xaml.cs /
    LCD.Drv 相关。**使 main 在无相机等硬件的开发机上也能构建/启动。**
  - `auto-create SQLite schema on Database.Open()`（31b4c58）—— `dataBase/Database.cs`
    首次打开自动建表，**修复点击测试闪退**。
  - 国显专属改动（Ctrl/ProcessCtrl.cs、LCD.Core/DisplayFormat.cs、LCD.Drv/BM7A.cs）
    未合入，保留在 guoxian。
- **新增多版本分支管理**：建立 `guoxian`（国显）、`suqian`（宿迁）两个分支，`main` 保持
  现代重构版基线。三分支均只提交源码（遵循 `.gitignore`）。
- **新增 `BRANCHES.md`**：三个客户版本的分支索引与维护约定。
- **新增本 `CHANGELOG.md`**：确立"每次改动都记录、推送时带上"的约定。

### guoxian（国显，来源 lcd_csharp.zip）
- 建分支：内容为国显部署版，与 main 几乎一致（约 16 个源文件差异）。详见该分支
  `VERSION.md` 与 `CHANGELOG.md`。

### suqian（宿迁，来源 LCD.zip + LCD.Core.zip）
- 建分支：内容为宿迁部署版，旧单体架构、仅 `LCD` + `LCD.Core`。详见该分支
  `VERSION.md` 与 `CHANGELOG.md`。
