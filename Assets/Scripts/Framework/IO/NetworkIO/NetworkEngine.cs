using System;
using System.Net;
using AW.IO;
using AW.FSM;
using AW.Framework;

public class NetworkEngine : IDispose {
    //Http client
    public HttpThread httpEngine;
    //Socket client
	public SocketEngine SockEngine;

    public NetworkEngine() {
        httpEngine = HttpThread.getInstance();
		SockEngine = SocketEngine.getInstance();
    }

	public void Dispose()
    {
        if (httpEngine != null)
        {
            HttpTask shutdownTask = new HttpTask(ThreadType.BackGround, TaskResponse.Default_Response);
            shutdownTask.AppendCmdParam(InternalRequestType.SHUT_DOWN);
            httpEngine.sendHttpTask(shutdownTask);
        }

        // Socket is still empty
		if(SockEngine != null) {
			SocketTask shutdownTask = new SocketTask(ThreadType.BackGround, TaskResponse.Default_Response);
			shutdownTask.AppendCmdParam(InternalRequestType.SHUT_DOWN);
			SockEngine.sendSocketTask(shutdownTask);
		}
    }

}
