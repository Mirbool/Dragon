using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ProtoBuf.Serializers;

namespace ProtoBuf.Meta
{
	// Token: 0x0200088E RID: 2190
	public abstract class TypeModel
	{
		// Token: 0x06008550 RID: 34128 RVA: 0x00108490 File Offset: 0x00106690
		protected internal Type MapType(Type type)
		{
			return this.MapType(type, true);
		}

		// Token: 0x06008551 RID: 34129 RVA: 0x001084AC File Offset: 0x001066AC
		protected internal virtual Type MapType(Type type, bool demand)
		{
			return type;
		}

        // Token: 0x06008552 RID: 34130 RVA: 0x001084C0 File Offset: 0x001066C0
        private WireType GetWireType(ProtoTypeCode code, DataFormat format, ref Type type, out int modelKey)
        {
            modelKey = -1;
            if (Helpers.IsEnum(type))
            {
                modelKey = this.GetKey(ref type);
                return WireType.Variant;
            }
            ProtoTypeCode code2 = code;
            switch (code2)
            {
                case ProtoTypeCode.Boolean:
                case ProtoTypeCode.Char:
                case ProtoTypeCode.SByte:
                case ProtoTypeCode.Byte:
                case ProtoTypeCode.Int16:
                case ProtoTypeCode.UInt16:
                case ProtoTypeCode.Int32:
                case ProtoTypeCode.UInt32:
                    return ((format == DataFormat.FixedSize) ? WireType.Fixed32 : WireType.Variant);

                case ProtoTypeCode.Int64:
                case ProtoTypeCode.UInt64:
                    return ((format == DataFormat.FixedSize) ? WireType.Fixed64 : WireType.Variant);

                case ProtoTypeCode.Single:
                    return WireType.Fixed32;

                case ProtoTypeCode.Double:
                    return WireType.Fixed64;

                case ProtoTypeCode.Decimal:
                case ProtoTypeCode.DateTime:
                case ProtoTypeCode.String:
                    break;

                case (ProtoTypeCode.DateTime | ProtoTypeCode.Unknown):
                    goto Label_0099;

                default:
                    if ((code2 - 100) > ProtoTypeCode.Boolean)
                    {
                        goto Label_0099;
                    }
                    break;
            }
            return WireType.String;
            Label_0099:
            if ((modelKey = this.GetKey(ref type)) >= 0)
            {
                return WireType.String;
            }
            return WireType.None;
        }



        // Token: 0x06008553 RID: 34131 RVA: 0x0010858C File Offset: 0x0010678C
        internal bool TrySerializeAuxiliaryType(ProtoWriter writer, Type type, DataFormat format, int tag, object value, bool isInsideList)
		{
			bool flag = type == null;
			if (flag)
			{
				type = value.GetType();
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			int num;
			WireType wireType = this.GetWireType(typeCode, format, ref type, out num);
			bool flag2 = num >= 0;
			bool result;
			if (flag2)
			{
				bool flag3 = Helpers.IsEnum(type);
				if (flag3)
				{
					this.Serialize(num, value, writer);
					result = true;
				}
				else
				{
					ProtoWriter.WriteFieldHeader(tag, wireType, writer);
					WireType wireType2 = wireType;
					if (wireType2 == WireType.None)
					{
						throw ProtoWriter.CreateException(writer);
					}
					if (wireType2 - WireType.String > 1)
					{
						this.Serialize(num, value, writer);
						result = true;
					}
					else
					{
						SubItemToken token = ProtoWriter.StartSubItem(value, writer);
						this.Serialize(num, value, writer);
						ProtoWriter.EndSubItem(token, writer);
						result = true;
					}
				}
			}
			else
			{
				bool flag4 = wireType != WireType.None;
				if (flag4)
				{
					ProtoWriter.WriteFieldHeader(tag, wireType, writer);
				}
				ProtoTypeCode protoTypeCode = typeCode;
				switch (protoTypeCode)
				{
				case ProtoTypeCode.Boolean:
					ProtoWriter.WriteBoolean((bool)value, writer);
					return true;
				case ProtoTypeCode.Char:
					ProtoWriter.WriteUInt16((ushort)((char)value), writer);
					return true;
				case ProtoTypeCode.SByte:
					ProtoWriter.WriteSByte((sbyte)value, writer);
					return true;
				case ProtoTypeCode.Byte:
					ProtoWriter.WriteByte((byte)value, writer);
					return true;
				case ProtoTypeCode.Int16:
					ProtoWriter.WriteInt16((short)value, writer);
					return true;
				case ProtoTypeCode.UInt16:
					ProtoWriter.WriteUInt16((ushort)value, writer);
					return true;
				case ProtoTypeCode.Int32:
					ProtoWriter.WriteInt32((int)value, writer);
					return true;
				case ProtoTypeCode.UInt32:
					ProtoWriter.WriteUInt32((uint)value, writer);
					return true;
				case ProtoTypeCode.Int64:
					ProtoWriter.WriteInt64((long)value, writer);
					return true;
				case ProtoTypeCode.UInt64:
					ProtoWriter.WriteUInt64((ulong)value, writer);
					return true;
				case ProtoTypeCode.Single:
					ProtoWriter.WriteSingle((float)value, writer);
					return true;
				case ProtoTypeCode.Double:
					ProtoWriter.WriteDouble((double)value, writer);
					return true;
				case ProtoTypeCode.Decimal:
					BclHelpers.WriteDecimal((decimal)value, writer);
					return true;
				case ProtoTypeCode.DateTime:
					BclHelpers.WriteDateTime((DateTime)value, writer);
					return true;
				case (ProtoTypeCode)17:
					break;
				case ProtoTypeCode.String:
					ProtoWriter.WriteString((string)value, writer);
					return true;
				default:
					switch (protoTypeCode)
					{
					case ProtoTypeCode.TimeSpan:
						BclHelpers.WriteTimeSpan((TimeSpan)value, writer);
						return true;
					case ProtoTypeCode.ByteArray:
						ProtoWriter.WriteBytes((byte[])value, writer);
						return true;
					case ProtoTypeCode.Guid:
						BclHelpers.WriteGuid((Guid)value, writer);
						return true;
					case ProtoTypeCode.Uri:
						ProtoWriter.WriteString(((Uri)value).AbsoluteUri, writer);
						return true;
					}
					break;
				}
				Helpers.DebugAssert(wireType == WireType.None);
				IEnumerable enumerable = value as IEnumerable;
				bool flag5 = enumerable != null;
				if (flag5)
				{
					if (isInsideList)
					{
						throw TypeModel.CreateNestedListsNotSupported();
					}
					foreach (object obj in enumerable)
					{
						bool flag6 = obj == null;
						if (flag6)
						{
							throw new NullReferenceException();
						}
						bool flag7 = !this.TrySerializeAuxiliaryType(writer, null, format, tag, obj, true);
						if (flag7)
						{
							TypeModel.ThrowUnexpectedType(obj.GetType());
						}
					}
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06008554 RID: 34132 RVA: 0x00108938 File Offset: 0x00106B38
		private void SerializeCore(ProtoWriter writer, object value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			Type type = value.GetType();
			int key = this.GetKey(ref type);
			bool flag2 = key >= 0;
			if (flag2)
			{
				this.Serialize(key, value, writer);
			}
			else
			{
				bool flag3 = !this.TrySerializeAuxiliaryType(writer, type, DataFormat.Default, 1, value, false);
				if (flag3)
				{
					TypeModel.ThrowUnexpectedType(type);
				}
			}
		}

		// Token: 0x06008555 RID: 34133 RVA: 0x001089A0 File Offset: 0x00106BA0
		public void Serialize(Stream dest, object value)
		{
			this.Serialize(dest, value, null);
		}

		// Token: 0x06008556 RID: 34134 RVA: 0x001089B0 File Offset: 0x00106BB0
		public void Serialize(Stream dest, object value, SerializationContext context)
		{
			using (ProtoWriter protoWriter = new ProtoWriter(dest, this, context))
			{
				protoWriter.SetRootObject(value);
				this.SerializeCore(protoWriter, value);
				protoWriter.Close();
			}
		}

		// Token: 0x06008557 RID: 34135 RVA: 0x00108A00 File Offset: 0x00106C00
		public void Serialize(ProtoWriter dest, object value)
		{
			bool flag = dest == null;
			if (flag)
			{
				throw new ArgumentNullException("dest");
			}
			dest.CheckDepthFlushlock();
			dest.SetRootObject(value);
			this.SerializeCore(dest, value);
			dest.CheckDepthFlushlock();
			ProtoWriter.Flush(dest);
		}

		// Token: 0x06008558 RID: 34136 RVA: 0x00108A48 File Offset: 0x00106C48
		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int fieldNumber)
		{
			int num;
			return this.DeserializeWithLengthPrefix(source, value, type, style, fieldNumber, null, out num);
		}

		// Token: 0x06008559 RID: 34137 RVA: 0x00108A6C File Offset: 0x00106C6C
		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
		{
			int num;
			return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out num);
		}

		// Token: 0x0600855A RID: 34138 RVA: 0x00108A90 File Offset: 0x00106C90
		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead)
		{
			bool flag;
			return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out bytesRead, out flag, null);
		}

		// Token: 0x0600855B RID: 34139 RVA: 0x00108AB8 File Offset: 0x00106CB8
		private object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead, out bool haveObject, SerializationContext context)
		{
			haveObject = false;
			bytesRead = 0;
            int num;

            bool flag = type == null && (style != PrefixStyle.Base128 || resolver == null);
			if (flag)
			{
				throw new InvalidOperationException("A type must be provided unless base-128 prefixing is being used in combination with a resolver");
			}
			for (;;)
			{
				bool flag2 = expectedField > 0 || resolver != null;
				int num2;
				int num3;
				 num = ProtoReader.ReadLengthPrefix(source, flag2, style, out num2, out num3);
				bool flag3 = num3 == 0;
				if (flag3)
				{
					break;
				}
				bytesRead += num3;
				bool flag4 = num < 0;
				if (flag4)
				{
					goto Block_6;
				}
				bool flag5;
				if (style != PrefixStyle.Base128)
				{
					flag5 = false;
				}
				else
				{
					bool flag6 = flag2 && expectedField == 0 && type == null && resolver != null;
					if (flag6)
					{
						type = resolver(num2);
						flag5 = (type == null);
					}
					else
					{
						flag5 = (expectedField != num2);
					}
				}
				bool flag7 = flag5;
				if (flag7)
				{
					bool flag8 = num == int.MaxValue;
					if (flag8)
					{
						goto Block_13;
					}
					ProtoReader.Seek(source, num, null);
					bytesRead += num;
				}
				if (!flag5)
				{
					goto Block_14;
				}
			}
			return value;
			Block_6:
			return value;
			Block_13:
			throw new InvalidOperationException();
			Block_14:
			ProtoReader protoReader = null;
			object result;
			try
			{
				protoReader = ProtoReader.Create(source, this, context, num);
				int key = this.GetKey(ref type);
				bool flag9 = key >= 0 && !Helpers.IsEnum(type);
				if (flag9)
				{
					value = this.Deserialize(key, value, protoReader);
				}
				else
				{
					bool flag10 = !this.TryDeserializeAuxiliaryType(protoReader, DataFormat.Default, 1, type, ref value, true, false, true, false) && num != 0;
					if (flag10)
					{
						TypeModel.ThrowUnexpectedType(type);
					}
				}
				bytesRead += protoReader.Position;
				haveObject = true;
				result = value;
			}
			finally
			{
				ProtoReader.Recycle(protoReader);
			}
			return result;
		}

		// Token: 0x0600855C RID: 34140 RVA: 0x00108C60 File Offset: 0x00106E60
		public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
		{
			return this.DeserializeItems(source, type, style, expectedField, resolver, null);
		}

		// Token: 0x0600855D RID: 34141 RVA: 0x00108C80 File Offset: 0x00106E80
		public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
		{
			return new TypeModel.DeserializeItemsIterator(this, source, type, style, expectedField, resolver, context);
		}

		// Token: 0x0600855E RID: 34142 RVA: 0x00108CA4 File Offset: 0x00106EA4
		public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField)
		{
			return this.DeserializeItems<T>(source, style, expectedField, null);
		}

		// Token: 0x0600855F RID: 34143 RVA: 0x00108CC0 File Offset: 0x00106EC0
		public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField, SerializationContext context)
		{
			return new TypeModel.DeserializeItemsIterator<T>(this, source, style, expectedField, context);
		}

		// Token: 0x06008560 RID: 34144 RVA: 0x00108CDD File Offset: 0x00106EDD
		public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber)
		{
			this.SerializeWithLengthPrefix(dest, value, type, style, fieldNumber, null);
		}

		// Token: 0x06008561 RID: 34145 RVA: 0x00108CF0 File Offset: 0x00106EF0
		public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber, SerializationContext context)
		{
			bool flag = type == null;
			if (flag)
			{
				bool flag2 = value == null;
				if (flag2)
				{
					throw new ArgumentNullException("value");
				}
				type = this.MapType(value.GetType());
			}
			int key = this.GetKey(ref type);
			using (ProtoWriter protoWriter = new ProtoWriter(dest, this, context))
			{
				if (style != PrefixStyle.None)
				{
					if (style - PrefixStyle.Base128 > 2)
					{
						throw new ArgumentOutOfRangeException("style");
					}
					ProtoWriter.WriteObject(value, key, protoWriter, style, fieldNumber);
				}
				else
				{
					this.Serialize(key, value, protoWriter);
				}
				protoWriter.Close();
			}
		}

		// Token: 0x06008562 RID: 34146 RVA: 0x00108D9C File Offset: 0x00106F9C
		public object Deserialize(Stream source, object value, Type type)
		{
			return this.Deserialize(source, value, type, null);
		}

		// Token: 0x06008563 RID: 34147 RVA: 0x00108DB8 File Offset: 0x00106FB8
		public object Deserialize(Stream source, object value, Type type, SerializationContext context)
		{
			bool noAutoCreate = this.PrepareDeserialize(value, ref type);
			ProtoReader protoReader = null;
			object result;
			try
			{
				protoReader = ProtoReader.Create(source, this, context, -1);
				bool flag = value != null;
				if (flag)
				{
					protoReader.SetRootObject(value);
				}
				object obj = this.DeserializeCore(protoReader, type, value, noAutoCreate);
				protoReader.CheckFullyConsumed();
				result = obj;
			}
			finally
			{
				ProtoReader.Recycle(protoReader);
			}
			return result;
		}

		// Token: 0x06008564 RID: 34148 RVA: 0x00108E24 File Offset: 0x00107024
		private bool PrepareDeserialize(object value, ref Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				bool flag2 = value == null;
				if (flag2)
				{
					throw new ArgumentNullException("type");
				}
				type = this.MapType(value.GetType());
			}
			bool result = true;
			Type underlyingType = Helpers.GetUnderlyingType(type);
			bool flag3 = underlyingType != null;
			if (flag3)
			{
				type = underlyingType;
				result = false;
			}
			return result;
		}

		// Token: 0x06008565 RID: 34149 RVA: 0x00108E84 File Offset: 0x00107084
		public object Deserialize(Stream source, object value, Type type, int length)
		{
			return this.Deserialize(source, value, type, length, null);
		}

		// Token: 0x06008566 RID: 34150 RVA: 0x00108EA4 File Offset: 0x001070A4
		public object Deserialize(Stream source, object value, Type type, int length, SerializationContext context)
		{
			bool noAutoCreate = this.PrepareDeserialize(value, ref type);
			ProtoReader protoReader = null;
			object result;
			try
			{
				protoReader = ProtoReader.Create(source, this, context, length);
				bool flag = value != null;
				if (flag)
				{
					protoReader.SetRootObject(value);
				}
				object obj = this.DeserializeCore(protoReader, type, value, noAutoCreate);
				protoReader.CheckFullyConsumed();
				result = obj;
			}
			finally
			{
				ProtoReader.Recycle(protoReader);
			}
			return result;
		}

		// Token: 0x06008567 RID: 34151 RVA: 0x00108F10 File Offset: 0x00107110
		public object Deserialize(ProtoReader source, object value, Type type)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool noAutoCreate = this.PrepareDeserialize(value, ref type);
			bool flag2 = value != null;
			if (flag2)
			{
				source.SetRootObject(value);
			}
			object result = this.DeserializeCore(source, type, value, noAutoCreate);
			source.CheckFullyConsumed();
			return result;
		}

		// Token: 0x06008568 RID: 34152 RVA: 0x00108F64 File Offset: 0x00107164
		private object DeserializeCore(ProtoReader reader, Type type, object value, bool noAutoCreate)
		{
			int key = this.GetKey(ref type);
			bool flag = key >= 0 && !Helpers.IsEnum(type);
			object result;
			if (flag)
			{
				result = this.Deserialize(key, value, reader);
			}
			else
			{
				this.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, noAutoCreate, false);
				result = value;
			}
			return result;
		}

		// Token: 0x06008569 RID: 34153 RVA: 0x00108FB4 File Offset: 0x001071B4
		internal static MethodInfo ResolveListAdd(TypeModel model, Type listType, Type itemType, out bool isList)
		{
			isList = model.MapType(TypeModel.ilist).IsAssignableFrom(listType);
			Type[] array = new Type[]
			{
				itemType
			};
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(listType, "Add", array);
			bool flag = instanceMethod == null;
			if (flag)
			{
				bool flag2 = listType.IsInterface && listType == model.MapType(typeof(IEnumerable<>)).MakeGenericType(array);
				Type type = model.MapType(typeof(ICollection<>)).MakeGenericType(array);
				bool flag3 = flag2 || type.IsAssignableFrom(listType);
				if (flag3)
				{
					instanceMethod = Helpers.GetInstanceMethod(type, "Add", array);
				}
			}
			bool flag4 = instanceMethod == null;
			if (flag4)
			{
				foreach (Type type2 in listType.GetInterfaces())
				{
					bool flag5 = type2.Name == "IProducerConsumerCollection`1" && type2.IsGenericType && type2.GetGenericTypeDefinition().FullName == "System.Collections.Concurrent.IProducerConsumerCollection`1";
					if (flag5)
					{
						instanceMethod = Helpers.GetInstanceMethod(type2, "TryAdd", array);
						bool flag6 = instanceMethod != null;
						if (flag6)
						{
							break;
						}
					}
				}
			}
			bool flag7 = instanceMethod == null;
			if (flag7)
			{
				array[0] = model.MapType(typeof(object));
				instanceMethod = Helpers.GetInstanceMethod(listType, "Add", array);
			}
			bool flag8 = instanceMethod == null & isList;
			if (flag8)
			{
				instanceMethod = Helpers.GetInstanceMethod(model.MapType(TypeModel.ilist), "Add", array);
			}
			return instanceMethod;
		}

		// Token: 0x0600856A RID: 34154 RVA: 0x00109140 File Offset: 0x00107340
		internal static Type GetListItemType(TypeModel model, Type listType)
		{
			Helpers.DebugAssert(listType != null);
			bool flag = listType == model.MapType(typeof(string)) || listType.IsArray || !model.MapType(typeof(IEnumerable)).IsAssignableFrom(listType);
			Type result;
			if (flag)
			{
				result = null;
			}
			else
			{
				BasicList basicList = new BasicList();
				foreach (MethodInfo methodInfo in listType.GetMethods())
				{
					bool flag2 = methodInfo.IsStatic || methodInfo.Name != "Add";
					if (!flag2)
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
                        Type parameterType = parameters[0].ParameterType;

                        bool flag3 = parameters.Length == 1 && !basicList.Contains(parameterType);
						if (flag3)
						{
							basicList.Add(parameterType);
						}
					}
				}
				string name = listType.Name;
				bool flag4 = name != null && (name.IndexOf("Queue") >= 0 || name.IndexOf("Stack") >= 0);
				bool flag5 = !flag4;
				if (flag5)
				{
					TypeModel.TestEnumerableListPatterns(model, basicList, listType);
					foreach (Type iType in listType.GetInterfaces())
					{
						TypeModel.TestEnumerableListPatterns(model, basicList, iType);
					}
				}
				foreach (PropertyInfo propertyInfo in listType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					bool flag6 = propertyInfo.Name != "Item" || basicList.Contains(propertyInfo.PropertyType);
					if (!flag6)
					{
						ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
						bool flag7 = indexParameters.Length != 1 || indexParameters[0].ParameterType != model.MapType(typeof(int));
						if (!flag7)
						{
							basicList.Add(propertyInfo.PropertyType);
						}
					}
				}
				switch (basicList.Count)
				{
				case 0:
					return null;
				case 1:
					return (Type)basicList[0];
				case 2:
				{
					bool flag8 = TypeModel.CheckDictionaryAccessors(model, (Type)basicList[0], (Type)basicList[1]);
					if (flag8)
					{
						return (Type)basicList[0];
					}
					bool flag9 = TypeModel.CheckDictionaryAccessors(model, (Type)basicList[1], (Type)basicList[0]);
					if (flag9)
					{
						return (Type)basicList[1];
					}
					break;
				}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600856B RID: 34155 RVA: 0x001093E0 File Offset: 0x001075E0
		private static void TestEnumerableListPatterns(TypeModel model, BasicList candidates, Type iType)
		{
			bool isGenericType = iType.IsGenericType;
			if (isGenericType)
			{
				Type genericTypeDefinition = iType.GetGenericTypeDefinition();
				bool flag = genericTypeDefinition == model.MapType(typeof(IEnumerable<>)) || genericTypeDefinition == model.MapType(typeof(ICollection<>)) || genericTypeDefinition.FullName == "System.Collections.Concurrent.IProducerConsumerCollection`1";
				if (flag)
				{
					Type[] genericArguments = iType.GetGenericArguments();
					bool flag2 = !candidates.Contains(genericArguments[0]);
					if (flag2)
					{
						candidates.Add(genericArguments[0]);
					}
				}
			}
		}

		// Token: 0x0600856C RID: 34156 RVA: 0x00109468 File Offset: 0x00107668
		private static bool CheckDictionaryAccessors(TypeModel model, Type pair, Type value)
		{
			return pair.IsGenericType && pair.GetGenericTypeDefinition() == model.MapType(typeof(KeyValuePair<, >)) && pair.GetGenericArguments()[1] == value;
		}

		// Token: 0x0600856D RID: 34157 RVA: 0x001094A8 File Offset: 0x001076A8
		private bool TryDeserializeList(TypeModel model, ProtoReader reader, DataFormat format, int tag, Type listType, Type itemType, ref object value)
		{
			bool flag;
			MethodInfo methodInfo = TypeModel.ResolveListAdd(model, listType, itemType, out flag);
			bool flag2 = methodInfo == null;
			if (flag2)
			{
				throw new NotSupportedException("Unknown list variant: " + listType.FullName);
			}
			bool result = false;
			object obj = null;
			IList list = value as IList;
			object[] array = flag ? null : ProtoDecoratorBase.s_argsRead;
			BasicList basicList = listType.IsArray ? new BasicList() : null;
			while (this.TryDeserializeAuxiliaryType(reader, format, tag, itemType, ref obj, true, true, true, true))
			{
				result = true;
				bool flag3 = value == null && basicList == null;
				if (flag3)
				{
					value = TypeModel.CreateListInstance(listType, itemType);
					list = (value as IList);
				}
				bool flag4 = list != null;
				if (flag4)
				{
					list.Add(obj);
				}
				else
				{
					bool flag5 = basicList != null;
					if (flag5)
					{
						basicList.Add(obj);
					}
					else
					{
						array[0] = obj;
						methodInfo.Invoke(value, array);
					}
				}
				obj = null;
			}
			bool flag6 = basicList != null;
			if (flag6)
			{
				bool flag7 = value != null;
				if (flag7)
				{
					bool flag8 = basicList.Count == 0;
					if (!flag8)
					{
						Array array2 = (Array)value;
						Array array3 = Array.CreateInstance(itemType, array2.Length + basicList.Count);
						Array.Copy(array2, array3, array2.Length);
						basicList.CopyTo(array3, array2.Length);
						value = array3;
					}
				}
				else
				{
					Array array3 = Array.CreateInstance(itemType, basicList.Count);
					basicList.CopyTo(array3, 0);
					value = array3;
				}
			}
			return result;
		}

		// Token: 0x0600856E RID: 34158 RVA: 0x0010964C File Offset: 0x0010784C
		private static object CreateListInstance(Type listType, Type itemType)
		{
			Type type = listType;
			bool isArray = listType.IsArray;
			object result;
			if (isArray)
			{
				result = Array.CreateInstance(itemType, 0);
			}
			else
			{
				bool flag = !listType.IsClass || listType.IsAbstract || Helpers.GetConstructor(listType, Helpers.EmptyTypes, true) == null;
				if (flag)
				{
					bool flag2 = false;
					string fullName;
					bool flag3 = listType.IsInterface && (fullName = listType.FullName) != null && fullName.IndexOf("Dictionary") >= 0;
					if (flag3)
					{
						bool flag4 = listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(IDictionary<, >);
						if (flag4)
						{
							Type[] genericArguments = listType.GetGenericArguments();
							type = typeof(Dictionary<, >).MakeGenericType(genericArguments);
							flag2 = true;
						}
						bool flag5 = !flag2 && listType == typeof(IDictionary);
						if (flag5)
						{
							type = typeof(Hashtable);
							flag2 = true;
						}
					}
					bool flag6 = !flag2;
					if (flag6)
					{
						type = typeof(List<>).MakeGenericType(new Type[]
						{
							itemType
						});
						flag2 = true;
					}
					bool flag7 = !flag2;
					if (flag7)
					{
						type = typeof(ArrayList);
					}
				}
				result = Activator.CreateInstance(type);
			}
			return result;
		}

		// Token: 0x0600856F RID: 34159 RVA: 0x00109790 File Offset: 0x00107990
		internal bool TryDeserializeAuxiliaryType(ProtoReader reader, DataFormat format, int tag, Type type, ref object value, bool skipOtherFields, bool asListItem, bool autoCreate, bool insideList)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			int num;
			WireType wireType = this.GetWireType(typeCode, format, ref type, out num);
			bool flag2 = false;
			bool flag3 = wireType == WireType.None;
			if (flag3)
			{
				Type type2 = TypeModel.GetListItemType(this, type);
				bool flag4 = type2 == null && type.IsArray && type.GetArrayRank() == 1 && type != typeof(byte[]);
				if (flag4)
				{
					type2 = type.GetElementType();
				}
				bool flag5 = type2 != null;
				if (flag5)
				{
					if (insideList)
					{
						throw TypeModel.CreateNestedListsNotSupported();
					}
					flag2 = this.TryDeserializeList(this, reader, format, tag, type, type2, ref value);
					bool flag6 = !flag2 && autoCreate;
					if (flag6)
					{
						value = TypeModel.CreateListInstance(type, type2);
					}
					return flag2;
				}
				else
				{
					TypeModel.ThrowUnexpectedType(type);
				}
			}
			int num2;
			for (;;)
			{
				bool flag7 = flag2 && asListItem;
				if (flag7)
				{
					break;
				}
				num2 = reader.ReadFieldHeader();
				bool flag8 = num2 <= 0;
				if (flag8)
				{
					break;
				}
				bool flag9 = num2 != tag;
				if (flag9)
				{
					if (!skipOtherFields)
					{
						goto IL_12E;
					}
					reader.SkipField();
				}
				else
				{
					flag2 = true;
					reader.Hint(wireType);
					bool flag10 = num >= 0;
					if (flag10)
					{
						WireType wireType2 = wireType;
						if (wireType2 - WireType.String > 1)
						{
							value = this.Deserialize(num, value, reader);
						}
						else
						{
							SubItemToken token = ProtoReader.StartSubItem(reader);
							value = this.Deserialize(num, value, reader);
							ProtoReader.EndSubItem(token, reader);
						}
					}
					else
					{
						ProtoTypeCode protoTypeCode = typeCode;
						switch (protoTypeCode)
						{
						case ProtoTypeCode.Boolean:
							value = reader.ReadBoolean();
							break;
						case ProtoTypeCode.Char:
							value = (char)reader.ReadUInt16();
							break;
						case ProtoTypeCode.SByte:
							value = reader.ReadSByte();
							break;
						case ProtoTypeCode.Byte:
							value = reader.ReadByte();
							break;
						case ProtoTypeCode.Int16:
							value = reader.ReadInt16();
							break;
						case ProtoTypeCode.UInt16:
							value = reader.ReadUInt16();
							break;
						case ProtoTypeCode.Int32:
							value = reader.ReadInt32();
							break;
						case ProtoTypeCode.UInt32:
							value = reader.ReadUInt32();
							break;
						case ProtoTypeCode.Int64:
							value = reader.ReadInt64();
							break;
						case ProtoTypeCode.UInt64:
							value = reader.ReadUInt64();
							break;
						case ProtoTypeCode.Single:
							value = reader.ReadSingle();
							break;
						case ProtoTypeCode.Double:
							value = reader.ReadDouble();
							break;
						case ProtoTypeCode.Decimal:
							value = BclHelpers.ReadDecimal(reader);
							break;
						case ProtoTypeCode.DateTime:
							value = BclHelpers.ReadDateTime(reader);
							break;
						case (ProtoTypeCode)17:
							break;
						case ProtoTypeCode.String:
							value = reader.ReadString();
							break;
						default:
							switch (protoTypeCode)
							{
							case ProtoTypeCode.TimeSpan:
								value = BclHelpers.ReadTimeSpan(reader);
								break;
							case ProtoTypeCode.ByteArray:
								value = ProtoReader.AppendBytes((byte[])value, reader);
								break;
							case ProtoTypeCode.Guid:
								value = BclHelpers.ReadGuid(reader);
								break;
							case ProtoTypeCode.Uri:
								value = new Uri(reader.ReadString());
								break;
							}
							break;
						}
					}
				}
			}
			goto IL_375;
			IL_12E:
			throw ProtoReader.AddErrorData(new InvalidOperationException("Expected field " + tag.ToString() + ", but found " + num2.ToString()), reader);
			IL_375:
			bool flag11 = !flag2 && !asListItem && autoCreate;
			if (flag11)
			{
				bool flag12 = type != typeof(string);
				if (flag12)
				{
					value = Activator.CreateInstance(type);
				}
			}
			return flag2;
		}

		// Token: 0x06008570 RID: 34160 RVA: 0x00109B54 File Offset: 0x00107D54
		public static RuntimeTypeModel Create()
		{
			return new RuntimeTypeModel(false);
		}

		// Token: 0x06008571 RID: 34161 RVA: 0x00109B6C File Offset: 0x00107D6C
		protected internal static Type ResolveProxies(Type type)
		{
			bool flag = type == null;
			Type result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool isGenericParameter = type.IsGenericParameter;
				if (isGenericParameter)
				{
					result = null;
				}
				else
				{
					Type underlyingType = Helpers.GetUnderlyingType(type);
					bool flag2 = underlyingType != null;
					if (flag2)
					{
						result = underlyingType;
					}
					else
					{
						string fullName = type.FullName;
						bool flag3 = fullName != null && fullName.StartsWith("System.Data.Entity.DynamicProxies.");
						if (flag3)
						{
							result = type.BaseType;
						}
						else
						{
							Type[] interfaces = type.GetInterfaces();
							for (int i = 0; i < interfaces.Length; i++)
							{
								string fullName2 = interfaces[i].FullName;
								if (fullName2 == "NHibernate.Proxy.INHibernateProxy" || fullName2 == "NHibernate.Proxy.DynamicProxy.IProxy" || fullName2 == "NHibernate.Intercept.IFieldInterceptorAccessor")
								{
									return type.BaseType;
								}
							}
							result = null;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06008572 RID: 34162 RVA: 0x00109C48 File Offset: 0x00107E48
		public bool IsDefined(Type type)
		{
			return this.GetKey(ref type) >= 0;
		}

		// Token: 0x06008573 RID: 34163 RVA: 0x00109C68 File Offset: 0x00107E68
		protected internal int GetKey(ref Type type)
		{
			bool flag = type == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				int keyImpl = this.GetKeyImpl(type);
				bool flag2 = keyImpl < 0;
				if (flag2)
				{
					Type type2 = TypeModel.ResolveProxies(type);
					bool flag3 = type2 != null;
					if (flag3)
					{
						type = type2;
						keyImpl = this.GetKeyImpl(type);
					}
				}
				result = keyImpl;
			}
			return result;
		}

		// Token: 0x06008574 RID: 34164
		protected abstract int GetKeyImpl(Type type);

		// Token: 0x06008575 RID: 34165
		protected internal abstract void Serialize(int key, object value, ProtoWriter dest);

		// Token: 0x06008576 RID: 34166
		protected internal abstract object Deserialize(int key, object value, ProtoReader source);

		// Token: 0x06008577 RID: 34167 RVA: 0x00109CC0 File Offset: 0x00107EC0
		public object DeepClone(object value)
		{
			bool flag = value == null;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Type type = value.GetType();
				int key = this.GetKey(ref type);
				bool flag2 = key >= 0 && !Helpers.IsEnum(type);
				if (flag2)
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (ProtoWriter protoWriter = new ProtoWriter(memoryStream, this, null))
						{
							protoWriter.SetRootObject(value);
							this.Serialize(key, value, protoWriter);
							protoWriter.Close();
						}
						memoryStream.Position = 0L;
						ProtoReader protoReader = null;
						try
						{
							protoReader = ProtoReader.Create(memoryStream, this, null, -1);
							return this.Deserialize(key, null, protoReader);
						}
						finally
						{
							ProtoReader.Recycle(protoReader);
						}
					}
				}
				bool flag3 = type == typeof(byte[]);
				if (flag3)
				{
					byte[] array = (byte[])value;
					byte[] array2 = new byte[array.Length];
					Helpers.BlockCopy(array, 0, array2, 0, array.Length);
					result = array2;
				}
				else
				{
					int num;
					bool flag4 = this.GetWireType(Helpers.GetTypeCode(type), DataFormat.Default, ref type, out num) != WireType.None && num < 0;
					if (flag4)
					{
						result = value;
					}
					else
					{
						using (MemoryStream memoryStream2 = new MemoryStream())
						{
							using (ProtoWriter protoWriter2 = new ProtoWriter(memoryStream2, this, null))
							{
								bool flag5 = !this.TrySerializeAuxiliaryType(protoWriter2, type, DataFormat.Default, 1, value, false);
								if (flag5)
								{
									TypeModel.ThrowUnexpectedType(type);
								}
								protoWriter2.Close();
							}
							memoryStream2.Position = 0L;
							ProtoReader reader = null;
							try
							{
								reader = ProtoReader.Create(memoryStream2, this, null, -1);
								value = null;
								this.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, true, false);
								result = value;
							}
							finally
							{
								ProtoReader.Recycle(reader);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06008578 RID: 34168 RVA: 0x00109ED8 File Offset: 0x001080D8
		protected internal static void ThrowUnexpectedSubtype(Type expected, Type actual)
		{
			bool flag = expected != TypeModel.ResolveProxies(actual);
			if (flag)
			{
				throw new InvalidOperationException("Unexpected sub-type: " + actual.FullName);
			}
		}

		// Token: 0x06008579 RID: 34169 RVA: 0x00109F10 File Offset: 0x00108110
		protected internal static void ThrowUnexpectedType(Type type)
		{
			string str = (type == null) ? "(unknown)" : type.FullName;
			bool flag = type != null;
			if (flag)
			{
				Type baseType = type.BaseType;
				bool flag2 = baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name == "GeneratedMessage`2";
				if (flag2)
				{
					throw new InvalidOperationException("Are you mixing protobuf-net and protobuf-csharp-port? See http://stackoverflow.com/q/11564914; type: " + str);
				}
			}
			throw new InvalidOperationException("Type is not expected, and no contract can be inferred: " + str);
		}

		// Token: 0x0600857A RID: 34170 RVA: 0x00109F8C File Offset: 0x0010818C
		internal static Exception CreateNestedListsNotSupported()
		{
			return new NotSupportedException("Nested or jagged lists and arrays are not supported");
		}

		// Token: 0x0600857B RID: 34171 RVA: 0x00109FA8 File Offset: 0x001081A8
		public static void ThrowCannotCreateInstance(Type type)
		{
			throw new ProtoException("No parameterless constructor found for " + ((type == null) ? "(null)" : type.Name));
		}

		// Token: 0x0600857C RID: 34172 RVA: 0x00109FCC File Offset: 0x001081CC
		internal static string SerializeType(TypeModel model, Type type)
		{
			bool flag = model != null;
			if (flag)
			{
				TypeFormatEventHandler dynamicTypeFormatting = model.DynamicTypeFormatting;
				bool flag2 = dynamicTypeFormatting != null;
				if (flag2)
				{
					TypeFormatEventArgs typeFormatEventArgs = new TypeFormatEventArgs(type);
					dynamicTypeFormatting(model, typeFormatEventArgs);
					bool flag3 = !Helpers.IsNullOrEmpty(typeFormatEventArgs.FormattedName);
					if (flag3)
					{
						return typeFormatEventArgs.FormattedName;
					}
				}
			}
			return type.AssemblyQualifiedName;
		}

		// Token: 0x0600857D RID: 34173 RVA: 0x0010A030 File Offset: 0x00108230
		internal static Type DeserializeType(TypeModel model, string value)
		{
			bool flag = model != null;
			if (flag)
			{
				TypeFormatEventHandler dynamicTypeFormatting = model.DynamicTypeFormatting;
				bool flag2 = dynamicTypeFormatting != null;
				if (flag2)
				{
					TypeFormatEventArgs typeFormatEventArgs = new TypeFormatEventArgs(value);
					dynamicTypeFormatting(model, typeFormatEventArgs);
					bool flag3 = typeFormatEventArgs.Type != null;
					if (flag3)
					{
						return typeFormatEventArgs.Type;
					}
				}
			}
			return Type.GetType(value);
		}

		// Token: 0x0600857E RID: 34174 RVA: 0x0010A090 File Offset: 0x00108290
		public bool CanSerializeContractType(Type type)
		{
			return this.CanSerialize(type, false, true, true);
		}

		// Token: 0x0600857F RID: 34175 RVA: 0x0010A0AC File Offset: 0x001082AC
		public bool CanSerialize(Type type)
		{
			return this.CanSerialize(type, true, true, true);
		}

		// Token: 0x06008580 RID: 34176 RVA: 0x0010A0C8 File Offset: 0x001082C8
		public bool CanSerializeBasicType(Type type)
		{
			return this.CanSerialize(type, true, false, true);
		}

		// Token: 0x06008581 RID: 34177 RVA: 0x0010A0E4 File Offset: 0x001082E4
		private bool CanSerialize(Type type, bool allowBasic, bool allowContract, bool allowLists)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			Type underlyingType = Helpers.GetUnderlyingType(type);
			bool flag2 = underlyingType != null;
			if (flag2)
			{
				type = underlyingType;
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			ProtoTypeCode protoTypeCode = typeCode;
			bool result;
			if (protoTypeCode > ProtoTypeCode.Unknown)
			{
				result = allowBasic;
			}
			else
			{
				int key = this.GetKey(ref type);
				bool flag3 = key >= 0;
				if (flag3)
				{
					result = allowContract;
				}
				else
				{
					if (allowLists)
					{
						Type type2 = null;
						bool isArray = type.IsArray;
						if (isArray)
						{
							bool flag4 = type.GetArrayRank() == 1;
							if (flag4)
							{
								type2 = type.GetElementType();
							}
						}
						else
						{
							type2 = TypeModel.GetListItemType(this, type);
						}
						bool flag5 = type2 != null;
						if (flag5)
						{
							return this.CanSerialize(type2, allowBasic, allowContract, false);
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06008582 RID: 34178 RVA: 0x00101967 File Offset: 0x000FFB67
		public virtual string GetSchema(Type type)
		{
			throw new NotSupportedException();
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06008583 RID: 34179 RVA: 0x0010A1AC File Offset: 0x001083AC
		// (remove) Token: 0x06008584 RID: 34180 RVA: 0x0010A1E4 File Offset: 0x001083E4
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event TypeFormatEventHandler DynamicTypeFormatting;

		// Token: 0x17002A16 RID: 10774
		// (get) Token: 0x06008585 RID: 34181 RVA: 0x0010A21C File Offset: 0x0010841C
		// (set) Token: 0x06008586 RID: 34182 RVA: 0x0010A234 File Offset: 0x00108434
		public bool ForwardsOnly
		{
			get
			{
				return this.forwardsOnly;
			}
			set
			{
				this.forwardsOnly = value;
			}
		}

		// Token: 0x06008587 RID: 34183 RVA: 0x0010A240 File Offset: 0x00108440
		internal virtual Type GetType(string fullName, Assembly context)
		{
			return TypeModel.ResolveKnownType(fullName, this, context);
		}

		// Token: 0x06008588 RID: 34184 RVA: 0x0010A25C File Offset: 0x0010845C
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static Type ResolveKnownType(string name, TypeModel model, Assembly assembly)
		{
			bool flag = Helpers.IsNullOrEmpty(name);
			Type result;
			if (flag)
			{
				result = null;
			}
			else
			{
				try
				{
					Type type = Type.GetType(name);
					bool flag2 = type != null;
					if (flag2)
					{
						return type;
					}
				}
				catch
				{
				}
				try
				{
					int num = name.IndexOf(',');
					string name2 = ((num > 0) ? name.Substring(0, num) : name).Trim();
					bool flag3 = assembly == null;
					if (flag3)
					{
						assembly = Assembly.GetCallingAssembly();
					}
					Type type2 = (assembly == null) ? null : assembly.GetType(name2);
					bool flag4 = type2 != null;
					if (flag4)
					{
						return type2;
					}
				}
				catch
				{
				}
				result = null;
			}
			return result;
		}

		// Token: 0x04002966 RID: 10598
		private static readonly Type ilist = typeof(IList);

		// Token: 0x04002968 RID: 10600
		private bool forwardsOnly;

		// Token: 0x0200194C RID: 6476
		private sealed class DeserializeItemsIterator<T> : TypeModel.DeserializeItemsIterator, IEnumerator<T>, IEnumerator, IDisposable, IEnumerable<T>, IEnumerable
		{
            // Token: 0x06010FD7 RID: 69591 RVA: 0x004529AC File Offset: 0x00450BAC
            IEnumerator<T> IEnumerable<T>.GetEnumerator()

            {
                return this;
            }

            // Token: 0x17003B39 RID: 15161
            // (get) Token: 0x06010FD8 RID: 69592 RVA: 0x004529C0 File Offset: 0x00450BC0
            public new T Current
			{
				get
				{
					return (T)((object)base.Current);
				}
			}

			// Token: 0x06010FD9 RID: 69593 RVA: 0x000FEEFC File Offset: 0x000FD0FC
			void IDisposable.Dispose()
			{
			}

			// Token: 0x06010FDA RID: 69594 RVA: 0x004529DD File Offset: 0x00450BDD
			public DeserializeItemsIterator(TypeModel model, Stream source, PrefixStyle style, int expectedField, SerializationContext context) : base(model, source, model.MapType(typeof(T)), style, expectedField, null, context)
			{
			}
		}

		// Token: 0x0200194D RID: 6477
		private class DeserializeItemsIterator : IEnumerator, IEnumerable
		{
			// Token: 0x06010FDB RID: 69595 RVA: 0x00452A00 File Offset: 0x00450C00
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}

			// Token: 0x06010FDC RID: 69596 RVA: 0x00452A14 File Offset: 0x00450C14
			public bool MoveNext()
			{
				bool flag = this.haveObject;
				if (flag)
				{
					int num;
					this.current = this.model.DeserializeWithLengthPrefix(this.source, null, this.type, this.style, this.expectedField, this.resolver, out num, out this.haveObject, this.context);
				}
				return this.haveObject;
			}

			// Token: 0x06010FDD RID: 69597 RVA: 0x00101967 File Offset: 0x000FFB67
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17003B3A RID: 15162
			// (get) Token: 0x06010FDE RID: 69598 RVA: 0x00452A78 File Offset: 0x00450C78
			public object Current
			{
				get
				{
					return this.current;
				}
			}

			// Token: 0x06010FDF RID: 69599 RVA: 0x00452A90 File Offset: 0x00450C90
			public DeserializeItemsIterator(TypeModel model, Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
			{
				this.haveObject = true;
				this.source = source;
				this.type = type;
				this.style = style;
				this.expectedField = expectedField;
				this.resolver = resolver;
				this.model = model;
				this.context = context;
			}

			// Token: 0x04007D79 RID: 32121
			private bool haveObject;

			// Token: 0x04007D7A RID: 32122
			private object current;

			// Token: 0x04007D7B RID: 32123
			private readonly Stream source;

			// Token: 0x04007D7C RID: 32124
			private readonly Type type;

			// Token: 0x04007D7D RID: 32125
			private readonly PrefixStyle style;

			// Token: 0x04007D7E RID: 32126
			private readonly int expectedField;

			// Token: 0x04007D7F RID: 32127
			private readonly Serializer.TypeResolver resolver;

			// Token: 0x04007D80 RID: 32128
			private readonly TypeModel model;

			// Token: 0x04007D81 RID: 32129
			private readonly SerializationContext context;
		}

		// Token: 0x0200194E RID: 6478
		protected internal enum CallbackType
		{
			// Token: 0x04007D83 RID: 32131
			BeforeSerialize,
			// Token: 0x04007D84 RID: 32132
			AfterSerialize,
			// Token: 0x04007D85 RID: 32133
			BeforeDeserialize,
			// Token: 0x04007D86 RID: 32134
			AfterDeserialize
		}
	}
}
