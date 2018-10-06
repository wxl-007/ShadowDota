using System;
using NetMQ;
using System.Threading;

namespace AW.Test {
	using NUnit.Framework;

	[TestFixture]
	class ZMQSubscriber {

		[Test]
		public void SubscriberAll() {

			string topic = "";
			ConsoleEx.DebugLog("Subscriber started for Topic : All", ConsoleEx.YELLOW);

			ThreadPool.QueueUserWorkItem( (obj) => {
				using (var context = NetMQContext.Create()) {

					using (var subSocket = context.CreateSubscriberSocket()) {

						subSocket.Options.ReceiveHighWatermark = 1000;
						subSocket.Connect("tcp://localhost:12345");
						subSocket.Subscribe(topic);
						ConsoleEx.DebugLog("Subscriber socket connecting...", ConsoleEx.YELLOW);

						int i = 0;
						while (i < 100) {
							string messageTopicReceived = subSocket.ReceiveString();
							string messageReceived = subSocket.ReceiveString();
							ConsoleEx.DebugLog ("Topic : " + messageTopicReceived + ". Content : " + messageReceived, ConsoleEx.YELLOW);
							++ i;
						}
					}
				}
			});

		}



		[Test]
		public void SubscriberA() {

			string topic = "TopicA";
			ConsoleEx.DebugLog("Subscriber started for Topic : " + topic, ConsoleEx.YELLOW);

			ThreadPool.QueueUserWorkItem( (obj) => {
				using (var context = NetMQContext.Create()) {

					using (var subSocket = context.CreateSubscriberSocket()) {

						subSocket.Options.ReceiveHighWatermark = 1000;
						subSocket.Connect("tcp://localhost:12345");
						subSocket.Subscribe(topic);
						ConsoleEx.DebugLog("Subscriber socket connecting...", ConsoleEx.YELLOW);

						int i = 0;
						while (i < 100) {
							string messageTopicReceived = subSocket.ReceiveString();
							string messageReceived = subSocket.ReceiveString();
							ConsoleEx.DebugLog ("Topic : " + messageTopicReceived + ". Content : " + messageReceived, ConsoleEx.YELLOW);
							++ i;
						}

					}

				}
			});
		}
	}


}

