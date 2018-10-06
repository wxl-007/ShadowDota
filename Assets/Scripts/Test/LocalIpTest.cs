using UnityEngine;
using System.Collections;
using AW.IO;
using System.Net;

public class LocalIpTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		IPAddress ip = LocalIp.localAddress;
		ConsoleEx.DebugLog(ip.ToString());
	}
	
}
