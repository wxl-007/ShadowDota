using UnityEngine;
using System.Collections;
using AW.Message;


namespace AW.Entity {
	/// 
	/// Entity的逻辑类型
	/// 
	public enum LogicalType {
		//闪屏界面，底层引擎的控制器
		Engine,
		//登陆控制器
		Login,
		//无名氏
		Anonymous,
		//主界面
		MainUIController,
		//战斗的控制器
		War,
	}


	/// 
	/// UI控制器类
	/// 
	public class ControllerEx : Entity {

		//控制器的类型，主要是用来方便的识别
		//挂在Controller上的脚本，就必须设定这个属于什么控制器
		//程序运行过程中，这个值不允许修改。
		public LogicalType CtrlType;

		void Start() {
			mEntityType = EntityType.Entity_Control;
			Core.EntityMgr.SignID(this);

		}

		void OnDestory() {
			Core.EntityMgr.ClearEntity(this);
		}


		/// 
		/// 接收其他ControllerEx发过来的信息
		/// 
		public virtual void UI_OnReceive(MsgParam param) {
			ConsoleEx.DebugLog( "Controller " + GetType().ToString() + " receive MsgParam : " + fastJSON.JSON.Instance.ToJSON(param));
			//TODO : add logical here ...
		}

		protected void SendHttpRequest(RequestType reqType, BaseRequestParam param) {
			HttpTaskEx task = new HttpTaskEx(UniqueId, ThreadType.MainThread, TaskResponse.Default_Response);
			task.AppendCommonParam(reqType, param);
			task.DispatchToRealHandler();
		}

		/// <summary>
		/// Http 发生错误的时候，这个错误是特别的指定网络异常，不是逻辑错误
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="error">Error.</param>
		public virtual void Http_ErrorOccured(BaseHttpRequest request, string error) {
			ConsoleEx.DebugLog("ControllerEx found a Http Error Occured.");
		}

		/// <summary>
		/// 当网络返回，逻辑正确的时候
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="response">Response.</param>
		public virtual void Http_OnReceive_OK(BaseHttpRequest request, BaseResponse response) {
			ConsoleEx.DebugLog("Controller " + GetType().ToString() + " receive response Ok");
		}

		/// <summary>
		/// 当网络返回有逻辑错误的时候
		/// </summary>
		/// <param name="request">Request.</param>
		/// <param name="response">Response.</param>
		public virtual void Http_OnReceive_Fail(BaseHttpRequest request, BaseResponse response) {
			ConsoleEx.DebugLog("Controller " + GetType().ToString() +  " receive response Fail");
		}
	}

}