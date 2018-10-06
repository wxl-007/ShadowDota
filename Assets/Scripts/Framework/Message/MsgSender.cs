using AW.Entity;
using AW.War;
using AW.Data;

namespace AW.Message {

	/// 
	/// 消息接受类型，
	/// 
	public enum MsgRecType {
		//确保有接受者，
		MakeSure,
		//如果接受者不存在，就忽略
		IgnoreIt,
	}

	/// 
	/// 消息参数, 所有的消息的参数类型都继承自此类
	/// 
	public class MsgParam {
		//发送者ID
		public int Sender;
		//接收者ID
		public int Receiver;
		//延迟多长时间发送
		//必须调用SendMessageAsync才有用
		public float Delay;
		//命令类型
		public int commond;

        //消息参数
        public object param;

		//参数1
		public int arg1;
		//参数2
		public int arg2;
	}

	//战斗里面的ui消息
	public enum WarUI_Cmd_Type
	{
		SwitchHero = 1,			//切换英雄
	}

	/// 
	/// ID 是EntityManager分配的唯一ID
	/// param是参数类型
	/// 
	public interface IMsgSender {

		/// <summary>
		/// 这是一个同步的发送消息的方法
		/// </summary>
		/// <returns><c>true</c>, if message was sent, <c>false</c> otherwise.</returns>
		/// <param name="ID">ID 必须是ControllerEx的ID</param>
		/// <param name="param">Parameter.</param>
		bool SendMessage(int senderID, int recID, MsgParam param);


		/// <summary>
		/// 这是一个异步的发送消息的方法
		/// </summary>
		/// <returns><c>true</c>, if message was sent, <c>false</c> otherwise. 异步回调的结果不准确。但结果是false则必定出错。</returns>
		/// <param name="ID">ID 必须是ControllerEx的ID</param>
		/// <param name="param">Parameter.</param>
		bool SendMessageAsync(int senderID, int recID, MsgParam param);
	}


	/// 
	/// 这个接口是更加上层的封装，实际上调用的是MsgSender
	/// 
	public interface MsgSenderEx {
		/// <summary>
		/// 这是一个同步的发送消息的方法, 倒数第二个参数代表是否匿名,即忽略发起者
		/// 最后一个参数代表是否在乎接收者
		/// </summary>
		bool sendMessage(LogicalType sender, LogicalType receiver, MsgParam param, bool anonymous, MsgRecType sure);

		/// <summary>
		/// 这是一个异步的发送消息的方法, 异步回调的结果不准确。但结果是false则必定出错。
		/// 倒数第二个参数代表是否匿名,即忽略发起者
		/// 最后一个参数代表是否在乎接收者
		/// </summary>
		bool SendMessageAsync(LogicalType sender, LogicalType receiver, MsgParam param, bool anonymous, MsgRecType sure);
	}

}

