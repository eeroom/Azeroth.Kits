# 调试到vclib代码
```
启用本机调试，调试的时候可以在c#和c代码之间无缝衔接
设置方法：项目属性》调试》启用本机调试
```

# WindowsBase.dll类库
```
 System.Windows.Interop.MSG  这个类型对应win32的窗体消息数据结构
```

# System.Windows.Forms.dll类库
```
System.Windows.Forms.UnsafeNativeMethods 这个类型里面有大量对应win32的方法，但是这个类不是public,需要反编译工具查看，
需要调用kernel32,user32等等里面的方法的时候可以来这里面参考
```