using UnityEngine;
using System.Collections;
using AW.Entity;
using AW.Message;
using AW.Framework;
using AW.Data;
using AW.FSM;
using AW.Resources;

namespace AW.Controller {

	[Controller(ctrlType = LogicalType.Login)]
	public class LoginCtrl : ControllerEx {

		public override void UI_OnReceive (MsgParam param) {
			base.UI_OnReceive (param);

			if(param is TryToLoginParam) {
				TryToLoginParam OkParam = (TryToLoginParam) param;
				switch(OkParam.commond) {
				case TryToLoginParam.SYNC_CONFIG_OK:
					//TODO : 登陆游戏服务器

					//异步读取配表
					AsyncTask.RunAsync(
						() => { 
							//Core.Data.readLocalConfig(); 
							tryToLoginGame();
						}
					);
					break;
				}
			}
		}

		void tryToLoginGame() {
			AccountData ad = Native.mInstace.m_thridParty.GetAccountData();

			EngineModel engineModel = Core.Data.getIModelNetwork<EngineModel>();
			Server chosen  = engineModel.ChosenServer;
			HttpClient.BaseUrl = chosen.url;

			LoginParam loginParam = new LoginParam(ad, engineModel.token, chosen.sid.ToString());
			SendHttpRequest(RequestType.LOGIN_GAME, loginParam);
		}

		/// <summary>
		/// Http 发生错误的时候，这个错误是特别的指定网络异常，不是逻辑错误
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="error">Error.</param>
		public override void Http_ErrorOccured(BaseHttpRequest request, string error) {
			base.Http_ErrorOccured(request, error);
		}

		/// <summary>
		/// 当网络返回，逻辑正确的时候
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="response">Response.</param>
		public override void Http_OnReceive_OK(BaseHttpRequest request, BaseResponse response) {
			base.Http_OnReceive_OK(request, response);

			HttpRequest req = request as HttpRequest;
			if(req != null) {
				//跳转场景,进入游戏
				if(req.Type == RequestType.LOGIN_GAME) {

					AccountData ad = Native.mInstace.m_thridParty.GetAccountData();
					EngineModel engineModel = Core.Data.getIModelNetwork<EngineModel>();
					Server chosen  = engineModel.ChosenServer;

					StateParam<GameState> state = new StateParam<GameState>();
					LoginInfo info = new LoginInfo() {
						UniqueId  = ad.uniqueId,
						curServer = chosen.name,
						LocalIOMgr= Core.DPM,
						logUtc    = 123244123,
					};
					state.obj = info;
					Core.GameFSM.handleStateChg(state, GameState.Logined);
					readComplete ();
				}
			}
		}

		void readComplete() {
			AsyncTask.QueueOnMainThread(
				() => {
//					PrefabLoader loader = Core.ResEng.getLoader<PrefabLoader>();
//					loader.loadFromUnPack("Login/EnterWar", false);
				    //界面的创建是在LUA层实现的()
				    //现在LUA没有网络,为确保战斗正常还是使用C#原来的登陆方式
				    //临时使用
					LuaLinker lua = GameObject.Find("UI Root").GetComponent<LuaLinker>();
					if(lua) lua.loginServerFinished();
				}
			);
		}

		/// <summary>
		/// 当网络返回有逻辑错误的时候
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="response">Response.</param>
		public override void Http_OnReceive_Fail(BaseHttpRequest request, BaseResponse response) {
			base.Http_OnReceive_Fail(request, response);
		}


		#region UI的控制操作


		#endregion

	}

}
