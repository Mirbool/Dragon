using System;
using System.Collections.Generic;
using KKSG;
using XUpdater;
using XUtliPoolLib;
using UnityEngine;
using Koakuma.Game;

namespace XMainClient
{
	// Token: 0x02001149 RID: 4425
	internal class Process_PtcG2C_SelectRoleNtf
	{
		// Token: 0x0600D9EF RID: 55791 RVA: 0x0032BF54 File Offset: 0x0032A154
		public static void Process(PtcG2C_SelectRoleNtf roPtc)
		{
			Debug.Log("服务区返回------PtcG2C_SelectRoleNtf------" + roPtc);
			SelectRoleComponent sm =GameManager.ECS.World.GetComponent<SelectRoleComponent>();

			sm.role = roPtc.Data.roleData;
		}
	}
}
