<<<<<<< HEAD
using System;
using KKSG;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
	// Token: 0x02001167 RID: 4455
	internal class Process_RpcC2G_DoEnterScene
=======
using KKSG;
using Koakuma.Game;

namespace XMainClient
{
    // Token: 0x02001167 RID: 4455
    internal class Process_RpcC2G_DoEnterScene
>>>>>>> 9783df7ccaaa6d66b300a2282af8eaa34b7e1e5b
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
<<<<<<< HEAD
				XSingleton<XDebug>.singleton.AddErrorLog("RpcC2G_DoEnterScene ERR_INVALID_REQUEST!", null, null, null, null, null);
			}
			else
			{
				
			}
		}
=======
				//XSingleton<XDebug>.singleton.AddErrorLog("RpcC2G_DoEnterScene ERR_INVALID_REQUEST!", null, null, null, null, null);
			}
			else
			{
             
                #region 原项目代码
                //bool flag2 = !XSingleton<XScene>.singleton.bSceneLoadedRpcSend;
                //if (!flag2)
                //{
                //	XSingleton<XScene>.singleton.bSceneServerReady = true;
                //	XSingleton<XScene>.singleton.bSceneLoadedRpcSend = false;
                //	XSingleton<XDebug>.singleton.AddLog("Enter scene ", XSingleton<XScene>.singleton.SceneID.ToString(), null, null, null, null, XDebugColor.XDebug_None);
                //	bool flag3 = oRes.errorcode == ErrorCode.ERR_DOENTERSCENE_FAILED;
                //	if (flag3)
                //	{
                //		XSingleton<XDebug>.singleton.AddLog("ERR_DOENTERSCENE_FAILED", null, null, null, null, null, XDebugColor.XDebug_None);
                //	}
                //	else
                //	{
                //		Process_RpcC2G_DoEnterScene._runstate = oRes.scenestate.runstate;
                //		Vector3 vector;
                //                    vector = new Vector3(oRes.pos.x, oRes.pos.y, oRes.pos.z);
                //		Vector3 vector2 = XSingleton<XCommon>.singleton.FloatToAngle(oRes.face);
                //		Vector3 face = XSingleton<XCommon>.singleton.FloatToAngle(oRes.initface);
                //		XSingleton<XEntityMgr>.singleton.Player.Attributes.OnFightGroupChange((XSingleton<XGame>.singleton.SyncModeValue != 0) ? oRes.fightgroup : 1U);
                //		XSingleton<XEntityMgr>.singleton.Player.Attributes.AppearAt = vector;
                //		XSingleton<XEntityMgr>.singleton.Player.Net.CorrectNet(vector, face, 0U, true);
                //		XBattleDocument.MiniMapSetRotation(oRes.initface);
                //		GameObject gameObject = GameObject.Find("Scene/BattlePoint");
                //		XSingleton<XScene>.singleton.BattleTargetPoint = ((gameObject != null) ? gameObject.transform.position : Vector3.zero);
                //		gameObject = GameObject.Find("Scene/NestPoint");
                //		XSingleton<XScene>.singleton.NestTargetPoint = ((gameObject != null) ? gameObject.transform.position : Vector3.zero);
                //		XSingleton<XScene>.singleton.GameCamera.Root_R_Y_Default = XSingleton<XEntityMgr>.singleton.Player.EngineObject.Rotation.eulerAngles.y;
                //		XSingleton<XScene>.singleton.GameCamera.Root_R_Y = XSingleton<XScene>.singleton.GameCamera.Root_R_Y_Default;
                //		bool flag4 = XSingleton<XScene>.singleton.GameCamera.Wall != null;
                //		if (flag4)
                //		{
                //			XSingleton<XScene>.singleton.GameCamera.Wall.TargetY = XSingleton<XScene>.singleton.GameCamera.Root_R_Y_Default;
                //		}
                //		bool bSpectator = XSingleton<XScene>.singleton.bSpectator;
                //		if (bSpectator)
                //		{
                //			XSingleton<XEntityMgr>.singleton.Puppets(XSingleton<XEntityMgr>.singleton.Player, true, true);
                //			XSingleton<XInput>.singleton.Freezed = true;
                //			XSingleton<XEntityMgr>.singleton.Player.Attributes.OnFightGroupChange(2U);
                //		}
                //		else
                //		{
                //			bool flag5 = XSingleton<XEntityMgr>.singleton.Player.Nav != null;
                //			if (flag5)
                //			{
                //				XSingleton<XEntityMgr>.singleton.Player.Nav.Active();
                //			}
                //			bool flag6 = XSingleton<XEntityMgr>.singleton.Player.AI != null;
                //			if (flag6)
                //			{
                //				XSingleton<XEntityMgr>.singleton.Player.AI.Active();
                //			}
                //			bool flag7 = ((ulong)oRes.specialstate & (ulong)(1L << (XFastEnumIntEqualityComparer<UnitSpecialState>.ToInt(UnitSpecialState.Unit_Puppet) & 31))) > 0UL;
                //			bool flag8 = ((ulong)oRes.specialstate & (ulong)(1L << (XFastEnumIntEqualityComparer<UnitSpecialState>.ToInt(UnitSpecialState.Unit_Invisible) & 31))) > 0UL;
                //			bool flag9 = flag7 || flag8;
                //			if (flag9)
                //			{
                //				XSingleton<XEntityMgr>.singleton.Player.UpdateSpecialStateFromServer(oRes.specialstate, uint.MaxValue);
                //			}
                //			else
                //			{
                //				XSingleton<XEntityMgr>.singleton.Player.Present.ShowUp();
                //			}
                //		}
                //		bool bSpectator2 = XSingleton<XScene>.singleton.bSpectator;
                //		if (bSpectator2)
                //		{
                //			XSpectateSceneDocument specificDocument = XDocuments.GetSpecificDocument<XSpectateSceneDocument>(XSpectateSceneDocument.uuID);
                //			specificDocument.IsCrossServerBattle = oRes.is_cross;
                //		}
                //		else
                //		{
                //			XBattleDocument specificDocument2 = XDocuments.GetSpecificDocument<XBattleDocument>(XBattleDocument.uuID);
                //			specificDocument2.IsCrossServerBattle = oRes.is_cross;
                //		}
                //		for (int i = 0; i < XSingleton<XGame>.singleton.Doc.Components.Count; i++)
                //		{
                //			(XSingleton<XGame>.singleton.Doc.Components[i] as XDocComponent).OnEnterSceneFinally();
                //		}
                //		XOutlookHelper.SetStatusState(XSingleton<XEntityMgr>.singleton.Player.Attributes, XSingleton<XEntityMgr>.singleton.Player, oRes.state, true);
                //		XSingleton<XScene>.singleton.SceneStarted = oRes.scenestate.isready;
                //		bool flag10 = !XSingleton<XScene>.singleton.SceneStarted && DlgBase<BattleMain, BattleMainBehaviour>.singleton.IsLoaded() && DlgBase<BattleMain, BattleMainBehaviour>.singleton.IsVisible();
                //		if (flag10)
                //		{
                //			DlgBase<BattleMain, BattleMainBehaviour>.singleton.uiBehaviour.m_PromptFrame.gameObject.SetActive(true);
                //			DlgBase<BattleMain, BattleMainBehaviour>.singleton.SetLoadingPrompt(null, false);
                //		}
                //		bool flag11 = XSingleton<XScene>.singleton.bSpectator && oRes.iswatchend;
                //		if (flag11)
                //		{
                //			DlgBase<SpectateSceneView, SpectateSceneBehaviour>.singleton.ShowBackToMainCityTips();
                //		}
                //		XSingleton<XLevelFinishMgr>.singleton.LevelRewardToken = oRes.battlestamp;
                //		bool isViewGridScene = XSingleton<XScene>.singleton.IsViewGridScene;
                //		if (isViewGridScene)
                //		{
                //			XSingleton<XEntityMgr>.singleton.Player.Net.SetHallSequence();
                //		}
                //		bool flag12 = XSingleton<XGame>.singleton.CurrentStage.Stage == EXStage.World;
                //		if (flag12)
                //		{
                //			XSingleton<XEntityMgr>.singleton.Player.Attributes.SecurityStatistics.OnStart();
                //		}
                //		XSingleton<XReconnection>.singleton.SetLoginReconnectFlag(oRes.lrdata != null);
                //		bool flag13 = oRes.lrdata != null;
                //		if (flag13)
                //		{
                //			XSingleton<XReconnection>.singleton.StartLoginReconnectSync(oRes.lrdata, oRes.otherunits);
                //		}
                //		else
                //		{
                //			XSingleton<XReconnection>.singleton.StartEnterSceneSync(oRes.otherunits);
                //		}
                //	}
                //}
                #endregion
                GameManager.Message.Post<MessageType.EnterLobby>(new MessageType.EnterLobby()).Coroutine();
            }
        }
>>>>>>> 9783df7ccaaa6d66b300a2282af8eaa34b7e1e5b

		// Token: 0x0600DA71 RID: 55921 RVA: 0x000FEEFC File Offset: 0x000FD0FC
		public static void OnTimeout(DoEnterSceneArg oArg)
		{
		}

		// Token: 0x04006239 RID: 25145
		private static uint _runstate = 0U;
	}
}
