using System;

namespace ProtoBuf
{
	// Token: 0x0200083A RID: 2106
	public static class BclHelpers
	{
		// Token: 0x06008218 RID: 33304 RVA: 0x000F8D3E File Offset: 0x000F6F3E
		public static object GetUninitializedObject(Type type)
		{
			throw new NotSupportedException("Constructor-skipping is not supported on this platform");
		}

		// Token: 0x06008219 RID: 33305 RVA: 0x000F8D4C File Offset: 0x000F6F4C
		public static void WriteTimeSpan(TimeSpan timeSpan, ProtoWriter dest)
		{
			bool flag = dest == null;
			if (flag)
			{
				throw new ArgumentNullException("dest");
			}
			WireType wireType = dest.WireType;
			if (wireType != WireType.Fixed64)
			{
				if (wireType - WireType.String > 1)
				{
					throw new ProtoException("Unexpected wire-type: " + dest.WireType.ToString());
				}
				long num = timeSpan.Ticks;
				bool flag2 = timeSpan == TimeSpan.MaxValue;
				TimeSpanScale timeSpanScale;
				if (flag2)
				{
					num = 1L;
					timeSpanScale = TimeSpanScale.MinMax;
				}
				else
				{
					bool flag3 = timeSpan == TimeSpan.MinValue;
					if (flag3)
					{
						num = -1L;
						timeSpanScale = TimeSpanScale.MinMax;
					}
					else
					{
						bool flag4 = num % 864000000000L == 0L;
						if (flag4)
						{
							timeSpanScale = TimeSpanScale.Days;
							num /= 864000000000L;
						}
						else
						{
							bool flag5 = num % 36000000000L == 0L;
							if (flag5)
							{
								timeSpanScale = TimeSpanScale.Hours;
								num /= 36000000000L;
							}
							else
							{
								bool flag6 = num % 600000000L == 0L;
								if (flag6)
								{
									timeSpanScale = TimeSpanScale.Minutes;
									num /= 600000000L;
								}
								else
								{
									bool flag7 = num % 10000000L == 0L;
									if (flag7)
									{
										timeSpanScale = TimeSpanScale.Seconds;
										num /= 10000000L;
									}
									else
									{
										bool flag8 = num % 10000L == 0L;
										if (flag8)
										{
											timeSpanScale = TimeSpanScale.Milliseconds;
											num /= 10000L;
										}
										else
										{
											timeSpanScale = TimeSpanScale.Ticks;
										}
									}
								}
							}
						}
					}
				}
				SubItemToken token = ProtoWriter.StartSubItem(null, dest);
				bool flag9 = num != 0L;
				if (flag9)
				{
					ProtoWriter.WriteFieldHeader(1, WireType.SignedVariant, dest);
					ProtoWriter.WriteInt64(num, dest);
				}
				bool flag10 = timeSpanScale > TimeSpanScale.Days;
				if (flag10)
				{
					ProtoWriter.WriteFieldHeader(2, WireType.Variant, dest);
					ProtoWriter.WriteInt32((int)timeSpanScale, dest);
				}
				ProtoWriter.EndSubItem(token, dest);
			}
			else
			{
				ProtoWriter.WriteInt64(timeSpan.Ticks, dest);
			}
		}

		// Token: 0x0600821A RID: 33306 RVA: 0x000F8F08 File Offset: 0x000F7108
		public static TimeSpan ReadTimeSpan(ProtoReader source)
		{
			long num = BclHelpers.ReadTimeSpanTicks(source);
			bool flag = num == long.MinValue;
			TimeSpan result;
			if (flag)
			{
				result = TimeSpan.MinValue;
			}
			else
			{
				bool flag2 = num == long.MaxValue;
				if (flag2)
				{
					result = TimeSpan.MaxValue;
				}
				else
				{
					result = TimeSpan.FromTicks(num);
				}
			}
			return result;
		}

		// Token: 0x0600821B RID: 33307 RVA: 0x000F8F58 File Offset: 0x000F7158
		public static DateTime ReadDateTime(ProtoReader source)
		{
			long num = BclHelpers.ReadTimeSpanTicks(source);
			bool flag = num == long.MinValue;
			DateTime result;
			if (flag)
			{
				result = DateTime.MinValue;
			}
			else
			{
				bool flag2 = num == long.MaxValue;
				if (flag2)
				{
					result = DateTime.MaxValue;
				}
				else
				{
					result = BclHelpers.EpochOrigin.AddTicks(num);
				}
			}
			return result;
		}

		// Token: 0x0600821C RID: 33308 RVA: 0x000F8FB0 File Offset: 0x000F71B0
		public static void WriteDateTime(DateTime value, ProtoWriter dest)
		{
			bool flag = dest == null;
			if (flag)
			{
				throw new ArgumentNullException("dest");
			}
			WireType wireType = dest.WireType;
			TimeSpan timeSpan;
			if (wireType - WireType.String > 1)
			{
				timeSpan = value - BclHelpers.EpochOrigin;
			}
			else
			{
				bool flag2 = value == DateTime.MaxValue;
				if (flag2)
				{
					timeSpan = TimeSpan.MaxValue;
				}
				else
				{
					bool flag3 = value == DateTime.MinValue;
					if (flag3)
					{
						timeSpan = TimeSpan.MinValue;
					}
					else
					{
						timeSpan = value - BclHelpers.EpochOrigin;
					}
				}
			}
			BclHelpers.WriteTimeSpan(timeSpan, dest);
		}

		// Token: 0x0600821D RID: 33309 RVA: 0x000F903C File Offset: 0x000F723C
		private static long ReadTimeSpanTicks(ProtoReader source)
		{
			WireType wireType = source.WireType;
			long result;
			if (wireType != WireType.Fixed64)
			{
				if (wireType - WireType.String > 1)
				{
					throw new ProtoException("Unexpected wire-type: " + source.WireType.ToString());
				}
				SubItemToken token = ProtoReader.StartSubItem(source);
				TimeSpanScale timeSpanScale = TimeSpanScale.Days;
				long num = 0L;
				int num2;
				while ((num2 = source.ReadFieldHeader()) > 0)
				{
					int num3 = num2;
					if (num3 != 1)
					{
						if (num3 != 2)
						{
							source.SkipField();
						}
						else
						{
							timeSpanScale = (TimeSpanScale)source.ReadInt32();
						}
					}
					else
					{
						source.Assert(WireType.SignedVariant);
						num = source.ReadInt64();
					}
				}
				ProtoReader.EndSubItem(token, source);
				TimeSpanScale timeSpanScale2 = timeSpanScale;
				switch (timeSpanScale2)
				{
				case TimeSpanScale.Days:
					result = num * 864000000000L;
					break;
				case TimeSpanScale.Hours:
					result = num * 36000000000L;
					break;
				case TimeSpanScale.Minutes:
					result = num * 600000000L;
					break;
				case TimeSpanScale.Seconds:
					result = num * 10000000L;
					break;
				case TimeSpanScale.Milliseconds:
					result = num * 10000L;
					break;
				case TimeSpanScale.Ticks:
					result = num;
					break;
				default:
				{
					if (timeSpanScale2 != TimeSpanScale.MinMax)
					{
						throw new ProtoException("Unknown timescale: " + timeSpanScale.ToString());
					}
					long num4 = num;
					if (num4 != -1L)
					{
						if (num4 != 1L)
						{
							throw new ProtoException("Unknown min/max value: " + num.ToString());
						}
						result = long.MaxValue;
					}
					else
					{
						result = long.MinValue;
					}
					break;
				}
				}
			}
			else
			{
				result = source.ReadInt64();
			}
			return result;
		}

		// Token: 0x0600821E RID: 33310 RVA: 0x000F91E8 File Offset: 0x000F73E8
		public static decimal ReadDecimal(ProtoReader reader)
		{
			ulong num = 0UL;
			uint num2 = 0U;
			uint num3 = 0U;
			SubItemToken token = ProtoReader.StartSubItem(reader);
			int num4;
			while ((num4 = reader.ReadFieldHeader()) > 0)
			{
				switch (num4)
				{
				case 1:
					num = reader.ReadUInt64();
					break;
				case 2:
					num2 = reader.ReadUInt32();
					break;
				case 3:
					num3 = reader.ReadUInt32();
					break;
				default:
					reader.SkipField();
					break;
				}
			}
			ProtoReader.EndSubItem(token, reader);
			bool flag = num == 0UL && num2 == 0U;
			decimal result;
			if (flag)
			{
				result = 0m;
			}
			else
			{
				int lo = (int)(num & 0XFFFFFFFFL);
				int mid = (int)(num >> 32 & 0XFFFFFFFFL);
				int hi = (int)num2;
				bool isNegative = (num3 & 1U) == 1U;
				byte scale = (byte)((num3 & 510U) >> 1);
				result = new decimal(lo, mid, hi, isNegative, scale);
			}
			return result;
		}

		// Token: 0x0600821F RID: 33311 RVA: 0x000F92B8 File Offset: 0x000F74B8
		public static void WriteDecimal(decimal value, ProtoWriter writer)
		{
			int[] bits = decimal.GetBits(value);
			ulong num = (ulong)((ulong)((long)bits[1]) << 32);
			ulong num2 = (ulong)((long)bits[0] & 0XFFFFFFFFL);
			ulong num3 = num | num2;
			uint num4 = (uint)bits[2];
			uint num5 = (uint)((bits[3] >> 15 & 510) | (bits[3] >> 31 & 1));
			SubItemToken token = ProtoWriter.StartSubItem(null, writer);
			bool flag = num3 > 0UL;
			if (flag)
			{
				ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
				ProtoWriter.WriteUInt64(num3, writer);
			}
			bool flag2 = num4 > 0U;
			if (flag2)
			{
				ProtoWriter.WriteFieldHeader(2, WireType.Variant, writer);
				ProtoWriter.WriteUInt32(num4, writer);
			}
			bool flag3 = num5 > 0U;
			if (flag3)
			{
				ProtoWriter.WriteFieldHeader(3, WireType.Variant, writer);
				ProtoWriter.WriteUInt32(num5, writer);
			}
			ProtoWriter.EndSubItem(token, writer);
		}

		// Token: 0x06008220 RID: 33312 RVA: 0x000F936C File Offset: 0x000F756C
		public static void WriteGuid(Guid value, ProtoWriter dest)
		{
			byte[] data = value.ToByteArray();
			SubItemToken token = ProtoWriter.StartSubItem(null, dest);
			bool flag = value != Guid.Empty;
			if (flag)
			{
				ProtoWriter.WriteFieldHeader(1, WireType.Fixed64, dest);
				ProtoWriter.WriteBytes(data, 0, 8, dest);
				ProtoWriter.WriteFieldHeader(2, WireType.Fixed64, dest);
				ProtoWriter.WriteBytes(data, 8, 8, dest);
			}
			ProtoWriter.EndSubItem(token, dest);
		}

		// Token: 0x06008221 RID: 33313 RVA: 0x000F93CC File Offset: 0x000F75CC
		public static Guid ReadGuid(ProtoReader source)
		{
			ulong num = 0UL;
			ulong num2 = 0UL;
			SubItemToken token = ProtoReader.StartSubItem(source);
			int num3;
			while ((num3 = source.ReadFieldHeader()) > 0)
			{
				int num4 = num3;
				if (num4 != 1)
				{
					if (num4 != 2)
					{
						source.SkipField();
					}
					else
					{
						num2 = source.ReadUInt64();
					}
				}
				else
				{
					num = source.ReadUInt64();
				}
			}
			ProtoReader.EndSubItem(token, source);
			bool flag = num == 0UL && num2 == 0UL;
			Guid result;
			if (flag)
			{
				result = Guid.Empty;
			}
			else
			{
				uint num5 = (uint)(num >> 32);
				uint a = (uint)num;
				uint num6 = (uint)(num2 >> 32);
				uint num7 = (uint)num2;
				result = new Guid((int)a, (short)num5, (short)(num5 >> 16), (byte)num7, (byte)(num7 >> 8), (byte)(num7 >> 16), (byte)(num7 >> 24), (byte)num6, (byte)(num6 >> 8), (byte)(num6 >> 16), (byte)(num6 >> 24));
			}
			return result;
		}

		// Token: 0x06008222 RID: 33314 RVA: 0x000F94A0 File Offset: 0x000F76A0
		public static object ReadNetObject(object value, ProtoReader source, int key, Type type, BclHelpers.NetObjectOptions options)
		{
			SubItemToken token = ProtoReader.StartSubItem(source);
			int num = -1;
			int num2 = -1;
			int num3;
			while ((num3 = source.ReadFieldHeader()) > 0)
			{
				switch (num3)
				{
				case 1:
				{
					int key2 = source.ReadInt32();
					value = source.NetCache.GetKeyedObject(key2);
					break;
				}
				case 2:
					num = source.ReadInt32();
					break;
				case 3:
				{
					int key2 = source.ReadInt32();
					type = (Type)source.NetCache.GetKeyedObject(key2);
					key = source.GetTypeKey(ref type);
					break;
				}
				case 4:
					num2 = source.ReadInt32();
					break;
				case 5:
				case 6:
				case 7:
				case 9:
					goto IL_27E;
				case 8:
				{
					string text = source.ReadString();
					type = source.DeserializeType(text);
					bool flag = type == null;
					if (flag)
					{
						throw new ProtoException("Unable to resolve type: " + text + " (you can use the TypeModel.DynamicTypeFormatting event to provide a custom mapping)");
					}
					bool flag2 = type == typeof(string);
					if (flag2)
					{
						key = -1;
					}
					else
					{
						key = source.GetTypeKey(ref type);
						bool flag3 = key < 0;
						if (flag3)
						{
							throw new InvalidOperationException("Dynamic type is not a contract-type: " + type.Name);
						}
					}
					break;
				}
				case 10:
				{
					bool flag4 = type == typeof(string);
					bool flag5 = value == null;
					bool flag6 = flag5 && (flag4 || (options & BclHelpers.NetObjectOptions.LateSet) > BclHelpers.NetObjectOptions.None);
					bool flag7 = num >= 0 && !flag6;
					if (flag7)
					{
						bool flag8 = value == null;
						if (flag8)
						{
							source.TrapNextObject(num);
						}
						else
						{
							source.NetCache.SetKeyedObject(num, value);
						}
						bool flag9 = num2 >= 0;
						if (flag9)
						{
							source.NetCache.SetKeyedObject(num2, type);
						}
					}
					object obj = value;
					bool flag10 = flag4;
					if (flag10)
					{
						value = source.ReadString();
					}
					else
					{
						value = ProtoReader.ReadTypedObject(obj, key, source, type);
					}
					bool flag11 = num >= 0;
					if (flag11)
					{
						bool flag12 = flag5 && !flag6;
						if (flag12)
						{
							obj = source.NetCache.GetKeyedObject(num);
						}
						bool flag13 = flag6;
						if (flag13)
						{
							source.NetCache.SetKeyedObject(num, value);
							bool flag14 = num2 >= 0;
							if (flag14)
							{
								source.NetCache.SetKeyedObject(num2, type);
							}
						}
					}
					bool flag15 = num >= 0 && !flag6 && obj != value;
					if (flag15)
					{
						throw new ProtoException("A reference-tracked object changed reference during deserialization");
					}
					bool flag16 = num < 0 && num2 >= 0;
					if (flag16)
					{
						source.NetCache.SetKeyedObject(num2, type);
					}
					break;
				}
				default:
					goto IL_27E;
				}
				continue;
				IL_27E:
				source.SkipField();
			}
			bool flag17 = num >= 0 && (options & BclHelpers.NetObjectOptions.AsReference) == BclHelpers.NetObjectOptions.None;
			if (flag17)
			{
				throw new ProtoException("Object key in input stream, but reference-tracking was not expected");
			}
			ProtoReader.EndSubItem(token, source);
			return value;
		}

		// Token: 0x06008223 RID: 33315 RVA: 0x000F9778 File Offset: 0x000F7978
		public static void WriteNetObject(object value, ProtoWriter dest, int key, BclHelpers.NetObjectOptions options)
		{
			bool flag = dest == null;
			if (flag)
			{
				throw new ArgumentNullException("dest");
			}
			bool flag2 = (options & BclHelpers.NetObjectOptions.DynamicType) > BclHelpers.NetObjectOptions.None;
			bool flag3 = (options & BclHelpers.NetObjectOptions.AsReference) > BclHelpers.NetObjectOptions.None;
			WireType wireType = dest.WireType;
			SubItemToken token = ProtoWriter.StartSubItem(null, dest);
			bool flag4 = true;
			bool flag5 = flag3;
			if (flag5)
			{
				bool flag6;
				int value2 = dest.NetCache.AddObjectKey(value, out flag6);
				ProtoWriter.WriteFieldHeader(flag6 ? 1 : 2, WireType.Variant, dest);
				ProtoWriter.WriteInt32(value2, dest);
				bool flag7 = flag6;
				if (flag7)
				{
					flag4 = false;
				}
			}
			bool flag8 = flag4;
			if (flag8)
			{
				bool flag9 = flag2;
				if (flag9)
				{
					Type type = value.GetType();
					bool flag10 = !(value is string);
					if (flag10)
					{
						key = dest.GetTypeKey(ref type);
						bool flag11 = key < 0;
						if (flag11)
						{
							throw new InvalidOperationException("Dynamic type is not a contract-type: " + type.Name);
						}
					}
					bool flag12;
					int value3 = dest.NetCache.AddObjectKey(type, out flag12);
					ProtoWriter.WriteFieldHeader(flag12 ? 3 : 4, WireType.Variant, dest);
					ProtoWriter.WriteInt32(value3, dest);
					bool flag13 = !flag12;
					if (flag13)
					{
						ProtoWriter.WriteFieldHeader(8, WireType.String, dest);
						ProtoWriter.WriteString(dest.SerializeType(type), dest);
					}
				}
				ProtoWriter.WriteFieldHeader(10, wireType, dest);
				bool flag14 = value is string;
				if (flag14)
				{
					ProtoWriter.WriteString((string)value, dest);
				}
				else
				{
					ProtoWriter.WriteObject(value, key, dest);
				}
			}
			ProtoWriter.EndSubItem(token, dest);
		}

		// Token: 0x04002838 RID: 10296
		private const int FieldTimeSpanValue = 1;

		// Token: 0x04002839 RID: 10297
		private const int FieldTimeSpanScale = 2;

		// Token: 0x0400283A RID: 10298
		internal static readonly DateTime EpochOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		// Token: 0x0400283B RID: 10299
		private const int FieldDecimalLow = 1;

		// Token: 0x0400283C RID: 10300
		private const int FieldDecimalHigh = 2;

		// Token: 0x0400283D RID: 10301
		private const int FieldDecimalSignScale = 3;

		// Token: 0x0400283E RID: 10302
		private const int FieldGuidLow = 1;

		// Token: 0x0400283F RID: 10303
		private const int FieldGuidHigh = 2;

		// Token: 0x04002840 RID: 10304
		private const int FieldExistingObjectKey = 1;

		// Token: 0x04002841 RID: 10305
		private const int FieldNewObjectKey = 2;

		// Token: 0x04002842 RID: 10306
		private const int FieldExistingTypeKey = 3;

		// Token: 0x04002843 RID: 10307
		private const int FieldNewTypeKey = 4;

		// Token: 0x04002844 RID: 10308
		private const int FieldTypeName = 8;

		// Token: 0x04002845 RID: 10309
		private const int FieldObject = 10;

		// Token: 0x0200193A RID: 6458
		[Flags]
		public enum NetObjectOptions : byte
		{
			// Token: 0x04007D37 RID: 32055
			None = 0,
			// Token: 0x04007D38 RID: 32056
			AsReference = 1,
			// Token: 0x04007D39 RID: 32057
			DynamicType = 2,
			// Token: 0x04007D3A RID: 32058
			UseConstructor = 4,
			// Token: 0x04007D3B RID: 32059
			LateSet = 8
		}
	}
}
