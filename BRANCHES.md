# 版本分支索引 (Branch Index)

本仓库用 **git 分支** 管理多个客户版本。每个分支只提交源码（遵循 `.gitignore`，不含
bin/obj/packages 等编译产物）。

## 分支一览

| 分支 | 客户 / 用途 | 架构 | 来源 | 与 main 差异 |
|------|-----------|------|------|-------------|
| **main** | 通用基线 / 开发 | 现代重构（`LCD.Core` + `LCD.Drv` 分层 + `LCD_V2`） | GitHub | — |
| **guoxian**（国显） | 国显客户部署版 | 现代重构（同 main 血脉） | `lcd_csharp.zip` | 约 16 个源文件；含 `OmniDriverStub.cs`、`shims/` 项目 |
| **suqian**（宿迁） | 宿迁客户部署版 | **旧单体**（驱动都在 `LCD/Ctrl`、`LCD/Dll` 内，无独立 `LCD.Drv`） | `LCD.zip` + `LCD.Core.zip` | 约 50 个源文件不同、文件集也不同；仅含 `LCD` + `LCD.Core` |

各客户分支根目录另有 `VERSION.md` 说明该版本特点。

## 维护约定

1. **共性修复**：先在一个分支改好，再用 `git cherry-pick <commit>` 或 `git merge`
   同步到其它分支；客户专属改动留在各自分支。
2. **每次改动都要记录**：改了什么、为什么，写进该分支的 `CHANGELOG.md`，**随提交一起推送**。
3. **对比差异**：`git diff main..guoxian`、`git diff main..suqian`。
4. **源码 zip** 原始包存放在仓库外 `d:\workspace\00-LCD-zips\`，不提交进仓库。

## 构建与运行

见 `README.md`。简言之：经典 WPF，用 Visual Studio MSBuild 构建（`dotnet build` 不跑
XAML 标记编译）；完整运行依赖相机 / 运动卡 / 色度计等硬件，开发机上会走无硬件降级路径。
