using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
/// <summary>
/// Event driven TCP client wrapper
/// </summary>
public class EventDrivenTCPClient : IDisposable
{
	#region Consts/Default values
	const int DEFAULTTIMEOUT = 5000; //Default to 5 seconds on all timeouts
	const int RECONNECTINTERVAL = 2000; //Default to 2 seconds reconnect attempt rate
	#endregion

	#region Components, Events, Delegates, and CTOR
	//Timer used to detect send timeouts
	private Timer socketSendingTimeout = null;
	private Timer SocketReceivingTimeout = null;

	public delegate void delDataReceived(EventDrivenTCPClient sender, object data);
	public event delDataReceived DataReceived;

	public delegate void delConnectionStatusChanged(EventDrivenTCPClient sender, ConnectionStatus status);
	public event delConnectionStatusChanged ConnectionStatusChanged;

	public Action DataSent;

	public Action HostConnected;

	public enum ConnectionStatus
	{
		NeverConnected,
		Connecting,
		Connected,
		AutoReconnecting,
		DisconnectedByUser,
		DisconnectedByHost,
		ConnectFail_Timeout,
		ReceiveFail_Timeout,
		SendFail_Timeout,
		SendFail_NotConnected,
		Error,
		//-- these status is for timeout --
		Sending,
		Receiving,
	}

	public EventDrivenTCPClient(IPAddress ip, int port, bool autoreconnect = true)
	{
		this._IP = ip;
		this._Port = port;
		this._AutoReconnect = autoreconnect;
		this._client = new TcpClient(AddressFamily.InterNetwork);
		this._client.NoDelay = true; //Disable the nagel algorithm for simplicity

		ReceiveTimeout = DEFAULTTIMEOUT;
		SendTimeout = DEFAULTTIMEOUT;
		ConnectTimeout = DEFAULTTIMEOUT;
		ReconnectInterval = RECONNECTINTERVAL;

		//initialize socket timeout
		socketSendingTimeout = new Timer(new TimerCallback(SocketTimer_OnSend), null, Timeout.Infinite, Timeout.Infinite);
		SocketReceivingTimeout = new Timer(new TimerCallback(SocketTimer_OnReceive), null, Timeout.Infinite, Timeout.Infinite);

		ConnectionState = ConnectionStatus.NeverConnected;
	}

	#endregion

	#region Private methods/Event Handlers

	void SocketTimer_OnSend (object status) {
		//we can check ConnectionState to declare what is timeout.
		//Connect time out or send data time out
		if(ConnectionState == ConnectionStatus.Connecting) {
			this.ConnectionState = ConnectionStatus.ConnectFail_Timeout;
			DisconnectByHost();
		} else if(ConnectionState == ConnectionStatus.Sending) {
			this.ConnectionState = ConnectionStatus.SendFail_Timeout;
			DisconnectByHost();
		}
	}

	void SocketTimer_OnReceive (object status) {
		//only for receive time out
		this.ConnectionState = ConnectionStatus.ReceiveFail_Timeout;
		DisconnectByHost();
	}

	private void DisconnectByHost()
	{
		this.ConnectionState = ConnectionStatus.DisconnectedByHost;
		//stop receive timer
		SocketReceivingTimeout.Change(Timeout.Infinite, Timeout.Infinite);
		if (AutoReconnect)
			Reconnect();
	}

	private void Reconnect()
	{
		if (this.ConnectionState == ConnectionStatus.Connected)
			return;

		this.ConnectionState = ConnectionStatus.AutoReconnecting;
		try
		{
			this._client.Client.BeginDisconnect(true, new AsyncCallback(cbDisconnectByHostComplete), this._client.Client);
		}
		catch { }
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Try connecting to the remote host
	/// </summary>
	public void Connect()
	{
		if (this.ConnectionState == ConnectionStatus.Connected)
			return;

		this.ConnectionState = ConnectionStatus.Connecting;

		//starting the connect timer
		socketSendingTimeout.Change(ConnectTimeout, Timeout.Infinite);
		this._client.BeginConnect(this._IP, this._Port, new AsyncCallback(cbConnect), this._client.Client);
	}

	/// <summary>
	/// Try disconnecting from the remote host
	/// </summary>
	public void Disconnect()
	{
		if (this.ConnectionState != ConnectionStatus.Connected)
			return;

		this._client.Client.BeginDisconnect(true, new AsyncCallback(cbDisconnectComplete), this._client.Client);
	}
	/// <summary>
	/// Try sending a string to the remote host
	/// </summary>
	/// <param name="data">The data to send</param>
	public void Send(string data)
	{
		if (this.ConnectionState != ConnectionStatus.Connected)
		{
			this.ConnectionState = ConnectionStatus.SendFail_NotConnected;
			return;
		}

		ConsoleEx.Write("Socket Request is going out : => " + data);
		var bytes = _encode.GetBytes(data);
		SocketError err = new SocketError();
		//send data timer start counting
		socketSendingTimeout.Change(SendTimeout, Timeout.Infinite);
		this._client.Client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, out err, new AsyncCallback(cbSendComplete), this._client.Client);
		if (err != SocketError.Success) {
			Action doDCHost = new Action(DisconnectByHost);
			doDCHost.Invoke();
		} else {
			ConnectionState = ConnectionStatus.Sending;
		}
	}
	/// <summary>
	/// Try sending byte data to the remote host
	/// </summary>
	/// <param name="data">The data to send</param>
	public void Send(byte[] data)
	{
		if (this.ConnectionState != ConnectionStatus.Connected)
			throw new InvalidOperationException("Cannot send data, socket is not connected");

		SocketError err = new SocketError();
		//send data timer start counting
		socketSendingTimeout.Change(SendTimeout, Timeout.Infinite);
		this._client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, out err, new AsyncCallback(cbSendComplete), this._client.Client);
		if (err != SocketError.Success) {
			Action doDCHost = new Action(DisconnectByHost);
			doDCHost.Invoke();
		} else {
			ConnectionState = ConnectionStatus.Sending;
		}
	}

	public void Dispose()
	{
		this._client.Close();
//		this._client.Client.Dispose();

		this.socketSendingTimeout.Change(Timeout.Infinite, Timeout.Infinite);
		this.SocketReceivingTimeout.Change(Timeout.Infinite, Timeout.Infinite);

		this.socketSendingTimeout.Dispose();
		this.SocketReceivingTimeout.Dispose();
	}
	#endregion

	#region Callbacks
	private void cbConnectComplete()
	{
		if (_client.Connected == true)
		{
			//connect timer stop
			socketSendingTimeout.Change(Timeout.Infinite, Timeout.Infinite);

			ConnectionState = ConnectionStatus.Connected;
			this._client.Client.BeginReceive(this.dataBuffer, 0, this.dataBuffer.Length, SocketFlags.None, new AsyncCallback(cbDataReceived), this._client.Client);
		} else {
			ConnectionState = ConnectionStatus.Error;
		}
	}
	private void cbDisconnectByHostComplete(IAsyncResult result)
	{
		var r = result.AsyncState as Socket;
		if (r == null)
			throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");

		r.EndDisconnect(result);
		if (this.AutoReconnect)
		{
			Action doConnect = new Action(Connect);
			doConnect.Invoke();
			return;
		}
	}

	private void cbDisconnectComplete(IAsyncResult result)
	{
		var r = result.AsyncState as Socket;
		if (r == null)
			throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");

		r.EndDisconnect(result);
		this.ConnectionState = ConnectionStatus.DisconnectedByUser;

	}

	private void cbConnect(IAsyncResult result)
	{
		var sock = result.AsyncState as Socket;
		if (result == null)
			throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");

		if (!sock.Connected) {
			if (AutoReconnect) {
				Thread.Sleep(ReconnectInterval);
				Action reconnect = new Action(Connect);
				reconnect.Invoke();
				return;
			} else {
				return;
			}
		}

		sock.EndConnect(result);

		var callBack = new Action(cbConnectComplete);
		callBack.Invoke();
	}

	private void cbSendComplete(IAsyncResult result) {
		var r = result.AsyncState as Socket;
		if (r == null)
			throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a socket object");

		SocketError err = new SocketError();
		r.EndSend(result, out err);
		if (err != SocketError.Success)
		{
			Action doDCHost = new Action(DisconnectByHost);
			doDCHost.Invoke();
		}
		else
		{
			if(DataSent != null) {
				DataSent();
			}

			ConnectionState = ConnectionStatus.Connected;
			//stop send timer
			socketSendingTimeout.Change(Timeout.Infinite, Timeout.Infinite);
		}
	}

	private void cbChangeConnectionStateComplete(IAsyncResult result) {
		var r = result.AsyncState as EventDrivenTCPClient;
		if (r == null)
			throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as a EDTC object");

		r.ConnectionStatusChanged.EndInvoke(result);
	}

	private void cbDataReceived(IAsyncResult result) {
		var sock = result.AsyncState as Socket;

		if (sock == null)
			throw new InvalidOperationException("Invalid IASyncResult - Could not interpret as a socket");
		SocketError err = new SocketError();
		int bytes = sock.EndReceive(result, out err);
		if (bytes == 0 || err != SocketError.Success)
		{
			lock (SyncLock)
			{
				SocketReceivingTimeout.Change(ReceiveTimeout, Timeout.Infinite);
				return;
			}
		}
		else
		{
			lock (SyncLock)
			{
				SocketReceivingTimeout.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		if (DataReceived != null)
			DataReceived.BeginInvoke(this, _encode.GetString(dataBuffer, 0, bytes), new AsyncCallback(cbDataRecievedCallbackComplete), this);
	}

	private void cbDataRecievedCallbackComplete(IAsyncResult result)
	{
		var r = result.AsyncState as EventDrivenTCPClient;
		if (r == null)
			throw new InvalidOperationException("Invalid IAsyncResult - Could not interpret as EDTC object");

		r.DataReceived.EndInvoke(result);
		SocketError err = new SocketError();
		this._client.Client.BeginReceive(this.dataBuffer, 0, this.dataBuffer.Length, SocketFlags.None, out err, new AsyncCallback(cbDataReceived), this._client.Client);
		if (err != SocketError.Success)
		{
			Action doDCHost = new Action(DisconnectByHost);
			doDCHost.Invoke();
		}
	}
	#endregion

	#region Properties and members
	private IPAddress _IP = IPAddress.None;
	private ConnectionStatus _ConStat;
	private TcpClient _client;
	private byte[] dataBuffer = new byte[5000];
	private bool _AutoReconnect = false;
	private int _Port = 0;
	private Encoding _encode = Encoding.Default;
	object _SyncLock = new object();
	/// <summary>
	/// Syncronizing object for asyncronous operations
	/// </summary>
	public object SyncLock
	{
		get
		{
			return _SyncLock;
		}
	}
	/// <summary>
	/// Encoding to use for sending and receiving
	/// </summary>
	public Encoding DataEncoding
	{
		get
		{
			return _encode;
		}
		set
		{
			_encode = value;
		}
	}
	/// <summary>
	/// Current state that the connection is in
	/// </summary>
	public ConnectionStatus ConnectionState
	{
		get
		{
			return _ConStat;
		}
		private set
		{
			bool raiseEvent = value != _ConStat;
			_ConStat = value;
			if (ConnectionStatusChanged != null && raiseEvent)
				ConnectionStatusChanged.BeginInvoke(this, _ConStat, new AsyncCallback(cbChangeConnectionStateComplete), this);
		}
	}
	/// <summary>
	/// True to autoreconnect at the given reconnection interval after a remote host closes the connection
	/// </summary>
	public bool AutoReconnect
	{
		get
		{
			return _AutoReconnect;
		}
		set
		{
			_AutoReconnect = value;
		}
	}
	public int ReconnectInterval { get; set; }
	/// <summary>
	/// IP of the remote host
	/// </summary>
	public IPAddress IP
	{
		get
		{
			return _IP;
		}
	}
	/// <summary>
	/// Port to connect to on the remote host
	/// </summary>
	public int Port
	{
		get
		{
			return _Port;
		}
	}
	/// <summary>
	/// Time to wait after a receive operation is attempted before a timeout event occurs
	/// </summary>
	private int _receiveTimeout;
	public int ReceiveTimeout
	{
		get { return _receiveTimeout; }
		set { _receiveTimeout = value; }
	}
	/// <summary>
	/// Time to wait after a send operation is attempted before a timeout event occurs
	/// </summary>
	private int _sendTimeout;
	public int SendTimeout
	{
		get { return _sendTimeout; }
		set { _sendTimeout = value; }
	}
	/// <summary>
	/// Time to wait after a connection is attempted before a timeout event occurs
	/// </summary>
	private int _connectTimeout;
	public int ConnectTimeout
	{
		get { return _connectTimeout; }
		set { _connectTimeout = value; }
	}
	#endregion       
}