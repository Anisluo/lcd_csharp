# 更新记录 (Changelog) — suqian（宿迁）

> 约定：每次改动都在此记录——**日期 / 改了什么 / 为什么**，随提交一起推送。
> 仓库级 / 跨分支变更见 `main` 分支的 `CHANGELOG.md`。

## 2026-08-25
- **建立 suqian 分支**：宿迁客户部署版，来源 `LCD.zip` + `LCD.Core.zip`。
  - 旧单体架构：设备驱动（BM7A / CS2000 / MovCtrl / TestMachine / USB2000 等）直接在
    `LCD/Ctrl`、`LCD/Dll` 内，无独立 `LCD.Drv` 项目。
  - 仅含 `LCD`（UI）+ `LCD.Core` 两个工程，无 `LCD.Drv` / `LCD_V2` / `.sln`（客户交付源码即如此）。
  - 与 `main` 差异较大：约 50 个源文件不同、文件集也不同。
- 仅提交源码，遵循 `.gitignore`（不含 bin/obj/packages 及 `xxx(1).cs` 复制副本）。
- 新增 `VERSION.md`、本 `CHANGELOG.md`。
