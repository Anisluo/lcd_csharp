# 更新记录 (Changelog) — guoxian（国显）

> 约定：每次改动都在此记录——**日期 / 改了什么 / 为什么**，随提交一起推送。
> 仓库级 / 跨分支变更见 `main` 分支的 `CHANGELOG.md`。

## 2026-08-25
- **建立 guoxian 分支**：国显客户部署版，来源 `lcd_csharp.zip`。现代重构架构，与 `main`
  几乎一致（约 16 个源文件差异）。
  - `LCD.Drv` 新增 `OmniDriverStub.cs`——用桩替代专有 NETOmniDriver 光谱仪 SDK。
  - 新增 `shims/` 项目（Microsoft.DwayneNeed 垫片）。
  - 并入国显在 main 之上两笔提交的效果：加固缺硬件 DLL 时的启动、`Database.Open()` 自动
    建 SQLite 表（修复点击测试闪退）。
- 新增 `VERSION.md`、本 `CHANGELOG.md`。
