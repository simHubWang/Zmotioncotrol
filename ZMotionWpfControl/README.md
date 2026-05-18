# 正运动控制卡 WPF 运动程序

这是一个 C# WPF 运动控制界面工程，当前使用 `MockMotionController` 模拟控制卡行为，便于先验证界面和流程。

## 功能

- 控制卡 IP/端口连接与断开
- 4 轴状态监控
- 伺服使能/断开
- 回零、清报警
- 正负向点动
- 绝对运动、相对运动
- 单轴停止、全部轴急停
- 运行日志

## 真实控制卡接入位置

运动控制接口在 `Services/IMotionController.cs`。

当前界面调用的是 `Services/MockMotionController.cs`。接入正运动控制卡时，新建一个真实实现类，例如 `ZMotionController`，在里面调用正运动提供的 `zauxdll.dll` / `zmcaux.cs` API，例如：

- `ZAux_OpenEth`
- `ZAux_Close`
- `ZAux_Direct_SetAtype`
- `ZAux_Direct_SetUnits`
- `ZAux_Direct_Single_Move`
- `ZAux_Direct_Single_Vmove`
- `ZAux_Direct_Single_Cancel`
- `ZAux_Direct_GetDpos`

然后在 `MainWindow.xaml.cs` 中把：

```csharp
private readonly IMotionController _controller = new MockMotionController();
```

替换为真实控制器：

```csharp
private readonly IMotionController _controller = new ZMotionController();
```

## 运行

需要 Windows 和支持 WPF 的 .NET SDK/Visual Studio。打开 `ZMotionWpfControl.csproj` 后运行即可。
