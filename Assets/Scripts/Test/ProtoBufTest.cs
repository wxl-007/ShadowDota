using UnityEngine;
using System.Collections;
using ProtoBuf.Meta;
using ProtoBuf;
using AW.War;
using System.Text;

public class ProtoBufTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		protobufTest();
	}
	
	// Update is called once per frame
	void protobufTest () {
		IpcCreateMapMsg msg = new IpcCreateMapMsg() {
			MapId = 10,
		};

		byte[] outBytes = ProtoLoader.serializeProtoObject<IpcMsg>(msg);
		StringBuilder sb = new StringBuilder();
		sb.Append("Out bytes = ");

		int cnt = outBytes.Length;
		for(int i = 0; i < cnt; ++ i)
			sb.Append(" ").Append(outBytes[i].ToString());

		ConsoleEx.DebugLog(sb.ToString());


		IpcCreateMapMsg deser = ProtoLoader.deserializeProtoObj<IpcCreateMapMsg>(outBytes);
		ConsoleEx.DebugLog("Map Id = " + deser.MapId);
	}
}
