using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using ProtoBuf.Serializers;

namespace ProtoBuf.Meta
{
	// Token: 0x02000888 RID: 2184
	public sealed class RuntimeTypeModel : TypeModel
	{
		// Token: 0x06008507 RID: 34055 RVA: 0x00106A74 File Offset: 0x00104C74
		private bool GetOption(byte option)
		{
			return (this.options & option) == option;
		}

		// Token: 0x06008508 RID: 34056 RVA: 0x00106A94 File Offset: 0x00104C94
		private void SetOption(byte option, bool value)
		{
			if (value)
			{
				this.options |= option;
			}
			else
			{

                this.options = (byte)(this.options & ~option);
            }
		}

		// Token: 0x17002A06 RID: 10758
		// (get) Token: 0x06008509 RID: 34057 RVA: 0x00106ACC File Offset: 0x00104CCC
		// (set) Token: 0x0600850A RID: 34058 RVA: 0x00106AE5 File Offset: 0x00104CE5
		public bool InferTagFromNameDefault
		{
			get
			{
				return this.GetOption(1);
			}
			set
			{
				this.SetOption(1, value);
			}
		}

		// Token: 0x17002A07 RID: 10759
		// (get) Token: 0x0600850B RID: 34059 RVA: 0x00106AF4 File Offset: 0x00104CF4
		// (set) Token: 0x0600850C RID: 34060 RVA: 0x00106B11 File Offset: 0x00104D11
		public bool AutoAddProtoContractTypesOnly
		{
			get
			{
				return this.GetOption(128);
			}
			set
			{
				this.SetOption(128, value);
			}
		}

		// Token: 0x17002A08 RID: 10760
		// (get) Token: 0x0600850D RID: 34061 RVA: 0x00106B24 File Offset: 0x00104D24
		// (set) Token: 0x0600850E RID: 34062 RVA: 0x00106B40 File Offset: 0x00104D40
		public bool UseImplicitZeroDefaults
		{
			get
			{
				return this.GetOption(32);
			}
			set
			{
				bool flag = !value && this.GetOption(2);
				if (flag)
				{
					throw new InvalidOperationException("UseImplicitZeroDefaults cannot be disabled on the default model");
				}
				this.SetOption(32, value);
			}
		}

		// Token: 0x17002A09 RID: 10761
		// (get) Token: 0x0600850F RID: 34063 RVA: 0x00106B78 File Offset: 0x00104D78
		// (set) Token: 0x06008510 RID: 34064 RVA: 0x00106B92 File Offset: 0x00104D92
		public bool AllowParseableTypes
		{
			get
			{
				return this.GetOption(64);
			}
			set
			{
				this.SetOption(64, value);
			}
		}

		// Token: 0x06008511 RID: 34065 RVA: 0x00106B9F File Offset: 0x00104D9F
		public static void SetMultiThread(bool multiThread)
		{
			RuntimeTypeModel.Singleton.SetMultiThread(multiThread);
		}

		// Token: 0x17002A0A RID: 10762
		// (get) Token: 0x06008512 RID: 34066 RVA: 0x00106BAC File Offset: 0x00104DAC
		public static RuntimeTypeModel Default
		{
			get
			{
				return RuntimeTypeModel.Singleton.Value;
			}
		}

		// Token: 0x17002A0B RID: 10763
		// (get) Token: 0x06008513 RID: 34067 RVA: 0x00106BC4 File Offset: 0x00104DC4
		public static RuntimeTypeModel ThreadDefault
		{
			get
			{
				return RuntimeTypeModel.Singleton.DefaultValue;
			}
		}

		// Token: 0x06008514 RID: 34068 RVA: 0x00106BDC File Offset: 0x00104DDC
		public IEnumerable GetTypes()
		{
			return this.types;
		}

		// Token: 0x06008515 RID: 34069 RVA: 0x00106BF4 File Offset: 0x00104DF4
		public override string GetSchema(Type type)
		{
			BasicList basicList = new BasicList();
			MetaType metaType = null;
			bool flag = false;
			bool flag2 = type == null;
			if (flag2)
			{
				foreach (object obj in this.types)
				{
					MetaType metaType2 = (MetaType)obj;
					MetaType surrogateOrBaseOrSelf = metaType2.GetSurrogateOrBaseOrSelf(false);
					bool flag3 = !basicList.Contains(surrogateOrBaseOrSelf);
					if (flag3)
					{
						basicList.Add(surrogateOrBaseOrSelf);
						this.CascadeDependents(basicList, surrogateOrBaseOrSelf);
					}
				}
			}
			else
			{
				Type underlyingType = Helpers.GetUnderlyingType(type);
				bool flag4 = underlyingType != null;
				if (flag4)
				{
					type = underlyingType;
				}
				WireType wireType;
				flag = (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out wireType, false, false, false, false) != null);
				bool flag5 = !flag;
				if (flag5)
				{
					int num = this.FindOrAddAuto(type, false, false, false);
					bool flag6 = num < 0;
					if (flag6)
					{
						throw new ArgumentException("The type specified is not a contract-type", "type");
					}
					metaType = ((MetaType)this.types[num]).GetSurrogateOrBaseOrSelf(false);
					basicList.Add(metaType);
					this.CascadeDependents(basicList, metaType);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			bool flag7 = !flag;
			if (flag7)
			{
				IEnumerable enumerable = (metaType == null) ? this.types : basicList;
				foreach (object obj2 in enumerable)
				{
					MetaType metaType3 = (MetaType)obj2;
					bool isList = metaType3.IsList;
					if (!isList)
					{
						string @namespace = metaType3.Type.Namespace;
						bool flag8 = !Helpers.IsNullOrEmpty(@namespace);
						if (flag8)
						{
							bool flag9 = @namespace.StartsWith("System.");
							if (!flag9)
							{
								bool flag10 = text == null;
								if (flag10)
								{
									text = @namespace;
								}
								else
								{
									bool flag11 = text == @namespace;
									if (!flag11)
									{
										text = null;
										break;
									}
								}
							}
						}
					}
				}
			}
			bool flag12 = !Helpers.IsNullOrEmpty(text);
			if (flag12)
			{
				stringBuilder.Append("package ").Append(text).Append(';');
				Helpers.AppendLine(stringBuilder);
			}
			bool flag13 = false;
			StringBuilder stringBuilder2 = new StringBuilder();
			MetaType[] array = new MetaType[basicList.Count];
			basicList.CopyTo(array, 0);
			Array.Sort<MetaType>(array, MetaType.Comparer.Default);
			bool flag14 = flag;
			if (flag14)
			{
				Helpers.AppendLine(stringBuilder2).Append("message ").Append(type.Name).Append(" {");
				MetaType.NewLine(stringBuilder2, 1).Append("optional ").Append(this.GetSchemaTypeName(type, DataFormat.Default, false, false, ref flag13)).Append(" value = 1;");
				Helpers.AppendLine(stringBuilder2).Append('}');
			}
			else
			{
				foreach (MetaType metaType4 in array)
				{
					bool flag15 = metaType4.IsList && metaType4 != metaType;
					if (!flag15)
					{
						metaType4.WriteSchema(stringBuilder2, 0, ref flag13);
					}
				}
			}
			bool flag16 = flag13;
			if (flag16)
			{
				stringBuilder.Append("import \"bcl.proto\"; // schema for protobuf-net's handling of core .NET types");
				Helpers.AppendLine(stringBuilder);
			}
			return Helpers.AppendLine(stringBuilder.Append(stringBuilder2)).ToString();
		}

		// Token: 0x06008516 RID: 34070 RVA: 0x00106F38 File Offset: 0x00105138
		private void CascadeDependents(BasicList list, MetaType metaType)
		{
			bool isList = metaType.IsList;
			if (isList)
			{
				Type listItemType = TypeModel.GetListItemType(this, metaType.Type);
				WireType wireType;
				IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(this, DataFormat.Default, listItemType, out wireType, false, false, false, false);
				bool flag = protoSerializer == null;
				if (flag)
				{
					int num = this.FindOrAddAuto(listItemType, false, false, false);
					bool flag2 = num >= 0;
					if (flag2)
					{
						MetaType metaType2 = ((MetaType)this.types[num]).GetSurrogateOrBaseOrSelf(false);
						bool flag3 = !list.Contains(metaType2);
						if (flag3)
						{
							list.Add(metaType2);
							this.CascadeDependents(list, metaType2);
						}
					}
				}
			}
			else
			{
				bool isAutoTuple = metaType.IsAutoTuple;
				MetaType metaType2;
				if (isAutoTuple)
				{
					MemberInfo[] array;
					bool flag4 = MetaType.ResolveTupleConstructor(metaType.Type, out array) != null;
					if (flag4)
					{
						for (int i = 0; i < array.Length; i++)
						{
							Type type = null;
							bool flag5 = array[i] is PropertyInfo;
							if (flag5)
							{
								type = ((PropertyInfo)array[i]).PropertyType;
							}
							else
							{
								bool flag6 = array[i] is FieldInfo;
								if (flag6)
								{
									type = ((FieldInfo)array[i]).FieldType;
								}
							}
							WireType wireType2;
							IProtoSerializer protoSerializer2 = ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out wireType2, false, false, false, false);
							bool flag7 = protoSerializer2 == null;
							if (flag7)
							{
								int num2 = this.FindOrAddAuto(type, false, false, false);
								bool flag8 = num2 >= 0;
								if (flag8)
								{
									metaType2 = ((MetaType)this.types[num2]).GetSurrogateOrBaseOrSelf(false);
									bool flag9 = !list.Contains(metaType2);
									if (flag9)
									{
										list.Add(metaType2);
										this.CascadeDependents(list, metaType2);
									}
								}
							}
						}
					}
				}
				else
				{
					foreach (object obj in metaType.Fields)
					{
						ValueMember valueMember = (ValueMember)obj;
						Type type2 = valueMember.ItemType;
						bool flag10 = type2 == null;
						if (flag10)
						{
							type2 = valueMember.MemberType;
						}
						WireType wireType3;
						IProtoSerializer protoSerializer3 = ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type2, out wireType3, false, false, false, false);
						bool flag11 = protoSerializer3 == null;
						if (flag11)
						{
							int num3 = this.FindOrAddAuto(type2, false, false, false);
							bool flag12 = num3 >= 0;
							if (flag12)
							{
								metaType2 = ((MetaType)this.types[num3]).GetSurrogateOrBaseOrSelf(false);
								bool flag13 = !list.Contains(metaType2);
								if (flag13)
								{
									list.Add(metaType2);
									this.CascadeDependents(list, metaType2);
								}
							}
						}
					}
				}
				bool hasSubtypes = metaType.HasSubtypes;
				if (hasSubtypes)
				{
					foreach (SubType subType in metaType.GetSubtypes())
					{
						metaType2 = subType.DerivedType.GetSurrogateOrSelf();
						bool flag14 = !list.Contains(metaType2);
						if (flag14)
						{
							list.Add(metaType2);
							this.CascadeDependents(list, metaType2);
						}
					}
				}
				metaType2 = metaType.BaseType;
				bool flag15 = metaType2 != null;
				if (flag15)
				{
					metaType2 = metaType2.GetSurrogateOrSelf();
				}
				bool flag16 = metaType2 != null && !list.Contains(metaType2);
				if (flag16)
				{
					list.Add(metaType2);
					this.CascadeDependents(list, metaType2);
				}
			}
		}

		// Token: 0x06008517 RID: 34071 RVA: 0x00107288 File Offset: 0x00105488
		internal RuntimeTypeModel(bool isDefault)
		{
			this.AutoAddMissingTypes = true;
			this.UseImplicitZeroDefaults = true;
			this.SetOption(2, isDefault);
		}

		// Token: 0x17002A0C RID: 10764
		public MetaType this[Type type]
		{
			get
			{
				return (MetaType)this.types[this.FindOrAddAuto(type, true, false, false)];
			}
		}

		// Token: 0x06008519 RID: 34073 RVA: 0x0010730C File Offset: 0x0010550C
		internal MetaType FindWithoutAdd(Type type)
		{
			foreach (object obj in this.types)
			{
				MetaType metaType = (MetaType)obj;
				bool flag = metaType.Type == type;
				if (flag)
				{
					bool pending = metaType.Pending;
					if (pending)
					{
						this.WaitOnLock(metaType);
					}
					return metaType;
				}
			}
			Type type2 = TypeModel.ResolveProxies(type);
			return (type2 == null) ? null : this.FindWithoutAdd(type2);
		}

		// Token: 0x0600851A RID: 34074 RVA: 0x00107384 File Offset: 0x00105584
		private static bool MetaTypeFinderImpl(object value, object ctx)
		{
			return ((MetaType)value).Type == (Type)ctx;
		}

		// Token: 0x0600851B RID: 34075 RVA: 0x001073AC File Offset: 0x001055AC
		private static bool BasicTypeFinderImpl(object value, object ctx)
		{
			return ((RuntimeTypeModel.BasicType)value).Type == (Type)ctx;
		}

		// Token: 0x0600851C RID: 34076 RVA: 0x001073D4 File Offset: 0x001055D4
		private void WaitOnLock(MetaType type)
		{
			int opaqueToken = 0;
			try
			{
				this.TakeLock(ref opaqueToken);
			}
			finally
			{
				this.ReleaseLock(opaqueToken);
			}
		}

		// Token: 0x0600851D RID: 34077 RVA: 0x0010740C File Offset: 0x0010560C
		internal IProtoSerializer TryGetBasicTypeSerializer(Type type)
		{
			int num = this.basicTypes.IndexOf(RuntimeTypeModel.BasicTypeFinder, type);
			bool flag = num >= 0;
			IProtoSerializer result;
			if (flag)
			{
				result = ((RuntimeTypeModel.BasicType)this.basicTypes[num]).Serializer;
			}
			else
			{
				BasicList obj = this.basicTypes;
				lock (obj)
				{
					num = this.basicTypes.IndexOf(RuntimeTypeModel.BasicTypeFinder, type);
					bool flag2 = num >= 0;
					if (flag2)
					{
						result = ((RuntimeTypeModel.BasicType)this.basicTypes[num]).Serializer;
					}
					else
					{
						MetaType.AttributeFamily contractFamily = MetaType.GetContractFamily(this, type, null);
						WireType wireType;
						IProtoSerializer protoSerializer = (contractFamily == MetaType.AttributeFamily.None) ? ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out wireType, false, false, false, false) : null;
						bool flag3 = protoSerializer != null;
						if (flag3)
						{
							this.basicTypes.Add(new RuntimeTypeModel.BasicType(type, protoSerializer));
						}
						result = protoSerializer;
					}
				}
			}
			return result;
		}

		// Token: 0x0600851E RID: 34078 RVA: 0x001074F8 File Offset: 0x001056F8
		public void Clear()
		{
			this.types.Clear();
		}

		// Token: 0x0600851F RID: 34079 RVA: 0x00107508 File Offset: 0x00105708
		internal int FindOrAddAuto(Type type, bool demand, bool addWithContractOnly, bool addEvenIfAutoDisabled)
		{
			int num = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type);
			bool flag = num >= 0;
			int result;
			if (flag)
			{
				MetaType metaType = (MetaType)this.types[num];
				bool pending = metaType.Pending;
				if (pending)
				{
					this.WaitOnLock(metaType);
				}
				result = num;
			}
			else
			{
				bool flag2 = this.AutoAddMissingTypes || addEvenIfAutoDisabled;
				bool flag3 = !Helpers.IsEnum(type) && this.TryGetBasicTypeSerializer(type) != null;
				if (flag3)
				{
					bool flag4 = flag2 && !addWithContractOnly;
					if (flag4)
					{
						throw MetaType.InbuiltType(type);
					}
					result = -1;
				}
				else
				{
					Type type2 = TypeModel.ResolveProxies(type);
					bool flag5 = type2 != null;
					if (flag5)
					{
						num = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type2);
						type = type2;
					}
					bool flag6 = num < 0;
					if (flag6)
					{
						int opaqueToken = 0;
						try
						{
							this.TakeLock(ref opaqueToken);
							MetaType metaType;
							bool flag7 = (metaType = this.RecogniseCommonTypes(type)) == null;
							if (flag7)
							{
								MetaType.AttributeFamily contractFamily = MetaType.GetContractFamily(this, type, null);
								bool flag8 = contractFamily == MetaType.AttributeFamily.AutoTuple;
								if (flag8)
								{
									addEvenIfAutoDisabled = (flag2 = true);
								}
								bool flag9 = !flag2 || (!Helpers.IsEnum(type) && addWithContractOnly && contractFamily == MetaType.AttributeFamily.None);
								if (flag9)
								{
									if (demand)
									{
										TypeModel.ThrowUnexpectedType(type);
									}
									return num;
								}
								metaType = this.Create(type);
							}
							metaType.Pending = true;
							bool flag10 = false;
							int num2 = this.types.IndexOf(RuntimeTypeModel.MetaTypeFinder, type);
							bool flag11 = num2 < 0;
							if (flag11)
							{
								this.ThrowIfFrozen();
								num = this.types.Add(metaType);
								flag10 = true;
							}
							else
							{
								num = num2;
							}
							bool flag12 = flag10;
							if (flag12)
							{
								metaType.ApplyDefaultBehaviour();
								metaType.Pending = false;
							}
						}
						finally
						{
							this.ReleaseLock(opaqueToken);
						}
					}
					result = num;
				}
			}
			return result;
		}

		// Token: 0x06008520 RID: 34080 RVA: 0x001076E8 File Offset: 0x001058E8
		private MetaType RecogniseCommonTypes(Type type)
		{
			return null;
		}

		// Token: 0x06008521 RID: 34081 RVA: 0x001076FC File Offset: 0x001058FC
		private MetaType Create(Type type)
		{
			this.ThrowIfFrozen();
			return new MetaType(this, type, this.defaultFactory);
		}

		// Token: 0x06008522 RID: 34082 RVA: 0x00107724 File Offset: 0x00105924
		public MetaType Add(Type type, bool applyDefaultBehaviour)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			MetaType metaType = this.FindWithoutAdd(type);
			bool flag2 = metaType != null;
			MetaType result;
			if (flag2)
			{
				result = metaType;
			}
			else
			{
				int opaqueToken = 0;
				bool flag3 = type.IsInterface && base.MapType(MetaType.ienumerable).IsAssignableFrom(type) && TypeModel.GetListItemType(this, type) == null;
				if (flag3)
				{
					throw new ArgumentException("IEnumerable[<T>] data cannot be used as a meta-type unless an Add method can be resolved");
				}
				try
				{
					metaType = this.RecogniseCommonTypes(type);
					bool flag4 = metaType != null;
					if (flag4)
					{
						bool flag5 = !applyDefaultBehaviour;
						if (flag5)
						{
							throw new ArgumentException("Default behaviour must be observed for certain types with special handling; " + type.FullName, "applyDefaultBehaviour");
						}
						applyDefaultBehaviour = false;
					}
					bool flag6 = metaType == null;
					if (flag6)
					{
						metaType = this.Create(type);
					}
					metaType.Pending = true;
					this.TakeLock(ref opaqueToken);
					bool flag7 = this.FindWithoutAdd(type) != null;
					if (flag7)
					{
						throw new ArgumentException("Duplicate type", "type");
					}
					this.ThrowIfFrozen();
					this.types.Add(metaType);
					bool flag8 = applyDefaultBehaviour;
					if (flag8)
					{
						metaType.ApplyDefaultBehaviour();
					}
					metaType.Pending = false;
				}
				finally
				{
					this.ReleaseLock(opaqueToken);
				}
				result = metaType;
			}
			return result;
		}

		// Token: 0x17002A0D RID: 10765
		// (get) Token: 0x06008523 RID: 34083 RVA: 0x0010786C File Offset: 0x00105A6C
		// (set) Token: 0x06008524 RID: 34084 RVA: 0x00107888 File Offset: 0x00105A88
		public bool AutoAddMissingTypes
		{
			get
			{
				return this.GetOption(8);
			}
			set
			{
				bool flag = !value && this.GetOption(2);
				if (flag)
				{
					throw new InvalidOperationException("The default model must allow missing types");
				}
				this.ThrowIfFrozen();
				this.SetOption(8, value);
			}
		}

		// Token: 0x06008525 RID: 34085 RVA: 0x001078C4 File Offset: 0x00105AC4
		private void ThrowIfFrozen()
		{
			bool option = this.GetOption(4);
			if (option)
			{
				throw new InvalidOperationException("The model cannot be changed once frozen");
			}
		}

		// Token: 0x06008526 RID: 34086 RVA: 0x001078E8 File Offset: 0x00105AE8
		public void Freeze()
		{
			bool option = this.GetOption(2);
			if (option)
			{
				throw new InvalidOperationException("The default model cannot be frozen");
			}
			this.SetOption(4, true);
		}

		// Token: 0x06008527 RID: 34087 RVA: 0x00107918 File Offset: 0x00105B18
		protected override int GetKeyImpl(Type type)
		{
			return this.GetKey(type, false, true);
		}

		// Token: 0x06008528 RID: 34088 RVA: 0x00107934 File Offset: 0x00105B34
		internal int GetKey(Type type, bool demand, bool getBaseKey)
		{
			Helpers.DebugAssert(type != null);
			int result;
			try
			{
				int num = this.FindOrAddAuto(type, demand, true, false);
				bool flag = num >= 0;
				if (flag)
				{
					MetaType metaType = (MetaType)this.types[num];
					if (getBaseKey)
					{
						metaType = MetaType.GetRootType(metaType);
						num = this.FindOrAddAuto(metaType.Type, true, true, false);
					}
				}
				result = num;
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch (Exception ex)
			{
				bool flag2 = ex.Message.IndexOf(type.FullName) >= 0;
				if (flag2)
				{
					throw;
				}
				throw new ProtoException(ex.Message + " (" + type.FullName + ")", ex);
			}
			return result;
		}

		// Token: 0x06008529 RID: 34089 RVA: 0x00107A04 File Offset: 0x00105C04
		protected internal override void Serialize(int key, object value, ProtoWriter dest)
		{
			((MetaType)this.types[key]).Serializer.Write(value, dest);
		}

		// Token: 0x0600852A RID: 34090 RVA: 0x00107A28 File Offset: 0x00105C28
		protected internal override object Deserialize(int key, object value, ProtoReader source)
		{
			IProtoSerializer serializer = ((MetaType)this.types[key]).Serializer;
			bool flag = value == null && Helpers.IsValueType(serializer.ExpectedType);
			object result;
			if (flag)
			{
				bool requiresOldValue = serializer.RequiresOldValue;
				if (requiresOldValue)
				{
					value = Activator.CreateInstance(serializer.ExpectedType);
				}
				result = serializer.Read(value, source);
			}
			else
			{
				result = serializer.Read(value, source);
			}
			return result;
		}

		// Token: 0x0600852B RID: 34091 RVA: 0x00107A94 File Offset: 0x00105C94
		internal bool IsPrepared(Type type)
		{
			MetaType metaType = this.FindWithoutAdd(type);
			return metaType != null && metaType.IsPrepared();
		}

		// Token: 0x0600852C RID: 34092 RVA: 0x00107ABC File Offset: 0x00105CBC
		internal EnumSerializer.EnumPair[] GetEnumMap(Type type)
		{
			int num = this.FindOrAddAuto(type, false, false, false);
			return (num < 0) ? null : ((MetaType)this.types[num]).GetEnumMap();
		}

		// Token: 0x17002A0E RID: 10766
		// (get) Token: 0x0600852D RID: 34093 RVA: 0x00107AF8 File Offset: 0x00105CF8
		// (set) Token: 0x0600852E RID: 34094 RVA: 0x00107B10 File Offset: 0x00105D10
		public int MetadataTimeoutMilliseconds
		{
			get
			{
				return this.metadataTimeoutMilliseconds;
			}
			set
			{
				bool flag = value <= 0;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("MetadataTimeoutMilliseconds");
				}
				this.metadataTimeoutMilliseconds = value;
			}
		}

		// Token: 0x17002A0F RID: 10767
		// (get) Token: 0x0600852F RID: 34095 RVA: 0x00107B3C File Offset: 0x00105D3C
		public int LockCount
		{
			get
			{
				return this.lockCount;
			}
		}

		// Token: 0x06008530 RID: 34096 RVA: 0x00107B54 File Offset: 0x00105D54
		internal void TakeLock(ref int opaqueToken)
		{
			opaqueToken = 0;
			bool flag = Monitor.TryEnter(this.types, this.metadataTimeoutMilliseconds);
			if (flag)
			{
				opaqueToken = this.GetContention();
				this.lockCount++;
				return;
			}
			this.AddContention();
			throw new TimeoutException("Timeout while inspecting metadata; this may indicate a deadlock. This can often be avoided by preparing necessary serializers during application initialization, rather than allowing multiple threads to perform the initial metadata inspection; please also see the LockContended event");
		}

		// Token: 0x06008531 RID: 34097 RVA: 0x00107BA8 File Offset: 0x00105DA8
		private int GetContention()
		{
			return Interlocked.CompareExchange(ref this.contentionCounter, 0, 0);
		}

		// Token: 0x06008532 RID: 34098 RVA: 0x00107BC7 File Offset: 0x00105DC7
		private void AddContention()
		{
			Interlocked.Increment(ref this.contentionCounter);
		}

		// Token: 0x06008533 RID: 34099 RVA: 0x00107BD8 File Offset: 0x00105DD8
		internal void ReleaseLock(int opaqueToken)
		{
			bool flag = opaqueToken != 0;
			if (flag)
			{
				Monitor.Exit(this.types);
				bool flag2 = opaqueToken != this.GetContention();
				if (flag2)
				{
					LockContentedEventHandler lockContended = this.LockContended;
					bool flag3 = lockContended != null;
					if (flag3)
					{
						string stackTrace;
						try
						{
							throw new ProtoException();
						}
						catch (Exception ex)
						{
							stackTrace = ex.StackTrace;
						}
						lockContended(this, new LockContentedEventArgs(stackTrace));
					}
				}
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06008534 RID: 34100 RVA: 0x00107C54 File Offset: 0x00105E54
		// (remove) Token: 0x06008535 RID: 34101 RVA: 0x00107C8C File Offset: 0x00105E8C
		public event LockContentedEventHandler LockContended;

		// Token: 0x06008536 RID: 34102 RVA: 0x00107CC4 File Offset: 0x00105EC4
		internal void ResolveListTypes(Type type, ref Type itemType, ref Type defaultType)
		{
			bool flag = type == null;
			if (!flag)
			{
				bool flag2 = Helpers.GetTypeCode(type) != ProtoTypeCode.Unknown;
				if (!flag2)
				{
					bool ignoreListHandling = this[type].IgnoreListHandling;
					if (!ignoreListHandling)
					{
						bool isArray = type.IsArray;
						if (isArray)
						{
							bool flag3 = type.GetArrayRank() != 1;
							if (flag3)
							{
								throw new NotSupportedException("Multi-dimension arrays are supported");
							}
							itemType = type.GetElementType();
							bool flag4 = itemType == base.MapType(typeof(byte));
							if (flag4)
							{
								Type type2;
								itemType = (type2 = null);
								defaultType = type2;
							}
							else
							{
								defaultType = type;
							}
						}
						bool flag5 = itemType == null;
						if (flag5)
						{
							itemType = TypeModel.GetListItemType(this, type);
						}
						bool flag6 = itemType != null;
						if (flag6)
						{
							Type type3 = null;
							Type type4 = null;
							this.ResolveListTypes(itemType, ref type3, ref type4);
							bool flag7 = type3 != null;
							if (flag7)
							{
								throw TypeModel.CreateNestedListsNotSupported();
							}
						}
						bool flag8 = itemType != null && defaultType == null;
						if (flag8)
						{
							bool flag9 = type.IsClass && !type.IsAbstract && Helpers.GetConstructor(type, Helpers.EmptyTypes, true) != null;
							if (flag9)
							{
								defaultType = type;
							}
							bool flag10 = defaultType == null;
							if (flag10)
							{
								bool isInterface = type.IsInterface;
								if (isInterface)
								{
									Type[] genericArguments;
									
									if (type.IsGenericType && type.GetGenericTypeDefinition() == base.MapType(typeof(IDictionary<,>)) && itemType == base.MapType(typeof(KeyValuePair<,>)).MakeGenericType(genericArguments = type.GetGenericArguments()))
									{
										defaultType = base.MapType(typeof(Dictionary<, >)).MakeGenericType(genericArguments);
									}
									else
									{
										defaultType = base.MapType(typeof(List<>)).MakeGenericType(new Type[]
										{
											itemType
										});
									}
								}
							}
							bool flag12 = defaultType != null && !Helpers.IsAssignableFrom(type, defaultType);
							if (flag12)
							{
								defaultType = null;
							}
						}
					}
				}
			}
		}

		// Token: 0x06008537 RID: 34103 RVA: 0x00107EC0 File Offset: 0x001060C0
		internal string GetSchemaTypeName(Type effectiveType, DataFormat dataFormat, bool asReference, bool dynamicType, ref bool requiresBclImport)
		{
			Type underlyingType = Helpers.GetUnderlyingType(effectiveType);
			bool flag = underlyingType != null;
			if (flag)
			{
				effectiveType = underlyingType;
			}
			bool flag2 = effectiveType == base.MapType(typeof(byte[]));
			string result;
			if (flag2)
			{
				result = "bytes";
			}
			else
			{
				WireType wireType;
				IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(this, dataFormat, effectiveType, out wireType, false, false, false, false);
				bool flag3 = protoSerializer == null;
				if (flag3)
				{
					bool flag4 = asReference || dynamicType;
					if (flag4)
					{
						requiresBclImport = true;
						result = "bcl.NetObjectProxy";
					}
					else
					{
						result = this[effectiveType].GetSurrogateOrBaseOrSelf(true).GetSchemaTypeName();
					}
				}
				else
				{
					bool flag5 = protoSerializer is ParseableSerializer;
					if (!flag5)
					{
						ProtoTypeCode typeCode = Helpers.GetTypeCode(effectiveType);
						switch (typeCode)
						{
						case ProtoTypeCode.Boolean:
							return "bool";
						case ProtoTypeCode.Char:
						case ProtoTypeCode.Byte:
						case ProtoTypeCode.UInt16:
						case ProtoTypeCode.UInt32:
							if (dataFormat != DataFormat.FixedSize)
							{
								return "uint32";
							}
							return "fixed32";
						case ProtoTypeCode.SByte:
						case ProtoTypeCode.Int16:
						case ProtoTypeCode.Int32:
							if (dataFormat == DataFormat.ZigZag)
							{
								return "sint32";
							}
							if (dataFormat != DataFormat.FixedSize)
							{
								return "int32";
							}
							return "sfixed32";
						case ProtoTypeCode.Int64:
							if (dataFormat == DataFormat.ZigZag)
							{
								return "sint64";
							}
							if (dataFormat != DataFormat.FixedSize)
							{
								return "int64";
							}
							return "sfixed64";
						case ProtoTypeCode.UInt64:
							if (dataFormat != DataFormat.FixedSize)
							{
								return "uint64";
							}
							return "fixed64";
						case ProtoTypeCode.Single:
							return "float";
						case ProtoTypeCode.Double:
							return "double";
						case ProtoTypeCode.Decimal:
							requiresBclImport = true;
							return "bcl.Decimal";
						case ProtoTypeCode.DateTime:
							requiresBclImport = true;
							return "bcl.DateTime";
						case (ProtoTypeCode)17:
							break;
						case ProtoTypeCode.String:
							if (asReference)
							{
								requiresBclImport = true;
							}
							return asReference ? "bcl.NetObjectProxy" : "string";
						default:
							if (typeCode == ProtoTypeCode.TimeSpan)
							{
								requiresBclImport = true;
								return "bcl.TimeSpan";
							}
							if (typeCode == ProtoTypeCode.Guid)
							{
								requiresBclImport = true;
								return "bcl.Guid";
							}
							break;
						}
						throw new NotSupportedException("No .proto map found for: " + effectiveType.FullName);
					}
					if (asReference)
					{
						requiresBclImport = true;
					}
					result = (asReference ? "bcl.NetObjectProxy" : "string");
				}
			}
			return result;
		}

		// Token: 0x06008538 RID: 34104 RVA: 0x00108121 File Offset: 0x00106321
		public void SetDefaultFactory(MethodInfo methodInfo)
		{
			this.VerifyFactory(methodInfo, null);
			this.defaultFactory = methodInfo;
		}

		// Token: 0x06008539 RID: 34105 RVA: 0x00108134 File Offset: 0x00106334
		internal void VerifyFactory(MethodInfo factory, Type type)
		{
			bool flag = factory != null;
			if (flag)
			{
				bool flag2 = type != null && Helpers.IsValueType(type);
				if (flag2)
				{
					throw new InvalidOperationException();
				}
				bool flag3 = !factory.IsStatic;
				if (flag3)
				{
					throw new ArgumentException("A factory-method must be static", "factory");
				}
				bool flag4 = type != null && factory.ReturnType != type && factory.ReturnType != base.MapType(typeof(object));
				if (flag4)
				{
					throw new ArgumentException("The factory-method must return object" + ((type == null) ? "" : (" or " + type.FullName)), "factory");
				}
				bool flag5 = !CallbackSet.CheckCallbackParameters(this, factory);
				if (flag5)
				{
					throw new ArgumentException("Invalid factory signature in " + factory.DeclaringType.FullName + "." + factory.Name, "factory");
				}
			}
		}

		// Token: 0x0400294D RID: 10573
		private byte options;

		// Token: 0x0400294E RID: 10574
		private const byte OPTIONS_InferTagFromNameDefault = 1;

		// Token: 0x0400294F RID: 10575
		private const byte OPTIONS_IsDefaultModel = 2;

		// Token: 0x04002950 RID: 10576
		private const byte OPTIONS_Frozen = 4;

		// Token: 0x04002951 RID: 10577
		private const byte OPTIONS_AutoAddMissingTypes = 8;

		// Token: 0x04002952 RID: 10578
		private const byte OPTIONS_UseImplicitZeroDefaults = 32;

		// Token: 0x04002953 RID: 10579
		private const byte OPTIONS_AllowParseableTypes = 64;

		// Token: 0x04002954 RID: 10580
		private const byte OPTIONS_AutoAddProtoContractTypesOnly = 128;

		// Token: 0x04002955 RID: 10581
		private static readonly BasicList.MatchPredicate MetaTypeFinder = new BasicList.MatchPredicate(RuntimeTypeModel.MetaTypeFinderImpl);

		// Token: 0x04002956 RID: 10582
		private static readonly BasicList.MatchPredicate BasicTypeFinder = new BasicList.MatchPredicate(RuntimeTypeModel.BasicTypeFinderImpl);

		// Token: 0x04002957 RID: 10583
		private BasicList basicTypes = new BasicList();

		// Token: 0x04002958 RID: 10584
		private readonly BasicList types = new BasicList();

		// Token: 0x04002959 RID: 10585
		private int metadataTimeoutMilliseconds = 5000;

		// Token: 0x0400295A RID: 10586
		private int lockCount;

		// Token: 0x0400295B RID: 10587
		private int contentionCounter = 1;

		// Token: 0x0400295D RID: 10589
		private MethodInfo defaultFactory;

		// Token: 0x02001949 RID: 6473
		private sealed class Singleton
		{
			// Token: 0x06010FCD RID: 69581 RVA: 0x0000311A File Offset: 0x0000131A
			private Singleton()
			{
			}

			// Token: 0x06010FCE RID: 69582 RVA: 0x00452894 File Offset: 0x00450A94
			public static void SetMultiThread(bool multiThread)
			{
				if (multiThread)
				{
					RuntimeTypeModel.Singleton.ThreadValue = new RuntimeTypeModel(true);
					RuntimeTypeModel.Singleton.DefaultValue = RuntimeTypeModel.Singleton.ThreadValue;
				}
				else
				{
					RuntimeTypeModel.Singleton.DefaultValue = RuntimeTypeModel.Singleton.Value;
				}
			}

			// Token: 0x04007D73 RID: 32115
			internal static readonly RuntimeTypeModel Value = new RuntimeTypeModel(true);

			// Token: 0x04007D74 RID: 32116
			private static RuntimeTypeModel ThreadValue = null;

			// Token: 0x04007D75 RID: 32117
			internal static RuntimeTypeModel DefaultValue = null;
		}

		// Token: 0x0200194A RID: 6474
		private sealed class BasicType
		{
			// Token: 0x17003B37 RID: 15159
			// (get) Token: 0x06010FD0 RID: 69584 RVA: 0x004528E8 File Offset: 0x00450AE8
			public Type Type
			{
				get
				{
					return this.type;
				}
			}

			// Token: 0x17003B38 RID: 15160
			// (get) Token: 0x06010FD1 RID: 69585 RVA: 0x00452900 File Offset: 0x00450B00
			public IProtoSerializer Serializer
			{
				get
				{
					return this.serializer;
				}
			}

			// Token: 0x06010FD2 RID: 69586 RVA: 0x00452918 File Offset: 0x00450B18
			public BasicType(Type type, IProtoSerializer serializer)
			{
				this.type = type;
				this.serializer = serializer;
			}

			// Token: 0x04007D76 RID: 32118
			private readonly Type type;

			// Token: 0x04007D77 RID: 32119
			private readonly IProtoSerializer serializer;
		}
	}
}
