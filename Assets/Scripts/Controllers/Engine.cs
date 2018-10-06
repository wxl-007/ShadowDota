using UnityEngine;
using System.Collections;
using AW.Entity;
using AW.Message;
using AW.Framework;

namespace AW.Controller {

	[Controller(ctrlType = LogicalType.Engine)]
	public class Engine : ControllerEx {

		public override void UI_OnReceive (MsgParam param) {
			base.UI_OnReceive (param);

			if(param is EngineInitOKParam) {
				EngineInitOKParam OkParam = (EngineInitOKParam) param;
				switch(OkParam.commond) {
				case EngineInitOKParam.COMMOAND_ENGINE_OK:
					//TODO : 获取服务器列表
					Native.mInstace.m_thridParty.getUniqueId(onThirdPartyOK);
					break;
				}
			}
		}

		/// <summary>
		/// 当获取第三方SDK账号中心成功之后
		/// </summary>
		/// <param name="ad">Ad.</param>
		void onThirdPartyOK(AccountData ad) {
			ThirdGetServerParam thirdParam = new ThirdGetServerParam(ad);
			SendHttpRequest(RequestType.THIRD_GET_SERVER, thirdParam);
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
				//跳转场景,当在新的场景的时候，执行同步配表等语句
				if(req.Type == RequestType.THIRD_GET_SERVER) {
					UnityUtils.JumpToScene(Core.GameFSM, SceneName.LoginScene);

//					TryToLoginParam param = new TryToLoginParam();
//					param.commond = TryToLoginParam.SYNC_CONFIG_OK;
//					Core.EntityMgr.sendMessage(CtrlType, LogicalType.Login, param, false, MsgRecType.MakeSure);
				}
					
			}
		}

		/// <summary>
		/// 当网络返回有逻辑错误的时候
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="response">Response.</param>
		public override void Http_OnReceive_Fail(BaseHttpRequest request, BaseResponse response) {
			base.Http_OnReceive_Fail(request, response);
		}

	}
}
