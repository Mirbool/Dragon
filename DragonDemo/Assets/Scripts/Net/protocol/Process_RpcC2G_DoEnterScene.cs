using System;
using KKSG;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
	// Token: 0x02001167 RID: 4455
	internal class Process_RpcC2G_DoEnterScene
	{
		// Token: 0x1700371A RID: 14106
		// (get) Token: 0x0600DA6F RID: 55919 RVA: 0x0032D4E8 File Offset: 0x0032B6E8
		public static uint runstate
		{
			get
			{
				return Process_RpcC2G_DoEnterScene._runstate;
			}
		}

		// Token: 0x0600DA70 RID: 55920 RVA: 0x0032D500 File Offset: 0x0032B700
		public static void OnReply(DoEnterSceneArg oArg, DoEnterSceneRes oRes)
		{
			bool flag = oRes.errorcode == ErrorCode.ERR_INVALID_REQUEST;
			if (flag)
			{
				XSingleton<XDebug>.singleton.AddErrorLog("RpcC2G_DoEnterScene ERR_INVALID_REQUEST!", null, null, null, null, null);
			}
			else
			{
				
			}
		}

		// Token: 0x0600DA71 RID: 55921 RVA: 0x000FEEFC File Offset: 0x000FD0FC
		public static void OnTimeout(DoEnterSceneArg oArg)
		{
		}

		// Token: 0x04006239 RID: 25145
		private static uint _runstate = 0U;
	}
}
