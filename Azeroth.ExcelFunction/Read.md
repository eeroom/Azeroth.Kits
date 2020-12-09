注册程序集为com组件，让excel调用
64位excel:	C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe $(TargetPath) /codebase
32位excel:   C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe $(TargetPath) /codebase

打开excel，开发工具》加载项》自动化》选择相应的功能