using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	// Token: 0x0200086E RID: 2158
	internal class ListDecorator : ProtoDecoratorBase
	{
		// Token: 0x060083CD RID: 33741 RVA: 0x0010067C File Offset: 0x000FE87C
		internal static bool CanPack(WireType wireType)
		{
			return wireType <= WireType.Fixed64 || wireType == WireType.Fixed32 || wireType == WireType.SignedVariant;
		}

		// Token: 0x1700299D RID: 10653
		// (get) Token: 0x060083CE RID: 33742 RVA: 0x001006A8 File Offset: 0x000FE8A8
		private bool IsList
		{
			get
			{
				return (this.options & 1) > 0;
			}
		}

		// Token: 0x1700299E RID: 10654
		// (get) Token: 0x060083CF RID: 33743 RVA: 0x001006C8 File Offset: 0x000FE8C8
		private bool SuppressIList
		{
			get
			{
				return (this.options & 2) > 0;
			}
		}

		// Token: 0x1700299F RID: 10655
		// (get) Token: 0x060083D0 RID: 33744 RVA: 0x001006E8 File Offset: 0x000FE8E8
		private bool WritePacked
		{
			get
			{
				return (this.options & 4) > 0;
			}
		}

		// Token: 0x170029A0 RID: 10656
		// (get) Token: 0x060083D1 RID: 33745 RVA: 0x00100708 File Offset: 0x000FE908
		private bool SupportNull
		{
			get
			{
				return (this.options & 32) > 0;
			}
		}

		// Token: 0x170029A1 RID: 10657
		// (get) Token: 0x060083D2 RID: 33746 RVA: 0x00100728 File Offset: 0x000FE928
		private bool ReturnList
		{
			get
			{
				return (this.options & 8) > 0;
			}
		}

		// Token: 0x060083D3 RID: 33747 RVA: 0x00100748 File Offset: 0x000FE948
		internal static ListDecorator Create(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull)
		{
			MethodInfo builderFactory;
			MethodInfo methodInfo;
			MethodInfo addRange;
			MethodInfo finish;
			
			ListDecorator result;
			if (returnList && ImmutableCollectionDecorator.IdentifyImmutable(model, declaredType, out builderFactory, out methodInfo, out addRange, out finish))
			{
				result = new ImmutableCollectionDecorator(model, declaredType, concreteType, tail, fieldNumber, writePacked, packedWireType, returnList, overwriteList, supportNull, builderFactory, methodInfo, addRange, finish);
			}
			else
			{
				result = new ListDecorator(model, declaredType, concreteType, tail, fieldNumber, writePacked, packedWireType, returnList, overwriteList, supportNull);
			}
			return result;
		}

		// Token: 0x060083D4 RID: 33748 RVA: 0x001007AC File Offset: 0x000FE9AC
		protected ListDecorator(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull) : base(tail)
		{
			if (returnList)
			{
				this.options |= 8;
			}
			if (overwriteList)
			{
				this.options |= 16;
			}
			if (supportNull)
			{
				this.options |= 32;
			}
			bool flag = (writePacked || packedWireType != WireType.None) && fieldNumber <= 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			bool flag2 = !ListDecorator.CanPack(packedWireType);
			if (flag2)
			{
				if (writePacked)
				{
					throw new InvalidOperationException("Only simple data-types can use packed encoding");
				}
				packedWireType = WireType.None;
			}
			this.fieldNumber = fieldNumber;
			if (writePacked)
			{
				this.options |= 4;
			}
			this.packedWireType = packedWireType;
			bool flag3 = declaredType == null;
			if (flag3)
			{
				throw new ArgumentNullException("declaredType");
			}
			bool isArray = declaredType.IsArray;
			if (isArray)
			{
				throw new ArgumentException("Cannot treat arrays as lists", "declaredType");
			}
			this.declaredType = declaredType;
			this.concreteType = concreteType;
			bool requireAdd = this.RequireAdd;
			if (requireAdd)
			{
				bool flag4;
				this.add = TypeModel.ResolveListAdd(model, declaredType, tail.ExpectedType, out flag4);
				bool flag5 = flag4;
				if (flag5)
				{
					this.options |= 1;
					string fullName = declaredType.FullName;
					bool flag6 = fullName != null && fullName.StartsWith("System.Data.Linq.EntitySet`1[[");
					if (flag6)
					{
						this.options |= 2;
					}
				}
				bool flag7 = this.add == null;
				if (flag7)
				{
					throw new InvalidOperationException("Unable to resolve a suitable Add method for " + declaredType.FullName);
				}
			}
		}

		// Token: 0x170029A2 RID: 10658
		// (get) Token: 0x060083D5 RID: 33749 RVA: 0x0010094C File Offset: 0x000FEB4C
		protected virtual bool RequireAdd
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170029A3 RID: 10659
		// (get) Token: 0x060083D6 RID: 33750 RVA: 0x00100960 File Offset: 0x000FEB60
		public override Type ExpectedType
		{
			get
			{
				return this.declaredType;
			}
		}

		// Token: 0x170029A4 RID: 10660
		// (get) Token: 0x060083D7 RID: 33751 RVA: 0x00100978 File Offset: 0x000FEB78
		public override bool RequiresOldValue
		{
			get
			{
				return this.AppendToCollection;
			}
		}

		// Token: 0x170029A5 RID: 10661
		// (get) Token: 0x060083D8 RID: 33752 RVA: 0x00100990 File Offset: 0x000FEB90
		public override bool ReturnsValue
		{
			get
			{
				return this.ReturnList;
			}
		}

		// Token: 0x170029A6 RID: 10662
		// (get) Token: 0x060083D9 RID: 33753 RVA: 0x001009A8 File Offset: 0x000FEBA8
		protected bool AppendToCollection
		{
			get
			{
				return (this.options & 16) == 0;
			}
		}

		// Token: 0x060083DA RID: 33754 RVA: 0x001009C8 File Offset: 0x000FEBC8
		protected MethodInfo GetEnumeratorInfo(TypeModel model, out MethodInfo moveNext, out MethodInfo current)
		{
			Type type = null;
			Type expectedType = this.ExpectedType;
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(expectedType, "GetEnumerator", null);
			Type expectedType2 = this.Tail.ExpectedType;
			bool flag = instanceMethod != null;
			if (flag)
			{
				Type returnType = instanceMethod.ReturnType;
				Type type2 = returnType;
				moveNext = Helpers.GetInstanceMethod(type2, "MoveNext", null);
				PropertyInfo property = Helpers.GetProperty(type2, "Current", false);
				current = ((property == null) ? null : Helpers.GetGetMethod(property, false, false));
				bool flag2 = moveNext == null && model.MapType(ListDecorator.ienumeratorType).IsAssignableFrom(type2);
				if (flag2)
				{
					moveNext = Helpers.GetInstanceMethod(model.MapType(ListDecorator.ienumeratorType), "MoveNext", null);
				}
				bool flag3 = moveNext != null && moveNext.ReturnType == model.MapType(typeof(bool)) && current != null && current.ReturnType == expectedType2;
				if (flag3)
				{
					return instanceMethod;
				}
				MethodInfo methodInfo;
				current = (methodInfo = null);
				moveNext = methodInfo;
			}
			Type type3 = model.MapType(typeof(IEnumerable<>), false);
			bool flag4 = type3 != null;
			if (flag4)
			{
				type3 = type3.MakeGenericType(new Type[]
				{
					expectedType2
				});
				type = type3;
			}
			bool flag5 = type != null && type.IsAssignableFrom(expectedType);
			MethodInfo result;
			if (flag5)
			{
				instanceMethod = Helpers.GetInstanceMethod(type, "GetEnumerator");
				Type returnType = instanceMethod.ReturnType;
				Type type2 = returnType;
				moveNext = Helpers.GetInstanceMethod(model.MapType(ListDecorator.ienumeratorType), "MoveNext");
				current = Helpers.GetGetMethod(Helpers.GetProperty(type2, "Current", false), false, false);
				result = instanceMethod;
			}
			else
			{
				type = model.MapType(ListDecorator.ienumerableType);
				instanceMethod = Helpers.GetInstanceMethod(type, "GetEnumerator");
				Type returnType = instanceMethod.ReturnType;
				Type type2 = returnType;
				moveNext = Helpers.GetInstanceMethod(type2, "MoveNext");
				current = Helpers.GetGetMethod(Helpers.GetProperty(type2, "Current", false), false, false);
				result = instanceMethod;
			}
			return result;
		}

		// Token: 0x060083DB RID: 33755 RVA: 0x00100BA8 File Offset: 0x000FEDA8
		public override void Write(object value, ProtoWriter dest)
		{
			bool writePacked = this.WritePacked;
			bool flag = writePacked;
			SubItemToken token;
			if (flag)
			{
				ProtoWriter.WriteFieldHeader(this.fieldNumber, WireType.String, dest);
				token = ProtoWriter.StartSubItem(value, dest);
				ProtoWriter.SetPackedField(this.fieldNumber, dest);
			}
			else
			{
				token = default(SubItemToken);
			}
			bool flag2 = !this.SupportNull;
			foreach (object obj in ((IEnumerable)value))
			{
				bool flag3 = flag2 && obj == null;
				if (flag3)
				{
					throw new NullReferenceException();
				}
				this.Tail.Write(obj, dest);
			}
			bool flag4 = writePacked;
			if (flag4)
			{
				ProtoWriter.EndSubItem(token, dest);
			}
		}

		// Token: 0x060083DC RID: 33756 RVA: 0x00100C84 File Offset: 0x000FEE84
		public override object Read(object value, ProtoReader source)
		{
			int field = source.FieldNumber;
			object obj = value;
			bool flag = value == null;
			if (flag)
			{
				value = Activator.CreateInstance(this.concreteType);
			}
			bool flag2 = this.IsList && !this.SuppressIList;
			bool flag3 = this.packedWireType != WireType.None && source.WireType == WireType.String;
			if (flag3)
			{
				SubItemToken token = ProtoReader.StartSubItem(source);
				bool flag4 = flag2;
				if (flag4)
				{
					IList list = (IList)value;
					while (ProtoReader.HasSubValue(this.packedWireType, source))
					{
						list.Add(this.Tail.Read(null, source));
					}
				}
				else
				{
					while (ProtoReader.HasSubValue(this.packedWireType, source))
					{
						ProtoDecoratorBase.s_argsRead[0] = this.Tail.Read(null, source);
						this.add.Invoke(value, ProtoDecoratorBase.s_argsRead);
					}
				}
				ProtoReader.EndSubItem(token, source);
			}
			else
			{
				bool flag5 = flag2;
				if (flag5)
				{
					IList list2 = (IList)value;
					do
					{
						list2.Add(this.Tail.Read(null, source));
					}
					while (source.TryReadFieldHeader(field));
				}
				else
				{
					do
					{
						ProtoDecoratorBase.s_argsRead[0] = this.Tail.Read(null, source);
						this.add.Invoke(value, ProtoDecoratorBase.s_argsRead);
					}
					while (source.TryReadFieldHeader(field));
				}
			}
			return (obj == value) ? null : value;
		}

		// Token: 0x040028EF RID: 10479
		private readonly byte options;

		// Token: 0x040028F0 RID: 10480
		private const byte OPTIONS_IsList = 1;

		// Token: 0x040028F1 RID: 10481
		private const byte OPTIONS_SuppressIList = 2;

		// Token: 0x040028F2 RID: 10482
		private const byte OPTIONS_WritePacked = 4;

		// Token: 0x040028F3 RID: 10483
		private const byte OPTIONS_ReturnList = 8;

		// Token: 0x040028F4 RID: 10484
		private const byte OPTIONS_OverwriteList = 16;

		// Token: 0x040028F5 RID: 10485
		private const byte OPTIONS_SupportNull = 32;

		// Token: 0x040028F6 RID: 10486
		private readonly Type declaredType;

		// Token: 0x040028F7 RID: 10487
		private readonly Type concreteType;

		// Token: 0x040028F8 RID: 10488
		private readonly MethodInfo add;

		// Token: 0x040028F9 RID: 10489
		private readonly int fieldNumber;

		// Token: 0x040028FA RID: 10490
		protected readonly WireType packedWireType;

		// Token: 0x040028FB RID: 10491
		private static readonly Type ienumeratorType = typeof(IEnumerator);

		// Token: 0x040028FC RID: 10492
		private static readonly Type ienumerableType = typeof(IEnumerable);
	}
}
