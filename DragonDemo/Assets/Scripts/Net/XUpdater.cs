using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using UnityEngine;
using XUtliPoolLib;
using Object = UnityEngine.Object;

namespace XUpdater
{
	// Token: 0x02000023 RID: 35
	public sealed class XUpdater : XSingleton<XUpdater>
	{
		//// Token: 0x1700000A RID: 10
		//// (get) Token: 0x0600009B RID: 155 RVA: 0x00004014 File Offset: 0x00002214
		//public static XLaunchMode LaunchMode
		//{
		//	get
		//	{
		//		return XUpdater._launch_mode;
		//	}
		//}

		//// Token: 0x1700000B RID: 11
		//// (get) Token: 0x0600009C RID: 156 RVA: 0x0000402C File Offset: 0x0000222C
		//public bool EditorMode
		//{
		//	get
		//	{
		//		return this._bEditorMode;
		//	}
		//}

		//// Token: 0x1700000C RID: 12
		//// (get) Token: 0x0600009D RID: 157 RVA: 0x00004044 File Offset: 0x00002244
		//public string Version
		//{
		//	get
		//	{
		//		return this._version;
		//	}
		//}

		//// Token: 0x1700000D RID: 13
		//// (get) Token: 0x0600009E RID: 158 RVA: 0x0000405C File Offset: 0x0000225C
		//public string TargetVersion
		//{
		//	get
		//	{
		//		return this._target_version;
		//	}
		//}

		//// Token: 0x1700000E RID: 14
		//// (get) Token: 0x0600009F RID: 159 RVA: 0x00004074 File Offset: 0x00002274
		//public bool NeedCheckFile
		//{
		//	get
		//	{
		//		return this._need_check_file;
		//	}
		//}

		//// Token: 0x1700000F RID: 15
		//// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000408C File Offset: 0x0000228C
		//public BuildTarget RunTimePlatform
		//{
		//	get
		//	{
		//		return this._runtime_platform;
		//	}
		//}

		//// Token: 0x17000010 RID: 16
		//// (get) Token: 0x060000A1 RID: 161 RVA: 0x000040A4 File Offset: 0x000022A4
		//public string Platform
		//{
		//	get
		//	{
		//		return this._platform_name;
		//	}
		//}

 
		public IPlatform XPlatform
		{
			get
			{
				return this._platform;
			}
		}

		//// Token: 0x17000012 RID: 18
		//// (get) Token: 0x060000A3 RID: 163 RVA: 0x000040D4 File Offset: 0x000022D4
		//public ILuaEngine XLuaEngine
		//{
		//	get
		//	{
		//		return this._lua_engine;
		//	}
		//}

		//// Token: 0x17000013 RID: 19
		//// (get) Token: 0x060000A4 RID: 164 RVA: 0x000040EC File Offset: 0x000022EC
		//public IApolloManager XApolloManager
		//{
		//	get
		//	{
		//		return this._apolloManager;
		//	}
		//}

		//// Token: 0x17000014 RID: 20
		//// (get) Token: 0x060000A5 RID: 165 RVA: 0x00004104 File Offset: 0x00002304
		//public IBroardcast XBroadCast
		//{
		//	get
		//	{
		//		return this._broadcast;
		//	}
		//}

		//// Token: 0x17000015 RID: 21
		//// (get) Token: 0x060000A6 RID: 166 RVA: 0x0000411C File Offset: 0x0000231C
		//public ITssSdk XTssSdk
		//{
		//	get
		//	{
		//		return this._tssSdk;
		//	}
		//}

		//// Token: 0x17000016 RID: 22
		//// (get) Token: 0x060000A7 RID: 167 RVA: 0x00004134 File Offset: 0x00002334
		//public IXPandoraMgr XPandoraManager
		//{
		//	get
		//	{
		//		return this._pandoraManager;
		//	}
		//}

		//// Token: 0x17000017 RID: 23
		//// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000414C File Offset: 0x0000234C
		//public IXGameSirControl GameSirControl
		//{
		//	get
		//	{
		//		return this._SirControl;
		//	}
		//}

		//// Token: 0x17000018 RID: 24
		//// (get) Token: 0x060000A9 RID: 169 RVA: 0x00004164 File Offset: 0x00002364
		//public IResourceHelp XResourceHelp
		//{
		//	get
		//	{
		//		return this._resourcehelp;
		//	}
		//}

		//// Token: 0x17000019 RID: 25
		//// (get) Token: 0x060000AA RID: 170 RVA: 0x0000417C File Offset: 0x0000237C
		//public bool IsDone
		//{
		//	get
		//	{
		//		return this._update_done;
		//	}
		//}

		//// Token: 0x1700001A RID: 26
		//// (set) Token: 0x060000AB RID: 171 RVA: 0x00004194 File Offset: 0x00002394
		//internal eUPdatePhase Phase
		//{
		//	set
		//	{
		//		this._phase = value;
		//	}
		//}

 
		public int ManagedThreadId
		{
			get
			{
				return this._main_threadId;
			}
		}

		//// Token: 0x1700001C RID: 28
		//// (get) Token: 0x060000AD RID: 173 RVA: 0x000041B8 File Offset: 0x000023B8
		//// (set) Token: 0x060000AE RID: 174 RVA: 0x000041C0 File Offset: 0x000023C0
		//public bool Reboot { get; set; }


		public override bool Init()
		{
			this._main_threadId = Thread.CurrentThread.ManagedThreadId;
		//	this._bEditorMode = ((int)Application.platform == 7 || Application.platform == null || (int)Application.platform == 2);
		//	this.Reboot = false;
		//	XUpdater.XGameRoot = GameObject.Find("XGamePoint");
		//	this.GetLaunchMode();
		//	RuntimePlatform platform = Application.platform;
		//	if (platform != RuntimePlatform.IPhonePlayer)
		//	{
		//		if (platform != RuntimePlatform.Android)
		//		{
		//			this._runtime_platform = BuildTarget.Standalone;
		//			this._platform_name = (this.PatchPrefix() ?? "");
		//		}
		//		else
		//		{
		//			this._runtime_platform = BuildTarget.Android;
		//			this._platform_name = this.PatchPrefix() + "Android/";
		//		}
		//	}
		//	else
		//	{
		//		this._runtime_platform = BuildTarget.IOS;
		//		this._platform_name = this.PatchPrefix() + "IOS/";
		//	}
		//	this._version_getter = XUpdater.XGameRoot.AddComponent<XVersion>();
		//	this._bundle_getter = XUpdater.XGameRoot.AddComponent<XBundle>();
		//	this._filelog_getter = XUpdater.XGameRoot.AddComponent<XFileLog>();
		//	this._fmod_listenter = (GameObject.Find("Main Camera").GetComponent("FMOD_Listener") as IFMOD_Listener);
		//	this._tssSdk = (XUpdater.XGameRoot.GetComponent("TssSDKManager") as ITssSdk);
		//	this._apolloManager = (XUpdater.XGameRoot.GetComponent("ApolloManager") as IApolloManager);
		//	this._broadcast = (XUpdater.XGameRoot.GetComponent("BroadcastManager") as IBroardcast);
		 	this._platform = (XUpdater.XGameRoot.GetComponent("XPlatform") as IPlatform);
		//	this._video = (XUpdater.XGameRoot.GetComponent("XVideoMgr") as IXVideo);
		//	this._lua_engine = (XUpdater.XGameRoot.GetComponent("LuaEngine") as ILuaEngine);
		//	this._pandoraManager = (XUpdater.XGameRoot.GetComponent("XPandoraMgr") as IXPandoraMgr);
		//	this._SirControl = (XUpdater.XGameRoot.GetComponent("XGameSirControl") as IXGameSirControl);
		//	bool flag = this._SirControl != null;
		//	if (flag)
		//	{
		//		this._SirControl.Init();
		//	}
		//	this.ABManager = XUpdater.XGameRoot.AddComponent<AssetBundleManager>();
		//	this.ABManager.Init();
		//	XSingleton<XDebug>.singleton.Init(this._platform, this._filelog_getter);
		//	XSingleton<XCaching>.singleton.Init();
		//	XSingleton<XLoadingUI>.singleton.Init();
		//	XBinaryReader.Init();
		//	Object.DontDestroyOnLoad(XUpdater.XGameRoot);
			return true;
		}

		//// Token: 0x060000B0 RID: 176 RVA: 0x00004434 File Offset: 0x00002634
		//public override void Uninit()
		//{
		//	foreach (AssetBundle assetBundle in this._bundles.Values)
		//	{
		//		assetBundle.Unload(false);
		//	}
		//	this._assets.Clear();
		//	this._persist_assets.Clear();
		//	this._bundles.Clear();
		//	this._res_list.Clear();
		//	bool flag = this._video != null && this._video.isPlaying;
		//	if (flag)
		//	{
		//		this._video.Stop();
		//	}
		//	this._phase = eUPdatePhase.xUP_Prepare;
		//	this._update_done = false;
		//	bool flag2 = this._fetch_version_network != null;
		//	if (flag2)
		//	{
		//		this._fetch_version_network.Close();
		//	}
		//	Object.Destroy(XUpdater.XGameRoot);
		//}

		//// Token: 0x060000B1 RID: 177 RVA: 0x0000451C File Offset: 0x0000271C
		//public void Clear()
		//{
		//	foreach (KeyValuePair<uint, AssetBundle> keyValuePair in this._bundles)
		//	{
		//		AssetBundle value = keyValuePair.Value;
		//		value.Unload(false);
		//	}
		//	this._bundles.Clear();
		//}

		//// Token: 0x060000B2 RID: 178 RVA: 0x0000456C File Offset: 0x0000276C
		//public void Update()
		//{
		//	switch (this._phase)
		//	{
		//	case eUPdatePhase.xUP_Prepare:
		//	{
		//		bool flag = this.Preparing();
		//		if (flag)
		//		{
		//			this._phase = eUPdatePhase.xUP_FetchVersion;
		//		}
		//		else
		//		{
		//			XSingleton<XLoadingUI>.singleton.SetStatus(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_FETCHLOCALVERSIONERROR"), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//			this.OnError();
		//		}
		//		break;
		//	}
		//	case eUPdatePhase.xUP_FetchVersion:
		//	{
		//		bool flag2 = !this._bFetchVersion;
		//		if (flag2)
		//		{
		//			bool flag3 = this._fetch_version_network == null;
		//			if (flag3)
		//			{
		//				this._fetch_version_network = new XFetchVersionNetwork();
		//			}
		//			XSingleton<XLoadingUI>.singleton.SetStatus(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_FETCHVERSION"), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//			this._fetch_version_network.Init();
		//			string host = XSingleton<XCaching>.singleton.VersionServer.Substring(0, XSingleton<XCaching>.singleton.VersionServer.LastIndexOf(':'));
		//			string s = XSingleton<XCaching>.singleton.VersionServer.Substring(XSingleton<XCaching>.singleton.VersionServer.LastIndexOf(':') + 1);
		//			bool flag4 = this._fetch_version_network.Connect(host, int.Parse(s));
		//			if (flag4)
		//			{
		//				this._bFetchVersion = true;
		//				this._fetch_version_time = Time.time;
		//			}
		//			else
		//			{
		//				XSingleton<XLoadingUI>.singleton.SetStatus(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_FETCHVERSIONERROR"), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//				XSingleton<XUpdater>.singleton.OnError();
		//				this._fetch_version_network.Close();
		//			}
		//		}
		//		else
		//		{
		//			bool flag5 = Time.time - this._fetch_version_time > 5f;
		//			if (flag5)
		//			{
		//				XSingleton<XLoadingUI>.singleton.SetStatus(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_FETCHVERSIONERROR"), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//				XSingleton<XUpdater>.singleton.OnError();
		//				this._fetch_version_network.Close();
		//			}
		//		}
		//		break;
		//	}
		//	case eUPdatePhase.xUP_LoadVersion:
		//	{
		//		bool flag6 = !this._bFetchServer;
		//		if (flag6)
		//		{
		//			this._cacheload_bundle.Clear();
		//			this._download_bundle.Clear();
		//			this._meta_bundle.Clear();
		//			this._version_getter.ServerDownload(new HandleVersionDownload(this.OnVersionDownloaded), new HandleVersionLoaded(this.OnVersionLoaded));
		//		}
		//		this._bFetchServer = true;
		//		break;
		//	}
		//	case eUPdatePhase.xUP_CompareVersion:
		//	{
		//		bool flag7 = this._comparer == null;
		//		if (flag7)
		//		{
		//			this._comparer = this.VersionComparer();
		//		}
		//		else
		//		{
		//			bool flag8 = !this._comparer.MoveNext();
		//			if (flag8)
		//			{
		//				this._comparer = null;
		//			}
		//		}
		//		break;
		//	}
		//	case eUPdatePhase.xUP_DownLoadBundle:
		//	{
		//		bool flag9 = this._downloader == null;
		//		if (flag9)
		//		{
		//			this._downloader = this.DownLoadBundles();
		//		}
		//		else
		//		{
		//			bool flag10 = !this._downloader.MoveNext();
		//			if (flag10)
		//			{
		//				this._downloader = null;
		//				this._phase = eUPdatePhase.xUP_ShowVersion;
		//			}
		//		}
		//		break;
		//	}
		//	case eUPdatePhase.xUP_ShowVersion:
		//		this.ShowVersionInfo(this._server, true);
		//		break;
		//	case eUPdatePhase.xUP_LaunchGame:
		//	{
		//		bool flag11 = this._launcher == null;
		//		if (flag11)
		//		{
		//			this._launcher = this.LaunchGame();
		//		}
		//		else
		//		{
		//			bool flag12 = !this._launcher.MoveNext();
		//			if (flag12)
		//			{
		//				this._launcher = null;
		//				this.OnEnding();
		//			}
		//		}
		//		break;
		//	}
		//	case eUPdatePhase.xUP_Finish:
		//	{
		//		bool flag13 = this._finish == null;
		//		if (flag13)
		//		{
		//			this._finish = this.Finish();
		//		}
		//		else
		//		{
		//			bool flag14 = !this._finish.MoveNext();
		//			if (flag14)
		//			{
		//				this._finish = null;
		//				this._update_done = true;
		//				XSingleton<XShell>.singleton.StartGame();
		//			}
		//		}
		//		break;
		//	}
		//	}
		//	XSingleton<XLoadingUI>.singleton.OnUpdate();
		//}

		//// Token: 0x060000B3 RID: 179 RVA: 0x00004948 File Offset: 0x00002B48
		//public void Begin()
		//{
		//	this._update_done = false;
		//	this._bFetchVersion = false;
		//	this._bFetchServer = false;
		//	bool flag = !XSingleton<XStringTable>.singleton.SyncInit();
		//	if (flag)
		//	{
		//		this._log.Remove(0, this._log.Length);
		//		this._log.Append("Error occurred when loading string table.");
		//		XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//		XSingleton<XUpdater>.singleton.OnError();
		//	}
		//	else
		//	{
		//		bool flag2 = !this.CheckMemory();
		//		if (flag2)
		//		{
		//			this._log.Remove(0, this._log.Length);
		//			this._log.Append(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_EXCLUDE1GPHONE"));
		//			XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//			XSingleton<XUpdater>.singleton.OnError();
		//		}
		//		else
		//		{
		//			bool flag3 = XSingleton<XCaching>.singleton.EnableCache();
		//			if (flag3)
		//			{
		//				this._phase = eUPdatePhase.xUP_Prepare;
		//			}
		//			else
		//			{
		//				this._log.Remove(0, this._log.Length);
		//				this._log.Append(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_ACCESSDENIED"));
		//				XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//				XSingleton<XUpdater>.singleton.OnError();
		//			}
		//		}
		//	}
		//}

		//// Token: 0x060000B4 RID: 180 RVA: 0x00004AD0 File Offset: 0x00002CD0
		//private IEnumerator Finish()
		//{
		//	bool flag = this._server != null;
		//	if (flag)
		//	{
		//		this._server.Bundles.Clear();
		//		this._version = this._server.ToString();
		//	}
		//	this._server = null;
		//	this._client = null;
		//	this._buildin = null;
		//	bool flag2 = this._need_play_cg && this._video != null;
		//	if (flag2)
		//	{
		//		this._video.Play(false);
		//		yield return null;
		//		while (this._video.isPlaying)
		//		{
		//			yield return null;
		//		}
		//	}
		//	Object.DestroyObject(this._version_getter);
		//	Object.DestroyObject(this._bundle_getter);
		//	yield break;
		//}

		//// Token: 0x060000B5 RID: 181 RVA: 0x00004ADF File Offset: 0x00002CDF
		//public void OnError()
		//{
		//	this._phase = eUPdatePhase.xUP_Error;
		//}

		//// Token: 0x060000B6 RID: 182 RVA: 0x00004AEA File Offset: 0x00002CEA
		//public void DevStart()
		//{
		//	this._phase = eUPdatePhase.xUP_ShowVersion;
		//}

		//// Token: 0x060000B7 RID: 183 RVA: 0x00004AF4 File Offset: 0x00002CF4
		//public void OnRetry()
		//{
		//	eUPdatePhase phase = this._phase;
		//	if (phase != eUPdatePhase.xUP_DownLoadBundle)
		//	{
		//		if (phase != eUPdatePhase.xUP_Finish)
		//		{
		//			if (phase == eUPdatePhase.xUP_Error)
		//			{
		//				bool flag = this._server == null;
		//				if (flag)
		//				{
		//					this.Begin();
		//				}
		//			}
		//		}
		//		else
		//		{
		//			bool isPlaying = this._video.isPlaying;
		//			if (isPlaying)
		//			{
		//				this._video.Stop();
		//			}
		//		}
		//	}
		//	else
		//	{
		//		bool on_file_download_need_retry = this._on_file_download_need_retry;
		//		if (on_file_download_need_retry)
		//		{
		//			this._on_file_download_retry = true;
		//		}
		//	}
		//}

		//// Token: 0x060000B8 RID: 184 RVA: 0x00004B6F File Offset: 0x00002D6F
		//private void OnEnding()
		//{
		//	XSingleton<XLoadingUI>.singleton.LoadingOK = true;
		//	this._download_bundle.Clear();
		//	this._cacheload_bundle.Clear();
		//	this._meta_bundle.Clear();
		//	this._phase = eUPdatePhase.xUP_Ending;
		//}

		//// Token: 0x060000B9 RID: 185 RVA: 0x00004BAC File Offset: 0x00002DAC
		//public bool ContainRes(uint hash)
		//{
		//	return this._res_list.ContainsKey(hash);
		//}

		//// Token: 0x060000BA RID: 186 RVA: 0x00004BCC File Offset: 0x00002DCC
		//public Object ResourceLoad(uint hash)
		//{
		//	Object result = null;
		//	XResPackage xresPackage = null;
		//	bool flag = this._res_list != null && this._res_list.TryGetValue(hash, out xresPackage);
		//	if (flag)
		//	{
		//		bool flag2 = !this._persist_assets.TryGetValue(hash, out result);
		//		if (flag2)
		//		{
		//			AssetBundle assetBundle = null;
		//			uint key = XSingleton<XCommon>.singleton.XHash(xresPackage.bundle);
		//			bool flag3 = !this._bundles.TryGetValue(key, out assetBundle);
		//			if (flag3)
		//			{
		//				byte[] array = null;
		//				bool flag4 = !this._persist_image.TryGetValue(key, out array);
		//				if (flag4)
		//				{
		//					XBundleData data = null;
		//					bool flag5 = this._assets.TryGetValue(key, out data);
		//					if (!flag5)
		//					{
		//						return null;
		//					}
		//					array = XSingleton<XCaching>.singleton.LoadFile(XSingleton<XCaching>.singleton.GetLocalPath(data));
		//				}
		//				assetBundle = AssetBundle.LoadFromMemory(array);
		//				this._bundles.Add(key, assetBundle);
		//			}
		//			string text = xresPackage.location.Substring(xresPackage.location.LastIndexOf('/') + 1);
		//			result = assetBundle.LoadAsset(text, XUpdater.Ass.GetType(xresPackage.type));
		//		}
		//	}
		//	return result;
		//}

		//// Token: 0x060000BB RID: 187 RVA: 0x00004CF8 File Offset: 0x00002EF8
		//private AsyncVersionProcessRequest OnVersionDownloaded(TextAsset text)
		//{
		//	AsyncVersionProcessRequest avpr = new AsyncVersionProcessRequest();
		//	XSingleton<XLoadingUI>.singleton.SetStatus(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_FETCHMANIFEST"), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//	bool flag = text != null;
		//	if (flag)
		//	{
		//		byte[] contents = text.bytes;
		//		new Thread(delegate()
		//		{
		//			avpr.IsCorrect = this.LoadVersion(contents);
		//			avpr.IsDone = true;
		//		}).Start();
		//	}
		//	else
		//	{
		//		avpr.IsDone = true;
		//		avpr.IsCorrect = false;
		//	}
		//	return avpr;
		//}

		//// Token: 0x060000BC RID: 188 RVA: 0x00004DAC File Offset: 0x00002FAC
		//private void OnVersionLoaded(bool correct)
		//{
		//	if (correct)
		//	{
		//		this._phase = eUPdatePhase.xUP_CompareVersion;
		//	}
		//	else
		//	{
		//		this._log.Remove(0, this._log.Length);
		//		this._log.Append(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_MANIFESTERROR"));
		//		XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//		XSingleton<XUpdater>.singleton.OnError();
		//	}
		//}

		//// Token: 0x060000BD RID: 189 RVA: 0x00004E30 File Offset: 0x00003030
		//private void OnBundleFetched(WWW www, byte[] bytes, XBundleData data, bool newdownload)
		//{
		//	if (newdownload)
		//	{
		//		XSingleton<XDebug>.singleton.AddLog("Finished Download ", data.Name, null, null, null, null, XDebugColor.XDebug_None);
		//		this._log.Remove(0, this._log.Length);
		//		this._log.AppendFormat(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_DOWNLOAD_FILE_COMPLETE"), data.Name);
		//		XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//	}
		//	else
		//	{
		//		XSingleton<XDebug>.singleton.AddLog("Finished Extract ", data.Name, null, null, null, null, XDebugColor.XDebug_None);
		//		this._log.Remove(0, this._log.Length);
		//		this._log.AppendFormat(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_EXTRACTING_COMPLETE"), data.Name);
		//		XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//	}
		//	uint key = XSingleton<XCommon>.singleton.XHash(data.Name);
		//	bool load = false;
		//	this._bundle_data = data;
		//	switch (this._bundle_data.Level)
		//	{
		//	case AssetLevel.Memory:
		//		load = true;
		//		break;
		//	case AssetLevel.Image:
		//	{
		//		byte[] array = new byte[bytes.Length];
		//		bytes.CopyTo(array, 0);
		//		this._persist_image.Add(key, array);
		//		break;
		//	}
		//	}
		//	this._assets.Add(key, this._bundle_data);
		//	this._bundle_getter.GetBundle(www, bytes, new HandleLoadBundle(this.OnBundleLoaded), load);
		//}

		//// Token: 0x060000BE RID: 190 RVA: 0x00004FD4 File Offset: 0x000031D4
		//private void OnBundleLoaded(AssetBundle bundle)
		//{
		//	bool flag = bundle != null;
		//	if (flag)
		//	{
		//		this._bundles.Add(XSingleton<XCommon>.singleton.XHash(this._bundle_data.Name), bundle);
		//	}
		//	this._bundle_fetching = false;
		//}

		//// Token: 0x060000BF RID: 191 RVA: 0x00005018 File Offset: 0x00003218
		//private void OnAssetLoaded(XResPackage package, Object asset)
		//{
		//	uint key = XSingleton<XCommon>.singleton.XHash(package.location);
		//	bool flag = asset != null;
		//	if (flag)
		//	{
		//		bool flag2 = !this._persist_assets.ContainsKey(key);
		//		if (flag2)
		//		{
		//			this._persist_assets.Add(key, asset);
		//		}
		//		else
		//		{
		//			this._persist_assets[key] = asset;
		//		}
		//	}
		//	this._asset_loading = false;
		//}

		//// Token: 0x060000C0 RID: 192 RVA: 0x0000507C File Offset: 0x0000327C
		//private void UpdateLocalVersion(Stream s)
		//{
		//	this._buildin = this.FetchBuildIn(s);
		//	this._client = this.FetchBuildOut();
		//	bool flag = this._buildin != null;
		//	if (flag)
		//	{
		//		XSingleton<XDebug>.singleton.AddLog("BuildIn version: ", this._buildin.ToString(), null, null, null, null, XDebugColor.XDebug_None);
		//	}
		//	bool flag2 = this._client != null;
		//	if (flag2)
		//	{
		//		XSingleton<XDebug>.singleton.AddLog("BuildOut version: ", this._client.ToString(), null, null, null, null, XDebugColor.XDebug_None);
		//	}
		//	this._need_play_cg = (this._client == null);
		//	bool flag3 = this._client == null;
		//	if (flag3)
		//	{
		//		this._client = new XVersionData(this._buildin);
		//		bool flag4 = !XSingleton<XCaching>.singleton.CleanCache();
		//		if (flag4)
		//		{
		//			this._log.Remove(0, this._log.Length);
		//			this._log.Append(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_DELETEDENIED"));
		//			XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//			XSingleton<XUpdater>.singleton.OnError();
		//		}
		//	}
		//	else
		//	{
		//		int num = this._client.CompareTo(this._buildin);
		//		bool flag5 = num < 0 || this._buildin.Build_Version != this._client.Build_Version;
		//		if (flag5)
		//		{
		//			this._need_play_cg = true;
		//			this._client.VersionCopy(this._buildin);
		//			bool flag6 = !XSingleton<XCaching>.singleton.CleanCache();
		//			if (flag6)
		//			{
		//				this._log.Remove(0, this._log.Length);
		//				this._log.Append(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_DELETEDENIED"));
		//				XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//				XSingleton<XUpdater>.singleton.OnError();
		//			}
		//		}
		//	}
		//	XSingleton<XDebug>.singleton.AddLog("Client version: ", this._client.ToString(), null, null, null, null, XDebugColor.XDebug_None);
		//}

		//// Token: 0x060000C1 RID: 193 RVA: 0x000052A0 File Offset: 0x000034A0
		//private XVersionData FetchBuildIn(Stream s)
		//{
		//	bool flag = s == null;
		//	XVersionData result;
		//	if (flag)
		//	{
		//		result = null;
		//	}
		//	else
		//	{
		//		StreamReader streamReader = new StreamReader(s);
		//		string version = streamReader.ReadToEnd();
		//		result = XVersionData.Convert2Version(version);
		//	}
		//	return result;
		//}

		//// Token: 0x060000C2 RID: 194 RVA: 0x000052D4 File Offset: 0x000034D4
		//private XVersionData FetchBuildOut()
		//{
		//	string localVersion = XVersion.GetLocalVersion();
		//	XSingleton<XDebug>.singleton.AddLog("Local Version Path " + localVersion, null, null, null, null, null, XDebugColor.XDebug_None);
		//	XVersionData result = null;
		//	bool flag = File.Exists(localVersion);
		//	if (flag)
		//	{
		//		byte[] buffer = this.XCryptography(File.ReadAllBytes(localVersion));
		//		using (MemoryStream memoryStream = new MemoryStream(buffer))
		//		{
		//			XmlSerializer xmlSerializer = new XmlSerializer(typeof(XVersionData));
		//			try
		//			{
		//				result = (xmlSerializer.Deserialize(memoryStream) as XVersionData);
		//			}
		//			catch (Exception ex)
		//			{
		//				result = null;
		//				XSingleton<XDebug>.singleton.AddLog("Build Out Version Fetching FAILED! " + ex.Message, null, null, null, null, null, XDebugColor.XDebug_None);
		//			}
		//			finally
		//			{
		//				memoryStream.Close();
		//			}
		//		}
		//	}
		//	else
		//	{
		//		XSingleton<XDebug>.singleton.AddLog("Local Version Path " + localVersion + " not exists.", null, null, null, null, null, XDebugColor.XDebug_None);
		//	}
		//	return result;
		//}

		//// Token: 0x060000C3 RID: 195 RVA: 0x000053EC File Offset: 0x000035EC
		//private bool LoadVersion(byte[] text)
		//{
		//	string a = BitConverter.ToString(text, 0, XUpdater.Md5Length);
		//	string b = XSingleton<XCaching>.singleton.CalculateMD5(text, XUpdater.Md5Length, text.Length - XUpdater.Md5Length);
		//	bool flag = a == b;
		//	if (flag)
		//	{
		//		using (MemoryStream memoryStream = new MemoryStream(text, XUpdater.Md5Length, text.Length - XUpdater.Md5Length))
		//		{
		//			XmlSerializer xmlSerializer = new XmlSerializer(typeof(XVersionData));
		//			this._server = (xmlSerializer.Deserialize(memoryStream) as XVersionData);
		//			bool flag2 = this._server != null && this._server.Target_Platform == this._runtime_platform;
		//			if (flag2)
		//			{
		//				XSingleton<XDebug>.singleton.AddLog("Server version: ", this._server.ToString(), null, null, null, null, XDebugColor.XDebug_None);
		//				return true;
		//			}
		//		}
		//	}
		//	XSingleton<XDebug>.singleton.AddLog("Analysis Server version error!", null, null, null, null, null, XDebugColor.XDebug_None);
		//	this._server = null;
		//	return false;
		//}

		//// Token: 0x060000C4 RID: 196 RVA: 0x000054F8 File Offset: 0x000036F8
		//private bool CheckMemory()
		//{
		//	return true;
		//}

		//// Token: 0x060000C5 RID: 197 RVA: 0x0000550C File Offset: 0x0000370C
		//private bool Preparing()
		//{
		//	Stream stream = null;
		//	XSingleton<XDebug>.singleton.AddLog("Fetch local version...", null, null, null, null, null, XDebugColor.XDebug_None);
		//	bool flag = (int)Application.platform == 8;
		//	if (flag)
		//	{
		//		stream = XSingleton<XResourceLoaderMgr>.singleton.ReadText("ios-version", ".bytes", false);
		//	}
		//	else
		//	{
		//		bool flag2 = (int)Application.platform == 11;
		//		if (flag2)
		//		{
		//			stream = XSingleton<XResourceLoaderMgr>.singleton.ReadText("android-version", ".bytes", false);
		//		}
		//	}
		//	bool flag3 = stream != null;
		//	bool result;
		//	if (flag3)
		//	{
		//		this.UpdateLocalVersion(stream);
		//		XSingleton<XResourceLoaderMgr>.singleton.ClearStream(stream);
		//		result = true;
		//	}
		//	else
		//	{
		//		result = (XUpdater.LaunchMode == XLaunchMode.Dev);
		//	}
		//	return result;
		//}

		//// Token: 0x060000C6 RID: 198 RVA: 0x000055AE File Offset: 0x000037AE
		//private IEnumerator VersionComparer()
		//{
		//	int compare = this._client.CompareTo(this._server);
		//	this._update_pakcage_size = 0UL;
		//	bool flag = compare > 0;
		//	if (flag)
		//	{
		//		this._server = this._client;
		//		this._download_prepare = null;
		//		this.DownLoadConfirmed(false);
		//	}
		//	else
		//	{
		//		bool flag2 = compare == 0 || (compare < 0 && this._client.CanUpdated(this._server));
		//		if (flag2)
		//		{
		//			bool flag3 = this._download_prepare == null;
		//			if (flag3)
		//			{
		//				this._download_prepare = this.DownLoadPrepare();
		//			}
		//			while (this._download_prepare.MoveNext())
		//			{
		//				yield return null;
		//			}
		//			this._download_prepare = null;
		//			this.DownLoadConfirmed(compare != 0);
		//		}
		//		else
		//		{
		//			XSingleton<XLoadingUI>.singleton.SetStatus(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_VERSIONNOTMATCH"), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//			this.ShowVersionInfo(this._client, false);
		//			Texture tex = null;
		//			bool flag4 = !this._is_download_update_pic;
		//			if (flag4)
		//			{
		//				WWW www = new WWW("https://image.lzgjx.qq.com/sharingicon/updateimg.jpg");
		//				while (!www.isDone)
		//				{
		//					yield return null;
		//				}
		//				bool flag5 = string.IsNullOrEmpty(www.error);
		//				if (flag5)
		//				{
		//					tex = www.texture;
		//				}
		//				www.Dispose();
		//				this._is_download_update_pic = true;
		//				www = null;
		//			}
		//			this._phase = eUPdatePhase.xUP_Error;
		//			XSingleton<XLoadingUI>.singleton.SetDownLoad(new XLoadingUI.OnSureCallBack(this.ToAppStore), tex);
		//			tex = null;
		//		}
		//	}
		//	yield break;
		//}

		//// Token: 0x060000C7 RID: 199 RVA: 0x000055C0 File Offset: 0x000037C0
		//private void ToAppStore()
		//{
		//	RuntimePlatform platform = Application.platform;
		//	if (platform != RuntimePlatform.IPhonePlayer)
		//	{
		//		if ((int)platform == 11)
		//		{
		//			for (int i = 0; i < this._client.Res.Count; i++)
		//			{
		//				bool flag = this._client.Res[i].location == "Table/StringTable";
		//				if (flag)
		//				{
		//					XBundleData specificBundle = this._client.GetSpecificBundle(this._client.Res[i].bundle);
		//					string localPath = XSingleton<XCaching>.singleton.GetLocalPath(specificBundle);
		//					try
		//					{
		//						byte[] array = File.ReadAllBytes(localPath);
		//						bool flag2 = array != null;
		//						if (flag2)
		//						{
		//							AssetBundle assetBundle = AssetBundle.LoadFromMemory(array);
		//							bool flag3 = assetBundle != null;
		//							if (flag3)
		//							{
		//								TextAsset textAsset = assetBundle.LoadAsset("StringTable", typeof(TextAsset)) as TextAsset;
		//								bool flag4 = textAsset != null;
		//								if (flag4)
		//								{
		//									bool flag5 = !XSingleton<XStringTable>.singleton.ReInit(textAsset);
		//									if (flag5)
		//									{
		//										XSingleton<XStringTable>.singleton.SyncInit();
		//									}
		//								}
		//							}
		//						}
		//					}
		//					catch (Exception ex)
		//					{
		//						XSingleton<XDebug>.singleton.AddErrorLog("ToAndroidAppStore: ", ex.Message, null, null, null, null);
		//					}
		//					break;
		//				}
		//			}
		//			string @string = XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_VERSIONNOTMATCH_ANDROID_URL");
		//			XSingleton<XDebug>.singleton.AddLog("AndroidAppStore Url: ", @string, null, null, null, null, XDebugColor.XDebug_None);
		//			Application.OpenURL(@string);
		//		}
		//	}
		//	else
		//	{
		//		for (int j = 0; j < this._client.Res.Count; j++)
		//		{
		//			bool flag6 = this._client.Res[j].location == "Table/StringTable";
		//			if (flag6)
		//			{
		//				XBundleData specificBundle2 = this._client.GetSpecificBundle(this._client.Res[j].bundle);
		//				string localPath2 = XSingleton<XCaching>.singleton.GetLocalPath(specificBundle2);
		//				try
		//				{
		//					byte[] array2 = File.ReadAllBytes(localPath2);
		//					bool flag7 = array2 != null;
		//					if (flag7)
		//					{
		//						AssetBundle assetBundle2 = AssetBundle.LoadFromMemory(array2);
		//						bool flag8 = assetBundle2 != null;
		//						if (flag8)
		//						{
		//							TextAsset textAsset2 = assetBundle2.LoadAsset("StringTable", typeof(TextAsset)) as TextAsset;
		//							bool flag9 = textAsset2 != null;
		//							if (flag9)
		//							{
		//								bool flag10 = !XSingleton<XStringTable>.singleton.ReInit(textAsset2);
		//								if (flag10)
		//								{
		//									XSingleton<XStringTable>.singleton.SyncInit();
		//								}
		//							}
		//						}
		//					}
		//				}
		//				catch (Exception ex2)
		//				{
		//					XSingleton<XDebug>.singleton.AddErrorLog("ToAppStore: ", ex2.Message, null, null, null, null);
		//				}
		//				break;
		//			}
		//		}
		//		string string2 = XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_VERSIONNOTMATCH_URL");
		//		XSingleton<XDebug>.singleton.AddLog("AppStore Url: ", string2, null, null, null, null, XDebugColor.XDebug_None);
		//		Application.OpenURL(string2);
		//	}
		//}

		//// Token: 0x060000C8 RID: 200 RVA: 0x000058C0 File Offset: 0x00003AC0
		//private void DownLoadConfirmedCallBack()
		//{
		//	this._phase = eUPdatePhase.xUP_DownLoadBundle;
		//}

		//// Token: 0x060000C9 RID: 201 RVA: 0x000058CA File Offset: 0x00003ACA
		//private void DownLoadCancelledCallBack()
		//{
		//	this._phase = eUPdatePhase.xUP_Error;
		//	XSingleton<XLoadingUI>.singleton.SetStatus("", byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//	this.ShowVersionInfo(this._client, false);
		//}

		//// Token: 0x060000CA RID: 202 RVA: 0x00005904 File Offset: 0x00003B04
		//private void DownLoadConfirmed(bool updated)
		//{
		//	if (updated)
		//	{
		//		bool flag = this._update_pakcage_size < 1048576UL;
		//		if (flag)
		//		{
		//			this.DownLoadConfirmedCallBack();
		//		}
		//		else
		//		{
		//			XSingleton<XLoadingUI>.singleton.SetDialog(this._update_pakcage_size, new XLoadingUI.OnSureCallBack(this.DownLoadConfirmedCallBack), new XLoadingUI.OnSureCallBack(this.DownLoadCancelledCallBack));
		//			this._phase = eUPdatePhase.xUP_DownLoadConfirm;
		//		}
		//	}
		//	else
		//	{
		//		this._phase = eUPdatePhase.xUP_DownLoadBundle;
		//	}
		//}

		//// Token: 0x060000CB RID: 203 RVA: 0x00005973 File Offset: 0x00003B73
		//private IEnumerator DownLoadPrepare()
		//{
		//	int num;
		//	for (int i = 0; i < this._server.Res.Count; i = num + 1)
		//	{
		//		bool flag = !this._buildin.NeedDownload(this._server.Res[i].bundle);
		//		if (!flag)
		//		{
		//			XBundleData bundle = this._server.GetSpecificBundle(this._server.Res[i].bundle);
		//			bool flag2 = bundle == null;
		//			if (flag2)
		//			{
		//				XSingleton<XDebug>.singleton.AddLog("Bundle ", this._server.Res[i].bundle, " is missing in sever bundle set.", null, null, null, XDebugColor.XDebug_None);
		//			}
		//			else
		//			{
		//				bool flag3 = this._cacheload_bundle.Contains(bundle);
		//				if (!flag3)
		//				{
		//					bool flag4 = this._download_bundle.Contains(bundle);
		//					if (!flag4)
		//					{
		//						AsyncCachedRequest acr = XSingleton<XCaching>.singleton.IsBundleCached(bundle, this._server.MD5_Size);
		//						while (!acr.IsDone)
		//						{
		//							yield return null;
		//						}
		//						bool flag5 = !acr.Cached && acr.MaybeCached;
		//						if (flag5)
		//						{
		//							XBundleData clientData = null;
		//							foreach (XBundleData data in this._client.Bundles)
		//							{
		//								bool flag6 = data.Name == bundle.Name;
		//								if (flag6)
		//								{
		//									clientData = data;
		//									break;
		//								}
		//								//data = null;
		//							}
		//							List<XBundleData>.Enumerator enumerator = default(List<XBundleData>.Enumerator);
		//							acr.Cached = (clientData != null && clientData.MD5 == bundle.MD5);
		//							clientData = null;
		//						}
		//						XSingleton<XDebug>.singleton.AddLog("Bundle ", bundle.Name, " cached is ", acr.Cached.ToString(), null, null, XDebugColor.XDebug_None);
		//						bool cached = acr.Cached;
		//						if (cached)
		//						{
		//							this._cacheload_bundle.Add(bundle);
		//						}
		//						else
		//						{
		//							this._download_bundle.Add(bundle);
		//							this._update_pakcage_size += (ulong)bundle.Size;
		//						}
		//						bundle = null;
		//						acr = null;
		//					}
		//				}
		//			}
		//		}
		//		num = i;
		//	}
		//	this.MetaPrepare(this._server.AB);
		//	this.MetaPrepare(this._server.Scene);
		//	this.MetaPrepare(this._server.FMOD);
		//	yield break;
		//}

		//// Token: 0x060000CC RID: 204 RVA: 0x00005984 File Offset: 0x00003B84
		//private void MetaPrepare(List<XMetaResPackage> meta)
		//{
		//	bool flag = this._platform != null && !this._platform.IsPublish();
		//	for (int i = 0; i < meta.Count; i++)
		//	{
		//		bool flag2 = !this._buildin.NeedDownload(meta[i].bundle) || !this._client.NeedDownload(meta[i].bundle);
		//		if (!flag2)
		//		{
		//			bool flag3 = !this._meta_bundle.Contains(meta[i]);
		//			if (flag3)
		//			{
		//				this._meta_bundle.Add(meta[i]);
		//			}
		//			bool flag4 = flag;
		//			if (flag4)
		//			{
		//				XSingleton<XDebug>.singleton.AddLog("Meta ", meta[i].download, " is prepared to downloading...", null, null, null, XDebugColor.XDebug_None);
		//			}
		//			this._update_pakcage_size += (ulong)meta[i].Size;
		//		}
		//	}
		//}

		//// Token: 0x060000CD RID: 205 RVA: 0x00005A79 File Offset: 0x00003C79
		//private IEnumerator DownLoadBundles()
		//{
		//	bool log = this._platform != null && !this._platform.IsPublish();
		//	int total = this._download_bundle.Count + this._cacheload_bundle.Count + this._meta_bundle.Count;
		//	int processed = 0;
		//	int num;
		//	for (int i = 0; i < this._download_bundle.Count; i = num + 1)
		//	{
		//		while (this._bundle_fetching)
		//		{
		//			yield return null;
		//		}
		//		bool flag = log;
		//		if (flag)
		//		{
		//			XSingleton<XDebug>.singleton.AddLog("Updating ", this._download_bundle[i].Name, " ... ", processed.ToString(), "/", total.ToString(), XDebugColor.XDebug_None);
		//		}
		//		this._bundle_fetching = true;
		//		num = processed + 1;
		//		processed = num;
		//		XSingleton<XCaching>.singleton.Download(this._download_bundle[i], new HandleFetchBundle(this.OnBundleFetched), (float)processed / (float)total);
		//		num = i;
		//	}
		//	for (int j = 0; j < this._cacheload_bundle.Count; j = num + 1)
		//	{
		//		while (this._bundle_fetching)
		//		{
		//			yield return null;
		//		}
		//		bool flag2 = log;
		//		if (flag2)
		//		{
		//			XSingleton<XDebug>.singleton.AddLog("Extracting ", this._cacheload_bundle[j].Name, " ... ", processed.ToString(), "/", total.ToString(), XDebugColor.XDebug_None);
		//		}
		//		this._bundle_fetching = true;
		//		num = processed + 1;
		//		processed = num;
		//		bool flag3 = (int)Application.platform == 8;
		//		if (flag3)
		//		{
		//			while (!XSingleton<XCaching>.singleton.Extract(this._cacheload_bundle[j], new HandleFetchBundle(this.OnBundleFetched), (float)processed / (float)total))
		//			{
		//				yield return null;
		//			}
		//		}
		//		else
		//		{
		//			XSingleton<XCaching>.singleton.Extract(this._cacheload_bundle[j], new HandleFetchBundle(this.OnBundleFetched), (float)processed / (float)total);
		//		}
		//		num = j;
		//	}
		//	while (this._bundle_fetching)
		//	{
		//		yield return null;
		//	}
		//	for (int k = 0; k < this._meta_bundle.Count; k = num + 1)
		//	{
		//		num = processed + 1;
		//		processed = num;
		//		AsyncWriteRequest awr = XSingleton<XCaching>.singleton.Download(this._meta_bundle[k].download, this._meta_bundle[k].Size, (float)processed / (float)total);
		//		bool flag4 = log;
		//		if (flag4)
		//		{
		//			XSingleton<XDebug>.singleton.AddLog("Download meta ", this._meta_bundle[k].download, " ... ", processed.ToString(), "/", total.ToString(), XDebugColor.XDebug_None);
		//		}
		//		while (!awr.IsDone)
		//		{
		//			bool hasError = awr.HasError;
		//			if (hasError)
		//			{
		//				bool on_file_download_retry = this._on_file_download_retry;
		//				if (on_file_download_retry)
		//				{
		//					this._on_file_download_retry = false;
		//					this._on_file_download_need_retry = false;
		//					num = k;
		//					k = num - 1;
		//					num = processed;
		//					processed = num - 1;
		//					this._log.Length = 0;
		//					this._log.Append(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_RETRY"));
		//					XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//					yield return null;
		//					break;
		//				}
		//				this._on_file_download_need_retry = true;
		//				this._log.Length = 0;
		//				this._log.AppendFormat(XSingleton<XStringTable>.singleton.GetString("XUPDATE_ERROR_DOWNLOADRESFAILED_AND_RETRY"), awr.Name);
		//				XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//				yield return null;
		//			}
		//			else
		//			{
		//				yield return null;
		//			}
		//		}
		//		awr = null;
		//		num = k;
		//	}
		//	for (int l = 0; l < this._server.Res.Count; l = num + 1)
		//	{
		//		bool flag5 = this.ProcessAssets(this._server.Res[l]);
		//		if (flag5)
		//		{
		//			bool flag6 = this._server.Res[l].rtype != ResourceType.Script;
		//			if (flag6)
		//			{
		//				uint hash = XSingleton<XCommon>.singleton.XHash(this._server.Res[l].location);
		//				this._res_list.Add(hash, this._server.Res[l]);
		//			}
		//			this._log.Remove(0, this._log.Length);
		//			this._log.AppendFormat(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_PRELOADING"), ((float)l / (float)this._server.Res.Count * 100f).ToString("F0"));
		//			XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//			bool flag7 = l << 30 == 0;
		//			if (flag7)
		//			{
		//				yield return null;
		//			}
		//		}
		//		num = l;
		//	}
		//	AsyncLogRequest alr = this.LogNewVersion();
		//	while (!alr.IsDone)
		//	{
		//		Thread.Sleep(1);
		//	}
		//	this.XPlatform.SetNoBackupFlag(XVersion.GetLocalVersion());
		//	yield break;
		//}

		//// Token: 0x060000CE RID: 206 RVA: 0x00005A88 File Offset: 0x00003C88
		//private AsyncLogRequest LogNewVersion()
		//{
		//	AsyncLogRequest alr = new AsyncLogRequest();
		//	bool flag = this._server != this._client;
		//	if (flag)
		//	{
		//		new Thread(delegate()
		//		{
		//			using (MemoryStream memoryStream = new MemoryStream())
		//			{
		//				XmlSerializer xmlSerializer = new XmlSerializer(typeof(XVersionData));
		//				XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
		//				xmlSerializerNamespaces.Add(string.Empty, string.Empty);
		//				xmlSerializer.Serialize(memoryStream, this._server, xmlSerializerNamespaces);
		//				File.WriteAllBytes(XVersion.GetLocalVersion(), this.XCryptography(memoryStream.ToArray()));
		//			}
		//			alr.IsDone = true;
		//		}).Start();
		//	}
		//	else
		//	{
		//		alr.IsDone = true;
		//	}
		//	return alr;
		//}

		//// Token: 0x060000CF RID: 207 RVA: 0x00005AF8 File Offset: 0x00003CF8
		//private void ShowVersionInfo(XVersionData data, bool launch = true)
		//{
		//	bool flag = data != null;
		//	if (flag)
		//	{
		//		this._log.Remove(0, this._log.Length);
		//		this._log.Append("v").Append(data.ToString());
		//		XSingleton<XLoadingUI>.singleton.SetVersion(this._log.ToString());
		//	}
		//	else
		//	{
		//		XSingleton<XLoadingUI>.singleton.SetVersion("Dev 0.0.0");
		//	}
		//	if (launch)
		//	{
		//		this._phase = eUPdatePhase.xUP_LaunchGame;
		//	}
		//}

		//// Token: 0x060000D0 RID: 208 RVA: 0x00005B78 File Offset: 0x00003D78
		//private bool ProcessAssets(XResPackage res)
		//{
		//	AssetBundle assetBundle = null;
		//	bool flag = this._bundles.TryGetValue(XSingleton<XCommon>.singleton.XHash(res.bundle), out assetBundle);
		//	bool result;
		//	if (flag)
		//	{
		//		string text = res.location.Substring(res.location.LastIndexOf('/') + 1);
		//		ResourceType rtype = res.rtype;
		//		if (rtype != ResourceType.Assets)
		//		{
		//			if (rtype == ResourceType.Script)
		//			{
		//				TextAsset textAsset = assetBundle.LoadAsset(text, XUpdater.Ass.GetType(res.type)) as TextAsset;
		//				this._script = textAsset.bytes;
		//				assetBundle.Unload(false);
		//				this._bundles.Remove(XSingleton<XCommon>.singleton.XHash(res.bundle));
		//			}
		//		}
		//		else
		//		{
		//			this.OnAssetLoaded(res, assetBundle.LoadAsset(text, XUpdater.Ass.GetType(res.type)));
		//		}
		//		result = true;
		//	}
		//	else
		//	{
		//		result = false;
		//	}
		//	return result;
		//}

		//// Token: 0x060000D1 RID: 209 RVA: 0x00005C5C File Offset: 0x00003E5C
		//private IEnumerator AsyncProcessAssets(XResPackage package, AssetBundle bundle)
		//{
		//	string name = package.location.Substring(package.location.LastIndexOf('/') + 1);
		//	this._asset_loading = true;
		//	this._bundle_getter.GetAsset(bundle, package, new HandleLoadAsset(this.OnAssetLoaded));
		//	while (this._asset_loading)
		//	{
		//		yield return null;
		//	}
		//	yield break;
		//}

		//// Token: 0x060000D2 RID: 210 RVA: 0x00005C79 File Offset: 0x00003E79
		//private IEnumerator LaunchGame()
		//{
		//	XSingleton<XLoadingUI>.singleton.SetStatus(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_LOADING"), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//	yield return null;
		//	this.ABManager.Init();
		//	this.XPlatform.ReloadFMOD();
		//	AsyncAssemblyRequest aar = new AsyncAssemblyRequest();
		//	ResourceRequest rrq = null;
		//	bool flag = Application.platform == RuntimePlatform.Android;
		//	//if (flag)
		//	//{
		//	//	string path = Application.persistentDataPath + "/XMainClient.bytes";
		//	//	bool flag2 = File.Exists(path);
		//	//	if (flag2)
		//	//	{
		//	//		this._script = File.ReadAllBytes(path);
		//	//	}
		//	//	bool flag3 = this._script == null && (int)Application.platform == 11;
		//	//	if (flag3)
		//	//	{
		//	//		rrq = Resources.LoadAsync("XMainClient", typeof(TextAsset));
		//	//		yield return rrq;
		//	//		this._script = (rrq.asset as TextAsset).bytes;
		//	//	}
		//	//	path = null;
		//	//}
		//	//RuntimePlatform platform = Application.platform;
		//	//if (platform != RuntimePlatform.IPhonePlayer)
		//	//{
		//	//	if (platform != RuntimePlatform.Android)
		//	//	{
		//	//	}
		//	//	aar.Main = ((this._script == null) ? Assembly.Load("XMainClient") : Assembly.Load(this._script));
		//	//}
		//	//else
		//	//{
		//	//	aar.Main = Assembly.Load("XMainClient");
		//	//}
		//	//XSingleton<XShell>.singleton.MakeEntrance(aar.Main);
		//          XSingleton<XShell>.singleton.MakeEntrance(null);
		//          bool flag4 = rrq != null && rrq.asset != null;
		//	if (flag4)
		//	{
		//		Resources.UnloadAsset(rrq.asset);
		//	}
		//	this._script = null;
		//	this._log.Remove(0, this._log.Length);
		//	this._log.Append(XSingleton<XStringTable>.singleton.GetString("XUPDATE_INFO_LAUNCHING"));
		//	XSingleton<XLoadingUI>.singleton.SetStatus(this._log.ToString(), byte.MaxValue, byte.MaxValue, byte.MaxValue);
		//	yield return null;
		//	XSingleton<XShell>.singleton.PreLaunch();
		//	while (!XSingleton<XShell>.singleton.Launched())
		//	{
		//		yield return null;
		//		XSingleton<XShell>.singleton.Launch();
		//	}
		//	this.XLuaEngine.InitLua();
		//	yield break;
		//}

		//// Token: 0x060000D3 RID: 211 RVA: 0x00005C88 File Offset: 0x00003E88
		//private byte[] XCryptography(byte[] bs)
		//{
		//	for (int i = 0; i < bs.Length; i++)
		//	{
		//		bs[i] ^= 154;
		//	}
		//	return bs;
		//}

		//// Token: 0x060000D4 RID: 212 RVA: 0x00005CBC File Offset: 0x00003EBC
		//private string PatchPrefix()
		//{
		//	XLaunchMode launchMode = XUpdater.LaunchMode;
		//	string result;
		//	if (launchMode != XLaunchMode.Live)
		//	{
		//		if (launchMode != XLaunchMode.PreProduct)
		//		{
		//			if (launchMode != XLaunchMode.Dev)
		//			{
		//				result = "Patch/Live/";
		//			}
		//			else
		//			{
		//				result = "Patch/Dev/";
		//			}
		//		}
		//		else
		//		{
		//			result = "Patch/PreProduct/";
		//		}
		//	}
		//	else
		//	{
		//		result = "Patch/Live/";
		//	}
		//	return result;
		//}

		//// Token: 0x060000D5 RID: 213 RVA: 0x00005D09 File Offset: 0x00003F09
		//private void GetLaunchMode()
		//{
		//	XUpdater._launch_mode = XLaunchMode.Dev;
		//}

		//// Token: 0x060000D6 RID: 214 RVA: 0x00005D16 File Offset: 0x00003F16
		//public void PlayCG(object o = null)
		//{
		//	this._video.Play(false);
		//}

		//// Token: 0x060000D7 RID: 215 RVA: 0x00005D28 File Offset: 0x00003F28
		//public void SetServerVersion(string data)
		//{
		//	string b = "";
		//	bool flag = this._buildin != null;
		//	if (flag)
		//	{
		//		b = this._buildin.Build_Version.ToString();
		//	}
		//	string[] array = data.Split(new char[]
		//	{
		//		'|'
		//	});
		//	RuntimePlatform platform = Application.platform;
		//	if (platform != RuntimePlatform.IPhonePlayer)
		//	{
		//		if ((int)platform == 11)
		//		{
		//			this._target_version = array[1];
		//			try
		//			{
		//				bool flag2 = array.Length > 4;
		//				if (flag2)
		//				{
		//					string[] array2 = array[4].Split(new char[]
		//					{
		//						':'
		//					});
		//					for (int i = 0; i < array2.Length; i++)
		//					{
		//						string[] array3 = array2[i].Split(new char[]
		//						{
		//							'.'
		//						});
		//						bool flag3 = array3.Length > 1 && array3[1] == b;
		//						if (flag3)
		//						{
		//							this._target_version = array2[i];
		//						}
		//					}
		//				}
		//			}
		//			catch (Exception ex)
		//			{
		//				XSingleton<XDebug>.singleton.AddErrorLog("GetServer ExData Error!!!   " + ex.Message, null, null, null, null, null);
		//			}
		//		}
		//	}
		//	else
		//	{
		//		this._target_version = array[0];
		//		try
		//		{
		//			bool flag4 = array.Length > 3;
		//			if (flag4)
		//			{
		//				string[] array4 = array[3].Split(new char[]
		//				{
		//					':'
		//				});
		//				for (int j = 0; j < array4.Length; j++)
		//				{
		//					string[] array5 = array4[j].Split(new char[]
		//					{
		//						'.'
		//					});
		//					bool flag5 = array5.Length > 1 && array5[1] == b;
		//					if (flag5)
		//					{
		//						this._target_version = array4[j];
		//					}
		//				}
		//			}
		//		}
		//		catch (Exception ex2)
		//		{
		//			XSingleton<XDebug>.singleton.AddErrorLog("GetServer ExData Error!!!   " + ex2.Message, null, null, null, null, null);
		//		}
		//	}
		//	try
		//	{
		//		bool flag6 = array.Length > 2;
		//		if (flag6)
		//		{
		//			this._need_check_file = (array[2] == "1");
		//		}
		//	}
		//	catch (Exception ex3)
		//	{
		//		XSingleton<XDebug>.singleton.AddErrorLog("GetServer CheckFile Flag Error!!!   " + ex3.Message, null, null, null, null, null);
		//	}
		//	XSingleton<XDebug>.singleton.AddGreenLog(data, null, null, null, null, null);
		//	this._fetch_version_network.Close();
		//	this._phase = eUPdatePhase.xUP_LoadVersion;
		//}

		//// Token: 0x04000077 RID: 119
		//public static readonly uint Major_Version = 1U;

		//// Token: 0x04000078 RID: 120
		//private static XLaunchMode _launch_mode = XLaunchMode.Live;

		 
		public static GameObject XGameRoot = null;

		 
		//public static Assembly Ass = Assembly.GetAssembly(typeof(GameObject));

		 
		//public static int Md5Length = 16;

	 
		private int _main_threadId = 0;

	 
		//private bool _bEditorMode = false;

		//// Token: 0x0400007E RID: 126
		//private bool _bFetchVersion = false;

		//// Token: 0x0400007F RID: 127
		//private bool _bFetchServer = false;

		//// Token: 0x04000080 RID: 128
		//private StringBuilder _log = new StringBuilder();

		//// Token: 0x04000081 RID: 129
		//private IEnumerator _downloader = null;

		//// Token: 0x04000082 RID: 130
		//private IEnumerator _download_prepare = null;

		//// Token: 0x04000083 RID: 131
		//private IEnumerator _comparer = null;

		//// Token: 0x04000084 RID: 132
		//private IEnumerator _launcher = null;

		//// Token: 0x04000085 RID: 133
		//private IEnumerator _finish = null;

		//// Token: 0x04000086 RID: 134
		//private byte[] _script = null;

		//// Token: 0x04000087 RID: 135
		//private bool _on_file_download_retry = false;

		//// Token: 0x04000088 RID: 136
		//private bool _on_file_download_need_retry = false;

		//// Token: 0x04000089 RID: 137
		//private bool _update_done = false;

		//// Token: 0x0400008A RID: 138
		//private bool _bundle_fetching = false;

		//// Token: 0x0400008B RID: 139
		//private bool _asset_loading = false;

		//// Token: 0x0400008C RID: 140
		//private XVersion _version_getter = null;

		//// Token: 0x0400008D RID: 141
		//private XBundle _bundle_getter = null;

		//// Token: 0x0400008E RID: 142
		//private XFileLog _filelog_getter = null;

		//// Token: 0x0400008F RID: 143
		//private IFMOD_Listener _fmod_listenter = null;

		//// Token: 0x04000090 RID: 144
		//private ITssSdk _tssSdk = null;

		//// Token: 0x04000091 RID: 145
		//private IApolloManager _apolloManager = null;

		//// Token: 0x04000092 RID: 146
		//private IBroardcast _broadcast = null;

		//// Token: 0x04000093 RID: 147
		//private ILuaEngine _lua_engine = null;

		//// Token: 0x04000094 RID: 148
		//private IXPandoraMgr _pandoraManager = null;

		//// Token: 0x04000095 RID: 149
		//private IXGameSirControl _SirControl = null;

		//// Token: 0x04000096 RID: 150
		//private IXVideo _video = null;

		//// Token: 0x04000097 RID: 151
		private IPlatform _platform = null;

		//// Token: 0x04000098 RID: 152
		//private BuildTarget _runtime_platform = BuildTarget.Unknown;

		//// Token: 0x04000099 RID: 153
		//private string _platform_name = "";

		//// Token: 0x0400009A RID: 154
		//private string _version = "0.0.0";

		//// Token: 0x0400009B RID: 155
		//private string _target_version = "0.0.0";

		//// Token: 0x0400009C RID: 156
		//private bool _need_check_file = false;

		//// Token: 0x0400009D RID: 157
		//private bool _need_play_cg = true;

		//// Token: 0x0400009E RID: 158
		//private XFetchVersionNetwork _fetch_version_network = null;

		//// Token: 0x0400009F RID: 159
		//private float _fetch_version_time = 0f;

		//// Token: 0x040000A0 RID: 160
		//private ulong _update_pakcage_size = 0UL;

		//// Token: 0x040000A1 RID: 161
		//private XVersionData _server = null;

		//// Token: 0x040000A2 RID: 162
		//private XVersionData _client = null;

		//// Token: 0x040000A3 RID: 163
		//private XVersionData _buildin = null;

		//// Token: 0x040000A4 RID: 164
		//private IResourceHelp _resourcehelp = null;

		//// Token: 0x040000A5 RID: 165
		//private XBundleData _bundle_data = null;

		//// Token: 0x040000A6 RID: 166
		//private eUPdatePhase _phase = eUPdatePhase.xUP_None;

		//// Token: 0x040000A7 RID: 167
		//private List<XBundleData> _download_bundle = new List<XBundleData>();

		//// Token: 0x040000A8 RID: 168
		//private List<XBundleData> _cacheload_bundle = new List<XBundleData>();

		//// Token: 0x040000A9 RID: 169
		//private List<XMetaResPackage> _meta_bundle = new List<XMetaResPackage>();

		//// Token: 0x040000AA RID: 170
		//private Dictionary<uint, Object> _persist_assets = new Dictionary<uint, Object>();

		//// Token: 0x040000AB RID: 171
		//private Dictionary<uint, byte[]> _persist_image = new Dictionary<uint, byte[]>();

		//// Token: 0x040000AC RID: 172
		//private Dictionary<uint, XBundleData> _assets = new Dictionary<uint, XBundleData>();

		//// Token: 0x040000AD RID: 173
		//private Dictionary<uint, XResPackage> _res_list = new Dictionary<uint, XResPackage>();

		//// Token: 0x040000AE RID: 174
		//private Dictionary<uint, AssetBundle> _bundles = new Dictionary<uint, AssetBundle>();

		//// Token: 0x040000AF RID: 175
		//public AssetBundleManager ABManager;

		//// Token: 0x040000B1 RID: 177
		//private bool _is_download_update_pic = false;
	}
}
