using UnityEngine;
using AW.Entity;
using System.Collections.Generic;

namespace AW.FSM {

	/// 
	/// 设备的状态控制器
	/// 所有的方法都必须运行在Unity主线程中
	/// 
	public class DeviceFSM {
		//绑定接口
		private ImplicitBinder binder;
		//之前的状态
		private DeviceState mPreState;
		//当前的状态
		private DeviceState mNowState;
		//收听者的列表
		private List<IDeviceState> Owner;
		//获取当前的运行环境
		public RuntimePlatform rtPlatform {
			get;
			private set;
		}

		private DeviceFSM() { 
			Owner = new List<IDeviceState>();

			mPreState = DeviceState.None;
			mNowState = DeviceState.None;

			rtPlatform = Application.platform;
			binder    = ImplicitBinder.Instance;
		}

		///
		///单例模式
		/// 
		public static DeviceFSM Instance {
			get { 
				return GenericSingleton<DeviceFSM>.Instance; 
			}
		}

		/// 
		/// 注册收听者
		/// 
		public void registerStateReceiver(IDeviceState Listener) {
			if(Listener != null)
				Owner.Add(Listener);
		}

		/// 
		/// 设备的状态发生改变的时候
		/// 
		public void handleStateChg(DeviceState NowState) {
			ConsoleEx.DebugLog("Device Status Changed. " + mPreState.ToString() + " -> " + NowState.ToString(), ConsoleEx.RED);

			mPreState = mNowState;
			mNowState = NowState;

			StateParam<DeviceState> param = new StateParam<DeviceState>();
			param.NowGameState  = mNowState;
			param.prevGameState = mPreState;

			//优先处理核心处理层
			foreach(IDeviceState listener in Owner) {
				switch(NowState) {
				case DeviceState.GameLaunched:
					listener.OnGameLaunched(param);
					break;
				case DeviceState.GamePaused:
					listener.OnPaused(param);
					break;
				case DeviceState.GameResume:
					listener.OnResume(param);
					break;
				case DeviceState.GameQuit:
					listener.OnQuit(param);
					break;
				}
			}

			EntityManager entityMgr = Core.EntityMgr;
			//再处理控制层
			foreach(LogicalType controller in binder.IControllerDevice) {

				ControllerEx ctrl = entityMgr.getEntityByLogicalType(controller);
				if(ctrl != null && ctrl is IDeviceState) {
					IDeviceState idev = ctrl as IDeviceState;
					switch(NowState) {
					case DeviceState.GameLaunched:
						idev.OnGameLaunched(param);
						break;
					case DeviceState.GamePaused:
						idev.OnPaused(param);
						break;
					case DeviceState.GameResume:
						idev.OnResume(param);
						break;
					case DeviceState.GameQuit:
						idev.OnQuit(param);
						break;
					}
				}
			}

		}

	}

}
