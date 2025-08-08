# Gold Monitor
## File Structure
```
MyAvaloniaApp/
│
├── Models/                 # 数据模型（业务实体）
├── ViewModels/             # ViewModel 层，负责 UI 逻辑
├── Views/                  # View 层（XAML + 代码后置）
├── Services/               # 服务类（数据访问、网络、配置等）
├── Assets/              # 静态资源（图标、图像、样式等）
├── App.axaml               # 应用入口 XAML
├── App.axaml.cs            # 应用启动逻辑
├── Program.cs              # 主程序入口（Avalonia 11+ 推荐）
└── Helpers/ 或 Utilities/  # 工具类（命令、转换器、扩展方法等）
```
