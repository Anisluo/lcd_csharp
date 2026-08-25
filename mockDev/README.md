# mockDev — LCD 硬件仿真工具

在**没有真实硬件**的机器上，模拟 LCD 上位机所需的**串口仪器**，让 LCD 能识别、通信、取到数据。
第一期支持 **SR3A**（分光辐射亮度计）；运动控制卡 MPC08 后续再做（它是原生 PCI DLL，需另做假 DLL）。

## 前置：安装 com0com（开源虚拟串口）

纯软件无法凭空创建 COM 口，必须靠虚拟串口驱动。用开源的 **com0com**：

1. 下载**已签名的 3.0.0.0 版本**（Win10/11 x64 必须签名版，否则驱动加载不了）：
   https://sourceforge.net/projects/com0com/
2. 默认安装到 `C:\Program Files (x86)\com0com\`（含 `setupc.exe`）。mockDev 会自动找到它。

> mockDev 会**自动创建**一对虚拟串口（用 setupc），**关闭时自动删除**，无需手动配对。

## 用法

1. **以管理员身份**运行 `mockDev.exe`（清单已声明，双击会弹 UAC）——创建/删除虚拟串口对需要管理员权限。
2. 界面上：
   - `com0com setupc`：一般自动检测到，没有就“浏览”指定。
   - `LCD 端口` / `mockDev 端口`：默认 `COM20 ↔ COM21`。**LCD 连左边**、mockDev 用右边。
   - `模拟仪器`：选 `SR3A`。
   - 点 **▶ 开始**：自动建好 COM20↔COM21，并在 COM21 上模拟 SR3A。
3. 配置 LCD：把 `Config.xml` 里对应仪器的 `comName` 改成 **LCD 端口**（如 `COM20`），
   `TESTMACHINE` 设为 `SR3A`，启动 LCD 即可通信、单点/光谱测试取到数据。
4. 关闭 mockDev（或点 **■ 停止**）→ 自动删除虚拟串口对。

## 加一个新仪器

1. 在 `Instruments/` 下新建 `XxxInstrument.cs`，继承 `InstrumentBase`，在构造里
   `On("指令", () => "响应")` 注册该仪器的指令与真实反馈。
2. 在 `MainWindow` 构造里 `InstrumentBox.Items.Add(new XxxInstrument())`。
3. 把新 `.cs` 加进 `mockDev.csproj` 的 `<Compile>`。

参考 `Instruments/SR3AInstrument.cs`（含 SR3A 的 `RM` / `D0 ST` 光谱 / `D1 ST` 色坐标 协议与响应格式）。
