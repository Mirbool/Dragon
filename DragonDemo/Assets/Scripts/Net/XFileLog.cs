using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XUpdater;

namespace XUtliPoolLib
{
	// Token: 0x02000219 RID: 537
	public class XFileLog : MonoBehaviour
	{
		// Token: 0x06000C12 RID: 3090 RVA: 0x0003F0B0 File Offset: 0x0003D2B0
		private void Start()
		{
			this._outpath = Application.persistentDataPath + string.Format("/{0}{1}{2}_{3}{4}{5}.log", new object[]
			{
				DateTime.Now.Year.ToString().PadLeft(2, '0'),
				DateTime.Now.Month.ToString().PadLeft(2, '0'),
				DateTime.Now.Day.ToString().PadLeft(2, '0'),
				DateTime.Now.Hour.ToString().PadLeft(2, '0'),
				DateTime.Now.Minute.ToString().PadLeft(2, '0'),
				DateTime.Now.Second.ToString().PadLeft(2, '0')
			});
			string path = ((int)Application.platform == 8) ? ("/private" + Application.persistentDataPath) : Application.persistentDataPath;
			bool flag = Directory.Exists(path);
			if (flag)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				FileInfo[] files = directoryInfo.GetFiles();
				bool flag2 = files != null;
				if (flag2)
				{
					for (int i = 0; i < files.Length; i++)
					{
						string a = files[i].Name.Substring(files[i].Name.LastIndexOf(".") + 1);
						bool flag3 = a != "log";
						if (!flag3)
						{
							bool flag4 = DateTime.Now.Subtract(files[i].CreationTime).TotalDays > 1.0;
							if (flag4)
							{
								try
								{
									files[i].Delete();
								}
								catch
								{
									XSingleton<XDebug>.singleton.AddErrorLog("Del Log File Error!!!", null, null, null, null, null);
								}
							}
						}
					}
				}
			}
			XFileLog.callBack = new Application.LogCallback(this.HandleLog);
			Application.logMessageReceived += XFileLog.callBack;
			XSingleton<XDebug>.singleton.AddLog(this._outpath, null, null, null, null, null, XDebugColor.XDebug_None);
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0003F2E8 File Offset: 0x0003D4E8
		public void HandleLog(string logString, string stackTrace, LogType type)
		{
			bool flag = !this._firstWrite;
			if (!flag)
			{
				bool logOpen = this._logOpen;
				if (logOpen)
				{
					this.WriterLog(logString);
				}
				bool flag2 = type == null || (int)type == 4;
				if (flag2)
				{
					this._firstWrite = false;
					this.Log(new object[]
					{
						logString
					});
					this.Log(new object[]
					{
						stackTrace
					});
					string text = string.Format("{0}\n{1}\n", logString, stackTrace);
					while (XFileLog.CustomLogQueue.Count > 0)
					{
						text = string.Format("{0}\n{1}", text, XFileLog.CustomLogQueue.Dequeue());
					}
					//this.SendBuglyReport(text);
					this._guiLog = text;
					this._showGuiLog = true;
				}
			}
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0003F3A4 File Offset: 0x0003D5A4
		//private void OnGUI()
		//{
		//	bool logBundleOpen = XFileLog._logBundleOpen;
		//	if (logBundleOpen)
		//	{
		//		bool flag = XSingleton<XUpdater.XUpdater>.singleton.ABManager != null;
		//		if (flag)
		//		{
		//			GUI.TextArea(new Rect(0f, 30f, 100f, 30f), XSingleton<XUpdater.XUpdater>.singleton.ABManager.BundleCount.ToString());
		//		}
		//	}
		//	bool flag2 = this.fontStyle == null;
		//	if (flag2)
		//	{
		//		this.fontStyle = new GUIStyle();
		//	}
		//	bool showGuiLog = this._showGuiLog;
		//	if (showGuiLog)
		//	{
		//		bool flag3 = GUI.Button(new Rect(0f, 0f, 100f, 30f), "CrashLog");
		//		if (flag3)
		//		{
		//			this._showGuiLog = !this._showGuiLog;
		//		}
		//		bool showGuiLog2 = this._showGuiLog;
		//		if (showGuiLog2)
		//		{
		//			this.fontStyle.normal.textColor = new Color(1f, 0f, 0f);
		//			this.fontStyle.fontSize = 14;
		//			this.fontStyle.normal.background = Texture2D.whiteTexture;
		//			GUI.TextArea(new Rect(0f, 40f, 1136f, 3200f), this._guiLog, this.fontStyle);
		//		}
		//	}
		//	bool key = Input.GetKey(KeyCode.F5);
		//	if (key)
		//	{
		//		XFileLog._OpenCustomBtn = !XFileLog._OpenCustomBtn;
		//	}
		//	bool openCustomBtn = XFileLog._OpenCustomBtn;
		//	if (openCustomBtn)
		//	{
		//		bool flag4 = GUI.Button(new Rect(250f, 0f, 150f, 50f), "Info");
		//		if (flag4)
		//		{
		//			this._showCustomInfo = !this._showCustomInfo;
		//		}
		//	}
		//	bool showCustomInfo = this._showCustomInfo;
		//	if (showCustomInfo)
		//	{
		//		this.fontStyle.normal.textColor = new Color(0f, 0f, 0f);
		//		this.fontStyle.fontSize = 16;
		//		this.fontStyle.normal.background = Texture2D.whiteTexture;
		//		this.scrollPosition = GUI.BeginScrollView(new Rect(0f, 30f, 1136f, 640f), this.scrollPosition, new Rect(0f, 30f, 1136f, (float)(XFileLog._customInfoHeight * (this.fontStyle.fontSize + 2) + 100)));
		//		GUI.Label(new Rect(0f, 30f, 1136f, (float)(XFileLog._customInfoHeight * (this.fontStyle.fontSize + 2) + 30)), XFileLog._customInfo, this.fontStyle);
		//		GUI.EndScrollView();
		//	}
		//}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0003F644 File Offset: 0x0003D844
		public void WriterLog(string logString)
		{
			using (StreamWriter streamWriter = new StreamWriter(this._outpath, true, Encoding.UTF8))
			{
				streamWriter.WriteLine(string.Format("[{0}]{1}", string.Format("{0}/{1}/{2} {3}:{4}:{5}.{6}", new object[]
				{
					DateTime.Now.Year,
					DateTime.Now.Month.ToString().PadLeft(2, '0'),
					DateTime.Now.Day.ToString().PadLeft(2, '0'),
					DateTime.Now.Hour.ToString().PadLeft(2, '0'),
					DateTime.Now.Minute.ToString().PadLeft(2, '0'),
					DateTime.Now.Second.ToString().PadLeft(2, '0'),
					DateTime.Now.Millisecond.ToString().PadLeft(3, '0')
				}), logString));
				XSingleton<XUpdater.XUpdater>.singleton.XPlatform.SetNoBackupFlag(this._outpath);
			}
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x00003284 File Offset: 0x00001484
		private void Update()
		{
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0003F79C File Offset: 0x0003D99C
		public void Log(params object[] objs)
		{
			string text = "";
			for (int i = 0; i < objs.Length; i++)
			{
				bool flag = i == 0;
				if (flag)
				{
					text += objs[i].ToString();
				}
				else
				{
					text = text + ", " + objs[i].ToString();
				}
			}
			this.WriterLog(text);
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0003F800 File Offset: 0x0003DA00
		//public void SendBuglyReport(string logstring)
		//{
		//	bool flag = (int)Application.platform != 7 && Application.platform > 0;
		//	if (flag)
		//	{
		//		IXBuglyMgr ixbuglyMgr = XUpdater.XUpdater.XGameRoot.GetComponent("XBuglyMgr") as IXBuglyMgr;
		//		ixbuglyMgr.ReportCrashToBugly(XFileLog.ServerID, XFileLog.RoleName, XFileLog.RoleLevel, XFileLog.RoleProf, XFileLog.OpenID, XSingleton<XUpdater.XUpdater>.singleton.Version, Time.realtimeSinceStartup.ToString(), "loaded", XFileLog.SceneID.ToString(), logstring);
		//	}
		//}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0003F884 File Offset: 0x0003DA84
		public static void AddCustomLog(string customLog)
		{
			XFileLog.CustomLogQueue.Enqueue(customLog);
			while (XFileLog.CustomLogQueue.Count > 20)
			{
				XFileLog.CustomLogQueue.Dequeue();
			}
		}

		// Token: 0x04000751 RID: 1873
		private static Queue<string> CustomLogQueue = new Queue<string>();

		// Token: 0x04000752 RID: 1874
		private const int QUEUE_SIZE = 20;

		// Token: 0x04000753 RID: 1875
		public static string RoleName = "";

		// Token: 0x04000754 RID: 1876
		public static uint RoleLevel = 0U;

		// Token: 0x04000755 RID: 1877
		public static int RoleProf = 0;

		// Token: 0x04000756 RID: 1878
		public static string ServerID = "";

		// Token: 0x04000757 RID: 1879
		public static string OpenID = "";

		// Token: 0x04000758 RID: 1880
		public static uint SceneID = 0U;

		// Token: 0x04000759 RID: 1881
		private static Application.LogCallback callBack = null;

		// Token: 0x0400075A RID: 1882
		private string _outpath;

		// Token: 0x0400075B RID: 1883
		public bool _logOpen = true;

		// Token: 0x0400075C RID: 1884
		private bool _firstWrite = true;

		// Token: 0x0400075D RID: 1885
		private string _guiLog = "";

		// Token: 0x0400075E RID: 1886
		private bool _showGuiLog = false;

		// Token: 0x0400075F RID: 1887
		private GUIStyle fontStyle = null;

		// Token: 0x04000760 RID: 1888
		public static string debugStr = "0";

		// Token: 0x04000761 RID: 1889
		public static bool _OpenCustomBtn = false;

		// Token: 0x04000762 RID: 1890
		public static bool _debugTrigger = true;

		// Token: 0x04000763 RID: 1891
		private Vector2 scrollPosition;

		// Token: 0x04000764 RID: 1892
		private bool _showCustomInfo = false;

		// Token: 0x04000765 RID: 1893
		public static bool _logBundleOpen = false;

		// Token: 0x04000766 RID: 1894
		public static string _customInfo = "";

		// Token: 0x04000767 RID: 1895
		public static int _customInfoHeight = 0;
	}
}
