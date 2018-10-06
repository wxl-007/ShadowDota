using System;
using ProtoBuf.Meta;
using System.IO;
using AW.War;

namespace ProtoBuf {
	public static class ProtoLoader {
		//We need to make a reference to the MsgSerialize that 
		//wil be handling all of the data
		private static MsgSerialize mSerialize = new MsgSerialize();

		//This function is actually useful for sending objects over an RPC function. Unity allows you to send a byte[]  
		//through an RPC even though its not documented, so here you can serialize object data, 
		//send it over the network and then deserialize the object on the other end.
		public static byte[] serializeProtoObject<T>(T obj) where T : IpcMsg
		{
			using(MemoryStream m = new MemoryStream())
			{
				mSerialize.Serialize(m, obj);
				return m.ToArray();
			}
		}


		//our first function will load an object from resources. 
		//This is useful for if you had an editor script that created data that would be stored for later usage in the game, 
		//and you had saved it with the .bytes extension
		public static T deserializeProtoObj<T>(byte[] bytes) where T : IpcMsg {
			T deserializedObj = default(T);

			//basically we just load the bytes of the text asset into a memory stream, and the deserialize it from there
			using(MemoryStream m = new MemoryStream(bytes)) {
				deserializedObj = (T)mSerialize.Deserialize(m, null, typeof(T));
			}

			return deserializedObj;
		}

		public static IpcMsg deserializeProtoObj(byte[] bytes, Type type) {
			object deserializedObj = null;
			//basically we just load the bytes of the text asset into a memory stream, and the deserialize it from there
			using(MemoryStream m = new MemoryStream(bytes)) {
				deserializedObj = mSerialize.Deserialize(m, null, type);
			}

			return (IpcMsg)deserializedObj;
		}


	}
}

