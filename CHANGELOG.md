# 更新记录 (Changelog)

> **约定**：每次改动都在此记录——**日期 / 分支 / 改了什么 / 为什么**，并随提交一起推送。
> 客户分支（guoxian、suqian）各自维护自己的 `CHANGELOG.md`；本文件记录 `main` 及仓库级
> （跨分支、结构性）的变更。

## 2026-08-25

### main
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
