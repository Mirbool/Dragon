using System;
using System.Collections.Generic;
using UnityEngine;

namespace XUtliPoolLib
{
	// Token: 0x020001CE RID: 462
	public sealed class XTimerMgr : XSingleton<XTimerMgr>
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000A9C RID: 2716 RVA: 0x00038DB4 File Offset: 0x00036FB4
		public double Elapsed
		{
			get
			{
				return this._elapsed;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000A9D RID: 2717 RVA: 0x00038DCC File Offset: 0x00036FCC
		public bool NeedFixedUpdate
		{
			get
			{
				return this._fixedUpdate;
			}
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x00038DE4 File Offset: 0x00036FE4
		public uint SetTimer(float interval, XTimerMgr.ElapsedEventHandler handler, object param)
		{
			this._token += 1U;
			bool flag = interval <= 0f;
			if (flag)
			{
				handler(param);
				this._token += 1U;
			}
			else
			{
				double trigger = this._elapsed + Math.Round((double)interval, 3);
				XTimerMgr.XTimer timer = this.GetTimer(trigger, handler, param, this._token, false, -1);
				this._timers.PushHeap(timer);
				this._dict.Add(this._token, timer);
			}
			return this._token;
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00038E78 File Offset: 0x00037078
		public uint SetTimer<TEnum>(float interval, XTimerMgr.ElapsedIDEventHandler handler, object param, TEnum e) where TEnum : struct
		{
			this._token += 1U;
			int id = XFastEnumIntEqualityComparer<TEnum>.ToInt(e);
			bool flag = interval <= 0f;
			if (flag)
			{
				handler(param, id);
				this._token += 1U;
			}
			else
			{
				double trigger = this._elapsed + Math.Round((double)interval, 3);
				XTimerMgr.XTimer timer = this.GetTimer(trigger, handler, param, this._token, false, id);
				this._timers.PushHeap(timer);
				this._dict.Add(this._token, timer);
			}
			return this._token;
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00038F18 File Offset: 0x00037118
		public uint SetGlobalTimer(float interval, XTimerMgr.ElapsedEventHandler handler, object param)
		{
			this._token += 1U;
			bool flag = interval <= 0f;
			if (flag)
			{
				handler(param);
				this._token += 1U;
			}
			else
			{
				double trigger = this._elapsed + Math.Round((double)interval, 3);
				XTimerMgr.XTimer timer = this.GetTimer(trigger, handler, param, this._token, true, -1);
				this._timers.PushHeap(timer);
				this._dict.Add(this._token, timer);
			}
			return this._token;
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00038FAC File Offset: 0x000371AC
		public uint SetTimerAccurate(float interval, XTimerMgr.AccurateElapsedEventHandler handler, object param)
		{
			this._token += 1U;
			bool flag = interval <= 0f;
			if (flag)
			{
				handler(param, 0f);
				this._token += 1U;
			}
			else
			{
				double trigger = this._elapsed + Math.Round((double)interval, 3);
				XTimerMgr.XTimer timer = this.GetTimer(trigger, handler, param, this._token, false, -1);
				this._timers.PushHeap(timer);
				this._dict.Add(this._token, timer);
			}
			return this._token;
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00039044 File Offset: 0x00037244
		public void AdjustTimer(float interval, uint token, bool closed = false)
		{
			XTimerMgr.XTimer xtimer = null;
			bool flag = this._dict.TryGetValue(token, out xtimer) && !xtimer.IsInPool;
			if (flag)
			{
				double trigger = closed ? (this._elapsed - (double)(Time.deltaTime * 0.5f) + Math.Round((double)interval, 3)) : (this._elapsed + Math.Round((double)interval, 3));
				double triggerTime = xtimer.TriggerTime;
				xtimer.Refine(trigger);
				this._timers.Adjust(xtimer, triggerTime < xtimer.TriggerTime);
			}
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x000390CC File Offset: 0x000372CC
		public void KillTimerAll()
		{
			List<XTimerMgr.XTimer> list = new List<XTimerMgr.XTimer>();
			foreach (XTimerMgr.XTimer xtimer in this._dict.Values)
			{
				bool isGlobaled = xtimer.IsGlobaled;
				if (!isGlobaled)
				{
					list.Add(xtimer);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.KillTimer(list[i]);
			}
			list.Clear();
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x00039170 File Offset: 0x00037370
		private void KillTimer(XTimerMgr.XTimer timer)
		{
			bool flag = timer == null;
			if (!flag)
			{
				this._timers.PopHeapAt(timer.Here);
				this.Discard(timer);
			}
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x000391A4 File Offset: 0x000373A4
		public void KillTimer(uint token)
		{
			bool flag = token == 0U;
			if (!flag)
			{
				XTimerMgr.XTimer timer = null;
				bool flag2 = this._dict.TryGetValue(token, out timer);
				if (flag2)
				{
					this.KillTimer(timer);
				}
			}
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x000391DC File Offset: 0x000373DC
		public double TimeLeft(uint token)
		{
			XTimerMgr.XTimer xtimer = null;
			bool flag = this._dict.TryGetValue(token, out xtimer);
			double result;
			if (flag)
			{
				result = xtimer.TimeLeft();
			}
			else
			{
				result = 0.0;
			}
			return result;
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00039218 File Offset: 0x00037418
		public void Update(float fDeltaT)
		{
			this._elapsed += (double)fDeltaT;
			this._intervalTime += fDeltaT;
			bool flag = this._intervalTime > this._updateTime;
			if (flag)
			{
				this._intervalTime = 0f;
				this._fixedUpdate = true;
			}
			this.TriggerTimers();
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x00039270 File Offset: 0x00037470
		public void PostUpdate()
		{
			this._fixedUpdate = false;
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x0003927C File Offset: 0x0003747C
		private void TriggerTimers()
		{
			while (this._timers.HeapSize > 0)
			{
				XTimerMgr.XTimer xtimer = this._timers.Peek();
				float num = (float)(this._elapsed - xtimer.TriggerTime);
				bool flag = num >= 0f;
				if (!flag)
				{
					break;
				}
				this.ExecuteTimer(this._timers.PopHeap(), num);
			}
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x000392E4 File Offset: 0x000374E4
		private void ExecuteTimer(XTimerMgr.XTimer timer, float delta)
		{
			this.Discard(timer);
			timer.Fire(delta);
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x000392F8 File Offset: 0x000374F8
		private void Discard(XTimerMgr.XTimer timer)
		{
			bool isInPool = timer.IsInPool;
			if (!isInPool)
			{
				bool flag = this._dict.Remove(timer.Token);
				if (flag)
				{
					timer.IsInPool = true;
					this._pool.Enqueue(timer);
				}
			}
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x00039340 File Offset: 0x00037540
		private XTimerMgr.XTimer GetTimer(double trigger, object handler, object parma, uint token, bool global, int id = -1)
		{
			bool flag = this._pool.Count > 0;
			XTimerMgr.XTimer result;
			if (flag)
			{
				XTimerMgr.XTimer xtimer = this._pool.Dequeue();
				xtimer.Refine(trigger, handler, parma, token, global, id);
				result = xtimer;
			}
			else
			{
				result = new XTimerMgr.XTimer(trigger, handler, parma, token, global, id);
			}
			return result;
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x00039394 File Offset: 0x00037594
		public override bool Init()
		{
			return true;
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x00003284 File Offset: 0x00001484
		public override void Uninit()
		{
		}

		// Token: 0x0400050A RID: 1290
		private uint _token = 0U;

		// Token: 0x0400050B RID: 1291
		private double _elapsed = 0.0;

		// Token: 0x0400050C RID: 1292
		private Queue<XTimerMgr.XTimer> _pool = new Queue<XTimerMgr.XTimer>();

		// Token: 0x0400050D RID: 1293
		private XHeap<XTimerMgr.XTimer> _timers = new XHeap<XTimerMgr.XTimer>();

		// Token: 0x0400050E RID: 1294
		private Dictionary<uint, XTimerMgr.XTimer> _dict = new Dictionary<uint, XTimerMgr.XTimer>(20);

		// Token: 0x0400050F RID: 1295
		private float _intervalTime = 0f;

		// Token: 0x04000510 RID: 1296
		private float _updateTime = 0.1f;

		// Token: 0x04000511 RID: 1297
		private bool _fixedUpdate = false;

		// Token: 0x04000512 RID: 1298
		public bool update = true;

		// Token: 0x04000513 RID: 1299
		public float updateStartTime = 0f;

		// Token: 0x020003A1 RID: 929
		// (Invoke) Token: 0x06000F74 RID: 3956
		public delegate void ElapsedEventHandler(object param);

		// Token: 0x020003A2 RID: 930
		// (Invoke) Token: 0x06000F78 RID: 3960
		public delegate void AccurateElapsedEventHandler(object param, float delay);

		// Token: 0x020003A3 RID: 931
		// (Invoke) Token: 0x06000F7C RID: 3964
		public delegate void ElapsedIDEventHandler(object param, int id);

		// Token: 0x020003A4 RID: 932
		private sealed class XTimer : IComparable<XTimerMgr.XTimer>, IHere
		{
			// Token: 0x06000F7F RID: 3967 RVA: 0x0004B864 File Offset: 0x00049A64
			public XTimer(double trigger, object handler, object parma, uint token, bool global, int id)
			{
				this.Refine(trigger, handler, parma, token, global, id);
			}

			// Token: 0x06000F80 RID: 3968 RVA: 0x0004B88C File Offset: 0x00049A8C
			public void Refine(double trigger, object handler, object parma, uint token, bool global, int id)
			{
				this._triggerTime = trigger;
				this._handler = handler;
				this._param = parma;
				this._global = global;
				this._token = token;
				this.Here = -1;
				this.IsInPool = false;
				this.Id = id;
			}

			// Token: 0x06000F81 RID: 3969 RVA: 0x0004B8D8 File Offset: 0x00049AD8
			public void Refine(double trigger)
			{
				this._triggerTime = trigger;
			}

			// Token: 0x17000124 RID: 292
			// (get) Token: 0x06000F82 RID: 3970 RVA: 0x0004B8E4 File Offset: 0x00049AE4
			public double TriggerTime
			{
				get
				{
					return this._triggerTime;
				}
			}

			// Token: 0x17000125 RID: 293
			// (get) Token: 0x06000F83 RID: 3971 RVA: 0x0004B8FC File Offset: 0x00049AFC
			public bool IsGlobaled
			{
				get
				{
					return this._global;
				}
			}

			// Token: 0x17000126 RID: 294
			// (get) Token: 0x06000F84 RID: 3972 RVA: 0x0004B914 File Offset: 0x00049B14
			// (set) Token: 0x06000F85 RID: 3973 RVA: 0x0004B91C File Offset: 0x00049B1C
			public bool IsInPool { get; set; }

			// Token: 0x17000127 RID: 295
			// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0004B928 File Offset: 0x00049B28
			public uint Token
			{
				get
				{
					return this._token;
				}
			}

			// Token: 0x17000128 RID: 296
			// (get) Token: 0x06000F87 RID: 3975 RVA: 0x0004B940 File Offset: 0x00049B40
			// (set) Token: 0x06000F88 RID: 3976 RVA: 0x0004B948 File Offset: 0x00049B48
			public int Here { get; set; }

			// Token: 0x17000129 RID: 297
			// (get) Token: 0x06000F89 RID: 3977 RVA: 0x0004B951 File Offset: 0x00049B51
			// (set) Token: 0x06000F8A RID: 3978 RVA: 0x0004B959 File Offset: 0x00049B59
			public int Id { get; set; }

			// Token: 0x06000F8B RID: 3979 RVA: 0x0004B964 File Offset: 0x00049B64
			public void Fire(float delta)
			{
				bool flag = this._handler is XTimerMgr.AccurateElapsedEventHandler;
				if (flag)
				{
					(this._handler as XTimerMgr.AccurateElapsedEventHandler)(this._param, delta);
				}
				else
				{
					bool flag2 = this._handler is XTimerMgr.ElapsedIDEventHandler;
					if (flag2)
					{
						(this._handler as XTimerMgr.ElapsedIDEventHandler)(this._param, this.Id);
					}
					else
					{
						(this._handler as XTimerMgr.ElapsedEventHandler)(this._param);
					}
				}
			}

			// Token: 0x06000F8C RID: 3980 RVA: 0x0004B9EC File Offset: 0x00049BEC
			public int CompareTo(XTimerMgr.XTimer other)
			{
				return (int)((this._triggerTime == other._triggerTime) ? (this._token - other.Token) : ((this._triggerTime < other._triggerTime) ? uint.MaxValue : 1U));
			}

			// Token: 0x06000F8D RID: 3981 RVA: 0x0004BA30 File Offset: 0x00049C30
			public double TimeLeft()
			{
				return this._triggerTime - XSingleton<XTimerMgr>.singleton.Elapsed;
			}

			// Token: 0x04001043 RID: 4163
			private double _triggerTime;

			// Token: 0x04001044 RID: 4164
			private object _param;

			// Token: 0x04001045 RID: 4165
			private object _handler;

			// Token: 0x04001046 RID: 4166
			private bool _global = false;

			// Token: 0x04001047 RID: 4167
			private uint _token = 0U;
		}
	}
}
