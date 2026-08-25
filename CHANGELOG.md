# 更新记录 (Changelog) — suqian（宿迁）

> 约定：每次改动都在此记录——**日期 / 改了什么 / 为什么**，随提交一起推送。
> 仓库级 / 跨分支变更见 `main` 分支的 `CHANGELOG.md`。

## 2026-08-25（续 3）
- **移植 SR-3A 视场角设置功能**（参照 guoxian，对齐其结构但只新增、不动 suqian 现有路径）：
  - 新增视场角弹窗 `View/ViewangleView.xaml(.cs)`（FDL 2 / 1 / 0.2 / 0.1 四档 + 关闭），
    去掉 guoxian 的多语言绑定改用中文；机型枚举无 SR5A，仅按 SR3A / CS2000 启用。
  - `Ctrl/SR3A.cs` 新增 `SenFdlCmd(cmd)`：先发 RM 等 OK，再发 `FLD1~FLD4`（2°/1°/0.2°/0.1°）。
  - `Ctrl/CS2000.cs` 新增 `SendCmd(cmd)`（直发指令，供 CS2000 视场角用）。
  - `MainWindow.xaml.cs` 新增 `OnBnClickedFdl`（打开弹窗）+ 静态 `sr3a_send_fdl_cmd` /
    `cs2000_send_cmd`；`MainWindow.xaml` 在“参数（P）”菜单加“视场角设置”入口。
  - 已编译通过；因本机无硬件（电机报警等 Win32 模态框阻塞 UI 线程）无法自动弹窗截图，
    需在真机上从“参数→视场角设置”点档位、对 SR3A 发 FLD 指令验证。
- **新增 mockDev 硬件仿真工具**（新项目 `mockDev/`，WPF/.NET 4.8）：在无硬件机器上模拟
  LCD 所需的**串口仪器**，让 LCD 能识别/通信/取数。
  - 配合开源虚拟串口 **com0com**：mockDev 用 `setupc.exe` **自动创建**虚拟串口对、
    **关闭时自动删除**（需管理员，清单已声明）。
  - 可扩展的仪器框架 `Instruments/`（`InstrumentBase` + 指令→响应注册表）；
    第一期实现 **SR3A**（`RM` / `D0 ST` 光谱 401 点 / `D1 ST` 色坐标，按 LCD 解析器格式回包）。
  - 运动控制卡 MPC08 是原生 PCI DLL，需另做假 DLL，列入后续。
  - 用法见 `mockDev/README.md`。本机无 com0com/管理员，未做端到端联调；已验证编译通过、
    程序可启动、界面正常。

## 2026-08-25（续）
- **修复：光谱测试采集的 401 个数据无法更新到界面**（`View/ResutView.xaml.cs`
  `AddSingleData` 的 `_03_SPECTRUM` 分支）：对照正常工作的 guoxian 版本，波长单元格
  原写入的是**裸 double**（`dataRow["380"] = objs.SpectrumData[i]`），改为写入
  **字符串** `objs.SpectrumData[i].ToString("E")`，与 guoxian 一致；DB 的 dataValue
  同步改用 `.ToString("E")`。列名维持数字 `"380"`~`"780"`（guoxian 也是数字列名，
  之前误判为列名问题已回退）。
  - L/X/Y/Cx/Cy 显示为 0 属正常（SR3A 该场景本就如此），不处理。
  - **已验证**：用下述 Demo 模拟光谱源在无硬件本机点“光谱测试”，401 波长列正常出数、
    界面正常更新。
- **DemoMachine 支持模拟测量**（`Ctrl/DemoMachine.cs` + `MainWindow.xaml.cs`）：
  给 DemoMachine 补上 `MeasureLxy` / `MeasureSpectrum`（模拟 401 点光谱曲线），并在
  MainWindow 里把 `ENUMMACHINE.Demo` 接上 `new DemoMachine()`（原来是空分支，选 Demo
  会在 `testMachine.Init()` 空引用崩溃）。便于无硬件开发机自测；配置 `TESTMACHINE=Demo`
  时启用，不影响真机（默认 SR3A）。
- **相机初始化按构建配置门控**（`View/CamView.xaml.cs` `InitCam()`）：`#if DEBUG` 时跳过
  海康 MV 相机初始化（本机未装 MvCameraControl 驱动，避免 DllNotFound / 启动异常），
  Release 版照常启用相机。
  - 注：运行还需 `bin/Debug/database/MyData.db`（SQLite 库，属运行数据、gitignore 不入库）；
    本机从客户包恢复。若要在任意无硬件机器上免依赖启动，可后续硬化 `Database.Open()`
    自动建库/建表（参考 main 已有的同类修复）。

## 2026-08-25
- **建立 suqian 分支**：宿迁客户部署版，来源 `LCD.zip` + `LCD.Core.zip`。
  - 旧单体架构：设备驱动（BM7A / CS2000 / MovCtrl / TestMachine / USB2000 等）直接在
    `LCD/Ctrl`、`LCD/Dll` 内，无独立 `LCD.Drv` 项目。
  - 仅含 `LCD`（UI）+ `LCD.Core` 两个工程，无 `LCD.Drv` / `LCD_V2` / `.sln`（客户交付源码即如此）。
  - 与 `main` 差异较大：约 50 个源文件不同、文件集也不同。
- 仅提交源码，遵循 `.gitignore`（不含 bin/obj/packages 及 `xxx(1).cs` 复制副本）。
- 新增 `VERSION.md`、本 `CHANGELOG.md`。
