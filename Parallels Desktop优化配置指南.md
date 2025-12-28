# Parallels Desktop 优化配置指南

## 针对 IDVLBS 获取失败的优化方案

你使用的是 Parallels Desktop，这是 macOS 上最好的虚拟机软件之一。但窗口操作仍然可能失败，需要优化配置。

## 第一步：Parallels Desktop 虚拟机设置

### 1. 打开虚拟机设置

1. 在 Parallels Desktop 中，选择你的 Windows 虚拟机
2. 点击右上角的"配置"（齿轮图标）
3. 或右键虚拟机 → "配置"

### 2. 硬件设置

**CPU 和内存：**
```
- 内存：至少 4GB（推荐 8GB）
- CPU：至少 2 核（推荐 4 核）
- 启用"启用嵌套虚拟化"（如果可用）
```

**图形：**
```
- 图形：选择"最佳性能"或"游戏"
- 启用 3D 加速：✅ 必须启用
- 分辨率：使用 Retina 分辨率（如果支持）
- 启用 DirectX 11：✅ 启用
```

**硬件：**
```
- 启用硬件加速：✅ 启用
- 启用虚拟化：✅ 启用
```

### 3. 选项设置

**优化：**
```
- 选择"游戏"或"开发"模式
- 不要选择"省电"模式
```

**共享：**
```
- 可以关闭不必要的共享（如剪贴板、文件夹）
- 这些共享可能影响窗口操作
```

**安全：**
```
- 关闭"隔离 Windows"（如果启用）
- 关闭"安全启动"（如果启用）
- 这些安全功能可能阻止窗口操作
```

**应用程序：**
```
- 可以启用"在 Mac 菜单栏中显示 Windows 应用程序"
- 但可能影响窗口查找，建议关闭
```

### 4. 高级设置

**其他：**
```
- 启用"使用硬件加速"
- 启用"优化 Windows 以在虚拟机中运行"
- 关闭"启用安全启动"（如果可用）
```

## 第二步：Windows 系统设置

### 1. 关闭 Windows Defender

1. 打开"Windows 安全中心"
2. 点击"病毒和威胁防护"
3. 点击"管理设置"
4. 关闭"实时保护"
5. 关闭"云提供的保护"
6. 关闭"自动提交样本"

**或者使用组策略（Windows Pro）：**
```
1. Win + R，输入 gpedit.msc
2. 计算机配置 → 管理模板 → Windows 组件 → Microsoft Defender 防病毒
3. 关闭"关闭 Microsoft Defender 防病毒"
```

### 2. 关闭 UAC（用户账户控制）

1. Win + R，输入 `msconfig`
2. 点击"工具"标签
3. 选择"更改 UAC 设置"
4. 将滑块拖到最底部（从不通知）
5. 重启 Windows

### 3. 以管理员身份运行程序

1. 右键破解程序
2. 选择"以管理员身份运行"
3. 或者设置程序始终以管理员身份运行：
   - 右键程序 → 属性
   - 兼容性标签
   - 勾选"以管理员身份运行此程序"

### 4. 设置程序兼容性

1. 右键破解程序
2. 属性 → 兼容性
3. 勾选"以兼容模式运行这个程序"
4. 选择"Windows 7"或"Windows 8"
5. 勾选"以管理员身份运行此程序"
6. 勾选"禁用全屏优化"（如果可用）

### 5. 关闭 Windows 防火墙（临时）

1. 控制面板 → Windows Defender 防火墙
2. 点击"启用或关闭 Windows Defender 防火墙"
3. 关闭所有网络的防火墙（临时，测试后可以重新开启）

## 第三步：Parallels Desktop 特殊设置

### 1. 启用 Coherence 模式（可选）

**注意：** Coherence 模式可能影响窗口查找，建议：
- 先尝试关闭 Coherence 模式
- 如果失败，再尝试启用

**关闭 Coherence：**
```
- 查看 → 退出 Coherence 模式
- 或按 Control + Option + C
```

### 2. 调整显示设置

1. 查看 → 全屏（或窗口模式）
2. 确保虚拟机窗口完全可见
3. 不要最小化虚拟机窗口

### 3. 检查 Parallels Tools

1. 确保 Parallels Tools 已安装并更新到最新版本
2. 虚拟机 → 安装 Parallels Tools
3. 如果已安装，检查更新

## 第四步：测试和调试

### 1. 测试窗口操作

1. 手动运行 `G4VNVLBS.exe`
2. 观察窗口是否正常显示
3. 检查窗口标题是否正确
4. 手动点击注册按钮，看是否能打开注册窗口

### 2. 查看调试输出

运行改进后的破解程序，查看详细调试信息：

```
DEBUG: GetVLBSKey - Starting process: ...
DEBUG: GetVLBSKey - Process started, PID: ...
DEBUG: GetVLBSKey - Waiting for main window...
DEBUG: GetVLBSKey - Main window found! Title: ...
```

根据调试信息定位具体失败点。

### 3. 如果仍然失败

**尝试手动获取 HWID：**

1. 在虚拟机中手动运行 `G4VNVLBS.exe`
2. 手动点击注册按钮（"§¨ng ký"）
3. 在注册窗口中复制 HWID
4. 编辑 `config.ini`：
   ```
   IDConfig=harddiskSSDZX3KD0WVE614S7HCFMW1
   IDVLBS=你复制的HWID
   ```
5. 重新运行破解程序

## 第五步：Parallels Desktop 版本检查

### 检查版本

1. Parallels Desktop → 关于 Parallels Desktop
2. 确保使用最新版本（至少 17 或更高）

### 如果版本较旧

- 考虑升级到最新版本
- 新版本对 Windows 11 和窗口操作支持更好

## 常见问题排查

### 问题 1：窗口找不到

**可能原因：**
- Coherence 模式影响窗口查找
- 窗口标题不匹配

**解决方案：**
- 关闭 Coherence 模式
- 查看调试输出中的窗口列表
- 检查实际窗口标题

### 问题 2：按钮点击无效

**可能原因：**
- PostMessage 在虚拟机中无效
- 需要真实鼠标点击

**解决方案：**
- 考虑修改代码使用真实鼠标点击
- 或手动操作

### 问题 3：文本框读取为空

**可能原因：**
- 权限不足
- 文本框还未加载内容

**解决方案：**
- 以管理员身份运行
- 增加等待时间
- 手动检查文本框是否有内容

## 推荐的 Parallels Desktop 配置总结

```
虚拟机配置：
- 内存：8GB
- CPU：4 核
- 图形：最佳性能
- 3D 加速：启用
- 硬件加速：启用
- 优化模式：游戏

Windows 设置：
- Windows Defender：关闭
- UAC：关闭
- 防火墙：临时关闭
- 程序：以管理员身份运行
- 兼容性：Windows 7

Parallels 设置：
- Coherence：关闭（测试时）
- Parallels Tools：最新版本
```

## 如果所有方法都失败

考虑以下替代方案：

1. **使用 Boot Camp**
   - 原生 Windows，100% 兼容

2. **手动获取 HWID**
   - 每次手动复制 HWID 到配置文件

3. **修改代码使用真实鼠标点击**
   - 替代 PostMessage，使用真实的鼠标操作

---

**更新日期：** 2024年
**适用版本：** Parallels Desktop 17+
