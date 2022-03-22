using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class IO
{
    [ExcelDna.Integration.ExcelFunction(Description = "获取当前时间")]
    public static string DateTimeNow()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
