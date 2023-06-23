using System;
using System.Reflection;
using KKSG;
using Koakuma.Game;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using XUtliPoolLib;

namespace XMainClient
{
	// Token: 0x02001147 RID: 4423
	internal class Process_RpcC2M_SelectRoleNew
	{
		// Token: 0x0600D9E7 RID: 55783 RVA: 0x0032BCC0 File Offset: 0x00329EC0
		public static void OnReply(SelectRoleNewArg oArg, SelectRoleNewRes oRes)
		{
			bool flag = oRes .result == ErrorCode.ERR_SUCCESS;
			if (flag)
			{
				
				Addressables.LoadSceneAsync("Assets/Scenes/MainScene.unity", LoadSceneMode.Single);
				Debug.Log("进入主场景场景");
				GameManager.UI.CloseUI(UIViewID.SelectRoleUI);
				GameManager.UI.OpenUI(UIViewID.MainSceneUI);
			}
			else
			{

			}
		}

		// Token: 0x0600D9E8 RID: 55784 RVA: 0x0032BEE4 File Offset: 0x0032A0E4
		public static void OnTimeout(SelectRoleNewArg oArg)
		{
			
		}
	}
}
