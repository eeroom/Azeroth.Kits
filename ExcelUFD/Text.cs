using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Text
{
    public static string ToLower(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.ToLower();
    }

    public static string NewGuid(string ff)
    {
        return string.IsNullOrEmpty(ff) ? Guid.NewGuid().ToString() : Guid.NewGuid().ToString(ff);
    }
}
