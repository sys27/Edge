using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Edge
{

    internal static class CastHelper
    {

        internal static T Cast<T>(object obj)
        {
            return (T)obj;
        }

        internal static void CheckCast(object obj, Type type)
        {
            MethodInfo castMethod = typeof(CastHelper).GetMethod("Cast", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type);
            object castedObject = castMethod.Invoke(null, new object[] { obj });
        }

    }

}
