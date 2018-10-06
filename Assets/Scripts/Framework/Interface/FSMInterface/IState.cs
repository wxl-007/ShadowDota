using System;

/// 
/// 所有游戏的核心状态一共分成两个部分：
/// 一个部分是游戏相关的状态，比如游戏登陆成功，游戏注销成功，到了特定的时间点（比如日期更改）等
/// 一个是设备相关的状态，比如启动游戏，切入后台，切回前台，wifi中断
/// 
namespace AW.FSM {

	/// <summary>
	/// 泛型的状态参数
	/// </summary>
	public class StateParam<T> {
		//状态转换时候，记录下状态值
		public T prevGameState;
		public T NowGameState;

		//参数
		public Object obj;
	}


	/// 
	/// *************************** Game Play Status **************************
	/// 
	public enum GameState {
		None    = 0x01,    //游戏启动时的默认值
		Logined = 0x02,    //游戏登陆成功
		Logout  = 0x03,    //注销 
		DayChanged = 0x04, //日期改变
		LevelChanged = 0x05,//跳转场景
		ReLogIn = 0x04,    //再次登录游戏
		Init    = 0x06,    //游戏的核心初始化
	}


	/// 
	/// 一个部分是游戏相关的状态，比如游戏登陆成功，游戏注销成功，到了特定的时间点（比如日期更改）等
	/// 
	public interface IGameState {
		/// <summary>
		/// 登陆成功
		/// </summary>
		/// <param name="obj">Object.</param>
		void OnLogin(StateParam<GameState> obj);
		/// <summary>
		/// 登出成功
		/// </summary>
		/// <param name="obj">Object.</param>
		void OnUnregister(StateParam<GameState> obj);
		/// <summary>
		/// 一天日期结束
		/// </summary>
		/// <param name="obj">Object.</param>
		void OnDayChanged(StateParam<GameState> obj);
		/// <summary>
		/// 场景跳转
		/// </summary>
		void OnLevelChanged(StateParam<GameState> obj);
	}

	/// 
	/// 游戏相关的状态, 但是更加的特殊，就特定的类会使用，
	/// 初始化完成，就只有GamePlayFSM类实现该接口
	/// 
	public interface IInit {
		void OnInitOk();
	}


	/// 
	/// *************************** Device Status ******************************
	/// 实现了IDeviceState的目前只有TimerMaster
	/// 
	public enum DeviceState {
		None         = 0x01, //游戏启动时的默认值
		GameLaunched = 0x02, //启动游戏
		GamePaused   = 0x03, //切入后台
		GameResume   = 0x04, //切回前台
		GameQuit     = 0x05, //游戏退出
	}

	///
	/// 一个是设备相关的状态，比如启动游戏，切入后台，切回前台，wifi中断
	/// 
	public interface IDeviceState {
		/// <summary>
		/// 启动游戏
		/// </summary>
		/// <param name="obj">Object.</param>
		void OnGameLaunched(StateParam<DeviceState> obj);
		/// <summary>
		/// 切入后台
		/// </summary>
		/// <param name="obj">Object.</param>
		void OnPaused(StateParam<DeviceState> obj);
		/// <summary>
		/// 切回前台
		/// </summary>
		/// <param name="obj">Object.</param>
		void OnResume(StateParam<DeviceState> obj);

		/// <summary>
		/// 游戏退出
		/// </summary>
		/// <param name="obj">Object.</param>
		void OnQuit(StateParam<DeviceState> obj);
	}
}


