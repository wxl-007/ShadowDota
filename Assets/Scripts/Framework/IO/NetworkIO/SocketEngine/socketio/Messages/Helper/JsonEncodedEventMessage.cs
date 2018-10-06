using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
    public class JsonEncodedEventMessage
    {
         public string name { get; set; }

         public object[] args { get; set; }

        public JsonEncodedEventMessage()
        {
        }
        
		public JsonEncodedEventMessage(string name, object payload) : this(name, new[]{payload})
        {

        }
        
		public JsonEncodedEventMessage(string name, object[] payloads)
        {
            this.name = name;
            this.args = payloads;
        }

        public T GetFirstArgAs<T>()
        {
            try
            {
                var firstArg = this.args.FirstOrDefault();
                if (firstArg != null)
					return fastJSON.JSON.Instance.ToObject<T>(firstArg.ToString());
            }
            catch (Exception ex)
            {
                // add error logging here
				Console.WriteLine(ex.ToString());
                throw;
            }
            return default(T);
        }
        public IEnumerable<T> GetArgsAs<T>()
        {
            List<T> items = new List<T>();
            foreach (var i in this.args)
            {
				items.Add( fastJSON.JSON.Instance.ToObject<T>(i.ToString()) );
            }
            return items.AsEnumerable();
        }

        public string ToJsonString()
        {
			return fastJSON.JSON.Instance.ToJSON(this);
        }

        public static JsonEncodedEventMessage Deserialize(string jsonString)
        {
			JsonEncodedEventMessage msg = null;
			try { msg = fastJSON.JSON.Instance.ToObject<JsonEncodedEventMessage>(jsonString); }
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
            return msg;
        }
    }
}
