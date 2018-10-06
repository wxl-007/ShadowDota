using System;
using fastJSON;
using xClient.Action;
using System.Collections.Generic;
using SuperSocket.ClientEngine;
using ActProtocol = xClient.Action.Protocol;

internal class JavaProtocolHandler : ISystemHandler {

	public bool OnConnect (NonBlockingConnection conn)
	{
		ConsoleEx.DebugLog("Collect to the Server    java  ");
		return true;
	}

	public bool OnData (NonBlockingConnection conn)
	{
		IList<string> json = conn.ReadStrings();
		if((json == null) || (json.Count == 0)) {
			return true;
		}

		foreach(string s in json) {
			ActProtocol p = JSON.Instance.ToObject<ActProtocol>(s);
			Dispatch.Instance.DispatchAct(p, conn);
		}
		return true;
	}

	public bool OnDisconnect (NonBlockingConnection conn)
	{
		ConsoleEx.DebugLog("DisCollect from the Server.");
		return true;
	}

	public bool OnException (NonBlockingConnection conn, Exception e)
	{
		ConsoleEx.DebugLog("Exception happen exception :: " + e.ToString());
		return true;
	}
}