## 本机互操作性
```
P/Invoke
	场景：c#调用c/c++函数库
	特别说明：CLR公共语言运行库中提供的调用本机代码函数库的办法称为P/Invoke
	平台：windows和linux,.net framework和mono都实现了P/Invoke,提供了配套的关键字和特性类
	本机调试：可以在c#和c代码之间无缝衔接，设置方法：项目属性->>调试->>启用本机调试
	c/c++函数转c#方法签名的参考资料：https://www.pinvoke.net/
cppsharper 
	场景：c#调用c/c++函数库
	开源项目https://github.com/mono/CppSharp,包装c/c++函数库给c#调用
COM互操作
	.net framework提供了配套的特性类和工具，.net调用
```

## Excel自定义函数（基于.net程序集）
```
创建普通类库项目
	修改程序集元数据信息：[assembly: ComVisible(true)]
	需要导出的class增加相关特性
注册程序集为com组件，因为excel严格区分32、64位版本，，只能加载对应版本注册的com组件
	64位:	C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe 程序集完整路径 /codebase
	32位:C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe 程序集完整路径 /codebase
加载com组件
	启动Excel，菜单：开发工具->>加载项->>自动化->>导出的class
```
## Excel自定义函数（基于dna框架）
```
本质是走office提供的c++扩展接口，利用互操作，c++扩展接口→dna提供的xll适配器→.net程序集
具体参看项目中的README.md文件
```
## Excel自定义函数（基于vba）
```
vba语法
```

## WindowsBase.dll类库
```
 System.Windows.Interop.MSG ：对应win32的窗体消息数据结构
```

## System.Windows.Forms.dll类库
```
System.Windows.Forms.UnsafeNativeMethods：非公开的类，但是包含大量win32的方法，需要调用kernel32,user32等里面的方法可以来这里面参考
```
