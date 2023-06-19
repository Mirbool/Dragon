using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ProtoBuf.Serializers;

namespace ProtoBuf.Meta
{
	// Token: 0x02000887 RID: 2183
	public class MetaType : ISerializerProxy
	{
		// Token: 0x060084AE RID: 33966 RVA: 0x00103558 File Offset: 0x00101758
		public override string ToString()
		{
			return this.type.ToString();
		}

		// Token: 0x170029F0 RID: 10736
		// (get) Token: 0x060084AF RID: 33967 RVA: 0x00103578 File Offset: 0x00101778
		IProtoSerializer ISerializerProxy.Serializer
		{
			get
			{
				return this.Serializer;
			}
		}

		// Token: 0x170029F1 RID: 10737
		// (get) Token: 0x060084B0 RID: 33968 RVA: 0x00103590 File Offset: 0x00101790
		public MetaType BaseType
		{
			get
			{
				return this.baseType;
			}
		}

		// Token: 0x170029F2 RID: 10738
		// (get) Token: 0x060084B1 RID: 33969 RVA: 0x001035A8 File Offset: 0x001017A8
		internal TypeModel Model
		{
			get
			{
				return this.model;
			}
		}

		// Token: 0x170029F3 RID: 10739
		// (get) Token: 0x060084B2 RID: 33970 RVA: 0x001035C0 File Offset: 0x001017C0
		// (set) Token: 0x060084B3 RID: 33971 RVA: 0x001035DC File Offset: 0x001017DC
		public bool IncludeSerializerMethod
		{
			get
			{
				return !this.HasFlag(8);
			}
			set
			{
				this.SetFlag(8, !value, true);
			}
		}

		// Token: 0x170029F4 RID: 10740
		// (get) Token: 0x060084B4 RID: 33972 RVA: 0x001035EC File Offset: 0x001017EC
		// (set) Token: 0x060084B5 RID: 33973 RVA: 0x00103606 File Offset: 0x00101806
		public bool AsReferenceDefault
		{
			get
			{
				return this.HasFlag(32);
			}
			set
			{
				this.SetFlag(32, value, true);
			}
		}

		// Token: 0x060084B6 RID: 33974 RVA: 0x00103614 File Offset: 0x00101814
		private bool IsValidSubType(Type subType)
		{
			return this.type.IsAssignableFrom(subType);
		}

		// Token: 0x060084B7 RID: 33975 RVA: 0x00103634 File Offset: 0x00101834
		public MetaType AddSubType(int fieldNumber, Type derivedType)
		{
			return this.AddSubType(fieldNumber, derivedType, DataFormat.Default);
		}

		// Token: 0x060084B8 RID: 33976 RVA: 0x00103650 File Offset: 0x00101850
		public MetaType AddSubType(int fieldNumber, Type derivedType, DataFormat dataFormat)
		{
			bool flag = derivedType == null;
			if (flag)
			{
				throw new ArgumentNullException("derivedType");
			}
			bool flag2 = fieldNumber < 1;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			bool flag3 = (!this.type.IsClass && !this.type.IsInterface) || this.type.IsSealed;
			if (flag3)
			{
				throw new InvalidOperationException("Sub-types can only be added to non-sealed classes");
			}
			bool flag4 = !this.IsValidSubType(derivedType);
			if (flag4)
			{
				throw new ArgumentException(derivedType.Name + " is not a valid sub-type of " + this.type.Name, "derivedType");
			}
			MetaType metaType = this.model[derivedType];
			this.ThrowIfFrozen();
			metaType.ThrowIfFrozen();
			SubType value = new SubType(fieldNumber, metaType, dataFormat);
			this.ThrowIfFrozen();
			metaType.SetBaseType(this);
			bool flag5 = this.subTypes == null;
			if (flag5)
			{
				this.subTypes = new BasicList();
			}
			this.subTypes.Add(value);
			return this;
		}

		// Token: 0x060084B9 RID: 33977 RVA: 0x00103758 File Offset: 0x00101958
		private void SetBaseType(MetaType baseType)
		{
			bool flag = baseType == null;
			if (flag)
			{
				throw new ArgumentNullException("baseType");
			}
			bool flag2 = this.baseType == baseType;
			if (!flag2)
			{
				bool flag3 = this.baseType != null;
				if (flag3)
				{
					throw new InvalidOperationException("A type can only participate in one inheritance hierarchy");
				}
				for (MetaType metaType = baseType; metaType != null; metaType = metaType.baseType)
				{
					bool flag4 = metaType == this;
					if (flag4)
					{
						throw new InvalidOperationException("Cyclic inheritance is not allowed");
					}
				}
				this.baseType = baseType;
			}
		}

		// Token: 0x170029F5 RID: 10741
		// (get) Token: 0x060084BA RID: 33978 RVA: 0x001037D4 File Offset: 0x001019D4
		public bool HasCallbacks
		{
			get
			{
				return this.callbacks != null && this.callbacks.NonTrivial;
			}
		}

		// Token: 0x170029F6 RID: 10742
		// (get) Token: 0x060084BB RID: 33979 RVA: 0x001037FC File Offset: 0x001019FC
		public bool HasSubtypes
		{
			get
			{
				return this.subTypes != null && this.subTypes.Count != 0;
			}
		}

		// Token: 0x170029F7 RID: 10743
		// (get) Token: 0x060084BC RID: 33980 RVA: 0x00103828 File Offset: 0x00101A28
		public CallbackSet Callbacks
		{
			get
			{
				bool flag = this.callbacks == null;
				if (flag)
				{
					this.callbacks = new CallbackSet(this);
				}
				return this.callbacks;
			}
		}

		// Token: 0x170029F8 RID: 10744
		// (get) Token: 0x060084BD RID: 33981 RVA: 0x0010385C File Offset: 0x00101A5C
		private bool IsValueType
		{
			get
			{
				return this.type.IsValueType;
			}
		}

		// Token: 0x060084BE RID: 33982 RVA: 0x0010387C File Offset: 0x00101A7C
		public MetaType SetCallbacks(MethodInfo beforeSerialize, MethodInfo afterSerialize, MethodInfo beforeDeserialize, MethodInfo afterDeserialize)
		{
			CallbackSet callbackSet = this.Callbacks;
			callbackSet.BeforeSerialize = beforeSerialize;
			callbackSet.AfterSerialize = afterSerialize;
			callbackSet.BeforeDeserialize = beforeDeserialize;
			callbackSet.AfterDeserialize = afterDeserialize;
			return this;
		}

		// Token: 0x060084BF RID: 33983 RVA: 0x001038B8 File Offset: 0x00101AB8
		public MetaType SetCallbacks(string beforeSerialize, string afterSerialize, string beforeDeserialize, string afterDeserialize)
		{
			bool isValueType = this.IsValueType;
			if (isValueType)
			{
				throw new InvalidOperationException();
			}
			CallbackSet callbackSet = this.Callbacks;
			callbackSet.BeforeSerialize = this.ResolveMethod(beforeSerialize, true);
			callbackSet.AfterSerialize = this.ResolveMethod(afterSerialize, true);
			callbackSet.BeforeDeserialize = this.ResolveMethod(beforeDeserialize, true);
			callbackSet.AfterDeserialize = this.ResolveMethod(afterDeserialize, true);
			return this;
		}

		// Token: 0x060084C0 RID: 33984 RVA: 0x00103920 File Offset: 0x00101B20
		internal string GetSchemaTypeName()
		{
			bool flag = this.surrogate != null;
			string result;
			if (flag)
			{
				result = this.model[this.surrogate].GetSchemaTypeName();
			}
			else
			{
				bool flag2 = !Helpers.IsNullOrEmpty(this.name);
				if (flag2)
				{
					result = this.name;
				}
				else
				{
					string text = this.type.Name;
					bool isGenericType = this.type.IsGenericType;
					if (isGenericType)
					{
						StringBuilder stringBuilder = new StringBuilder(text);
						int num = text.IndexOf('`');
						bool flag3 = num >= 0;
						if (flag3)
						{
							stringBuilder.Length = num;
						}
						foreach (Type type in this.type.GetGenericArguments())
						{
							stringBuilder.Append('_');
							Type type2 = type;
							int key = this.model.GetKey(ref type2);
							MetaType metaType;
						
							if (key >= 0 && (metaType = this.model[type2]) != null && metaType.surrogate == null)
							{
								stringBuilder.Append(metaType.GetSchemaTypeName());
							}
							else
							{
								stringBuilder.Append(type2.Name);
							}
						}
						result = stringBuilder.ToString();
					}
					else
					{
						result = text;
					}
				}
			}
			return result;
		}

		// Token: 0x170029F9 RID: 10745
		// (get) Token: 0x060084C1 RID: 33985 RVA: 0x00103A68 File Offset: 0x00101C68
		// (set) Token: 0x060084C2 RID: 33986 RVA: 0x00103A80 File Offset: 0x00101C80
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.ThrowIfFrozen();
				this.name = value;
			}
		}

		// Token: 0x060084C3 RID: 33987 RVA: 0x00103A94 File Offset: 0x00101C94
		public MetaType SetFactory(MethodInfo factory)
		{
			this.model.VerifyFactory(factory, this.type);
			this.ThrowIfFrozen();
			this.factory = factory;
			return this;
		}

		// Token: 0x060084C4 RID: 33988 RVA: 0x00103AC8 File Offset: 0x00101CC8
		public MetaType SetFactory(string factory)
		{
			return this.SetFactory(this.ResolveMethod(factory, false));
		}

		// Token: 0x060084C5 RID: 33989 RVA: 0x00103AE8 File Offset: 0x00101CE8
		private MethodInfo ResolveMethod(string name, bool instance)
		{
			bool flag = Helpers.IsNullOrEmpty(name);
			MethodInfo result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = (instance ? Helpers.GetInstanceMethod(this.type, name) : Helpers.GetStaticMethod(this.type, name));
			}
			return result;
		}

		// Token: 0x060084C6 RID: 33990 RVA: 0x00103B28 File Offset: 0x00101D28
		internal static Exception InbuiltType(Type type)
		{
			return new ArgumentException("Data of this type has inbuilt behaviour, and cannot be added to a model in this way: " + type.FullName);
		}

		// Token: 0x060084C7 RID: 33991 RVA: 0x00103B50 File Offset: 0x00101D50
		internal MetaType(RuntimeTypeModel model, Type type, MethodInfo factory)
		{
			this.factory = factory;
			bool flag = model == null;
			if (flag)
			{
				throw new ArgumentNullException("model");
			}
			bool flag2 = type == null;
			if (flag2)
			{
				throw new ArgumentNullException("type");
			}
			IProtoSerializer protoSerializer = model.TryGetBasicTypeSerializer(type);
			bool flag3 = protoSerializer != null;
			if (flag3)
			{
				throw MetaType.InbuiltType(type);
			}
			this.type = type;
			this.model = model;
			bool flag4 = Helpers.IsEnum(type);
			if (flag4)
			{
				this.EnumPassthru = type.IsDefined(model.MapType(typeof(FlagsAttribute)), false);
			}
		}

		// Token: 0x060084C8 RID: 33992 RVA: 0x00103BF0 File Offset: 0x00101DF0
		protected internal void ThrowIfFrozen()
		{
			bool flag = (this.flags & 4) > 0;
			if (flag)
			{
				throw new InvalidOperationException("The type cannot be changed once a serializer has been generated for " + this.type.FullName);
			}
		}

		// Token: 0x170029FA RID: 10746
		// (get) Token: 0x060084C9 RID: 33993 RVA: 0x00103C2C File Offset: 0x00101E2C
		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x170029FB RID: 10747
		// (get) Token: 0x060084CA RID: 33994 RVA: 0x00103C44 File Offset: 0x00101E44
		internal IProtoTypeSerializer Serializer
		{
			get
			{
				bool flag = this.serializer == null;
				if (flag)
				{
					int opaqueToken = 0;
					try
					{
						this.model.TakeLock(ref opaqueToken);
						bool flag2 = this.serializer == null;
						if (flag2)
						{
							this.SetFlag(4, true, false);
							this.serializer = this.BuildSerializer();
						}
					}
					finally
					{
						this.model.ReleaseLock(opaqueToken);
					}
				}
				return this.serializer;
			}
		}

		// Token: 0x170029FC RID: 10748
		// (get) Token: 0x060084CB RID: 33995 RVA: 0x00103CC4 File Offset: 0x00101EC4
		internal bool IsList
		{
			get
			{
				Type type = this.IgnoreListHandling ? null : TypeModel.GetListItemType(this.model, this.type);
				return type != null;
			}
		}

		// Token: 0x060084CC RID: 33996 RVA: 0x00103CF8 File Offset: 0x00101EF8
		private IProtoTypeSerializer BuildSerializer()
		{
			bool flag = Helpers.IsEnum(this.type);
			IProtoTypeSerializer result;
			if (flag)
			{
				result = new TagDecorator(1, WireType.Variant, false, new EnumSerializer(this.type, this.GetEnumMap()));
			}
			else
			{
				Type type = this.IgnoreListHandling ? null : TypeModel.GetListItemType(this.model, this.type);
				bool flag2 = type != null;
				if (flag2)
				{
					bool flag3 = this.surrogate != null;
					if (flag3)
					{
						throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot use a surrogate");
					}
					bool flag4 = this.subTypes != null && this.subTypes.Count != 0;
					if (flag4)
					{
						throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be subclassed");
					}
					Type defaultType = null;
					MetaType.ResolveListTypes(this.model, this.type, ref type, ref defaultType);
					ValueMember valueMember = new ValueMember(this.model, 1, this.type, type, defaultType, DataFormat.Default);
					result = new TypeSerializer(this.model, this.type, new int[]
					{
						1
					}, new IProtoSerializer[]
					{
						valueMember.Serializer
					}, null, true, true, null, this.constructType, this.factory);
				}
				else
				{
					bool flag5 = this.surrogate != null;
					if (flag5)
					{
						MetaType metaType = this.model[this.surrogate];
						MetaType metaType2;
						while ((metaType2 = metaType.baseType) != null)
						{
							metaType = metaType2;
						}
						result = new SurrogateSerializer(this.model, this.type, this.surrogate, metaType.Serializer);
					}
					else
					{
						bool isAutoTuple = this.IsAutoTuple;
						if (isAutoTuple)
						{
							MemberInfo[] members;
							ConstructorInfo constructorInfo = MetaType.ResolveTupleConstructor(this.type, out members);
							bool flag6 = constructorInfo == null;
							if (flag6)
							{
								throw new InvalidOperationException();
							}
							result = new TupleSerializer(this.model, constructorInfo, members);
						}
						else
						{
							this.fields.Trim();
							int count = this.fields.Count;
							int num = (this.subTypes == null) ? 0 : this.subTypes.Count;
							int[] array = new int[count + num];
							IProtoSerializer[] array2 = new IProtoSerializer[count + num];
							int num2 = 0;
							bool flag7 = num != 0;
							if (flag7)
							{
								foreach (object obj in this.subTypes)
								{
									SubType subType = (SubType)obj;
									bool flag8 = !subType.DerivedType.IgnoreListHandling && this.model.MapType(MetaType.ienumerable).IsAssignableFrom(subType.DerivedType.Type);
									if (flag8)
									{
										throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be used as a subclass");
									}
									array[num2] = subType.FieldNumber;
									array2[num2++] = subType.Serializer;
								}
							}
							bool flag9 = count != 0;
							if (flag9)
							{
								foreach (object obj2 in this.fields)
								{
									ValueMember valueMember2 = (ValueMember)obj2;
									array[num2] = valueMember2.FieldNumber;
									array2[num2++] = valueMember2.Serializer;
								}
							}
							BasicList basicList = null;
							for (MetaType metaType3 = this.BaseType; metaType3 != null; metaType3 = metaType3.BaseType)
							{
								MethodInfo methodInfo = metaType3.HasCallbacks ? metaType3.Callbacks.BeforeDeserialize : null;
								bool flag10 = methodInfo != null;
								if (flag10)
								{
									bool flag11 = basicList == null;
									if (flag11)
									{
										basicList = new BasicList();
									}
									basicList.Add(methodInfo);
								}
							}
							MethodInfo[] array3 = null;
							bool flag12 = basicList != null;
							if (flag12)
							{
								array3 = new MethodInfo[basicList.Count];
								basicList.CopyTo(array3, 0);
								Array.Reverse(array3);
							}
							result = new TypeSerializer(this.model, this.type, array, array2, array3, this.baseType == null, this.UseConstructor, this.callbacks, this.constructType, this.factory);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060084CD RID: 33997 RVA: 0x001040C4 File Offset: 0x001022C4
		private static Type GetBaseType(MetaType type)
		{
			return type.type.BaseType;
		}

		// Token: 0x060084CE RID: 33998 RVA: 0x001040E4 File Offset: 0x001022E4
		internal static bool GetAsReferenceDefault(RuntimeTypeModel model, Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			bool flag2 = Helpers.IsEnum(type);
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				AttributeMap[] array = AttributeMap.Create(model, type, false);
				for (int i = 0; i < array.Length; i++)
				{
					bool flag3 = array[i].AttributeType.FullName == "ProtoBuf.ProtoContractAttribute";
					if (flag3)
					{
						object obj;
						bool flag4 = array[i].TryGet("AsReferenceDefault", out obj);
						if (flag4)
						{
							return (bool)obj;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060084CF RID: 33999 RVA: 0x0010417C File Offset: 0x0010237C
		internal void ApplyDefaultBehaviour()
		{
			Type type = MetaType.GetBaseType(this);
			bool flag = type != null && this.model.FindWithoutAdd(type) == null && MetaType.GetContractFamily(this.model, type, null) > MetaType.AttributeFamily.None;
			if (flag)
			{
				this.model.FindOrAddAuto(type, true, false, false);
			}
			AttributeMap[] array = AttributeMap.Create(this.model, this.type, false);
			MetaType.AttributeFamily attributeFamily = MetaType.GetContractFamily(this.model, this.type, array);
			bool flag2 = attributeFamily == MetaType.AttributeFamily.AutoTuple;
			if (flag2)
			{
				this.SetFlag(64, true, true);
			}
			bool flag3 = !this.EnumPassthru && Helpers.IsEnum(this.type);
			bool flag4 = attributeFamily == MetaType.AttributeFamily.None && !flag3;
			if (!flag4)
			{
				BasicList basicList = null;
				BasicList basicList2 = null;
				int dataMemberOffset = 0;
				int num = 1;
				bool flag5 = this.model.InferTagFromNameDefault;
				ImplicitFields implicitFields = ImplicitFields.None;
				string text = null;
				foreach (AttributeMap attributeMap in array)
				{
					string fullName = attributeMap.AttributeType.FullName;
					bool flag6 = !flag3 && fullName == "ProtoBuf.ProtoIncludeAttribute";
					object obj;
					if (flag6)
					{
						int fieldNumber = 0;
						bool flag7 = attributeMap.TryGet("tag", out obj);
						if (flag7)
						{
							fieldNumber = (int)obj;
						}
						DataFormat dataFormat = DataFormat.Default;
						bool flag8 = attributeMap.TryGet("DataFormat", out obj);
						if (flag8)
						{
							dataFormat = (DataFormat)((int)obj);
						}
						Type type2 = null;
						try
						{
							bool flag9 = attributeMap.TryGet("knownTypeName", out obj);
							if (flag9)
							{
								type2 = this.model.GetType((string)obj, this.type.Assembly);
							}
							else
							{
								bool flag10 = attributeMap.TryGet("knownType", out obj);
								if (flag10)
								{
									type2 = (Type)obj;
								}
							}
						}
						catch (Exception innerException)
						{
							throw new InvalidOperationException("Unable to resolve sub-type of: " + this.type.FullName, innerException);
						}
						bool flag11 = type2 == null;
						if (flag11)
						{
							throw new InvalidOperationException("Unable to resolve sub-type of: " + this.type.FullName);
						}
						bool flag12 = this.IsValidSubType(type2);
						if (flag12)
						{
							this.AddSubType(fieldNumber, type2, dataFormat);
						}
					}
					bool flag13 = fullName == "ProtoBuf.ProtoPartialIgnoreAttribute";
					if (flag13)
					{
						bool flag14 = attributeMap.TryGet("MemberName", out obj) && obj != null;
						if (flag14)
						{
							bool flag15 = basicList == null;
							if (flag15)
							{
								basicList = new BasicList();
							}
							basicList.Add((string)obj);
						}
					}
					bool flag16 = !flag3 && fullName == "ProtoBuf.ProtoPartialMemberAttribute";
					if (flag16)
					{
						bool flag17 = basicList2 == null;
						if (flag17)
						{
							basicList2 = new BasicList();
						}
						basicList2.Add(attributeMap);
					}
					bool flag18 = fullName == "ProtoBuf.ProtoContractAttribute";
					if (flag18)
					{
						bool flag19 = attributeMap.TryGet("Name", out obj);
						if (flag19)
						{
							text = (string)obj;
						}
						bool flag20 = Helpers.IsEnum(this.type);
						if (flag20)
						{
							bool flag21 = attributeMap.TryGet("EnumPassthruHasValue", false, out obj) && (bool)obj;
							if (flag21)
							{
								bool flag22 = attributeMap.TryGet("EnumPassthru", out obj);
								if (flag22)
								{
									this.EnumPassthru = (bool)obj;
									bool enumPassthru = this.EnumPassthru;
									if (enumPassthru)
									{
										flag3 = false;
									}
								}
							}
						}
						else
						{
							bool flag23 = attributeMap.TryGet("DataMemberOffset", out obj);
							if (flag23)
							{
								dataMemberOffset = (int)obj;
							}
							bool flag24 = attributeMap.TryGet("InferTagFromNameHasValue", false, out obj) && (bool)obj;
							if (flag24)
							{
								bool flag25 = attributeMap.TryGet("InferTagFromName", out obj);
								if (flag25)
								{
									flag5 = (bool)obj;
								}
							}
							bool flag26 = attributeMap.TryGet("ImplicitFields", out obj) && obj != null;
							if (flag26)
							{
								implicitFields = (ImplicitFields)((int)obj);
							}
							bool flag27 = attributeMap.TryGet("SkipConstructor", out obj);
							if (flag27)
							{
								this.UseConstructor = !(bool)obj;
							}
							bool flag28 = attributeMap.TryGet("IgnoreListHandling", out obj);
							if (flag28)
							{
								this.IgnoreListHandling = (bool)obj;
							}
							bool flag29 = attributeMap.TryGet("AsReferenceDefault", out obj);
							if (flag29)
							{
								this.AsReferenceDefault = (bool)obj;
							}
							bool flag30 = attributeMap.TryGet("ImplicitFirstTag", out obj) && (int)obj > 0;
							if (flag30)
							{
								num = (int)obj;
							}
						}
					}
					bool flag31 = fullName == "System.Runtime.Serialization.DataContractAttribute";
					if (flag31)
					{
						if (text == null && attributeMap.TryGet("Name", out obj))
						{
							text = (string)obj;
						}
					}
					bool flag33 = fullName == "System.Xml.Serialization.XmlTypeAttribute";
					if (flag33)
					{
						if (text == null && attributeMap.TryGet("TypeName", out obj))
						{
							text = (string)obj;
						}
					}
				}
				bool flag35 = !Helpers.IsNullOrEmpty(text);
				if (flag35)
				{
					this.Name = text;
				}
				bool flag36 = implicitFields > ImplicitFields.None;
				if (flag36)
				{
					attributeFamily &= MetaType.AttributeFamily.ProtoBuf;
				}
				MethodInfo[] array2 = null;
				BasicList basicList3 = new BasicList();
				MemberInfo[] members = this.type.GetMembers(flag3 ? (BindingFlags.Static | BindingFlags.Public) : (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				foreach (MemberInfo memberInfo in members)
				{
					bool flag37 = memberInfo.DeclaringType != this.type;
					if (!flag37)
					{
						bool flag38 = !ProtoBuf.Serializer.isSkipProtoIgnore;
						if (flag38)
						{
							bool flag39 = memberInfo.IsDefined(this.model.MapType(typeof(ProtoIgnoreAttribute)), true);
							if (flag39)
							{
								goto IL_756;
							}
						}
						bool flag40 = basicList != null && basicList.Contains(memberInfo.Name);
						if (!flag40)
						{
							bool flag41 = false;
							PropertyInfo propertyInfo;
							bool flag42 = (propertyInfo = (memberInfo as PropertyInfo)) != null;
							if (flag42)
							{
								bool flag43 = flag3;
								if (!flag43)
								{
									Type type3 = propertyInfo.PropertyType;
									bool isPublic = Helpers.GetGetMethod(propertyInfo, false, false) != null;
									bool isField = false;
									MetaType.ApplyDefaultBehaviour_AddMembers(this.model, attributeFamily, flag3, basicList2, dataMemberOffset, flag5, implicitFields, basicList3, memberInfo, ref flag41, isPublic, isField, ref type3);
								}
							}
							else
							{
								FieldInfo fieldInfo;
								bool flag44 = (fieldInfo = (memberInfo as FieldInfo)) != null;
								if (flag44)
								{
									Type type3 = fieldInfo.FieldType;
									bool isPublic = fieldInfo.IsPublic;
									bool isField = true;
									bool flag45 = flag3 && !fieldInfo.IsStatic;
									if (!flag45)
									{
										MetaType.ApplyDefaultBehaviour_AddMembers(this.model, attributeFamily, flag3, basicList2, dataMemberOffset, flag5, implicitFields, basicList3, memberInfo, ref flag41, isPublic, isField, ref type3);
									}
								}
								else
								{
									MethodInfo methodInfo;
									bool flag46 = (methodInfo = (memberInfo as MethodInfo)) != null;
									if (flag46)
									{
										bool flag47 = flag3;
										if (!flag47)
										{
											AttributeMap[] array4 = AttributeMap.Create(this.model, methodInfo, false);
											bool flag48 = array4 != null && array4.Length != 0;
											if (flag48)
											{
												MetaType.CheckForCallback(methodInfo, array4, "ProtoBuf.ProtoBeforeSerializationAttribute", ref array2, 0);
												MetaType.CheckForCallback(methodInfo, array4, "ProtoBuf.ProtoAfterSerializationAttribute", ref array2, 1);
												MetaType.CheckForCallback(methodInfo, array4, "ProtoBuf.ProtoBeforeDeserializationAttribute", ref array2, 2);
												MetaType.CheckForCallback(methodInfo, array4, "ProtoBuf.ProtoAfterDeserializationAttribute", ref array2, 3);
												MetaType.CheckForCallback(methodInfo, array4, "System.Runtime.Serialization.OnSerializingAttribute", ref array2, 4);
												MetaType.CheckForCallback(methodInfo, array4, "System.Runtime.Serialization.OnSerializedAttribute", ref array2, 5);
												MetaType.CheckForCallback(methodInfo, array4, "System.Runtime.Serialization.OnDeserializingAttribute", ref array2, 6);
												MetaType.CheckForCallback(methodInfo, array4, "System.Runtime.Serialization.OnDeserializedAttribute", ref array2, 7);
											}
										}
									}
								}
							}
						}
					}
					IL_756:;
				}
				ProtoMemberAttribute[] array5 = new ProtoMemberAttribute[basicList3.Count];
				basicList3.CopyTo(array5, 0);
				bool flag49 = flag5 || implicitFields > ImplicitFields.None;
				if (flag49)
				{
					Array.Sort<ProtoMemberAttribute>(array5);
					int num2 = num;
					foreach (ProtoMemberAttribute protoMemberAttribute in array5)
					{
						bool flag50 = !protoMemberAttribute.TagIsPinned;
						if (flag50)
						{
							protoMemberAttribute.Rebase(num2++);
						}
					}
				}
				foreach (ProtoMemberAttribute normalizedAttribute in array5)
				{
					ValueMember valueMember = this.ApplyDefaultBehaviour(flag3, normalizedAttribute);
					bool flag51 = valueMember != null;
					if (flag51)
					{
						this.Add(valueMember);
					}
				}
				bool flag52 = array2 != null;
				if (flag52)
				{
					this.SetCallbacks(MetaType.Coalesce(array2, 0, 4), MetaType.Coalesce(array2, 1, 5), MetaType.Coalesce(array2, 2, 6), MetaType.Coalesce(array2, 3, 7));
				}
			}
		}

		// Token: 0x060084D0 RID: 34000 RVA: 0x001049F8 File Offset: 0x00102BF8
		private static void ApplyDefaultBehaviour_AddMembers(TypeModel model, MetaType.AttributeFamily family, bool isEnum, BasicList partialMembers, int dataMemberOffset, bool inferTagByName, ImplicitFields implicitMode, BasicList members, MemberInfo member, ref bool forced, bool isPublic, bool isField, ref Type effectiveType)
		{
			if (implicitMode != ImplicitFields.AllPublic)
			{
				if (implicitMode == ImplicitFields.AllFields)
				{
					if (isField)
					{
						forced = true;
					}
				}
			}
			else if (isPublic)
			{
				forced = true;
			}
			bool flag = effectiveType.IsSubclassOf(model.MapType(typeof(Delegate)));
			if (flag)
			{
				effectiveType = null;
			}
			bool flag2 = effectiveType != null;
			if (flag2)
			{
				ProtoMemberAttribute protoMemberAttribute = MetaType.NormalizeProtoMember(model, member, family, forced, isEnum, partialMembers, dataMemberOffset, inferTagByName);
				bool flag3 = protoMemberAttribute != null;
				if (flag3)
				{
					members.Add(protoMemberAttribute);
				}
			}
		}

		// Token: 0x060084D1 RID: 34001 RVA: 0x00104A84 File Offset: 0x00102C84
		private static MethodInfo Coalesce(MethodInfo[] arr, int x, int y)
		{
			MethodInfo methodInfo = arr[x];
			bool flag = methodInfo == null;
			if (flag)
			{
				methodInfo = arr[y];
			}
			return methodInfo;
		}

		// Token: 0x060084D2 RID: 34002 RVA: 0x00104AA8 File Offset: 0x00102CA8
		internal static MetaType.AttributeFamily GetContractFamily(RuntimeTypeModel model, Type type, AttributeMap[] attributes)
		{
			MetaType.AttributeFamily attributeFamily = MetaType.AttributeFamily.None;
			bool flag = attributes == null;
			if (flag)
			{
				attributes = AttributeMap.Create(model, type, false);
			}
			for (int i = 0; i < attributes.Length; i++)
			{
				string fullName = attributes[i].AttributeType.FullName;
				if (!(fullName == "ProtoBuf.ProtoContractAttribute"))
				{
					if (!(fullName == "System.Xml.Serialization.XmlTypeAttribute"))
					{
						if (fullName == "System.Runtime.Serialization.DataContractAttribute")
						{
							bool flag2 = !model.AutoAddProtoContractTypesOnly;
							if (flag2)
							{
								attributeFamily |= MetaType.AttributeFamily.DataContractSerialier;
							}
						}
					}
					else
					{
						bool flag3 = !model.AutoAddProtoContractTypesOnly;
						if (flag3)
						{
							attributeFamily |= MetaType.AttributeFamily.XmlSerializer;
						}
					}
				}
				else
				{
					bool flag4 = false;
					MetaType.GetFieldBoolean(ref flag4, attributes[i], "UseProtoMembersOnly");
					bool flag5 = flag4;
					if (flag5)
					{
						return MetaType.AttributeFamily.ProtoBuf;
					}
					attributeFamily |= MetaType.AttributeFamily.ProtoBuf;
				}
			}
			bool flag6 = attributeFamily == MetaType.AttributeFamily.None;
			if (flag6)
			{
				MemberInfo[] array;
				bool flag7 = MetaType.ResolveTupleConstructor(type, out array) != null;
				if (flag7)
				{
					attributeFamily |= MetaType.AttributeFamily.AutoTuple;
				}
			}
			return attributeFamily;
		}

		// Token: 0x060084D3 RID: 34003 RVA: 0x00104B9C File Offset: 0x00102D9C
		internal static ConstructorInfo ResolveTupleConstructor(Type type, out MemberInfo[] mappedMembers)
		{
			mappedMembers = null;
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			bool isAbstract = type.IsAbstract;
			ConstructorInfo result;
			if (isAbstract)
			{
				result = null;
			}
			else
			{
				ConstructorInfo[] constructors = Helpers.GetConstructors(type, false);
				bool flag2 = constructors.Length == 0 || (constructors.Length == 1 && constructors[0].GetParameters().Length == 0);
				if (flag2)
				{
					result = null;
				}
				else
				{
					MemberInfo[] instanceFieldsAndProperties = Helpers.GetInstanceFieldsAndProperties(type, true);
					BasicList basicList = new BasicList();
					for (int i = 0; i < instanceFieldsAndProperties.Length; i++)
					{
						PropertyInfo propertyInfo = instanceFieldsAndProperties[i] as PropertyInfo;
						bool flag3 = propertyInfo != null;
						if (flag3)
						{
							bool flag4 = !propertyInfo.CanRead;
							if (flag4)
							{
								return null;
							}
							bool flag5 = propertyInfo.CanWrite && Helpers.GetSetMethod(propertyInfo, false, false) != null;
							if (flag5)
							{
								return null;
							}
							basicList.Add(propertyInfo);
						}
						else
						{
							FieldInfo fieldInfo = instanceFieldsAndProperties[i] as FieldInfo;
							bool flag6 = fieldInfo != null;
							if (flag6)
							{
								bool flag7 = !fieldInfo.IsInitOnly;
								if (flag7)
								{
									return null;
								}
								basicList.Add(fieldInfo);
							}
						}
					}
					bool flag8 = basicList.Count == 0;
					if (flag8)
					{
						result = null;
					}
					else
					{
						MemberInfo[] array = new MemberInfo[basicList.Count];
						basicList.CopyTo(array, 0);
						int[] array2 = new int[array.Length];
						int num = 0;
						ConstructorInfo constructorInfo = null;
						mappedMembers = new MemberInfo[array2.Length];
						for (int j = 0; j < constructors.Length; j++)
						{
							ParameterInfo[] parameters = constructors[j].GetParameters();
							bool flag9 = parameters.Length != array.Length;
							if (!flag9)
							{
								for (int k = 0; k < array2.Length; k++)
								{
									array2[k] = -1;
								}
								for (int l = 0; l < parameters.Length; l++)
								{
									string b = parameters[l].Name.ToLower();
									for (int m = 0; m < array.Length; m++)
									{
										bool flag10 = array[m].Name.ToLower() != b;
										if (!flag10)
										{
											Type memberType = Helpers.GetMemberType(array[m]);
											bool flag11 = memberType != parameters[l].ParameterType;
											if (!flag11)
											{
												array2[l] = m;
											}
										}
									}
								}
								bool flag12 = false;
								for (int n = 0; n < array2.Length; n++)
								{
									bool flag13 = array2[n] < 0;
									if (flag13)
									{
										flag12 = true;
										break;
									}
									mappedMembers[n] = array[array2[n]];
								}
								bool flag14 = flag12;
								if (!flag14)
								{
									num++;
									constructorInfo = constructors[j];
								}
							}
						}
						result = ((num == 1) ? constructorInfo : null);
					}
				}
			}
			return result;
		}

		// Token: 0x060084D4 RID: 34004 RVA: 0x00104E70 File Offset: 0x00103070
		private static void CheckForCallback(MethodInfo method, AttributeMap[] attributes, string callbackTypeName, ref MethodInfo[] callbacks, int index)
		{
			for (int i = 0; i < attributes.Length; i++)
			{
				bool flag = attributes[i].AttributeType.FullName == callbackTypeName;
				if (flag)
				{
					bool flag2 = callbacks == null;
					if (flag2)
					{
						callbacks = new MethodInfo[8];
					}
					else
					{
						bool flag3 = callbacks[index] != null;
						if (flag3)
						{
							Type reflectedType = method.ReflectedType;
							throw new ProtoException("Duplicate " + callbackTypeName + " callbacks on " + reflectedType.FullName);
						}
					}
					callbacks[index] = method;
				}
			}
		}

		// Token: 0x060084D5 RID: 34005 RVA: 0x00104EFC File Offset: 0x001030FC
		private static bool HasFamily(MetaType.AttributeFamily value, MetaType.AttributeFamily required)
		{
			return (value & required) == required;
		}

		// Token: 0x060084D6 RID: 34006 RVA: 0x00104F14 File Offset: 0x00103114
		private static ProtoMemberAttribute NormalizeProtoMember(TypeModel model, MemberInfo member, MetaType.AttributeFamily family, bool forced, bool isEnum, BasicList partialMembers, int dataMemberOffset, bool inferByTagName)
		{
			bool flag = member == null || (family == MetaType.AttributeFamily.None && !isEnum);
			ProtoMemberAttribute result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int num = int.MinValue;
				int num2 = inferByTagName ? -1 : 1;
				string text = null;
				bool isPacked = false;
				bool flag2 = false;
				bool flag3 = false;
				bool isRequired = false;
				bool asReference = false;
				bool flag4 = false;
				bool dynamicType = false;
				bool tagIsPinned = false;
				bool overwriteList = false;
				DataFormat dataFormat = DataFormat.Default;
				if (isEnum)
				{
					forced = true;
				}
				AttributeMap[] attribs = AttributeMap.Create(model, member, true);
				if (isEnum)
				{
					AttributeMap attribute = MetaType.GetAttribute(attribs, "ProtoBuf.ProtoIgnoreAttribute");
					bool flag5 = attribute != null;
					if (flag5)
					{
						flag2 = true;
					}
					else
					{
						attribute = MetaType.GetAttribute(attribs, "ProtoBuf.ProtoEnumAttribute");
						num = Convert.ToInt32(((FieldInfo)member).GetRawConstantValue());
						bool flag6 = attribute != null;
						if (flag6)
						{
							MetaType.GetFieldName(ref text, attribute, "Name");
							bool flag7 = (bool)Helpers.GetInstanceMethod(attribute.AttributeType, "HasValue").Invoke(attribute.Target, null);
							if (flag7)
							{
								object obj;
								bool flag8 = attribute.TryGet("Value", out obj);
								if (flag8)
								{
									num = (int)obj;
								}
							}
						}
					}
					flag3 = true;
				}
				bool flag9 = !flag2 && !flag3;
				if (flag9)
				{
					AttributeMap attribute = MetaType.GetAttribute(attribs, "ProtoBuf.ProtoMemberAttribute");
					MetaType.GetIgnore(ref flag2, attribute, attribs, "ProtoBuf.ProtoIgnoreAttribute");
					bool flag10 = !flag2 && attribute != null;
					if (flag10)
					{
						MetaType.GetFieldNumber(ref num, attribute, "Tag");
						MetaType.GetFieldName(ref text, attribute, "Name");
						MetaType.GetFieldBoolean(ref isRequired, attribute, "IsRequired");
						MetaType.GetFieldBoolean(ref isPacked, attribute, "IsPacked");
						MetaType.GetFieldBoolean(ref overwriteList, attribute, "OverwriteList");
						MetaType.GetDataFormat(ref dataFormat, attribute, "DataFormat");
						MetaType.GetFieldBoolean(ref flag4, attribute, "AsReferenceHasValue", false);
						bool flag11 = flag4;
						if (flag11)
						{
							flag4 = MetaType.GetFieldBoolean(ref asReference, attribute, "AsReference", true);
						}
						MetaType.GetFieldBoolean(ref dynamicType, attribute, "DynamicType");
						tagIsPinned = (flag3 = (num > 0));
					}
					bool flag12 = !flag3 && partialMembers != null;
					if (flag12)
					{
						foreach (object obj2 in partialMembers)
						{
							AttributeMap attributeMap = (AttributeMap)obj2;
							object obj3;
							bool flag13 = attributeMap.TryGet("MemberName", out obj3) && (string)obj3 == member.Name;
							if (flag13)
							{
								MetaType.GetFieldNumber(ref num, attributeMap, "Tag");
								MetaType.GetFieldName(ref text, attributeMap, "Name");
								MetaType.GetFieldBoolean(ref isRequired, attributeMap, "IsRequired");
								MetaType.GetFieldBoolean(ref isPacked, attributeMap, "IsPacked");
								MetaType.GetFieldBoolean(ref overwriteList, attribute, "OverwriteList");
								MetaType.GetDataFormat(ref dataFormat, attributeMap, "DataFormat");
								MetaType.GetFieldBoolean(ref flag4, attribute, "AsReferenceHasValue", false);
								bool flag14 = flag4;
								if (flag14)
								{
									flag4 = MetaType.GetFieldBoolean(ref asReference, attributeMap, "AsReference", true);
								}
								MetaType.GetFieldBoolean(ref dynamicType, attributeMap, "DynamicType");
								bool flag15;
								flag3 = (flag15 = (tagIsPinned = (num > 0)));
								if (flag15)
								{
									break;
								}
							}
						}
					}
				}
				bool flag16 = !flag2 && !flag3 && MetaType.HasFamily(family, MetaType.AttributeFamily.DataContractSerialier);
				if (flag16)
				{
					AttributeMap attribute = MetaType.GetAttribute(attribs, "System.Runtime.Serialization.DataMemberAttribute");
					bool flag17 = attribute != null;
					if (flag17)
					{
						MetaType.GetFieldNumber(ref num, attribute, "Order");
						MetaType.GetFieldName(ref text, attribute, "Name");
						MetaType.GetFieldBoolean(ref isRequired, attribute, "IsRequired");
						flag3 = (num >= num2);
						bool flag18 = flag3;
						if (flag18)
						{
							num += dataMemberOffset;
						}
					}
				}
				bool flag19 = !flag2 && !flag3 && MetaType.HasFamily(family, MetaType.AttributeFamily.XmlSerializer);
				if (flag19)
				{
					AttributeMap attribute = MetaType.GetAttribute(attribs, "System.Xml.Serialization.XmlElementAttribute");
					bool flag20 = attribute == null;
					if (flag20)
					{
						attribute = MetaType.GetAttribute(attribs, "System.Xml.Serialization.XmlArrayAttribute");
					}
					MetaType.GetIgnore(ref flag2, attribute, attribs, "System.Xml.Serialization.XmlIgnoreAttribute");
					bool flag21 = attribute != null && !flag2;
					if (flag21)
					{
						MetaType.GetFieldNumber(ref num, attribute, "Order");
						MetaType.GetFieldName(ref text, attribute, "ElementName");
						flag3 = (num >= num2);
					}
				}
				bool flag22 = !flag2 && !flag3;
				if (flag22)
				{
					bool flag23 = MetaType.GetAttribute(attribs, "System.NonSerializedAttribute") != null;
					if (flag23)
					{
						flag2 = true;
					}
				}
				bool flag24 = flag2 || (num < num2 && !forced);
				if (flag24)
				{
					result = null;
				}
				else
				{
					result = new ProtoMemberAttribute(num, forced || inferByTagName)
					{
						AsReference = asReference,
						AsReferenceHasValue = flag4,
						DataFormat = dataFormat,
						DynamicType = dynamicType,
						IsPacked = isPacked,
						OverwriteList = overwriteList,
						IsRequired = isRequired,
						Name = (Helpers.IsNullOrEmpty(text) ? member.Name : text),
						Member = member,
						TagIsPinned = tagIsPinned
					};
				}
			}
			return result;
		}

		// Token: 0x060084D7 RID: 34007 RVA: 0x00105414 File Offset: 0x00103614
		private ValueMember ApplyDefaultBehaviour(bool isEnum, ProtoMemberAttribute normalizedAttribute)
		{
			MemberInfo member;
			ValueMember result;
			if (normalizedAttribute == null || (member = normalizedAttribute.Member) == null)
			{
				result = null;
			}
			else
			{
				Type memberType = Helpers.GetMemberType(member);
				Type type = null;
				Type defaultType = null;
				MetaType.ResolveListTypes(this.model, memberType, ref type, ref defaultType);
				bool flag2 = type != null;
				if (flag2)
				{
					int num = this.model.FindOrAddAuto(memberType, false, true, false);
					bool flag3 = num >= 0 && this.model[memberType].IgnoreListHandling;
					if (flag3)
					{
						type = null;
						defaultType = null;
					}
				}
				AttributeMap[] attribs = AttributeMap.Create(this.model, member, true);
				object defaultValue = null;
				bool useImplicitZeroDefaults = this.model.UseImplicitZeroDefaults;
				if (useImplicitZeroDefaults)
				{
					ProtoTypeCode typeCode = Helpers.GetTypeCode(memberType);
					switch (typeCode)
					{
					case ProtoTypeCode.Boolean:
						defaultValue = false;
						break;
					case ProtoTypeCode.Char:
						defaultValue = '\0';
						break;
					case ProtoTypeCode.SByte:
						defaultValue = 0;
						break;
					case ProtoTypeCode.Byte:
						defaultValue = 0;
						break;
					case ProtoTypeCode.Int16:
						defaultValue = 0;
						break;
					case ProtoTypeCode.UInt16:
						defaultValue = 0;
						break;
					case ProtoTypeCode.Int32:
						defaultValue = 0;
						break;
					case ProtoTypeCode.UInt32:
						defaultValue = 0U;
						break;
					case ProtoTypeCode.Int64:
						defaultValue = 0L;
						break;
					case ProtoTypeCode.UInt64:
						defaultValue = 0UL;
						break;
					case ProtoTypeCode.Single:
						defaultValue = 0f;
						break;
					case ProtoTypeCode.Double:
						defaultValue = 0.0;
						break;
					case ProtoTypeCode.Decimal:
						defaultValue = 0m;
						break;
					default:
						if (typeCode != ProtoTypeCode.TimeSpan)
						{
							if (typeCode == ProtoTypeCode.Guid)
							{
								defaultValue = Guid.Empty;
							}
						}
						else
						{
							defaultValue = TimeSpan.Zero;
						}
						break;
					}
				}
				AttributeMap attribute;
				bool flag4 = (attribute = MetaType.GetAttribute(attribs, "System.ComponentModel.DefaultValueAttribute")) != null;
				if (flag4)
				{
					object obj;
					bool flag5 = attribute.TryGet("Value", out obj);
					if (flag5)
					{
						defaultValue = obj;
					}
				}
				ValueMember valueMember = (isEnum || normalizedAttribute.Tag > 0) ? new ValueMember(this.model, this.type, normalizedAttribute.Tag, member, memberType, type, defaultType, normalizedAttribute.DataFormat, defaultValue) : null;
				bool flag6 = valueMember != null;
				if (flag6)
				{
					Type declaringType = this.type;
					PropertyInfo propertyInfo = Helpers.GetProperty(declaringType, member.Name + "Specified", true);
					MethodInfo getMethod = Helpers.GetGetMethod(propertyInfo, true, true);
					bool flag7 = getMethod == null || getMethod.IsStatic;
					if (flag7)
					{
						propertyInfo = null;
					}
					bool flag8 = propertyInfo != null;
					if (flag8)
					{
						valueMember.SetSpecified(getMethod, Helpers.GetSetMethod(propertyInfo, true, true));
					}
					else
					{
						MethodInfo instanceMethod = Helpers.GetInstanceMethod(declaringType, "ShouldSerialize" + member.Name, Helpers.EmptyTypes);
						bool flag9 = instanceMethod != null && instanceMethod.ReturnType == this.model.MapType(typeof(bool));
						if (flag9)
						{
							valueMember.SetSpecified(instanceMethod, null);
						}
					}
					bool flag10 = !Helpers.IsNullOrEmpty(normalizedAttribute.Name);
					if (flag10)
					{
						valueMember.SetName(normalizedAttribute.Name);
					}
					valueMember.IsPacked = normalizedAttribute.IsPacked;
					valueMember.IsRequired = normalizedAttribute.IsRequired;
					valueMember.OverwriteList = normalizedAttribute.OverwriteList;
					bool asReferenceHasValue = normalizedAttribute.AsReferenceHasValue;
					if (asReferenceHasValue)
					{
						valueMember.AsReference = normalizedAttribute.AsReference;
					}
					valueMember.DynamicType = normalizedAttribute.DynamicType;
				}
				result = valueMember;
			}
			return result;
		}

		// Token: 0x060084D8 RID: 34008 RVA: 0x00105794 File Offset: 0x00103994
		private static void GetDataFormat(ref DataFormat value, AttributeMap attrib, string memberName)
		{
			bool flag = attrib == null || value > DataFormat.Default;
			if (!flag)
			{
				object obj;
				bool flag2 = attrib.TryGet(memberName, out obj) && obj != null;
				if (flag2)
				{
					value = (DataFormat)obj;
				}
			}
		}

		// Token: 0x060084D9 RID: 34009 RVA: 0x001057D4 File Offset: 0x001039D4
		private static void GetIgnore(ref bool ignore, AttributeMap attrib, AttributeMap[] attribs, string fullName)
		{
			bool flag = ignore || attrib == null;
			if (!flag)
			{
				ignore = (MetaType.GetAttribute(attribs, fullName) != null);
			}
		}

		// Token: 0x060084DA RID: 34010 RVA: 0x00105801 File Offset: 0x00103A01
		private static void GetFieldBoolean(ref bool value, AttributeMap attrib, string memberName)
		{
			MetaType.GetFieldBoolean(ref value, attrib, memberName, true);
		}

		// Token: 0x060084DB RID: 34011 RVA: 0x00105810 File Offset: 0x00103A10
		private static bool GetFieldBoolean(ref bool value, AttributeMap attrib, string memberName, bool publicOnly)
		{
			bool flag = attrib == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = value;
				if (flag2)
				{
					result = true;
				}
				else
				{
					object obj;
					bool flag3 = attrib.TryGet(memberName, publicOnly, out obj) && obj != null;
					if (flag3)
					{
						value = (bool)obj;
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x060084DC RID: 34012 RVA: 0x00105860 File Offset: 0x00103A60
		private static void GetFieldNumber(ref int value, AttributeMap attrib, string memberName)
		{
			bool flag = attrib == null || value > 0;
			if (!flag)
			{
				object obj;
				bool flag2 = attrib.TryGet(memberName, out obj) && obj != null;
				if (flag2)
				{
					value = (int)obj;
				}
			}
		}

		// Token: 0x060084DD RID: 34013 RVA: 0x001058A0 File Offset: 0x00103AA0
		private static void GetFieldName(ref string name, AttributeMap attrib, string memberName)
		{
			bool flag = attrib == null || !Helpers.IsNullOrEmpty(name);
			if (!flag)
			{
				object obj;
				bool flag2 = attrib.TryGet(memberName, out obj) && obj != null;
				if (flag2)
				{
					name = (string)obj;
				}
			}
		}

		// Token: 0x060084DE RID: 34014 RVA: 0x001058E4 File Offset: 0x00103AE4
		private static AttributeMap GetAttribute(AttributeMap[] attribs, string fullName)
		{
			foreach (AttributeMap attributeMap in attribs)
			{
				bool flag = attributeMap != null && attributeMap.AttributeType.FullName == fullName;
				if (flag)
				{
					return attributeMap;
				}
			}
			return null;
		}

		// Token: 0x060084DF RID: 34015 RVA: 0x00105930 File Offset: 0x00103B30
		public MetaType Add(int fieldNumber, string memberName)
		{
			this.AddField(fieldNumber, memberName, null, null, null);
			return this;
		}

		// Token: 0x060084E0 RID: 34016 RVA: 0x00105950 File Offset: 0x00103B50
		public ValueMember AddField(int fieldNumber, string memberName)
		{
			return this.AddField(fieldNumber, memberName, null, null, null);
		}

		// Token: 0x170029FD RID: 10749
		// (get) Token: 0x060084E1 RID: 34017 RVA: 0x00105970 File Offset: 0x00103B70
		// (set) Token: 0x060084E2 RID: 34018 RVA: 0x0010598D File Offset: 0x00103B8D
		public bool UseConstructor
		{
			get
			{
				return !this.HasFlag(16);
			}
			set
			{
				this.SetFlag(16, !value, true);
			}
		}

		// Token: 0x170029FE RID: 10750
		// (get) Token: 0x060084E3 RID: 34019 RVA: 0x001059A0 File Offset: 0x00103BA0
		// (set) Token: 0x060084E4 RID: 34020 RVA: 0x001059B8 File Offset: 0x00103BB8
		public Type ConstructType
		{
			get
			{
				return this.constructType;
			}
			set
			{
				this.ThrowIfFrozen();
				this.constructType = value;
			}
		}

		// Token: 0x060084E5 RID: 34021 RVA: 0x001059CC File Offset: 0x00103BCC
		public MetaType Add(string memberName)
		{
			this.Add(this.GetNextFieldNumber(), memberName);
			return this;
		}

		// Token: 0x060084E6 RID: 34022 RVA: 0x001059F0 File Offset: 0x00103BF0
		public void SetSurrogate(Type surrogateType)
		{
			bool flag = surrogateType == this.type;
			if (flag)
			{
				surrogateType = null;
			}
			bool flag2 = surrogateType != null;
			if (flag2)
			{
				bool flag3 = surrogateType != null && Helpers.IsAssignableFrom(this.model.MapType(typeof(IEnumerable)), surrogateType);
				if (flag3)
				{
					throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be used as a surrogate");
				}
			}
			this.ThrowIfFrozen();
			this.surrogate = surrogateType;
		}

		// Token: 0x060084E7 RID: 34023 RVA: 0x00105A58 File Offset: 0x00103C58
		internal MetaType GetSurrogateOrSelf()
		{
			bool flag = this.surrogate != null;
			MetaType result;
			if (flag)
			{
				result = this.model[this.surrogate];
			}
			else
			{
				result = this;
			}
			return result;
		}

		// Token: 0x060084E8 RID: 34024 RVA: 0x00105A8C File Offset: 0x00103C8C
		internal MetaType GetSurrogateOrBaseOrSelf(bool deep)
		{
			bool flag = this.surrogate != null;
			MetaType result;
			if (flag)
			{
				result = this.model[this.surrogate];
			}
			else
			{
				MetaType metaType = this.baseType;
				bool flag2 = metaType != null;
				if (flag2)
				{
					if (deep)
					{
						MetaType metaType2;
						do
						{
							metaType2 = metaType;
							metaType = metaType.baseType;
						}
						while (metaType != null);
						result = metaType2;
					}
					else
					{
						result = metaType;
					}
				}
				else
				{
					result = this;
				}
			}
			return result;
		}

		// Token: 0x060084E9 RID: 34025 RVA: 0x00105AF8 File Offset: 0x00103CF8
		private int GetNextFieldNumber()
		{
			int num = 0;
			foreach (object obj in this.fields)
			{
				ValueMember valueMember = (ValueMember)obj;
				bool flag = valueMember.FieldNumber > num;
				if (flag)
				{
					num = valueMember.FieldNumber;
				}
			}
			bool flag2 = this.subTypes != null;
			if (flag2)
			{
				foreach (object obj2 in this.subTypes)
				{
					SubType subType = (SubType)obj2;
					bool flag3 = subType.FieldNumber > num;
					if (flag3)
					{
						num = subType.FieldNumber;
					}
				}
			}
			return num + 1;
		}

		// Token: 0x060084EA RID: 34026 RVA: 0x00105BA0 File Offset: 0x00103DA0
		public MetaType Add(params string[] memberNames)
		{
			bool flag = memberNames == null;
			if (flag)
			{
				throw new ArgumentNullException("memberNames");
			}
			int nextFieldNumber = this.GetNextFieldNumber();
			for (int i = 0; i < memberNames.Length; i++)
			{
				this.Add(nextFieldNumber++, memberNames[i]);
			}
			return this;
		}

		// Token: 0x060084EB RID: 34027 RVA: 0x00105BF4 File Offset: 0x00103DF4
		public MetaType Add(int fieldNumber, string memberName, object defaultValue)
		{
			this.AddField(fieldNumber, memberName, null, null, defaultValue);
			return this;
		}

		// Token: 0x060084EC RID: 34028 RVA: 0x00105C14 File Offset: 0x00103E14
		public MetaType Add(int fieldNumber, string memberName, Type itemType, Type defaultType)
		{
			this.AddField(fieldNumber, memberName, itemType, defaultType, null);
			return this;
		}

		// Token: 0x060084ED RID: 34029 RVA: 0x00105C34 File Offset: 0x00103E34
		public ValueMember AddField(int fieldNumber, string memberName, Type itemType, Type defaultType)
		{
			return this.AddField(fieldNumber, memberName, itemType, defaultType, null);
		}

		// Token: 0x060084EE RID: 34030 RVA: 0x00105C54 File Offset: 0x00103E54
		private ValueMember AddField(int fieldNumber, string memberName, Type itemType, Type defaultType, object defaultValue)
		{
			MemberInfo memberInfo = null;
			MemberInfo[] member = this.type.GetMember(memberName, Helpers.IsEnum(this.type) ? (BindingFlags.Static | BindingFlags.Public) : (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			bool flag = member != null && member.Length == 1;
			if (flag)
			{
				memberInfo = member[0];
			}
			bool flag2 = memberInfo == null;
			if (flag2)
			{
				throw new ArgumentException("Unable to determine member: " + memberName, "memberName");
			}
			MemberTypes memberType = memberInfo.MemberType;
			Type memberType2;
			if (memberType != MemberTypes.Field)
			{
				if (memberType != MemberTypes.Property)
				{
					throw new NotSupportedException(memberInfo.MemberType.ToString());
				}
				memberType2 = ((PropertyInfo)memberInfo).PropertyType;
			}
			else
			{
				memberType2 = ((FieldInfo)memberInfo).FieldType;
			}
			MetaType.ResolveListTypes(this.model, memberType2, ref itemType, ref defaultType);
			ValueMember valueMember = new ValueMember(this.model, this.type, fieldNumber, memberInfo, memberType2, itemType, defaultType, DataFormat.Default, defaultValue);
			this.Add(valueMember);
			return valueMember;
		}

		// Token: 0x060084EF RID: 34031 RVA: 0x00105D44 File Offset: 0x00103F44
		internal static void ResolveListTypes(TypeModel model, Type type, ref Type itemType, ref Type defaultType)
		{
			bool flag = type == null;
			if (!flag)
			{
				bool isArray = type.IsArray;
				if (isArray)
				{
					bool flag2 = type.GetArrayRank() != 1;
					if (flag2)
					{
						throw new NotSupportedException("Multi-dimension arrays are supported");
					}
					itemType = type.GetElementType();
					bool flag3 = itemType == model.MapType(typeof(byte));
					if (flag3)
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
				bool flag4 = itemType == null;
				if (flag4)
				{
					itemType = TypeModel.GetListItemType(model, type);
				}
				bool flag5 = itemType != null;
				if (flag5)
				{
					Type type3 = null;
					Type type4 = null;
					MetaType.ResolveListTypes(model, itemType, ref type3, ref type4);
					bool flag6 = type3 != null;
					if (flag6)
					{
						throw TypeModel.CreateNestedListsNotSupported();
					}
				}
				bool flag7 = itemType != null && defaultType == null;
				if (flag7)
				{
					bool flag8 = type.IsClass && !type.IsAbstract && Helpers.GetConstructor(type, Helpers.EmptyTypes, true) != null;
					if (flag8)
					{
						defaultType = type;
					}
					bool flag9 = defaultType == null;
					if (flag9)
					{
						bool isInterface = type.IsInterface;
						if (isInterface)
						{
							Type[] genericArguments;
					
							if (type.IsGenericType && type.GetGenericTypeDefinition() == model.MapType(typeof(IDictionary<,>)) && itemType == model.MapType(typeof(KeyValuePair<,>)).MakeGenericType(genericArguments = type.GetGenericArguments()))
							{
								defaultType = model.MapType(typeof(Dictionary<, >)).MakeGenericType(genericArguments);
							}
							else
							{
								defaultType = model.MapType(typeof(List<>)).MakeGenericType(new Type[]
								{
									itemType
								});
							}
						}
					}
					bool flag11 = defaultType != null && !Helpers.IsAssignableFrom(type, defaultType);
					if (flag11)
					{
						defaultType = null;
					}
				}
			}
		}

		// Token: 0x060084F0 RID: 34032 RVA: 0x00105F10 File Offset: 0x00104110
		private void Add(ValueMember member)
		{
			int opaqueToken = 0;
			try
			{
				this.model.TakeLock(ref opaqueToken);
				this.ThrowIfFrozen();
				this.fields.Add(member);
			}
			finally
			{
				this.model.ReleaseLock(opaqueToken);
			}
		}

		// Token: 0x170029FF RID: 10751
		public ValueMember this[int fieldNumber]
		{
			get
			{
				foreach (object obj in this.fields)
				{
					ValueMember valueMember = (ValueMember)obj;
					bool flag = valueMember.FieldNumber == fieldNumber;
					if (flag)
					{
						return valueMember;
					}
				}
				return null;
			}
		}

		// Token: 0x17002A00 RID: 10752
		public ValueMember this[MemberInfo member]
		{
			get
			{
				bool flag = member == null;
				ValueMember result;
				if (flag)
				{
					result = null;
				}
				else
				{
					foreach (object obj in this.fields)
					{
						ValueMember valueMember = (ValueMember)obj;
						bool flag2 = valueMember.Member == member;
						if (flag2)
						{
							return valueMember;
						}
					}
					result = null;
				}
				return result;
			}
		}

		// Token: 0x060084F3 RID: 34035 RVA: 0x00106010 File Offset: 0x00104210
		public ValueMember[] GetFields()
		{
			ValueMember[] array = new ValueMember[this.fields.Count];
			this.fields.CopyTo(array, 0);
			Array.Sort<ValueMember>(array, ValueMember.Comparer.Default);
			return array;
		}

		// Token: 0x060084F4 RID: 34036 RVA: 0x00106050 File Offset: 0x00104250
		public SubType[] GetSubtypes()
		{
			bool flag = this.subTypes == null || this.subTypes.Count == 0;
			SubType[] result;
			if (flag)
			{
				result = new SubType[0];
			}
			else
			{
				SubType[] array = new SubType[this.subTypes.Count];
				this.subTypes.CopyTo(array, 0);
				Array.Sort<SubType>(array, SubType.Comparer.Default);
				result = array;
			}
			return result;
		}

		// Token: 0x060084F5 RID: 34037 RVA: 0x001060B4 File Offset: 0x001042B4
		internal bool IsDefined(int fieldNumber)
		{
			foreach (object obj in this.fields)
			{
				ValueMember valueMember = (ValueMember)obj;
				bool flag = valueMember.FieldNumber == fieldNumber;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060084F6 RID: 34038 RVA: 0x00106100 File Offset: 0x00104300
		internal int GetKey(bool demand, bool getBaseKey)
		{
			return this.model.GetKey(this.type, demand, getBaseKey);
		}

		// Token: 0x060084F7 RID: 34039 RVA: 0x00106128 File Offset: 0x00104328
		internal EnumSerializer.EnumPair[] GetEnumMap()
		{
			bool flag = this.HasFlag(2);
			EnumSerializer.EnumPair[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				EnumSerializer.EnumPair[] array = new EnumSerializer.EnumPair[this.fields.Count];
				for (int i = 0; i < array.Length; i++)
				{
					ValueMember valueMember = (ValueMember)this.fields[i];
					int fieldNumber = valueMember.FieldNumber;
					object rawEnumValue = valueMember.GetRawEnumValue();
					array[i] = new EnumSerializer.EnumPair(fieldNumber, rawEnumValue, valueMember.MemberType);
				}
				result = array;
			}
			return result;
		}

		// Token: 0x17002A01 RID: 10753
		// (get) Token: 0x060084F8 RID: 34040 RVA: 0x001061B0 File Offset: 0x001043B0
		// (set) Token: 0x060084F9 RID: 34041 RVA: 0x001061C9 File Offset: 0x001043C9
		public bool EnumPassthru
		{
			get
			{
				return this.HasFlag(2);
			}
			set
			{
				this.SetFlag(2, value, true);
			}
		}

		// Token: 0x17002A02 RID: 10754
		// (get) Token: 0x060084FA RID: 34042 RVA: 0x001061D8 File Offset: 0x001043D8
		// (set) Token: 0x060084FB RID: 34043 RVA: 0x001061F5 File Offset: 0x001043F5
		public bool IgnoreListHandling
		{
			get
			{
				return this.HasFlag(128);
			}
			set
			{
				this.SetFlag(128, value, true);
			}
		}

		// Token: 0x17002A03 RID: 10755
		// (get) Token: 0x060084FC RID: 34044 RVA: 0x00106208 File Offset: 0x00104408
		// (set) Token: 0x060084FD RID: 34045 RVA: 0x00106221 File Offset: 0x00104421
		internal bool Pending
		{
			get
			{
				return this.HasFlag(1);
			}
			set
			{
				this.SetFlag(1, value, false);
			}
		}

		// Token: 0x060084FE RID: 34046 RVA: 0x00106230 File Offset: 0x00104430
		private bool HasFlag(byte flag)
		{
			return (this.flags & flag) == flag;
		}

		// Token: 0x060084FF RID: 34047 RVA: 0x00106250 File Offset: 0x00104450
		private void SetFlag(byte flag, bool value, bool throwIfFrozen)
		{
            if (throwIfFrozen && (this.HasFlag(flag) != value))
            {
                this.ThrowIfFrozen();
            }
            if (value)
            {
                this.flags = (byte)(this.flags | flag);
            }
            else
            {
                this.flags = (byte)(this.flags & ~flag);
            }

        }

        // Token: 0x06008500 RID: 34048 RVA: 0x001062AC File Offset: 0x001044AC
        internal static MetaType GetRootType(MetaType source)
		{
			MetaType result;
			while (source.serializer != null)
			{
				MetaType metaType = source.baseType;
				bool flag = metaType == null;
				if (flag)
				{
					result = source;
					return result;
				}
				source = metaType;
			}
			RuntimeTypeModel runtimeTypeModel = source.model;
			int opaqueToken = 0;
			try
			{
				runtimeTypeModel.TakeLock(ref opaqueToken);
				MetaType metaType2;
				while ((metaType2 = source.baseType) != null)
				{
					source = metaType2;
				}
				result = source;
			}
			finally
			{
				runtimeTypeModel.ReleaseLock(opaqueToken);
			}
			return result;
		}

		// Token: 0x06008501 RID: 34049 RVA: 0x00106334 File Offset: 0x00104534
		internal bool IsPrepared()
		{
			return false;
		}

		// Token: 0x17002A04 RID: 10756
		// (get) Token: 0x06008502 RID: 34050 RVA: 0x00106348 File Offset: 0x00104548
		internal IEnumerable Fields
		{
			get
			{
				return this.fields;
			}
		}

		// Token: 0x06008503 RID: 34051 RVA: 0x00106360 File Offset: 0x00104560
		internal static StringBuilder NewLine(StringBuilder builder, int indent)
		{
			return Helpers.AppendLine(builder).Append(' ', indent * 3);
		}

		// Token: 0x17002A05 RID: 10757
		// (get) Token: 0x06008504 RID: 34052 RVA: 0x00106384 File Offset: 0x00104584
		internal bool IsAutoTuple
		{
			get
			{
				return this.HasFlag(64);
			}
		}

		// Token: 0x06008505 RID: 34053 RVA: 0x001063A0 File Offset: 0x001045A0
		internal void WriteSchema(StringBuilder builder, int indent, ref bool requiresBclImport)
		{
			bool flag = this.surrogate != null;
			if (!flag)
			{
				ValueMember[] array = new ValueMember[this.fields.Count];
				this.fields.CopyTo(array, 0);
				Array.Sort<ValueMember>(array, ValueMember.Comparer.Default);
				bool isList = this.IsList;
				if (isList)
				{
					string schemaTypeName = this.model.GetSchemaTypeName(TypeModel.GetListItemType(this.model, this.type), DataFormat.Default, false, false, ref requiresBclImport);
					MetaType.NewLine(builder, indent).Append("message ").Append(this.GetSchemaTypeName()).Append(" {");
					MetaType.NewLine(builder, indent + 1).Append("repeated ").Append(schemaTypeName).Append(" items = 1;");
					MetaType.NewLine(builder, indent).Append('}');
				}
				else
				{
					bool isAutoTuple = this.IsAutoTuple;
					if (isAutoTuple)
					{
						MemberInfo[] array2;
						bool flag2 = MetaType.ResolveTupleConstructor(this.type, out array2) != null;
						if (flag2)
						{
							MetaType.NewLine(builder, indent).Append("message ").Append(this.GetSchemaTypeName()).Append(" {");
							for (int i = 0; i < array2.Length; i++)
							{
								bool flag3 = array2[i] is PropertyInfo;
								Type effectiveType;
								if (flag3)
								{
									effectiveType = ((PropertyInfo)array2[i]).PropertyType;
								}
								else
								{
									bool flag4 = array2[i] is FieldInfo;
									if (!flag4)
									{
										throw new NotSupportedException("Unknown member type: " + array2[i].GetType().Name);
									}
									effectiveType = ((FieldInfo)array2[i]).FieldType;
								}
								MetaType.NewLine(builder, indent + 1).Append("optional ").Append(this.model.GetSchemaTypeName(effectiveType, DataFormat.Default, false, false, ref requiresBclImport).Replace('.', '_')).Append(' ').Append(array2[i].Name).Append(" = ").Append(i + 1).Append(';');
							}
							MetaType.NewLine(builder, indent).Append('}');
						}
					}
					else
					{
						bool flag5 = Helpers.IsEnum(this.type);
						if (flag5)
						{
							MetaType.NewLine(builder, indent).Append("enum ").Append(this.GetSchemaTypeName()).Append(" {");
							bool flag6 = array.Length == 0 && this.EnumPassthru;
							if (flag6)
							{
								bool flag7 = this.type.IsDefined(this.model.MapType(typeof(FlagsAttribute)), false);
								if (flag7)
								{
									MetaType.NewLine(builder, indent + 1).Append("// this is a composite/flags enumeration");
								}
								else
								{
									MetaType.NewLine(builder, indent + 1).Append("// this enumeration will be passed as a raw value");
								}
								foreach (FieldInfo fieldInfo in this.type.GetFields())
								{
									bool flag8 = fieldInfo.IsStatic && fieldInfo.IsLiteral;
									if (flag8)
									{
										object rawConstantValue = fieldInfo.GetRawConstantValue();
										MetaType.NewLine(builder, indent + 1).Append(fieldInfo.Name).Append(" = ").Append(rawConstantValue).Append(";");
									}
								}
							}
							else
							{
								foreach (ValueMember valueMember in array)
								{
									MetaType.NewLine(builder, indent + 1).Append(valueMember.Name).Append(" = ").Append(valueMember.FieldNumber).Append(';');
								}
							}
							MetaType.NewLine(builder, indent).Append('}');
						}
						else
						{
							MetaType.NewLine(builder, indent).Append("message ").Append(this.GetSchemaTypeName()).Append(" {");
							foreach (ValueMember valueMember2 in array)
							{
								string value = (valueMember2.ItemType != null) ? "repeated" : (valueMember2.IsRequired ? "required" : "optional");
								MetaType.NewLine(builder, indent + 1).Append(value).Append(' ');
								bool flag9 = valueMember2.DataFormat == DataFormat.Group;
								if (flag9)
								{
									builder.Append("group ");
								}
								string schemaTypeName2 = valueMember2.GetSchemaTypeName(true, ref requiresBclImport);
								builder.Append(schemaTypeName2).Append(" ").Append(valueMember2.Name).Append(" = ").Append(valueMember2.FieldNumber);
								bool flag10 = valueMember2.DefaultValue != null;
								if (flag10)
								{
									bool flag11 = valueMember2.DefaultValue is string;
									if (flag11)
									{
										builder.Append(" [default = \"").Append(valueMember2.DefaultValue).Append("\"]");
									}
									else
									{
										bool flag12 = valueMember2.DefaultValue is bool;
										if (flag12)
										{
											builder.Append(((bool)valueMember2.DefaultValue) ? " [default = true]" : " [default = false]");
										}
										else
										{
											builder.Append(" [default = ").Append(valueMember2.DefaultValue).Append(']');
										}
									}
								}
								bool flag13 = valueMember2.ItemType != null && valueMember2.IsPacked;
								if (flag13)
								{
									builder.Append(" [packed=true]");
								}
								builder.Append(';');
								bool flag14 = schemaTypeName2 == "bcl.NetObjectProxy" && valueMember2.AsReference && !valueMember2.DynamicType;
								if (flag14)
								{
									builder.Append(" // reference-tracked ").Append(valueMember2.GetSchemaTypeName(false, ref requiresBclImport));
								}
							}
							bool flag15 = this.subTypes != null && this.subTypes.Count != 0;
							if (flag15)
							{
								MetaType.NewLine(builder, indent + 1).Append("// the following represent sub-types; at most 1 should have a value");
								SubType[] array6 = new SubType[this.subTypes.Count];
								this.subTypes.CopyTo(array6, 0);
								Array.Sort<SubType>(array6, SubType.Comparer.Default);
								foreach (SubType subType in array6)
								{
									string schemaTypeName3 = subType.DerivedType.GetSchemaTypeName();
									MetaType.NewLine(builder, indent + 1).Append("optional ").Append(schemaTypeName3).Append(" ").Append(schemaTypeName3).Append(" = ").Append(subType.FieldNumber).Append(';');
								}
							}
							MetaType.NewLine(builder, indent).Append('}');
						}
					}
				}
			}
		}

		// Token: 0x04002938 RID: 10552
		private MetaType baseType;

		// Token: 0x04002939 RID: 10553
		private BasicList subTypes;

		// Token: 0x0400293A RID: 10554
		internal static readonly Type ienumerable = typeof(IEnumerable);

		// Token: 0x0400293B RID: 10555
		private CallbackSet callbacks;

		// Token: 0x0400293C RID: 10556
		private string name;

		// Token: 0x0400293D RID: 10557
		private MethodInfo factory;

		// Token: 0x0400293E RID: 10558
		private readonly RuntimeTypeModel model;

		// Token: 0x0400293F RID: 10559
		private readonly Type type;

		// Token: 0x04002940 RID: 10560
		private IProtoTypeSerializer serializer;

		// Token: 0x04002941 RID: 10561
		private Type constructType;

		// Token: 0x04002942 RID: 10562
		private Type surrogate;

		// Token: 0x04002943 RID: 10563
		private readonly BasicList fields = new BasicList();

		// Token: 0x04002944 RID: 10564
		private const byte OPTIONS_Pending = 1;

		// Token: 0x04002945 RID: 10565
		private const byte OPTIONS_EnumPassThru = 2;

		// Token: 0x04002946 RID: 10566
		private const byte OPTIONS_Frozen = 4;

		// Token: 0x04002947 RID: 10567
		private const byte OPTIONS_PrivateOnApi = 8;

		// Token: 0x04002948 RID: 10568
		private const byte OPTIONS_SkipConstructor = 16;

		// Token: 0x04002949 RID: 10569
		private const byte OPTIONS_AsReferenceDefault = 32;

		// Token: 0x0400294A RID: 10570
		private const byte OPTIONS_AutoTuple = 64;

		// Token: 0x0400294B RID: 10571
		private const byte OPTIONS_IgnoreListHandling = 128;

		// Token: 0x0400294C RID: 10572
		private volatile byte flags;

		// Token: 0x02001947 RID: 6471
		internal sealed class Comparer : IComparer, IComparer<MetaType>
		{
			// Token: 0x06010FC9 RID: 69577 RVA: 0x0045281C File Offset: 0x00450A1C
			public int Compare(object x, object y)
			{
				return this.Compare(x as MetaType, y as MetaType);
			}

			// Token: 0x06010FCA RID: 69578 RVA: 0x00452840 File Offset: 0x00450A40
			public int Compare(MetaType x, MetaType y)
			{
				bool flag = x == y;
				int result;
				if (flag)
				{
					result = 0;
				}
				else
				{
					bool flag2 = x == null;
					if (flag2)
					{
						result = -1;
					}
					else
					{
						bool flag3 = y == null;
						if (flag3)
						{
							result = 1;
						}
						else
						{
							result = string.Compare(x.GetSchemaTypeName(), y.GetSchemaTypeName(), StringComparison.Ordinal);
						}
					}
				}
				return result;
			}

			// Token: 0x04007D6C RID: 32108
			public static readonly MetaType.Comparer Default = new MetaType.Comparer();
		}

		// Token: 0x02001948 RID: 6472
		[Flags]
		internal enum AttributeFamily
		{
			// Token: 0x04007D6E RID: 32110
			None = 0,
			// Token: 0x04007D6F RID: 32111
			ProtoBuf = 1,
			// Token: 0x04007D70 RID: 32112
			DataContractSerialier = 2,
			// Token: 0x04007D71 RID: 32113
			XmlSerializer = 4,
			// Token: 0x04007D72 RID: 32114
			AutoTuple = 8
		}
	}
}
