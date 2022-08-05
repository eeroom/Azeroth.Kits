## VSTO
```
excel的UDF（用户自定义函数）,excel插件,outlook插件等
```
## 本机互操作性
```
P/Invoke(平台调用)
	使用场景：c#调用c/c++函数库
	特别说明：CLR公共语言运行库中提供的调用本机代码函数库的办法称为P/Invoke
	平台：windows和linux,.net framework和mono都实现了P/Invoke,提供了配套的关键字和特性类
	c/c++函数转c#方法签名的参考资料：https://www.pinvoke.net/
cppsharper 
	场景：c#调用c/c++函数库
	开源项目https://github.com/mono/CppSharp,包装c/c++函数库给c#调用
COM互操作 .net framework提供了配套的特性类和工具，.net调用

注册.net程序集为COM组件	.net framework提供了配套的特性类和注册工具，使用场景（excel的自定义函数）


```
