using System;
using System.Collections.Generic;
using KontinuityCRM.Helpers;

namespace KontinuityCRM.Tests.Models
{
    public abstract class EnumTests
    {
        protected static List<string> Names<TEnum>() where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return EnumHelper<TEnum>.Names();
        }
    }
}