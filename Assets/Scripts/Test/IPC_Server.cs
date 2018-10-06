using UnityEngine;
using System.Collections;
using NetMQ;
using System.Threading;

public class IPC_Server : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ThreadPool.QueueUserWorkItem((o) => (MIPC_Server()));
	}
	
	// Update is called once per frame
	void MIPC_Server () {
		var context = Core.ZeroMQ;
		using (var responseSocket = context.CreateResponseSocket())
		{

			responseSocket.Bind("inproc://Inproc_1323");
			using (var requestSocket1 = context.CreateRequestSocket())
			{

				requestSocket1.Connect("inproc://Inproc_1323");


				var requestSocket2 = context.CreateRequestSocket();
				requestSocket2.Connect("inproc://Inproc_1323");


				
				ConsoleEx.DebugLog("requestSocket1 : Sending 'Hello1'");
				requestSocket1.Send("Hello1");

				ConsoleEx.DebugLog("requestSocket2 : Sending 'Hello2'");
				requestSocket2.Send("Hello2");



				var message = responseSocket.ReceiveString();
				ConsoleEx.DebugLog("responseSocket : Server Received " + message);


				ConsoleEx.DebugLog("responseSocket Sending 'World1'");
				responseSocket.Send("World1");


				var message2 = responseSocket.ReceiveString();
				ConsoleEx.DebugLog("responseSocket : Server Received " + message2);

				ConsoleEx.DebugLog("responseSocket Sending 'World2'");
				responseSocket.Send("World2");


				message = requestSocket1.ReceiveString();
				ConsoleEx.DebugLog("requestSocket1 : Received " + message);
				message = requestSocket2.ReceiveString();
				ConsoleEx.DebugLog("requestSocket2 : Received " + message);
			
			


				requestSocket2.Close();
			}


		}
	}
}
