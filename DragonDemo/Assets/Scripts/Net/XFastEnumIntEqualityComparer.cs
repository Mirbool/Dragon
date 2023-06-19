using System;
using System.Collections.Generic;

namespace XUtliPoolLib
{
	// Token: 0x020001AB RID: 427
	public struct XFastEnumIntEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct
	{
		// Token: 0x06000962 RID: 2402 RVA: 0x00030E40 File Offset: 0x0002F040
		public static int ToInt(TEnum en)
		{
            return  EnumInt32ToInt.Convert1<TEnum>(en);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x00030E58 File Offset: 0x0002F058
		public bool Equals(TEnum lhs, TEnum rhs)
		{
			return XFastEnumIntEqualityComparer<TEnum>.ToInt(lhs) == XFastEnumIntEqualityComparer<TEnum>.ToInt(rhs);
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x00030E78 File Offset: 0x0002F078
		public int GetHashCode(TEnum en)
		{
			return XFastEnumIntEqualityComparer<TEnum>.ToInt(en);
		}
	}

    internal class EnumInt32ToInt
    {
        public static int Convert1<TEnum>(TEnum value) where TEnum : struct
        {
            return Convert.ToInt32(value);
        }

    }
}
