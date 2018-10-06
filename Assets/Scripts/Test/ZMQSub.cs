using UnityEngine;
using System;
using NetMQ;
using System.Threading;

namespace AW.Test {
	using NetMQ.Sockets;

	public class ZMQSub : MonoBehaviour {
		private SubscriberSocket subSocket;
		// Use this for initialization
		void Start () {
			Invoke("SubscriberAll", 2f);
			//SubscriberA();
		}

		public void SubscriberAll() {

			string topic = "";
			ConsoleEx.DebugLog("Subscriber started for Topic : All", ConsoleEx.YELLOW);

			ThreadPool.QueueUserWorkItem( (obj) => {
				using (var context = NetMQContext.Create())
				using (subSocket = context.CreateSubscriberSocket()) {

					subSocket.Options.ReceiveHighWatermark = 1000;
					subSocket.Connect("tcp://127.0.0.1:52323");
					subSocket.Subscribe(topic);
					ConsoleEx.DebugLog("Subscriber socket connecting...", ConsoleEx.YELLOW);

					int i = 0;
					while (i < 2) {
						string messageTopicReceived = subSocket.ReceiveString();
						string messageReceived = subSocket.ReceiveString();
						ConsoleEx.DebugLog ("Topic : " + messageTopicReceived + ". Content : " + messageReceived, ConsoleEx.YELLOW);
						++ i;
					}
				}


				ConsoleEx.DebugLog("Job is over.", ConsoleEx.YELLOW);
			});

		}


		public void SubscriberA() {

			string topic = "TopicA";
			ConsoleEx.DebugLog("Subscriber started for Topic : " + topic, ConsoleEx.YELLOW);

			ThreadPool.QueueUserWorkItem( (obj) => {

				using (var context = NetMQContext.Create())
				using (var subSocket = context.CreateSubscriberSocket()) {

					subSocket.Options.ReceiveHighWatermark = 1000;
					subSocket.Connect("tcp://127.0.0.1:12345");
					subSocket.Subscribe(topic);
					ConsoleEx.DebugLog("Subscriber socket connecting...", ConsoleEx.YELLOW);

					int i = 0;
					while (i < 2) {
						string messageTopicReceived = subSocket.ReceiveString();
						string messageReceived = subSocket.ReceiveString();
						ConsoleEx.DebugLog ("Topic : " + messageTopicReceived + ". Content : " + messageReceived, ConsoleEx.YELLOW);
						++ i;
					}

				}

			});
		}



		void OnApplicationQuit() {
			if(subSocket != null) subSocket.Close();
		}

	}

}

