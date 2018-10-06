using UnityEngine;
using System.Collections;
using NetMQ;
using System.Threading;

public class ZMQAll : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Main();
	}
	
	static void Main()
	{

		ThreadPool.QueueUserWorkItem( (o) => {
			using(var context = NetMQContext.Create())
			{
				using (var responseSocket = context.CreateResponseSocket())
				{
					responseSocket.Bind("inproc://Inproc_52387");

					using (var requestSocket = context.CreateRequestSocket())
					{
						requestSocket.Connect("inproc://Inproc_52387");
						ConsoleEx.DebugLog("requestSocket : Sending 'Hello'");
						requestSocket.Send("Hello");

						var message = responseSocket.ReceiveString();

						ConsoleEx.DebugLog("responseSocket : Server Received '{0}'", message);

						ConsoleEx.DebugLog("responseSocket Sending 'World'");
						responseSocket.Send("World");

						message = requestSocket.ReceiveString();
						ConsoleEx.DebugLog("requestSocket : Received '{0}'", message);

					}

				}

				ConsoleEx.DebugLog("OK", ConsoleEx.YELLOW);
			}
			ConsoleEx.DebugLog("Termian OK", ConsoleEx.YELLOW);
		});

	}
}
