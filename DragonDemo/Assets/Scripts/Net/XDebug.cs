using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace XUtliPoolLib
{

    public class XDebug : XSingleton<XDebug>
	{
		public static void TraceStack(object obj = null)
		{
			string content = "";
			if (obj is string)
			{
				content = obj.ToString();
			}
			else
			{
				content = (obj == null ? " - null - " : obj.GetType().Name);
			}

			UnityEngine.Debug.Log(new string('*', 39) + content + new string('*', 39));
			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
			System.Diagnostics.StackFrame[] sfs = st.GetFrames();
			for (int u = 0; u < sfs.Length; ++u)
			{
				System.Reflection.MethodBase mb = sfs[u].GetMethod();

				UnityEngine.Debug.Log(string.Format("[CALL STACK][{0}]: {1}.{2}  line:{3}", u, mb.DeclaringType.FullName, mb.Name, sfs[u].GetFileLineNumber()));
			}
		}
		 
		public XDebug()
		{
			this._OutputChannels = 5;
		}

	 
		public void Init(IPlatform platform, XFileLog log)
		{
			this._platform = XSingleton<XUpdater.XUpdater>.singleton.XPlatform;
		}

	 
		public void AddLog(string log1, string log2 = null, string log3 = null, string log4 = null, string log5 = null, string log6 = null, XDebugColor color = XDebugColor.XDebug_None)
		{
			bool flag = this._platform != null && !this._platform.IsPublish();
			if (flag)
			{
				this.AddLog(XDebugChannel.XDebug_Default, log1, log2, log3, log4, log5, log6, color);
			}
		}

	 
		public void AddLog2(string format, params object[] args)
		{
			bool flag = this._showLog && this._platform != null && !this._platform.IsPublish();
			if (flag)
			{
				this.AddLog(string.Format(format, args), null, null, null, null, null, XDebugColor.XDebug_None);
			}
		}

	 
		public void AddLog(XDebugChannel channel, string log1, string log2 = null, string log3 = null, string log4 = null, string log5 = null, string log6 = null, XDebugColor color = XDebugColor.XDebug_None)
		{
			bool flag = this._showLog && this._platform != null && !this._platform.IsPublish();
			if (flag)
			{
				bool flag2 = (this._OutputChannels & (int)channel) > 0;
				if (flag2)
				{
					this._buffer.Length = 0;
					this._buffer.Append(log1).Append(log2).Append(log3).Append(log4).Append(log5).Append(log6);
					bool flag3 = color == XDebugColor.XDebug_Green;
					if (flag3)
					{
						this._buffer.Insert(0, "<color=green>");
						this._buffer.Append("</color>");
					}
					bool showTimeStick = this._showTimeStick;
					if (showTimeStick)
					{
						bool flag4 = Thread.CurrentThread.ManagedThreadId == XSingleton<XUpdater.XUpdater>.singleton.ManagedThreadId;
						if (flag4)
						{
							this._buffer.Append(" (at Frame: ").Append(Time.frameCount).Append(" sec: ").Append(Time.realtimeSinceStartup.ToString("F3")).Append(')');
						}
						else
						{
							bool flag5 = string.IsNullOrEmpty(Thread.CurrentThread.Name);
							if (flag5)
							{
								this._buffer.Append(" (from anonymous thread").Append(" with id ").Append(Thread.CurrentThread.ManagedThreadId).Append(")");
							}
							else
							{
								this._buffer.Append(" (from thread ").Append(Thread.CurrentThread.Name).Append(" with id ").Append(Thread.CurrentThread.ManagedThreadId).Append(")");
							}
						}
					}
					bool flag6 = color == XDebugColor.XDebug_Red;
					if (flag6)
					{
						Debug.LogError(this._buffer);
					}
					else
					{
						bool flag7 = color == XDebugColor.XDebug_Yellow;
						if (flag7)
						{
							Debug.LogWarning(this._buffer);
						}
						else
						{
							Debug.Log(this._buffer);
						}
					}
				}
			}
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x000308BC File Offset: 0x0002EABC
		public void AddGreenLog(string log1, string log2 = null, string log3 = null, string log4 = null, string log5 = null, string log6 = null)
		{
			bool flag = this._platform != null && !this._platform.IsPublish();
			if (flag)
			{
				this.AddLog(XDebugChannel.XDebug_Default, log1, log2, log3, log4, log5, log6, XDebugColor.XDebug_Green);
			}
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x000308FC File Offset: 0x0002EAFC
		public void AddWarningLog(string log1, string log2 = null, string log3 = null, string log4 = null, string log5 = null, string log6 = null)
		{
			bool flag = this._platform != null && !this._platform.IsPublish();
			if (flag)
			{
				this.AddLog(XDebugChannel.XDebug_Default, log1, log2, log3, log4, log5, log6, XDebugColor.XDebug_Yellow);
			}
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x0003093C File Offset: 0x0002EB3C
		public void AddWarningLog2(string format, params object[] args)
		{
			bool flag = this._showLog && this._platform != null && !this._platform.IsPublish();
			if (flag)
			{
				this.AddWarningLog(string.Format(format, args), null, null, null, null, null);
			}
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x00030984 File Offset: 0x0002EB84
		public void AddErrorLog2(string format, params object[] args)
		{
			bool flag = this._showLog && this._platform != null && !this._platform.IsPublish();
			if (flag)
			{
				this.AddErrorLog(string.Format(format, args), null, null, null, null, null);
			}
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x000309CC File Offset: 0x0002EBCC
		public void AddErrorLog(string log1, string log2 = null, string log3 = null, string log4 = null, string log5 = null, string log6 = null)
		{
			this._buffer.Length = 0;
			bool flag = Thread.CurrentThread.ManagedThreadId == XSingleton<XUpdater.XUpdater>.singleton.ManagedThreadId;
			if (flag)
			{
				this._buffer.Append(log1).Append(log2).Append(log3).Append(log4).Append(log5).Append(log6).Append(" (at Frame: ").Append(Time.frameCount).Append(" sec: ").Append(Time.realtimeSinceStartup.ToString("F3")).Append(')');
			}
			else
			{
				this._buffer.Append(log1).Append(log2).Append(log3).Append(log4).Append(log5).Append(log6);
			}
			XFileLog.AddCustomLog("AddErrorLog:  " + this._buffer);
			this.AddLog(XDebugChannel.XDebug_Default, log1, log2, log3, log4, log5, log6, XDebugColor.XDebug_Red);
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x00030AC8 File Offset: 0x0002ECC8
		public override bool Init()
		{
			return true;
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x00003284 File Offset: 0x00001484
		public override void Uninit()
		{
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x00003284 File Offset: 0x00001484
		public void BeginProfile(string title)
		{
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x00003284 File Offset: 0x00001484
		public void RegisterGroupProfile(string title)
		{
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x00003284 File Offset: 0x00001484
		public void BeginGroupProfile(string title)
		{
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x00003284 File Offset: 0x00001484
		public void EndProfile()
		{
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x00003284 File Offset: 0x00001484
		public void EndGroupProfile(string title)
		{
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x00003284 File Offset: 0x00001484
		public void InitProfiler()
		{
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x00003284 File Offset: 0x00001484
		public void PrintProfiler()
		{
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x00030ADC File Offset: 0x0002ECDC
		public void StartRecord(XDebug.RecordChannel channel)
		{
			this.m_record = true;
			this.m_recordStart = false;
			this.m_RecordChannel = channel;
			XDebug.RecordChannel recordChannel = this.m_RecordChannel;
			if (recordChannel != XDebug.RecordChannel.ENetwork)
			{
				if (recordChannel == XDebug.RecordChannel.EResourceLoad)
				{
					this.m_LayerInfo[0].Name = "SharedRes";
					this.m_LayerInfo[1].Name = "Prefab";
				}
			}
			else
			{
				this.m_LayerInfo[0].Name = "Recv";
				this.m_LayerInfo[1].Name = "Send";
			}
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x00030B5E File Offset: 0x0002ED5E
		public void EndRecord()
		{
			this.m_record = false;
			this.m_recordStart = false;
			this.m_RecordChannel = XDebug.RecordChannel.ENone;
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x00030B78 File Offset: 0x0002ED78
		public void BeginRecord()
		{
			bool record = this.m_record;
			if (record)
			{
				this.m_recordStart = true;
			}
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x00030B99 File Offset: 0x0002ED99
		public void ClearRecord()
		{
			this.m_LayerInfo[0].Clear();
			this.m_LayerInfo[1].Clear();
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x00030BB8 File Offset: 0x0002EDB8
		public bool EnableRecord()
		{
			return this.m_recordStart;
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x00030BD0 File Offset: 0x0002EDD0
		public void AddPoint(uint key, string name, float size, int layer, XDebug.RecordChannel channel)
		{
			bool flag = this.m_recordStart && channel == this.m_RecordChannel && layer >= 0 && layer < this.m_MaxLayer;
			if (flag)
			{
				Dictionary<uint, XDebug.SizeInfo> sizeInfoMap = this.m_LayerInfo[layer].SizeInfoMap;
				XDebug.SizeInfo sizeInfo = null;
				bool flag2 = !sizeInfoMap.TryGetValue(key, out sizeInfo);
				if (flag2)
				{
					sizeInfo = new XDebug.SizeInfo();
					sizeInfo.Name = name;
					sizeInfoMap.Add(key, sizeInfo);
				}
				sizeInfo.Count++;
				sizeInfo.Size += size;
			}
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x00030C60 File Offset: 0x0002EE60
		public void Print()
		{
			//float num = 0f;
			//for (int i = 0; i < this.m_MaxLayer; i++)
			//{
			//	XSingleton<XCommon>.singleton.CleanStringCombine();
			//	float num2 = 0f;
			//	XDebug.SizeInfo sizeInfo = null;
			//	XDebug.LayerInfo layerInfo = this.m_LayerInfo[i];
			//	XSingleton<XCommon>.singleton.AppendString(layerInfo.Name, ":\r\n");
			//	foreach (KeyValuePair<uint, XDebug.SizeInfo> keyValuePair in layerInfo.SizeInfoMap)
			//	{
			//		XDebug.SizeInfo value = keyValuePair.Value;
			//		XSingleton<XCommon>.singleton.AppendString(string.Format("Name:{0} Count:{1} Size：{2}\r\n", value.Name, value.Count, value.Size));
			//		num += value.Size;
			//		num2 += value.Size;
			//		bool flag = sizeInfo == null || sizeInfo.Size < value.Size;
			//		if (flag)
			//		{
			//			sizeInfo = value;
			//		}
			//	}
			//	XSingleton<XDebug>.singleton.AddWarningLog(XSingleton<XCommon>.singleton.GetString(), null, null, null, null, null);
			//	bool flag2 = sizeInfo != null;
			//	if (flag2)
			//	{
			//		XSingleton<XDebug>.singleton.AddWarningLog2("max name:{0} Count:{1} Size：{2}", new object[]
			//		{
			//			sizeInfo.Name,
			//			sizeInfo.Count,
			//			sizeInfo.Size
			//		});
			//	}
			//	XSingleton<XDebug>.singleton.AddWarningLog2("total {0} size:{1}", new object[]
			//	{
			//		layerInfo.Name,
			//		num2
			//	});
			//}
			//XSingleton<XDebug>.singleton.AddWarningLog("total size:", num.ToString(), null, null, null, null);
		}

		// Token: 0x04000421 RID: 1057
		private int _OutputChannels = 0;

		// Token: 0x04000422 RID: 1058
		private IPlatform _platform = null;

		// Token: 0x04000423 RID: 1059
		private bool _showTimeStick = true;

		// Token: 0x04000424 RID: 1060
		private bool _showLog = true;

		// Token: 0x04000425 RID: 1061
		private StringBuilder _buffer = new StringBuilder();

		// Token: 0x04000426 RID: 1062
		private XDebug.LayerInfo[] m_LayerInfo = new XDebug.LayerInfo[]
		{
			new XDebug.LayerInfo(),
			new XDebug.LayerInfo()
		};

		// Token: 0x04000427 RID: 1063
		private int m_MaxLayer = 2;

		// Token: 0x04000428 RID: 1064
		private bool m_record = false;

		// Token: 0x04000429 RID: 1065
		private bool m_recordStart = false;

		// Token: 0x0400042A RID: 1066
		private XDebug.RecordChannel m_RecordChannel = XDebug.RecordChannel.ENone;

		// Token: 0x02000391 RID: 913
		public enum RecordChannel
		{
			// Token: 0x04000FCE RID: 4046
			ENone,
			// Token: 0x04000FCF RID: 4047
			ENetwork,
			// Token: 0x04000FD0 RID: 4048
			EResourceLoad
		}

		// Token: 0x02000392 RID: 914
		public enum RecordLayer
		{
			// Token: 0x04000FD2 RID: 4050
			ELayer0,
			// Token: 0x04000FD3 RID: 4051
			ELayer1,
			// Token: 0x04000FD4 RID: 4052
			ENum
		}

		// Token: 0x02000393 RID: 915
		public class SizeInfo
		{
			// Token: 0x04000FD5 RID: 4053
			public string Name = "";

			// Token: 0x04000FD6 RID: 4054
			public float Size = 0f;

			// Token: 0x04000FD7 RID: 4055
			public int Count = 0;
		}

		// Token: 0x02000394 RID: 916
		public class LayerInfo
		{
			// Token: 0x06000F2D RID: 3885 RVA: 0x0004A539 File Offset: 0x00048739
			public void Clear()
			{
				this.SizeInfoMap.Clear();
			}

			// Token: 0x04000FD8 RID: 4056
			public string Name = "";

			// Token: 0x04000FD9 RID: 4057
			public Dictionary<uint, XDebug.SizeInfo> SizeInfoMap = new Dictionary<uint, XDebug.SizeInfo>();
		}
	}
}
