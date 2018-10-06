using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace xClient.Common
{
    using SectMap = Dictionary<String, String>;
    using ConfMap = Dictionary<String, Dictionary<String, String>>;

    public class Conf
    {
        private static readonly Conf instance 	= new Conf();
        private static readonly Object conflock = new Object();

	    private FileInfo conffile; 
	    private DateTime mtime;
	    private ConfMap confhash;
	    private String charset = "UTF-8";

        private Conf() { confhash = new ConfMap(); }
        
        private void parse(StreamReader sr)
	    {
		    String  section = null;
            SectMap sechash = new SectMap();
		    confhash.Clear();

            String line;
            while (sr.Peek() >= 0)
		    {
                line = sr.ReadLine().Trim();
			    if (line.Length == 0) continue;
			    Char c = line[0];
			    if (c == '#' || c == ';') continue;
			
			    if (c == '[')
			    {
				    line = line.Substring(1, line.IndexOf(']')-1).Trim();
				    if (section != null)
				    {
					    confhash[section] = sechash;
                        sechash = new SectMap();
				    }
				    section = line;
			    }
			    else
			    {
				    String[] key_value = line.Split("=".ToCharArray(), 2);
				    sechash[key_value[0].Trim()] = key_value[1].Trim();
			    }
		    }
		    if (section != null)
			    confhash[section] = sechash;
	    }

        private void reload()
        {
            try
            {
                for (DateTime last = conffile.LastWriteTime; last != mtime; last = conffile.LastWriteTime)
                {
                    mtime = last;
                    StreamReader sr = new StreamReader(conffile.FullName, Encoding.GetEncoding(charset));
                    parse(sr);
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                ConsoleEx.DebugLog(e.StackTrace);
            }
        }

	    public String find(String section, String key)
	    {
            lock (conflock)
            {
                SectMap sechash;
                if (confhash.TryGetValue(section, out sechash))
                {
                    String val = "";
                    if (sechash.TryGetValue(key, out val))
                    {
                        return String.Copy(val);
                    }
                }
                return "";
            }
	    }
	    
	    public String find(String section, String key, String defValue)
	    {
	    	String skValue = find(section, key);
	    	if((skValue == null) || (skValue.Trim().Equals("")))
	    	{
	    		return defValue;
	    	}
	    	return skValue;
	    }	    

        public void put(String section, String key, String val)
        {
            lock (conflock)
            {
                SectMap sechash;
                if (!confhash.TryGetValue(section, out sechash))
                {
                    sechash = new SectMap();
                    confhash[section] = sechash;
                }
                sechash[key] = val;
            }
        }

        public static Conf Get(String filename, String charset)
        {
            lock (conflock)
            {
                if (charset != null && charset != "")
                {
                    instance.charset = charset;
                }
                FileInfo file = new FileInfo(filename);
                if (file.Exists)
                {
                    instance.conffile = file;
                    instance.reload();
                }
                return instance;
            }
        }
		
        public static Conf Get(String filename)
        {
        	return Get(filename , null);
        }
                               
        public static Conf Get() { return instance; }
    }
}
