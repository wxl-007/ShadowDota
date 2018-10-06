/*
 * 由SharpDevelop创建。
 * 用户： siena
 * 日期: 2014/3/4
 * 时间: 12:16
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using fastJSON;
using System.Net;
using xClient.Action;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using AW.Framework;

namespace SuperSocket.ClientEngine
{
	/// <summary>
	/// Description of NonBlockingConnection.
	/// </summary>
	public class NonBlockingConnection
	{
		private static Encoding DEFAULT_CHARSET = Encoding.GetEncoding("UTF-8");
		private static byte[] Mark = Encoding.GetEncoding("UTF-8").GetBytes("\r\n");
		
		private int port;
		private string hostname;
		private int receiveBufferSize;
		
		private ISystemHandler handler { get; set; }
		
		private AsyncTcpSession session ;
		private InnerEventHandler eHandler;
		
		// 用来存放处理过的Byte数据
		private ConcurrentQueue<ArraySegment<byte>> m_ReceQueue = new ConcurrentQueue<ArraySegment<byte>>();
		
		public NonBlockingConnection(string host, int port, ISystemHandler handler) : this(host, port, 2048, handler){ }
		public NonBlockingConnection(string host, int port, int receiveBufferSize, ISystemHandler handler)
		{
			this.hostname = host;
			this.port = port;
			this.receiveBufferSize = receiveBufferSize;
			this.handler = handler;
			this.eHandler = new InnerEventHandler(handler, this);
			
			// IP解析
			EndPoint e = new IPEndPoint(IPAddress.Parse(this.hostname), this.port);
			session = new AsyncTcpSession(e, this.receiveBufferSize);
			
			// 注册回调事件
			session.Error += this.eHandler.ConnException;
			session.Closed += this.eHandler.ConnDisConnected;
			session.Connected += this.eHandler.ConnConnected;
			session.DataReceived += this.eHandler.DataReceive;
		}

		// 创建连接
		public void Connect() { 
			Utils.Assert(session == null, "NonBlockConnection should be instanced at first");
			session.Connect();
		}

		public void Disconnect() {
			if(session != null) {
				session.Close();
			}
		}

		// 发送数据		
		public bool Write(string message) {
			byte[] data = DEFAULT_CHARSET.GetBytes(message);
			return Write(data, 0, data.Length);			
		}
		
		// 发送数据		
		public bool Write(string message, string encoding) {
			Encoding encode = null;
			if((encoding == null) || encoding.Equals("UTF-8")) {
				encode = DEFAULT_CHARSET;	
			} else {
				encode = Encoding.GetEncoding(encoding);
			}
			
			byte[] data = encode.GetBytes(message);
			return Write(data, 0, data.Length);
		}
		
		// 发送数据		
		public bool Write(byte[] data)
		{
			return Write(data, 0, data.Length);
		}

		// 发送数据		
		public bool Write(byte[] data, int offset, int length) {
			if(session.IsConnected)
			{
				session.Send(data, offset, length);
				return true;
			}
			return false;
		}		
		
		// 读取字符串
		public string ReadString() {
			return ReadString("UTF-8");
		}

		// 读取字符串
		public string ReadString(string encoding) {
			ArraySegment<byte> segment = ReadByteArray();
			if(segment.Count == 0) {
				return null;
			}
			
			return EncodeString(segment, "UTF-8");
		}
		
		private string EncodeString(ArraySegment<byte> data, string encoding) {
			Encoding encode;
			if((encoding == null) || (encoding.Equals("UTF-8"))) {
				encode = DEFAULT_CHARSET;
			} else {
				encode = Encoding.GetEncoding(encoding);
			}
			return encode.GetString(data.Array);			
		}
		
		// 读取字符串数组
		public IList<String> ReadStrings() {
			return ReadStrings("UTF-8");
		}
	
		// 读取字符串数组
		public IList<String> ReadStrings(string encoding) {
			IList<ArraySegment<byte>> temp = ReadByteArrays();
			IList<String> result = new List<String>();
			
			if(temp.Count == 0) {
				return result;
			}
			
			foreach(ArraySegment<byte> arr in temp)
			{
				result.Add(EncodeString(arr, encoding));
			}
			
			return result;
		}
		
		public ArraySegment<byte> ReadByteArray()
		{
			ArraySegment<byte> segment;
            if (!m_ReceQueue.TryDequeue(out segment))
            {
            	return new ArraySegment<byte>();
            }			
            return segment;
		}
		
		public IList<ArraySegment<byte>> ReadByteArrays()
		{
			IList<ArraySegment<byte>> result = new List<ArraySegment<byte>>();
			lock(m_ReceQueue)
			{
				if(m_ReceQueue.IsEmpty)
				{
					return result;
				}
				result = m_ReceQueue.ToArray();
				// 清空队列
				ArraySegment<byte> item;
				while(m_ReceQueue.TryDequeue(out item)){}
			}
			return result;
		}
		
		internal class InnerEventHandler {
			
			private ISystemHandler handler;
			private NonBlockingConnection conn;

			private OctetsStream stream = new OctetsStream(16384);
			//数据缓冲池			
			
			public InnerEventHandler(ISystemHandler handler, NonBlockingConnection conn)
			{
				this.conn = conn;
				this.handler = handler;
			}
			
       		// 处理接受数据
        	public void DataReceive(object sender, DataEventArgs dataEvent)
        	{
        		// 将数据分段，保存到阻塞队列中
        		byte[] data = dataEvent.Data;

				stream.insert(stream.size(), data, dataEvent.Offset, dataEvent.Length);
        		// 分割
        		IList<ArraySegment<byte>> result = stream.getByteArrayByMark(Mark);
        		foreach(ArraySegment<byte> arr in result)
        		{
        			conn.m_ReceQueue.Enqueue(arr);		
        		}
				
        		handler.OnData(conn);
				GC.Collect ();
        	}
        	
        	// 连接断开
        	public void ConnDisConnected(object sender, EventArgs e)
        	{
        		handler.OnDisconnect(conn);
        	}
        	
        	// 连接建立
        	public void ConnConnected(object sender, EventArgs e)
        	{
        		handler.OnConnect(conn);
        	}
        	
        	// 连接异常
        	public void ConnException(object sender, ErrorEventArgs error)
        	{
        		handler.OnException(conn, error.Exception);
        	}
		}

	}
}
