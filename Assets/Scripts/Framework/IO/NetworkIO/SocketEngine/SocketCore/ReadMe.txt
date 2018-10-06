目前的工程里面包含的Socket模块：

1. SuperSocket.ClientEngine - 目前使用中
   这个是socket通讯的底层代码，如果做这方面的开发，请详读代码
   目前都是基于AsyncTcpSession.cs来开发的。
2. WebSocket - 尚未使用 

3. 本客户端也包含了关于 JAVA服务器端开发的思想，详细请详读SocketEnging/SocketCore/JAVATHINGKING下的Action和Comm模块。
   思想-根据ACT来完成反射

4. 本客户端使用SocketEnging/SocketCore/C#THINGKING下的代码， lambda或者Action来完成socket开发的思想。



Socket的概念：
   1. Socket是双工的，所以服务器可以主动给客户端发送东西。
   2. 如果是客户端主动发送，则服务器会返回相应的信息，但是可能不是一次性返回，可能会多次返回。
   3. 本游戏基于前两点，第一点应该不会发生，第二可能发生。
      所以发送socket 请求使用下面：
     	SocketTask task = new SocketTask(ThreadType.MainThread, TaskResponse.Default_Response);

		task.AppendCommonParam(RequestType.SOCK_LOGIN, new SockLoginParam("allen", "password", new string[]{"YY", "LiangZai"}));

		task.ErrorOccured = SocketResp_Error;
		task.afterCompleted = SocketResp_UI;

		task.DispatchToRealHandler();

	  消息会源源不断的到SocketResp_UI这里。当UI退出的时候，即不想在使用Socket服务的时候，请务必使用下面的函数，否则Socket会一直服务中
	  SocketTask task = new SocketTask(ThreadType.MainThread, TaskResponse.Default_Response);

	  task.AppendCmdParam(InternalRequestType.SHUT_DOWN);

	  task.DispatchToRealHandler();

关于新增加socket命令的步骤：
	  1. 在SocketRequestParam里添加一个新的socket请求类A。A继承自BaseRequestParam
	  2. 在SockAllResponse里添加一个新的socket相应类B.B继承自BaseResponse
	  3. 在socketRequestFactory里面添加，请求和响应的的关联。在PreDefined定义好
	  4. 如果需要自己转换返回的响应消息，可以在ActionOnReceiveSockResponse里面的
		    public readonly static Dictionary<RequestType, Action> ACTION_LIST = new Dictionary<RequestType, Action>() {

			};
   创建处理的Action。