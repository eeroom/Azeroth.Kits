# .net官方的实现windows服务关键点
```
1、一个windows服务对应一个继承于System.ServiceProcess.ServiceBase的类
	需要明确指定ServiceName的值，后续的安装部分也会涉及到这个ServiceName，
	两个地方需要保持一致，才能关联起来，后续才能正常启动
2、需要一个继承于System.Configuration.Install.Installer的类型，
	这个是给安装服务的执行InstallUtil的时候用到的
	需要包含一个System.ServiceProcess.ServiceProcessInstaller的实例，用于指定服务运行的账号
	需要包含一个System.ServiceProcess.ServiceInstaller，通过指定ServiceName关联上具体的windows服务，
3、InstallUtil.exe 工具，.net会自动这个工具，需要以管理员权限运行，才能正常完成安装
4、安装服务：InstallUtil.exe 服务程序
5、卸载服务：InstallUtil.exe /u 服务程序
```