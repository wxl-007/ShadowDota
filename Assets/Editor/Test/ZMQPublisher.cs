using System;
using System.Threading;
using NetMQ;

namespace AW.Test {

	using NUnit.Framework;

	[TestFixture]
	public class ZMQPublisher  {
		[Test]
		public void Main() {

			ThreadPool.QueueUserWorkItem( (obj) => {

				Random rand = new Random(50);

				using (var context = NetMQContext.Create()) {

					using (var pubSocket = context.CreatePublisherSocket()) {

						ConsoleEx.DebugLog("Publisher socket binding...", ConsoleEx.RED);

						pubSocket.Options.SendHighWatermark = 1000;
						pubSocket.Bind("tcp://localhost:12345");

						for (var i = 0; i < 100; i++) {
							var randomizedTopic = rand.NextDouble();
							if (randomizedTopic > 0.5)
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
					}
				}

			} );


		}

	}
}