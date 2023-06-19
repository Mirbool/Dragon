using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	// Token: 0x02000855 RID: 2133
	public sealed class ProtoReader : IDisposable
	{
		// Token: 0x1700295E RID: 10590
		// (get) Token: 0x060082BB RID: 33467 RVA: 0x000FB048 File Offset: 0x000F9248
		public int FieldNumber
		{
			get
			{
				return this.fieldNumber;
			}
		}

		// Token: 0x1700295F RID: 10591
		// (get) Token: 0x060082BC RID: 33468 RVA: 0x000FB060 File Offset: 0x000F9260
		public WireType WireType
		{
			get
			{
				return this.wireType;
			}
		}

		// Token: 0x060082BD RID: 33469 RVA: 0x000FB078 File Offset: 0x000F9278
		public ProtoReader(Stream source, TypeModel model, SerializationContext context)
		{
			ProtoReader.Init(this, source, model, context, -1);
		}

		// Token: 0x17002960 RID: 10592
		// (get) Token: 0x060082BE RID: 33470 RVA: 0x000FB090 File Offset: 0x000F9290
		// (set) Token: 0x060082BF RID: 33471 RVA: 0x000FB0A8 File Offset: 0x000F92A8
		public bool InternStrings
		{
			get
			{
				return this.internStrings;
			}
			set
			{
				this.internStrings = value;
			}
		}

		// Token: 0x060082C0 RID: 33472 RVA: 0x000FB0B2 File Offset: 0x000F92B2
		public ProtoReader(Stream source, TypeModel model, SerializationContext context, int length)
		{
			ProtoReader.Init(this, source, model, context, length);
		}

		// Token: 0x060082C1 RID: 33473 RVA: 0x000FB0C8 File Offset: 0x000F92C8
		private static void Init(ProtoReader reader, Stream source, TypeModel model, SerializationContext context, int length)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = !source.CanRead;
			if (flag2)
			{
				throw new ArgumentException("Cannot read from stream", "source");
			}
			reader.source = source;
			reader.ioBuffer = BufferPool.GetBuffer();
			reader.model = model;
			bool flag3 = length >= 0;
			reader.isFixedLength = flag3;
			reader.dataRemaining = (flag3 ? length : 0);
			bool flag4 = context == null;
			if (flag4)
			{
				context = SerializationContext.Default;
			}
			else
			{
				context.Freeze();
			}
			reader.context = context;
			reader.position = (reader.available = (reader.depth = (reader.fieldNumber = (reader.ioIndex = 0))));
			reader.blockEnd = int.MaxValue;
			reader.internStrings = true;
			reader.wireType = WireType.None;
			reader.trapCount = 1U;
			bool flag5 = reader.netCache == null;
			if (flag5)
			{
				reader.netCache = new NetObjectCache();
			}
		}

		// Token: 0x17002961 RID: 10593
		// (get) Token: 0x060082C2 RID: 33474 RVA: 0x000FB1D0 File Offset: 0x000F93D0
		public SerializationContext Context
		{
			get
			{
				return this.context;
			}
		}

		// Token: 0x060082C3 RID: 33475 RVA: 0x000FB1E8 File Offset: 0x000F93E8
		public void Dispose()
		{
			this.source = null;
			this.model = null;
			BufferPool.ReleaseBufferToPool(ref this.ioBuffer);
			bool flag = this.stringInterner != null;
			if (flag)
			{
				this.stringInterner.Clear();
			}
			bool flag2 = this.netCache != null;
			if (flag2)
			{
				this.netCache.Clear();
			}
		}

		// Token: 0x060082C4 RID: 33476 RVA: 0x000FB244 File Offset: 0x000F9444
		internal int TryReadUInt32VariantWithoutMoving(bool trimNegative, out uint value)
		{
			bool flag = this.available < 10;
			if (flag)
			{
				this.Ensure(10, false);
			}
			bool flag2 = this.available == 0;
			int result;
			if (flag2)
			{
				value = 0U;
				result = 0;
			}
			else
			{
				int num = this.ioIndex;
				value = (uint)this.ioBuffer[num++];
				bool flag3 = (value & 128U) == 0U;
				if (flag3)
				{
					result = 1;
				}
				else
				{
					value &= 127U;
					bool flag4 = this.available == 1;
					if (flag4)
					{
						throw ProtoReader.EoF(this);
					}
					uint num2 = (uint)this.ioBuffer[num++];
					value |= (num2 & 127U) << 7;
					bool flag5 = (num2 & 128U) == 0U;
					if (flag5)
					{
						result = 2;
					}
					else
					{
						bool flag6 = this.available == 2;
						if (flag6)
						{
							throw ProtoReader.EoF(this);
						}
						num2 = (uint)this.ioBuffer[num++];
						value |= (num2 & 127U) << 14;
						bool flag7 = (num2 & 128U) == 0U;
						if (flag7)
						{
							result = 3;
						}
						else
						{
							bool flag8 = this.available == 3;
							if (flag8)
							{
								throw ProtoReader.EoF(this);
							}
							num2 = (uint)this.ioBuffer[num++];
							value |= (num2 & 127U) << 21;
							bool flag9 = (num2 & 128U) == 0U;
							if (flag9)
							{
								result = 4;
							}
							else
							{
								bool flag10 = this.available == 4;
								if (flag10)
								{
									throw ProtoReader.EoF(this);
								}
								num2 = (uint)this.ioBuffer[num];
								value |= num2 << 28;
								bool flag11 = (num2 & 240U) == 0U;
								if (flag11)
								{
									result = 5;
								}
								else
								{
									bool flag12 = trimNegative && (num2 & 240U) == 240U && this.available >= 10 && this.ioBuffer[++num] == byte.MaxValue && this.ioBuffer[++num] == byte.MaxValue && this.ioBuffer[++num] == byte.MaxValue && this.ioBuffer[++num] == byte.MaxValue && this.ioBuffer[num + 1] == 1;
									if (!flag12)
									{
										throw ProtoReader.AddErrorData(new OverflowException(), this);
									}
									result = 10;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060082C5 RID: 33477 RVA: 0x000FB460 File Offset: 0x000F9660
		private uint ReadUInt32Variant(bool trimNegative)
		{
			uint result;
			int num = this.TryReadUInt32VariantWithoutMoving(trimNegative, out result);
			bool flag = num > 0;
			if (flag)
			{
				this.ioIndex += num;
				this.available -= num;
				this.position += num;
				return result;
			}
			throw ProtoReader.EoF(this);
		}

		// Token: 0x060082C6 RID: 33478 RVA: 0x000FB4B8 File Offset: 0x000F96B8
		private bool TryReadUInt32Variant(out uint value)
		{
			int num = this.TryReadUInt32VariantWithoutMoving(false, out value);
			bool flag = num > 0;
			bool result;
			if (flag)
			{
				this.ioIndex += num;
				this.available -= num;
				this.position += num;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

        // Token: 0x060082C7 RID: 33479 RVA: 0x000FB50C File Offset: 0x000F970C
        public uint ReadUInt32()
        {
            switch (this.wireType)
            {
                case WireType.Variant:
                    return this.ReadUInt32Variant(false);

                case WireType.Fixed64:
                    {
                        ulong num = this.ReadUInt64();
                        return (uint)num;
                    }
                case WireType.Fixed32:
                    {
                        if (this.available < 4)
                        {
                            this.Ensure(4, true);
                        }
                        this.position += 4;
                        this.available -= 4;
                        int ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        return (uint)(((this.ioBuffer[ioIndex] | (this.ioBuffer[ioIndex] << 8)) | (this.ioBuffer[ioIndex] << 0x10)) | (this.ioBuffer[ioIndex] << 0x18));
                    }
            }
            throw this.CreateWireTypeException();
        }



        // Token: 0x17002962 RID: 10594
        // (get) Token: 0x060082C8 RID: 33480 RVA: 0x000FB608 File Offset: 0x000F9808
        public int Position
		{
			get
			{
				return this.position;
			}
		}

		// Token: 0x060082C9 RID: 33481 RVA: 0x000FB620 File Offset: 0x000F9820
		internal void Ensure(int count, bool strict)
		{
			Helpers.DebugAssert(this.available <= count, "Asking for data without checking first");
			bool flag = count > this.ioBuffer.Length;
			if (flag)
			{
				BufferPool.ResizeAndFlushLeft(ref this.ioBuffer, count, this.ioIndex, this.available);
				this.ioIndex = 0;
			}
			else
			{
				bool flag2 = this.ioIndex + count >= this.ioBuffer.Length;
				if (flag2)
				{
					Helpers.BlockCopy(this.ioBuffer, this.ioIndex, this.ioBuffer, 0, this.available);
					this.ioIndex = 0;
				}
			}
			count -= this.available;
			int num = this.ioIndex + this.available;
			int num2 = this.ioBuffer.Length - num;
			bool flag3 = this.isFixedLength;
			if (flag3)
			{
				bool flag4 = this.dataRemaining < num2;
				if (flag4)
				{
					num2 = this.dataRemaining;
				}
			}
			int num3;
			while (count > 0 && num2 > 0 && (num3 = this.source.Read(this.ioBuffer, num, num2)) > 0)
			{
				this.available += num3;
				count -= num3;
				num2 -= num3;
				num += num3;
				bool flag5 = this.isFixedLength;
				if (flag5)
				{
					this.dataRemaining -= num3;
				}
			}
			bool flag6 = strict && count > 0;
			if (flag6)
			{
				throw ProtoReader.EoF(this);
			}
		}

		// Token: 0x060082CA RID: 33482 RVA: 0x000FB780 File Offset: 0x000F9980
		public short ReadInt16()
		{
			return checked((short)this.ReadInt32());
		}

		// Token: 0x060082CB RID: 33483 RVA: 0x000FB79C File Offset: 0x000F999C
		public ushort ReadUInt16()
		{
			return checked((ushort)this.ReadUInt32());
		}

		// Token: 0x060082CC RID: 33484 RVA: 0x000FB7B8 File Offset: 0x000F99B8
		public byte ReadByte()
		{
			return checked((byte)this.ReadUInt32());
		}

		// Token: 0x060082CD RID: 33485 RVA: 0x000FB7D4 File Offset: 0x000F99D4
		public sbyte ReadSByte()
		{
			return checked((sbyte)this.ReadInt32());
		}

		// Token: 0x060082CE RID: 33486 RVA: 0x000FB7F0 File Offset: 0x000F99F0
		public int ReadInt32()
		{
			WireType wireType = this.wireType;
			if (wireType <= WireType.Fixed64)
			{
				if (wireType == WireType.Variant)
				{
					return (int)this.ReadUInt32Variant(true);
				}
				if (wireType == WireType.Fixed64)
				{
					long num = this.ReadInt64();
					return checked((int)num);
				}
			}
			else
			{
				if (wireType == WireType.Fixed32)
				{
					bool flag = this.available < 4;
					if (flag)
					{
						this.Ensure(4, true);
					}
					this.position += 4;
					this.available -= 4;
					byte[] array = this.ioBuffer;
					int num2 = this.ioIndex;
					this.ioIndex = num2 + 1;
					int num3 = array[num2];
					byte[] array2 = this.ioBuffer;
					num2 = this.ioIndex;
					this.ioIndex = num2 + 1;
					int num4 = num3 | array2[num2] << 8;
					byte[] array3 = this.ioBuffer;
					num2 = this.ioIndex;
					this.ioIndex = num2 + 1;
					int num5 = num4 | array3[num2] << 16;
					byte[] array4 = this.ioBuffer;
					num2 = this.ioIndex;
					this.ioIndex = num2 + 1;
					return num5 | array4[num2] << 24;
				}
				if (wireType == WireType.SignedVariant)
				{
					return ProtoReader.Zag(this.ReadUInt32Variant(true));
				}
			}
			throw this.CreateWireTypeException();
		}

		// Token: 0x060082CF RID: 33487 RVA: 0x000FB90C File Offset: 0x000F9B0C
		private static int Zag(uint ziggedValue)
		{
			return (int)(-(ziggedValue & 1U) ^ (uint)((int)ziggedValue >> 1 & int.MaxValue));
		}

		// Token: 0x060082D0 RID: 33488 RVA: 0x000FB930 File Offset: 0x000F9B30
		private static long Zag(ulong ziggedValue)
		{
            long num = (long)ziggedValue;
            return (-(num & 1L) ^ ((num >> 1) & 0x7fffffffffffffffL));
        }

		// Token: 0x060082D1 RID: 33489 RVA: 0x000FB958 File Offset: 0x000F9B58
		public long ReadInt64()
		{
			WireType wireType = this.wireType;
			if (wireType <= WireType.Fixed64)
			{
				if (wireType == WireType.Variant)
				{
					return (long)this.ReadUInt64Variant();
				}
				if (wireType == WireType.Fixed64)
				{
					bool flag = this.available < 8;
					if (flag)
					{
						this.Ensure(8, true);
					}
					this.position += 8;
					this.available -= 8;
					byte[] array = this.ioBuffer;
					int num = this.ioIndex;
					this.ioIndex = num + 1;
					long num2 = (long)array[num];
					byte[] array2 = this.ioBuffer;
					num = this.ioIndex;
					this.ioIndex = num + 1;
					long num3 = num2 | (long)((long)array2[num] << 8);
					byte[] array3 = this.ioBuffer;
					num = this.ioIndex;
					this.ioIndex = num + 1;
					long num4 = num3 | (long)((long)array3[num] << 16);
					byte[] array4 = this.ioBuffer;
					num = this.ioIndex;
					this.ioIndex = num + 1;
					long num5 = num4 | (long)((long)array4[num] << 24);
					byte[] array5 = this.ioBuffer;
					num = this.ioIndex;
					this.ioIndex = num + 1;
					long num6 = num5 | (long)((long)array5[num] << 32);
					byte[] array6 = this.ioBuffer;
					num = this.ioIndex;
					this.ioIndex = num + 1;
					long num7 = num6 | (long)((long)array6[num] << 40);
					byte[] array7 = this.ioBuffer;
					num = this.ioIndex;
					this.ioIndex = num + 1;
					long num8 = num7 | (long)((long)array7[num] << 48);
					byte[] array8 = this.ioBuffer;
					num = this.ioIndex;
					this.ioIndex = num + 1;
					return num8 | (long)((long)array8[num] << 56);
				}
			}
			else
			{
				if (wireType == WireType.Fixed32)
				{
					return (long)this.ReadInt32();
				}
				if (wireType == WireType.SignedVariant)
				{
					return ProtoReader.Zag(this.ReadUInt64Variant());
				}
			}
			throw this.CreateWireTypeException();
		}

		// Token: 0x060082D2 RID: 33490 RVA: 0x000FBAD8 File Offset: 0x000F9CD8
		private int TryReadUInt64VariantWithoutMoving(out ulong value)
		{
			bool flag = this.available < 10;
			if (flag)
			{
				this.Ensure(10, false);
			}
			bool flag2 = this.available == 0;
			int result;
			if (flag2)
			{
				value = 0UL;
				result = 0;
			}
			else
			{
				int num = this.ioIndex;
				value = (ulong)this.ioBuffer[num++];
				bool flag3 = (value & 128UL) == 0UL;
				if (flag3)
				{
					result = 1;
				}
				else
				{
					value &= 127UL;
					bool flag4 = this.available == 1;
					if (flag4)
					{
						throw ProtoReader.EoF(this);
					}
					ulong num2 = (ulong)this.ioBuffer[num++];
					value |= (num2 & 127UL) << 7;
					bool flag5 = (num2 & 128UL) == 0UL;
					if (flag5)
					{
						result = 2;
					}
					else
					{
						bool flag6 = this.available == 2;
						if (flag6)
						{
							throw ProtoReader.EoF(this);
						}
						num2 = (ulong)this.ioBuffer[num++];
						value |= (num2 & 127UL) << 14;
						bool flag7 = (num2 & 128UL) == 0UL;
						if (flag7)
						{
							result = 3;
						}
						else
						{
							bool flag8 = this.available == 3;
							if (flag8)
							{
								throw ProtoReader.EoF(this);
							}
							num2 = (ulong)this.ioBuffer[num++];
							value |= (num2 & 127UL) << 21;
							bool flag9 = (num2 & 128UL) == 0UL;
							if (flag9)
							{
								result = 4;
							}
							else
							{
								bool flag10 = this.available == 4;
								if (flag10)
								{
									throw ProtoReader.EoF(this);
								}
								num2 = (ulong)this.ioBuffer[num++];
								value |= (num2 & 127UL) << 28;
								bool flag11 = (num2 & 128UL) == 0UL;
								if (flag11)
								{
									result = 5;
								}
								else
								{
									bool flag12 = this.available == 5;
									if (flag12)
									{
										throw ProtoReader.EoF(this);
									}
									num2 = (ulong)this.ioBuffer[num++];
									value |= (num2 & 127UL) << 35;
									bool flag13 = (num2 & 128UL) == 0UL;
									if (flag13)
									{
										result = 6;
									}
									else
									{
										bool flag14 = this.available == 6;
										if (flag14)
										{
											throw ProtoReader.EoF(this);
										}
										num2 = (ulong)this.ioBuffer[num++];
										value |= (num2 & 127UL) << 42;
										bool flag15 = (num2 & 128UL) == 0UL;
										if (flag15)
										{
											result = 7;
										}
										else
										{
											bool flag16 = this.available == 7;
											if (flag16)
											{
												throw ProtoReader.EoF(this);
											}
											num2 = (ulong)this.ioBuffer[num++];
											value |= (num2 & 127UL) << 49;
											bool flag17 = (num2 & 128UL) == 0UL;
											if (flag17)
											{
												result = 8;
											}
											else
											{
												bool flag18 = this.available == 8;
												if (flag18)
												{
													throw ProtoReader.EoF(this);
												}
												num2 = (ulong)this.ioBuffer[num++];
												value |= (num2 & 127UL) << 56;
												bool flag19 = (num2 & 128UL) == 0UL;
												if (flag19)
												{
													result = 9;
												}
												else
												{
													bool flag20 = this.available == 9;
													if (flag20)
													{
														throw ProtoReader.EoF(this);
													}
													num2 = (ulong)this.ioBuffer[num];
													value |= num2 << 63;
													bool flag21 = (num2 & 18446744073709551614UL) > 0UL;
													if (flag21)
													{
														throw ProtoReader.AddErrorData(new OverflowException(), this);
													}
													result = 10;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060082D3 RID: 33491 RVA: 0x000FBDF4 File Offset: 0x000F9FF4
		private ulong ReadUInt64Variant()
		{
			ulong result;
			int num = this.TryReadUInt64VariantWithoutMoving(out result);
			bool flag = num > 0;
			if (flag)
			{
				this.ioIndex += num;
				this.available -= num;
				this.position += num;
				return result;
			}
			throw ProtoReader.EoF(this);
		}

		// Token: 0x060082D4 RID: 33492 RVA: 0x000FBE4C File Offset: 0x000FA04C
		private string Intern(string value)
		{
			bool flag = value == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = value.Length == 0;
				if (flag2)
				{
					result = "";
				}
				else
				{
					bool flag3 = this.stringInterner == null;
					if (flag3)
					{
						this.stringInterner = new Dictionary<string, string>();
						this.stringInterner.Add(value, value);
					}
					else
					{
						string text;
						bool flag4 = this.stringInterner.TryGetValue(value, out text);
						if (flag4)
						{
							value = text;
						}
						else
						{
							this.stringInterner.Add(value, value);
						}
					}
					result = value;
				}
			}
			return result;
		}

		// Token: 0x060082D5 RID: 33493 RVA: 0x000FBED8 File Offset: 0x000FA0D8
		public string ReadString()
		{
			bool flag = this.wireType == WireType.String;
			if (flag)
			{
				int num = (int)this.ReadUInt32Variant(false);
				bool flag2 = num == 0;
				string result;
				if (flag2)
				{
					result = "";
				}
				else
				{
					bool flag3 = this.available < num;
					if (flag3)
					{
						this.Ensure(num, true);
					}
					string text = ProtoReader.encoding.GetString(this.ioBuffer, this.ioIndex, num);
					bool flag4 = this.internStrings;
					if (flag4)
					{
						text = this.Intern(text);
					}
					this.available -= num;
					this.position += num;
					this.ioIndex += num;
					result = text;
				}
				return result;
			}
			throw this.CreateWireTypeException();
		}

		// Token: 0x060082D6 RID: 33494 RVA: 0x000FBF90 File Offset: 0x000FA190
		public void ThrowEnumException(Type type, int value)
		{
			string str = (type == null) ? "<null>" : type.FullName;
			throw ProtoReader.AddErrorData(new ProtoException("No " + str + " enum is mapped to the wire-value " + value.ToString()), this);
		}

		// Token: 0x060082D7 RID: 33495 RVA: 0x000FBFD4 File Offset: 0x000FA1D4
		private Exception CreateWireTypeException()
		{
			return this.CreateException("Invalid wire-type; this usually means you have over-written a file without truncating or setting the length; see http://stackoverflow.com/q/2152978/23354");
		}

		// Token: 0x060082D8 RID: 33496 RVA: 0x000FBFF4 File Offset: 0x000FA1F4
		private Exception CreateException(string message)
		{
			return ProtoReader.AddErrorData(new ProtoException(message), this);
		}

		// Token: 0x060082D9 RID: 33497 RVA: 0x000FC014 File Offset: 0x000FA214
		public unsafe double ReadDouble()
		{
			WireType wireType = this.wireType;
			double result;
			if (wireType != WireType.Fixed64)
			{
				if (wireType != WireType.Fixed32)
				{
					throw this.CreateWireTypeException();
				}
				result = (double)this.ReadSingle();
			}
			else
			{
				long num = this.ReadInt64();
				result = *(double*)(&num);
			}
			return result;
		}

		// Token: 0x060082DA RID: 33498 RVA: 0x000FC058 File Offset: 0x000FA258
		public static object ReadObject(object value, int key, ProtoReader reader)
		{
			return ProtoReader.ReadTypedObject(value, key, reader, null);
		}

		// Token: 0x060082DB RID: 33499 RVA: 0x000FC074 File Offset: 0x000FA274
		internal static object ReadTypedObject(object value, int key, ProtoReader reader, Type type)
		{
			bool flag = reader.model == null;
			if (flag)
			{
				throw ProtoReader.AddErrorData(new InvalidOperationException("Cannot deserialize sub-objects unless a model is provided"), reader);
			}
			SubItemToken token = ProtoReader.StartSubItem(reader);
			bool flag2 = key >= 0;
			if (flag2)
			{
				value = reader.model.Deserialize(key, value, reader);
			}
			else
			{
				bool flag3 = type != null && reader.model.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, true, false);
				if (!flag3)
				{
					TypeModel.ThrowUnexpectedType(type);
				}
			}
			ProtoReader.EndSubItem(token, reader);
			return value;
		}

		// Token: 0x060082DC RID: 33500 RVA: 0x000FC104 File Offset: 0x000FA304
		public static void EndSubItem(SubItemToken token, ProtoReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			int value = token.value;
			WireType wireType = reader.wireType;
			if (wireType != WireType.EndGroup)
			{
				bool flag2 = value < reader.position;
				if (flag2)
				{
					throw reader.CreateException("Sub-message not read entirely");
				}
				bool flag3 = reader.blockEnd != reader.position && reader.blockEnd != int.MaxValue;
				if (flag3)
				{
					throw reader.CreateException("Sub-message not read correctly");
				}
				reader.blockEnd = value;
				reader.depth--;
			}
			else
			{
				bool flag4 = value >= 0;
				if (flag4)
				{
					throw ProtoReader.AddErrorData(new ArgumentException("token"), reader);
				}
				bool flag5 = -value != reader.fieldNumber;
				if (flag5)
				{
					throw reader.CreateException("Wrong group was ended");
				}
				reader.wireType = WireType.None;
				reader.depth--;
			}
		}

		// Token: 0x060082DD RID: 33501 RVA: 0x000FC1F4 File Offset: 0x000FA3F4
		public static SubItemToken StartSubItem(ProtoReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			WireType wireType = reader.wireType;
			SubItemToken result;
			if (wireType != WireType.String)
			{
				if (wireType != WireType.StartGroup)
				{
					throw reader.CreateWireTypeException();
				}
				reader.wireType = WireType.None;
				reader.depth++;
				result = new SubItemToken(-reader.fieldNumber);
			}
			else
			{
				int num = (int)reader.ReadUInt32Variant(false);
				bool flag2 = num < 0;
				if (flag2)
				{
					throw ProtoReader.AddErrorData(new InvalidOperationException(), reader);
				}
				int value = reader.blockEnd;
				reader.blockEnd = reader.position + num;
				reader.depth++;
				result = new SubItemToken(value);
			}
			return result;
		}

		// Token: 0x060082DE RID: 33502 RVA: 0x000FC2A4 File Offset: 0x000FA4A4
		public int ReadFieldHeader()
		{
			bool flag = this.blockEnd <= this.position || this.wireType == WireType.EndGroup;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				uint num;
				bool flag2 = this.TryReadUInt32Variant(out num);
				if (flag2)
				{
					this.wireType = (WireType)(num & 7U);
					this.fieldNumber = (int)(num >> 3);
					bool flag3 = this.fieldNumber < 1;
					if (flag3)
					{
						throw new ProtoException("Invalid field in source data: " + this.fieldNumber.ToString());
					}
				}
				else
				{
					this.wireType = WireType.None;
					this.fieldNumber = 0;
				}
				bool flag4 = this.wireType == WireType.EndGroup;
				if (flag4)
				{
					bool flag5 = this.depth > 0;
					if (!flag5)
					{
						throw new ProtoException("Unexpected end-group in source data; this usually means the source data is corrupt");
					}
					result = 0;
				}
				else
				{
					result = this.fieldNumber;
				}
			}
			return result;
		}

		// Token: 0x060082DF RID: 33503 RVA: 0x000FC36C File Offset: 0x000FA56C
		public bool TryReadFieldHeader(int field)
		{
			bool flag = this.blockEnd <= this.position || this.wireType == WireType.EndGroup;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				uint num2;
				int num = this.TryReadUInt32VariantWithoutMoving(false, out num2);
				WireType wireType;
				if (num > 0 && (int)num2 >> 3 == field && (wireType = (WireType)(num2 & 7U)) != WireType.EndGroup)
				{
					this.wireType = wireType;
					this.fieldNumber = field;
					this.position += num;
					this.ioIndex += num;
					this.available -= num;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x17002963 RID: 10595
		// (get) Token: 0x060082E0 RID: 33504 RVA: 0x000FC40C File Offset: 0x000FA60C
		public TypeModel Model
		{
			get
			{
				return this.model;
			}
		}

		// Token: 0x060082E1 RID: 33505 RVA: 0x000FC424 File Offset: 0x000FA624
		public void Hint(WireType wireType)
		{
			bool flag = this.wireType == wireType;
			if (!flag)
			{
				bool flag2 = (wireType & (WireType)7) == this.wireType;
				if (flag2)
				{
					this.wireType = wireType;
				}
			}
		}

		// Token: 0x060082E2 RID: 33506 RVA: 0x000FC45C File Offset: 0x000FA65C
		public void Assert(WireType wireType)
		{
			bool flag = this.wireType == wireType;
			if (!flag)
			{
				bool flag2 = (wireType & (WireType)7) == this.wireType;
				if (!flag2)
				{
					throw this.CreateWireTypeException();
				}
				this.wireType = wireType;
			}
		}

		// Token: 0x060082E3 RID: 33507 RVA: 0x000FC4A0 File Offset: 0x000FA6A0
		public void SkipField()
		{
			switch (this.wireType)
			{
			case WireType.Variant:
			case WireType.SignedVariant:
				this.ReadUInt64Variant();
				return;
			case WireType.Fixed64:
			{
				bool flag = this.available < 8;
				if (flag)
				{
					this.Ensure(8, true);
				}
				this.available -= 8;
				this.ioIndex += 8;
				this.position += 8;
				return;
			}
			case WireType.String:
			{
				int num = (int)this.ReadUInt32Variant(false);
				bool flag2 = num <= this.available;
				if (flag2)
				{
					this.available -= num;
					this.ioIndex += num;
					this.position += num;
					return;
				}
				this.position += num;
				num -= this.available;
				this.ioIndex = (this.available = 0);
				bool flag3 = this.isFixedLength;
				if (flag3)
				{
					bool flag4 = num > this.dataRemaining;
					if (flag4)
					{
						throw ProtoReader.EoF(this);
					}
					this.dataRemaining -= num;
				}
				ProtoReader.Seek(this.source, num, this.ioBuffer);
				return;
			}
			case WireType.StartGroup:
			{
				int num2 = this.fieldNumber;
				this.depth++;
				while (this.ReadFieldHeader() > 0)
				{
					this.SkipField();
				}
				this.depth--;
				bool flag5 = this.wireType == WireType.EndGroup && this.fieldNumber == num2;
				if (flag5)
				{
					this.wireType = WireType.None;
					return;
				}
				throw this.CreateWireTypeException();
			}
			case WireType.Fixed32:
			{
				bool flag6 = this.available < 4;
				if (flag6)
				{
					this.Ensure(4, true);
				}
				this.available -= 4;
				this.ioIndex += 4;
				this.position += 4;
				return;
			}
			}
			throw this.CreateWireTypeException();
		}

        // Token: 0x060082E4 RID: 33508 RVA: 0x000FC6AC File Offset: 0x000FA8AC
        public ulong ReadUInt64()
        {
            switch (this.wireType)
            {
                case WireType.Variant:
                    return this.ReadUInt64Variant();

                case WireType.Fixed64:
                    {
                        if (this.available < 8)
                        {
                            this.Ensure(8, true);
                        }
                        this.position += 8;
                        this.available -= 8;
                        int ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        ioIndex = this.ioIndex;
                        this.ioIndex = ioIndex + 1;
                        return (ulong)(((((((this.ioBuffer[ioIndex] | (this.ioBuffer[ioIndex] << 8)) | (this.ioBuffer[ioIndex] << 0x10)) | (this.ioBuffer[ioIndex] << 0x18)) | (this.ioBuffer[ioIndex] << 0x20)) | (this.ioBuffer[ioIndex] << 40)) | (this.ioBuffer[ioIndex] << 0x30)) | (this.ioBuffer[ioIndex] << 0x38));
                    }
                case WireType.Fixed32:
                    return (ulong)this.ReadUInt32();
            }
            throw this.CreateWireTypeException();
        }



        // Token: 0x060082E5 RID: 33509 RVA: 0x000FC810 File Offset: 0x000FAA10
        public unsafe float ReadSingle()
		{
			WireType wireType = this.wireType;
			float result;
			if (wireType != WireType.Fixed64)
			{
				if (wireType != WireType.Fixed32)
				{
					throw this.CreateWireTypeException();
				}
				int num = this.ReadInt32();
				result = *(float*)(&num);
			}
			else
			{
				double num2 = this.ReadDouble();
				float num3 = (float)num2;
				bool flag = Helpers.IsInfinity(num3) && !Helpers.IsInfinity(num2);
				if (flag)
				{
					throw ProtoReader.AddErrorData(new OverflowException(), this);
				}
				result = num3;
			}
			return result;
		}

		// Token: 0x060082E6 RID: 33510 RVA: 0x000FC884 File Offset: 0x000FAA84
		public bool ReadBoolean()
		{
			uint num = this.ReadUInt32();
			bool result;
			if (num != 0U)
			{
				if (num != 1U)
				{
					throw this.CreateException("Unexpected boolean value");
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060082E7 RID: 33511 RVA: 0x000FC8BC File Offset: 0x000FAABC
		public static byte[] AppendBytes(byte[] value, ProtoReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			WireType wireType = reader.wireType;
			if (wireType != WireType.String)
			{
				throw reader.CreateWireTypeException();
			}
			int i = (int)reader.ReadUInt32Variant(false);
			reader.wireType = WireType.None;
			bool flag2 = i == 0;
			byte[] result;
			if (flag2)
			{
				result = ((value == null) ? ProtoReader.EmptyBlob : value);
			}
			else
			{
				bool flag3 = value == null || value.Length == 0;
				int num;
				if (flag3)
				{
					num = 0;
					value = new byte[i];
				}
				else
				{
					num = value.Length;
					byte[] array = new byte[value.Length + i];
					Helpers.BlockCopy(value, 0, array, 0, value.Length);
					value = array;
				}
				reader.position += i;
				while (i > reader.available)
				{
					bool flag4 = reader.available > 0;
					if (flag4)
					{
						Helpers.BlockCopy(reader.ioBuffer, reader.ioIndex, value, num, reader.available);
						i -= reader.available;
						num += reader.available;
						reader.ioIndex = (reader.available = 0);
					}
					int num2 = (i > reader.ioBuffer.Length) ? reader.ioBuffer.Length : i;
					bool flag5 = num2 > 0;
					if (flag5)
					{
						reader.Ensure(num2, true);
					}
				}
				bool flag6 = i > 0;
				if (flag6)
				{
					Helpers.BlockCopy(reader.ioBuffer, reader.ioIndex, value, num, i);
					reader.ioIndex += i;
					reader.available -= i;
				}
				result = value;
			}
			return result;
		}

		// Token: 0x060082E8 RID: 33512 RVA: 0x000FCA48 File Offset: 0x000FAC48
		private static int ReadByteOrThrow(Stream source)
		{
			int num = source.ReadByte();
			bool flag = num < 0;
			if (flag)
			{
				throw ProtoReader.EoF(null);
			}
			return num;
		}

		// Token: 0x060082E9 RID: 33513 RVA: 0x000FCA74 File Offset: 0x000FAC74
		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber)
		{
			int num;
			return ProtoReader.ReadLengthPrefix(source, expectHeader, style, out fieldNumber, out num);
		}

		// Token: 0x060082EA RID: 33514 RVA: 0x000FCA94 File Offset: 0x000FAC94
		public static int DirectReadLittleEndianInt32(Stream source)
		{
			return ProtoReader.ReadByteOrThrow(source) | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 24;
		}

		// Token: 0x060082EB RID: 33515 RVA: 0x000FCACC File Offset: 0x000FACCC
		public static int DirectReadBigEndianInt32(Stream source)
		{
			return ProtoReader.ReadByteOrThrow(source) << 24 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source);
		}

		// Token: 0x060082EC RID: 33516 RVA: 0x000FCB04 File Offset: 0x000FAD04
		public static int DirectReadVarintInt32(Stream source)
		{
			uint result;
			int num = ProtoReader.TryReadUInt32Variant(source, out result);
			bool flag = num <= 0;
			if (flag)
			{
				throw ProtoReader.EoF(null);
			}
			return (int)result;
		}

		// Token: 0x060082ED RID: 33517 RVA: 0x000FCB34 File Offset: 0x000FAD34
		public static void DirectReadBytes(Stream source, byte[] buffer, int offset, int count)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			int num;
			while (count > 0 && (num = source.Read(buffer, offset, count)) > 0)
			{
				count -= num;
				offset += num;
			}
			bool flag2 = count > 0;
			if (flag2)
			{
				throw ProtoReader.EoF(null);
			}
		}

		// Token: 0x060082EE RID: 33518 RVA: 0x000FCB8C File Offset: 0x000FAD8C
		public static byte[] DirectReadBytes(Stream source, int count)
		{
			byte[] array = new byte[count];
			ProtoReader.DirectReadBytes(source, array, 0, count);
			return array;
		}

		// Token: 0x060082EF RID: 33519 RVA: 0x000FCBB0 File Offset: 0x000FADB0
		public static string DirectReadString(Stream source, int length)
		{
			byte[] array = new byte[length];
			ProtoReader.DirectReadBytes(source, array, 0, length);
			return Encoding.UTF8.GetString(array, 0, length);
		}

		// Token: 0x060082F0 RID: 33520 RVA: 0x000FCBE0 File Offset: 0x000FADE0
		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber, out int bytesRead)
		{
			fieldNumber = 0;
			int result;
			switch (style)
			{
			case PrefixStyle.None:
				bytesRead = 0;
				result = int.MaxValue;
				break;
			case PrefixStyle.Base128:
				bytesRead = 0;
				if (expectHeader)
				{
					uint num2;
					int num = ProtoReader.TryReadUInt32Variant(source, out num2);
					bytesRead += num;
					bool flag = num > 0;
					if (flag)
					{
						bool flag2 = (num2 & 7U) != 2U;
						if (flag2)
						{
							throw new InvalidOperationException();
						}
						fieldNumber = (int)(num2 >> 3);
						num = ProtoReader.TryReadUInt32Variant(source, out num2);
						bytesRead += num;
						bool flag3 = bytesRead == 0;
						if (flag3)
						{
							throw ProtoReader.EoF(null);
						}
						result = (int)num2;
					}
					else
					{
						bytesRead = 0;
						result = -1;
					}
				}
				else
				{
					uint num2;
					int num = ProtoReader.TryReadUInt32Variant(source, out num2);
					bytesRead += num;
					result = (int)((bytesRead < 0) ? uint.MaxValue : num2);
				}
				break;
			case PrefixStyle.Fixed32:
			{
				int num3 = source.ReadByte();
				bool flag4 = num3 < 0;
				if (flag4)
				{
					bytesRead = 0;
					result = -1;
				}
				else
				{
					bytesRead = 4;
					result = (num3 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 24);
				}
				break;
			}
			case PrefixStyle.Fixed32BigEndian:
			{
				int num4 = source.ReadByte();
				bool flag5 = num4 < 0;
				if (flag5)
				{
					bytesRead = 0;
					result = -1;
				}
				else
				{
					bytesRead = 4;
					result = (num4 << 24 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source));
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("style");
			}
			return result;
		}

		// Token: 0x060082F1 RID: 33521 RVA: 0x000FCD4C File Offset: 0x000FAF4C
		private static int TryReadUInt32Variant(Stream source, out uint value)
		{
			value = 0U;
			int num = source.ReadByte();
			bool flag = num < 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				value = (uint)num;
				bool flag2 = (value & 128U) == 0U;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					value &= 127U;
					num = source.ReadByte();
					bool flag3 = num < 0;
					if (flag3)
					{
						throw ProtoReader.EoF(null);
					}
					value |= (uint)((uint)(num & 127) << 7);
					bool flag4 = (num & 128) == 0;
					if (flag4)
					{
						result = 2;
					}
					else
					{
						num = source.ReadByte();
						bool flag5 = num < 0;
						if (flag5)
						{
							throw ProtoReader.EoF(null);
						}
						value |= (uint)((uint)(num & 127) << 14);
						bool flag6 = (num & 128) == 0;
						if (flag6)
						{
							result = 3;
						}
						else
						{
							num = source.ReadByte();
							bool flag7 = num < 0;
							if (flag7)
							{
								throw ProtoReader.EoF(null);
							}
							value |= (uint)((uint)(num & 127) << 21);
							bool flag8 = (num & 128) == 0;
							if (flag8)
							{
								result = 4;
							}
							else
							{
								num = source.ReadByte();
								bool flag9 = num < 0;
								if (flag9)
								{
									throw ProtoReader.EoF(null);
								}
								value |= (uint)((uint)num << 28);
								bool flag10 = (num & 240) == 0;
								if (!flag10)
								{
									throw new OverflowException();
								}
								result = 5;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060082F2 RID: 33522 RVA: 0x000FCE7C File Offset: 0x000FB07C
		internal static void Seek(Stream source, int count, byte[] buffer)
		{
			bool canSeek = source.CanSeek;
			if (canSeek)
			{
				source.Seek((long)count, SeekOrigin.Current);
				count = 0;
			}
			else
			{
				bool flag = buffer != null;
				if (flag)
				{
					int num;
					while (count > buffer.Length && (num = source.Read(buffer, 0, buffer.Length)) > 0)
					{
						count -= num;
					}
					while (count > 0 && (num = source.Read(buffer, 0, count)) > 0)
					{
						count -= num;
					}
				}
				else
				{
					buffer = BufferPool.GetBuffer();
					try
					{
						int num2;
						while (count > buffer.Length && (num2 = source.Read(buffer, 0, buffer.Length)) > 0)
						{
							count -= num2;
						}
						while (count > 0 && (num2 = source.Read(buffer, 0, count)) > 0)
						{
							count -= num2;
						}
					}
					finally
					{
						BufferPool.ReleaseBufferToPool(ref buffer);
					}
				}
			}
			bool flag2 = count > 0;
			if (flag2)
			{
				throw ProtoReader.EoF(null);
			}
		}

		// Token: 0x060082F3 RID: 33523 RVA: 0x000FCF88 File Offset: 0x000FB188
		internal static Exception AddErrorData(Exception exception, ProtoReader source)
		{
			bool flag = exception != null && source != null && !exception.Data.Contains("protoSource");
			if (flag)
			{
				exception.Data.Add("protoSource", string.Format("tag={0}; wire-type={1}; offset={2}; depth={3}", new object[]
				{
					source.fieldNumber,
					source.wireType,
					source.position,
					source.depth
				}));
			}
			return exception;
		}

		// Token: 0x060082F4 RID: 33524 RVA: 0x000FD018 File Offset: 0x000FB218
		private static Exception EoF(ProtoReader source)
		{
			return ProtoReader.AddErrorData(new EndOfStreamException(), source);
		}

		// Token: 0x060082F5 RID: 33525 RVA: 0x000FD038 File Offset: 0x000FB238
		public void AppendExtensionData(IExtensible instance)
		{
			bool flag = instance == null;
			if (flag)
			{
				throw new ArgumentNullException("instance");
			}
			IExtension extensionObject = instance.GetExtensionObject(true);
			bool commit = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, this.model, null))
				{
					this.AppendExtensionField(protoWriter);
					protoWriter.Close();
				}
				commit = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, commit);
			}
		}

		// Token: 0x060082F6 RID: 33526 RVA: 0x000FD0CC File Offset: 0x000FB2CC
		private void AppendExtensionField(ProtoWriter writer)
		{
			ProtoWriter.WriteFieldHeader(this.fieldNumber, this.wireType, writer);
			switch (this.wireType)
			{
			case WireType.Variant:
			case WireType.Fixed64:
			case WireType.SignedVariant:
				ProtoWriter.WriteInt64(this.ReadInt64(), writer);
				return;
			case WireType.String:
				ProtoWriter.WriteBytes(ProtoReader.AppendBytes(null, this), writer);
				return;
			case WireType.StartGroup:
			{
				SubItemToken token = ProtoReader.StartSubItem(this);
				SubItemToken token2 = ProtoWriter.StartSubItem(null, writer);
				while (this.ReadFieldHeader() > 0)
				{
					this.AppendExtensionField(writer);
				}
				ProtoReader.EndSubItem(token, this);
				ProtoWriter.EndSubItem(token2, writer);
				return;
			}
			case WireType.Fixed32:
				ProtoWriter.WriteInt32(this.ReadInt32(), writer);
				return;
			}
			throw this.CreateWireTypeException();
		}

		// Token: 0x060082F7 RID: 33527 RVA: 0x000FD198 File Offset: 0x000FB398
		public static bool HasSubValue(WireType wireType, ProtoReader source)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = source.blockEnd <= source.position || wireType == WireType.EndGroup;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				source.wireType = wireType;
				result = true;
			}
			return result;
		}

		// Token: 0x060082F8 RID: 33528 RVA: 0x000FD1E4 File Offset: 0x000FB3E4
		internal int GetTypeKey(ref Type type)
		{
			return this.model.GetKey(ref type);
		}

		// Token: 0x17002964 RID: 10596
		// (get) Token: 0x060082F9 RID: 33529 RVA: 0x000FD204 File Offset: 0x000FB404
		internal NetObjectCache NetCache
		{
			get
			{
				return this.netCache;
			}
		}

		// Token: 0x060082FA RID: 33530 RVA: 0x000FD21C File Offset: 0x000FB41C
		internal Type DeserializeType(string value)
		{
			return TypeModel.DeserializeType(this.model, value);
		}

		// Token: 0x060082FB RID: 33531 RVA: 0x000FD23A File Offset: 0x000FB43A
		internal void SetRootObject(object value)
		{
			this.netCache.SetKeyedObject(0, value);
			this.trapCount -= 1U;
		}

		// Token: 0x060082FC RID: 33532 RVA: 0x000FD25C File Offset: 0x000FB45C
		public static void NoteObject(object value, ProtoReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			bool flag2 = reader.trapCount > 0U;
			if (flag2)
			{
				reader.netCache.RegisterTrappedObject(value);
				reader.trapCount -= 1U;
			}
		}

		// Token: 0x060082FD RID: 33533 RVA: 0x000FD2A8 File Offset: 0x000FB4A8
		public Type ReadType()
		{
			return TypeModel.DeserializeType(this.model, this.ReadString());
		}

		// Token: 0x060082FE RID: 33534 RVA: 0x000FD2CB File Offset: 0x000FB4CB
		internal void TrapNextObject(int newObjectKey)
		{
			this.trapCount += 1U;
			this.netCache.SetKeyedObject(newObjectKey, null);
		}

		// Token: 0x060082FF RID: 33535 RVA: 0x000FD2EC File Offset: 0x000FB4EC
		internal void CheckFullyConsumed()
		{
			bool flag = this.isFixedLength;
			if (flag)
			{
				bool flag2 = this.dataRemaining != 0;
				if (flag2)
				{
					throw new ProtoException("Incorrect number of bytes consumed");
				}
			}
			else
			{
				bool flag3 = this.available != 0;
				if (flag3)
				{
					throw new ProtoException("Unconsumed data left in the buffer; this suggests corrupt input");
				}
			}
		}

		// Token: 0x06008300 RID: 33536 RVA: 0x000FD33C File Offset: 0x000FB53C
		public static object Merge(ProtoReader parent, object from, object to)
		{
			bool flag = parent == null;
			if (flag)
			{
				throw new ArgumentNullException("parent");
			}
			TypeModel typeModel = parent.Model;
			SerializationContext serializationContext = parent.Context;
			bool flag2 = typeModel == null;
			if (flag2)
			{
				throw new InvalidOperationException("Types cannot be merged unless a type-model has been specified");
			}
			object result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				typeModel.Serialize(memoryStream, from, serializationContext);
				memoryStream.Position = 0L;
				result = typeModel.Deserialize(memoryStream, to, null);
			}
			return result;
		}

		// Token: 0x06008301 RID: 33537 RVA: 0x000FD3C8 File Offset: 0x000FB5C8
		internal static ProtoReader Create(Stream source, TypeModel model, SerializationContext context, int len)
		{
			ProtoReader recycled = ProtoReader.GetRecycled();
			bool flag = recycled == null;
			ProtoReader result;
			if (flag)
			{
				result = new ProtoReader(source, model, context, len);
			}
			else
			{
				ProtoReader.Init(recycled, source, model, context, len);
				result = recycled;
			}
			return result;
		}

		// Token: 0x06008302 RID: 33538 RVA: 0x000FD404 File Offset: 0x000FB604
		private static ProtoReader GetRecycled()
		{
			ProtoReader result = ProtoReader.lastReader;
			ProtoReader.lastReader = null;
			return result;
		}

		// Token: 0x06008303 RID: 33539 RVA: 0x000FD424 File Offset: 0x000FB624
		internal static void Recycle(ProtoReader reader)
		{
			bool flag = reader != null;
			if (flag)
			{
				reader.Dispose();
				ProtoReader.lastReader = reader;
			}
		}

		// Token: 0x0400289B RID: 10395
		private Stream source;

		// Token: 0x0400289C RID: 10396
		private byte[] ioBuffer;

		// Token: 0x0400289D RID: 10397
		private TypeModel model;

		// Token: 0x0400289E RID: 10398
		private int fieldNumber;

		// Token: 0x0400289F RID: 10399
		private int depth;

		// Token: 0x040028A0 RID: 10400
		private int dataRemaining;

		// Token: 0x040028A1 RID: 10401
		private int ioIndex;

		// Token: 0x040028A2 RID: 10402
		private int position;

		// Token: 0x040028A3 RID: 10403
		private int available;

		// Token: 0x040028A4 RID: 10404
		private int blockEnd;

		// Token: 0x040028A5 RID: 10405
		private WireType wireType;

		// Token: 0x040028A6 RID: 10406
		private bool isFixedLength;

		// Token: 0x040028A7 RID: 10407
		private bool internStrings;

		// Token: 0x040028A8 RID: 10408
		private NetObjectCache netCache;

		// Token: 0x040028A9 RID: 10409
		private uint trapCount;

		// Token: 0x040028AA RID: 10410
		internal const int TO_EOF = -1;

		// Token: 0x040028AB RID: 10411
		private SerializationContext context;

		// Token: 0x040028AC RID: 10412
		private const long Int64Msb = -9223372036854775808L;

		// Token: 0x040028AD RID: 10413
		private const int Int32Msb = -2147483648;

		// Token: 0x040028AE RID: 10414
		private Dictionary<string, string> stringInterner;

		// Token: 0x040028AF RID: 10415
		private static readonly UTF8Encoding encoding = new UTF8Encoding();

		// Token: 0x040028B0 RID: 10416
		private static readonly byte[] EmptyBlob = new byte[0];

		// Token: 0x040028B1 RID: 10417
		[ThreadStatic]
		private static ProtoReader lastReader;
	}
}
