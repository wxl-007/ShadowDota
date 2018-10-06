using System.Net;
using System.Collections.Generic;
using AW.Entity;
using AW.Message;

namespace AW.FSM {
	/// 
	/// 游戏逻辑的状态控制器
	/// 
	public class GamePlayFSM : IInit {

		//绑定接口
		private ImplicitBinder binder;
		//初始化完成了吗？
		private bool initOK = false;
		public bool InitOK {
			get {
				return initOK;
			}
		}

		//之前的状态
		private GameState mPreState;
		//当前的状态
		private GameState mNowState;
		//收听者的列表
		private List<IGameState> Owner;

		//之前的场景名称
		private string lastScene;
		public string LastScene {
			get { return lastScene; }
		}
		//当前的场景名称
		private string curScene;
		public string CurScene {
			get { return curScene; }
		}


		private GamePlayFSM() { 
			Owner = new List<IGameState>();

			mPreState = GameState.None;
			mNowState = GameState.None;
			initOK    = Consts.FAILURE;
			curScene  = SceneName.SplashScene;
			binder    = ImplicitBinder.Instance;
		}

		///
		///单例模式
		/// 
		public static GamePlayFSM Instance {
			get { 
				return GenericSingleton<GamePlayFSM>.Instance; 
			}
		}

		/// 
		/// 注册收听者
		/// 
		public void registerStateReceiver(IGameState Listener) {
			if(Listener != null)
				Owner.Add(Listener);
		}

		/// 
		/// 游戏逻辑的状态发生改变的时候
		/// 
		public void handleStateChg(StateParam<GameState> param, GameState NowState) {
			ConsoleEx.DebugLog("GamePlay Status Changed. " + mPreState.ToString() + " -> " + NowState.ToString(), ConsoleEx.RED);

			mPreState = mNowState;
			mNowState = NowState;
			param.NowGameState  = mNowState;
			param.prevGameState = mPreState;

			//优先处理核心处理层
			foreach(IGameState listener in Owner) {
				switch(NowState) {
				case GameState.DayChanged:
					break;
				case GameState.Logined:
					listener.OnLogin(param);
					break;
				case GameState.Logout:

					break;
				case GameState.LevelChanged:
					listener.OnLevelChanged(param);

					LevelChanged lvl = param.obj as LevelChanged;
					if(lvl != null) lastScene = lvl.curLevel;
					break;
				}
			}


			EntityManager entityMgr = Core.EntityMgr;
			//再处理控制层
			foreach(LogicalType controller in binder.IControllerGamePlay) {

				ControllerEx ctrl = entityMgr.getEntityByLogicalType(controller);
				if(ctrl != null && ctrl is IGameState) {
					IGameState igame = ctrl as IGameState;
					switch(NowState) {
					case GameState.DayChanged:
						break;
					case GameState.Logined:
						igame.OnLogin(param);
						break;
					case GameState.Logout:
						break;
					case GameState.LevelChanged:
						igame.OnLevelChanged(param);
						break;
					}
				}
			}
		}


		#region IInit implementation
		/// <summary>
		/// 引擎初始化完成
		/// </summary>
		public void OnInitOk () {
			initOK = Consts.OK;
			EngineInitOKParam EngInitOK = new EngineInitOKParam() {
				commond = EngineInitOKParam.COMMOAND_ENGINE_OK,
			};
			Core.EntityMgr.sendMessage(LogicalType.Anonymous, LogicalType.Engine, EngInitOK, true, MsgRecType.MakeSure);
		}

		#endregion
	}


	#region 所有状态需要传递的数据信息

	/// <summary>
	/// 登陆状态要传递的数据
	/// </summary>
	public class LoginInfo {
		//用户的唯一ID
		public string UniqueId;
		//当前选中的ID
		public string curServer;
		//当前服务器的时间
		public long logUtc;
		//最基础的底层IO类
		public AW.IO.LocalIOManager LocalIOMgr;
		//Socket的IP & Port 的信息
		public DnsEndPoint dep;
	}

	/// 
	/// 跳转场景的时候
	/// 
	public class LevelChanged {
		//从一个level跳转到另个一个level
		public string curLevel;
		public string targetLevel;
	}

	#endregion
}
