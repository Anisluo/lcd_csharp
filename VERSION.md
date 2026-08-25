# 宿迁 (Suqian) 版本

宿迁客户部署版。来源：`LCD.zip` + `LCD.Core.zip`。

## 特点
- **旧单体架构**：设备驱动（BM7A / CS2000 / MovCtrl / TestMachine / USB2000 等）直接在
  `LCD/Ctrl/`、`LCD/Dll/` 内，未拆分出独立的 `LCD.Drv` 项目。
- 仅含 `LCD`（UI）+ `LCD.Core` 两个工程，无 `LCD.Drv` / `LCD_V2` / `.sln`
  （客户交付的源码即如此）。
- 与 `main`（现代重构版）差异较大：~50 个源文件内容不同，且文件集不同。

## 说明
- 仅提交源码，遵循 `.gitignore`（不含 bin/obj/packages，及 `xxx(1).cs` 复制副本——
  经核对这些副本未被 `LCD.csproj` 编译，属 Windows 复制垃圾）。
- 如需在无硬件的开发机构建，缺少的仪器 SDK DLL 见各 `Dll/` 目录内既有二进制。
