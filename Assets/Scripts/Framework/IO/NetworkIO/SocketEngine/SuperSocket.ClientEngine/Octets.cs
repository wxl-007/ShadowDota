using System;
using System.Collections.Generic;
using System.Text;

namespace SuperSocket.ClientEngine
{
    public class Octets : ICloneable, IComparable
    {
        private static readonly int DEFAULT_SIZE = 128;
        private static Encoding DEFAULT_CHARSET = Encoding.GetEncoding("UTF-8");
        
        private int count = 0;
        private byte[] buffer = null;

        private byte[] roundup(int size)
        {
            int capacity = 16;
            while (size > capacity)
            {
            	capacity <<= 1;
            }
            return new byte[capacity];
        }

        public void reserve(int size)
        {
            if (buffer == null)
            {
                buffer = roundup(size);
            }
            else if (size > buffer.Length)
            {
                byte[] tmp = roundup(size);
                Array.Copy(buffer, 0, tmp, 0, count);
                buffer = tmp;
            }
        }

        public Octets replace(byte[] data, int pos, int size)
        {
            reserve(size);
            Array.Copy(data, pos, buffer, 0, size);
            count = size;
            return this;
        }

        public Octets replace(Octets data, int pos, int size)
        {
            return replace(data.buffer, pos, size);
        }

        public Octets replace(byte[] data)
        {
            return replace(data, 0, data.Length);
        }

        public Octets replace(Octets data)
        {
            return replace(data.buffer, 0, data.count);
        }

        public Octets()
        {
            reserve(DEFAULT_SIZE);
        }

        public Octets(int size)
        {
            reserve(size);
        }

        public Octets(Octets rhs)
        {
            replace(rhs);
        }

        public Octets(byte[] rhs)
        {
            replace(rhs);
        }

        public Octets(String str)
        {
            replace(DEFAULT_CHARSET.GetBytes(str));
        }

        private Octets(byte[] bytes, int length)
        {
            this.buffer = bytes;
            this.count = length;
        }

        public static Octets wrap(byte[] bytes, int length)
        {
            return new Octets(bytes, length);
        }

        public static Octets wrap(byte[] bytes)
        {
            return wrap(bytes, bytes.Length);
        }

        public static Octets wrap(String str, String encoding)
        {
            try
            {
                return wrap(Encoding.GetEncoding(encoding).GetBytes(str));
            }
            catch (ArgumentException x)
            {
                throw x;
            }
        }

        public Octets(byte[] rhs, int pos, int size)
        {
            replace(rhs, pos, size);
        }

        public Octets(Octets rhs, int pos, int size)
        {
            replace(rhs, pos, size);
        }

        public Octets resize(int size)
        {
            reserve(size);
            count = size;
            return this;
        }

        public int size() { return count; }
        public int capacity() { return buffer.Length; }
        public Octets clear() { count = 0; return this; }

        public Octets swap(Octets rhs)
        {
            int size = count; count = rhs.count; rhs.count = size;
            byte[] tmp = rhs.buffer; rhs.buffer = buffer; buffer = tmp;
            return this;
        }

        public Octets push_back(byte data)
        {
            reserve(count + 1);
            buffer[count++] = data;
            return this;
        }

        public Octets erase(int from, int to)
        {
            Array.Copy(buffer, to, buffer, from, count - to);
            count -= (to - from);
            return this;
        }

        public Octets insert(int from, byte[] data, int pos, int size)
        {
            reserve(count + size);
            Array.Copy(buffer, from, buffer, from + size, count - from);
            Array.Copy(data, pos, buffer, from, size);
            count += size;
            return this;

        }

        public Octets insert(int from, Octets data, int pos, int size)
        {
            return insert(from, data.buffer, pos, size);
        }

        public Octets insert(int from, byte[] data)
        {
            return insert(from, data, 0, data.Length);
        }

        public Octets insert(int from, Octets data)
        {
            return insert(from, data.buffer, 0, data.size());
        }

        public Object Clone()
        {
            return new Octets(this);
        }

        public int CompareTo(Octets rhs)
        {
            int c = count - rhs.count;
            if (c != 0) return c;

            byte[] v1 = buffer;
            byte[] v2 = rhs.buffer;
            for (int i = 0; i < count; i++)
            {
                int v = v1[i] - v2[i];
                if (v != 0)
                {
                    return v;
                }
            }
            return 0;
        }

        public int CompareTo(Object o)
        {
            return CompareTo((Octets)o);
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            return CompareTo(o) == 0;
        }

        public override int GetHashCode()
        {
            if (buffer == null)
                return 0;

            int result = 1;
            for (int i = 0; i < count; i++)
                result = 31 * result + buffer[i];

            return result;
        }

        public override String ToString()
        {
            return "octets.size=" + count;
        }

        public byte[] getBytes()
        {
            byte[] tmp = new byte[count];
            Array.Copy(buffer, 0, tmp, 0, count);
            return tmp;
        }

        public byte[] array()
        {
            return buffer;
        }

        public byte getByte(int pos)
        {
            return buffer[pos];
        }

        public void setByte(int pos, byte b)
        {
            buffer[pos] = b;
        }

        public String getString(int pos, int len)
        {
            return DEFAULT_CHARSET.GetString(buffer, pos, len);
        }

        public String getString()
        {
            return getString(0, count);
        }

        public String getString(int pos, int len, String encoding)
        {
            try
            {
                return Encoding.GetEncoding(encoding).GetString(buffer, pos, len);
            }
            catch (SystemException x)
            {
                throw new SystemException(x.Message);
            }
        }

        public String getString(String encoding)
        {
            return getString(0, count, encoding);
        }

        public void setString(String str)
        {
            buffer = DEFAULT_CHARSET.GetBytes(str);
            count = buffer.Length;
        }
        
        public void setString(String str, String encoding)
        {
            Encoding en = Encoding.GetEncoding(encoding);
            buffer = en.GetBytes(str);
            count = buffer.Length;
        }

        public void dump()
        {
            for (int i = 0; i < size(); i++)
            {
                Console.Write(buffer[i] + " ");
            }
            ConsoleEx.DebugLog("");
        }
		
        static public void setDefaultCharset(String name)
        {
            DEFAULT_CHARSET = Encoding.GetEncoding(name);
        }
    }
}