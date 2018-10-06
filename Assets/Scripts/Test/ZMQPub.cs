using UnityEngine;
using System.Collections;
using System.Threading;
using NetMQ;
using AW.Framework;


namespace AW.Test {
	using NetMQ.Sockets;

	public class ZMQPub : MonoBehaviour {
		private PublisherSocket pubSocket;
		private bool quite = false;
		void Start() {

			PseudoRandom rand = PseudoRandom.getInstance();

			ThreadPool.QueueUserWorkItem( (obj) => {

				using (var context = NetMQContext.Create())
				using (pubSocket = context.CreatePublisherSocket()) {

					ConsoleEx.DebugLog("Publisher socket binding...", ConsoleEx.RED);

					pubSocket.Options.SendHighWatermark = 1000;
					pubSocket.Bind("tcp://*:52323");

					for (var i = 0; i < 50; i++) {
						if(quite) break;

						var randomizedTopic = rand.next(1000);
						if (randomizedTopic > 50)
						{
							var msg = "TopicA msg-" + i;
							ConsoleEx.DebugLog("Sending message : " + msg, ConsoleEx.RED);
							pubSocket.SendMore("TopicA").Send(msg);
						}
						else
						{
							var msg = "TopicB msg-" + i;
							ConsoleEx.DebugLog("Sending message : " + msg, ConsoleEx.RED);
							pubSocket.SendMore("TopicB").Send(msg);
						}

						Thread.Sleep(500);
					}
					ConsoleEx.DebugLog("Publisher job is down.", ConsoleEx.RED);
				}

			} );

		}


		void OnApplicationQuit() {
			quite = true;
			if(pubSocket != null) pubSocket.Close();
		}

	}

}
