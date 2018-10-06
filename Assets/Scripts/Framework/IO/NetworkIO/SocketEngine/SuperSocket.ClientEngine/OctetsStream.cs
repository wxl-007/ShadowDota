using System;
using System.Collections.Generic;
using System.Text;

namespace SuperSocket.ClientEngine
{
    public class OctetsStream : Octets
    {
		private static readonly int MAXSPARE = 16380;
        
	    private int pos = 0;
	    private int tranpos = 0;

	    public OctetsStream (){}
        public OctetsStream(int size) : base(size){}
        public OctetsStream(Octets o) : base(o){}

        public static OctetsStream wrap(Octets o)
        {
            OctetsStream os = new OctetsStream();
            os.swap(o);
            return os;
        }

        public new Object Clone()
        {
            OctetsStream os = (OctetsStream)base.Clone();
            os.pos = pos;
            os.tranpos = pos;
            return os;
        }

        public bool eos()
        {
            return pos == size();
        }

        public int position(int pos)
        {
            this.pos = pos;
            return this.pos;
        }

        public int position()
        {
            return pos;
        }

        public int remain()
        {
            return size() - pos;
        }

        public OctetsStream marshal(byte x)
        {
            push_back(x);
            return this;
        }

        public OctetsStream marshal(bool b)
        {
            push_back((byte)(b ? 1 : 0));
            return this;
        }

        public OctetsStream marshal(short x)
        {
            return
                marshal((byte)(x >> 8)).
                marshal((byte)(x));
        }

        public OctetsStream marshal(char x)
        {
            return
                marshal((byte)(x >> 8)).
                marshal((byte)(x));
        }

        public OctetsStream marshal(int x)
        {
            return
                marshal((byte)(x >> 24)).
                marshal((byte)(x >> 16)).
                marshal((byte)(x >> 8)).
                marshal((byte)(x));
        }

        public OctetsStream marshal(long x)
        {
            return
                marshal((byte)(x >> 56)).
                marshal((byte)(x >> 48)).
                marshal((byte)(x >> 40)).
                marshal((byte)(x >> 32)).
                marshal((byte)(x >> 24)).
                marshal((byte)(x >> 16)).
                marshal((byte)(x >> 8)).
                marshal((byte)(x));
        }

        public OctetsStream marshal(float x)
        {
            return marshal(BitConverter.ToInt32(BitConverter.GetBytes(x), 0));
        }

        public OctetsStream marshal(double x)
        {
            return marshal(BitConverter.ToInt64(BitConverter.GetBytes(x), 0));
        }

        public OctetsStream compact_uint32(int x)
        {
            if (x < 0x40) 
            	return marshal((byte)x);
            else if (x < 0x4000) 
            	return marshal((short)(x | 0x8000));
            else if (x < 0x20000000) 
            	return marshal((int)((uint)x | 0xc0000000));
            
            marshal((byte)0xe0);
            return marshal(x);
        }

        public OctetsStream compact_sint32(int x)
        {
            if (x >= 0)
            {
                if (x < 0x40) return marshal((byte)x);
                else if (x < 0x2000) return marshal((short)(x | 0x8000));
                else if (x < 0x10000000) return marshal((int)((uint)x | 0xc0000000));
                marshal((byte)0xe0);
                return marshal(x);
            }
            if (-x > 0)
            {
                x = -x;
                if (x < 0x40) return marshal((byte)(x | 0x40));
                else if (x < 0x2000) return marshal((short)(x | 0xa000));
                else if (x < 0x10000000) return marshal((int)((uint)x | 0xd0000000));
                marshal((byte)0xf0);
                return marshal(x);
            }
            marshal((byte)0xf0);
            return marshal(x);
        }

        public OctetsStream marshal(byte[] bytes)
        {
            compact_uint32(bytes.Length);
            insert(size(), bytes);
            return this;
        }

        public OctetsStream marshal(Octets o)
        {
            compact_uint32(o.size());
            insert(size(), o);
            return this;
        }
        public OctetsStream marshal(String str)
        {
            return marshal(str, null);
        }

        public OctetsStream marshal(String str, String charset)
        {
            try
            {
                marshal((charset == null) ? Encoding.Default.GetBytes(str) : Encoding.GetEncoding(charset).GetBytes(str));
            }
            catch (Exception e)
            {
                throw new SystemException(e.Message);
            }
            return this;
        }

        public byte unmarshal_byte()
        {
            if (pos + 1 > size()) 
            	throw new DragonException();
            
            return getByte(pos++);
        }

        public bool unmarshal_boolean()
        {
            return unmarshal_byte() == 1;
        }

        public short unmarshal_short()
        {
            if (pos + 2 > size()) 
            	throw new DragonException();
            
            byte b0 = getByte(pos++);
            byte b1 = getByte(pos++);
            return (short)(((b0 & 0xff) << 8) | (b1 & 0xff));
        }

        public int unmarshal_int()
        {
            if (pos + 4 > size()) throw new DragonException();
            byte b0 = getByte(pos++);
            byte b1 = getByte(pos++);
            byte b2 = getByte(pos++);
            byte b3 = getByte(pos++);
            return (int)((
                ((b0 & 0xff) << 24) |
                ((b1 & 0xff) << 16) |
                ((b2 & 0xff) << 8) |
                ((b3 & 0xff) << 0)));
        }

        public long unmarshal_long()
        {
            if (pos + 8 > size()) throw new DragonException();
            byte b0 = getByte(pos++);
            byte b1 = getByte(pos++);
            byte b2 = getByte(pos++);
            byte b3 = getByte(pos++);
            byte b4 = getByte(pos++);
            byte b5 = getByte(pos++);
            byte b6 = getByte(pos++);
            byte b7 = getByte(pos++);
            return ((((long)b0 & 0xff) << 56) |
                (((long)b1 & 0xff) << 48) |
                (((long)b2 & 0xff) << 40) |
                (((long)b3 & 0xff) << 32) |
                (((long)b4 & 0xff) << 24) |
                (((long)b5 & 0xff) << 16) |
                (((long)b6 & 0xff) << 8) |
                (((long)b7 & 0xff) << 0));
        }

        public float unmarshal_float()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(unmarshal_int()), 0);
        }

        public double unmarshal_double()
        {
            return BitConverter.ToDouble(BitConverter.GetBytes(unmarshal_long()), 0);
        }

        public int uncompact_uint32()
        {
            if (pos == size()) throw new DragonException();
            switch (getByte(pos) & 0xe0)
            {
                case 0xe0:
                    unmarshal_byte();
                    return unmarshal_int();
                case 0xc0:
                    return unmarshal_int() & (int)~0xc0000000;
                case 0xa0:
                case 0x80:
                    return (ushort)unmarshal_short() & ~0x8000;
            }
            return unmarshal_byte();
        }

        public int uncompact_sint32()
        {
            if (pos == size()) throw new DragonException();
            switch (getByte(pos) & 0xf0)
            {
                case 0xf0:
                    unmarshal_byte();
                    return -unmarshal_int();
                case 0xe0:
                    unmarshal_byte();
                    return unmarshal_int();
                case 0xd0:
                    return (int)(-(unmarshal_int() & ~0xd0000000));
                case 0xc0:
                    return (int)(unmarshal_int() & ~0xc0000000);
                case 0xb0:
                case 0xa0:
                    return -((ushort)unmarshal_short() & ~0xa000);
                case 0x90:
                case 0x80:
                    return (ushort)unmarshal_short() & ~0x8000;
                case 0x70:
                case 0x60:
                case 0x50:
                case 0x40:
                    return -(unmarshal_byte() & ~0x40);
            }
            return unmarshal_byte();
        }

        public Octets unmarshal_Octets()
        {
            int size = uncompact_uint32();
            
            if (pos + size > this.size())
            	throw new DragonException();
            
            Octets o = new Octets(this, pos, size);
            pos += size;
            return o;
        }
        
		public Octets unmarshal_Octets(int size, int marksize)
        {
			if ((pos + size + marksize) > this.size())
            	throw new DragonException();
            
            Octets o = new Octets(this, pos, size);
            pos += size;
            pos += marksize;
            return o;        	
        }
                
        public byte[] unmarshal_bytes()
        {
            int size = uncompact_uint32();
            if (pos + size > this.size()) throw new DragonException();
            byte[] copy = new byte[size];
            Array.Copy(array(), pos, copy, 0, size);
            pos += size;
            return copy;
        }

        public OctetsStream unmarshal(Octets os)
        {
            int size = uncompact_uint32();
            if (pos + size > this.size()) throw new DragonException();
            os.replace(this, pos, size);
            pos += size;
            return this;
        }
        
        public String unmarshal_String()
        {
            return unmarshal_String(null);
        }
        
        public String unmarshal_String(String charset)
        {
            try
            {
                int size = uncompact_uint32();
                if (pos + size > this.size())
                {
                	throw new DragonException();
                }
                
                int cur = pos;
                pos += size;
                return (charset == null) ? String.Copy(getString(cur, size)) : String.Copy(getString(cur, size, charset));
            }
            catch (Exception e)
            {
                throw new SystemException(e.Message);
            }
        }
        
        public IList<ArraySegment<byte>> getByteArrayByMark(byte[] mark)
		{
        	IList<ArraySegment<byte>> result = new List<ArraySegment<byte>>();
        	int marksize = mark.Length;
        	
        	Begin();
        	/*lock(this)
        	{*/
        		int index = -1; 
        		int byteSize = 0;
			try {
        		do{

					index = Extensions.SearchMark(this.array(), this.pos, this.size(), mark);
			
        			byteSize = index - this.pos;
				
        			if(index != -1)
        			{
        				if(byteSize <= 0)
        				{
        					break;
        				}
        				else
        				{			
        					Octets o = this.unmarshal_Octets(byteSize, marksize);					
        					result.Add(new ArraySegment<byte>(o.getBytes()));
        				}
        			}
        			
        		}while(index > 0);
			}catch(Exception ex) {
				ConsoleEx.DebugLog (ex.ToString());
			}
			//}
        	Commit();
			return result;
        }
        
        // 开始获取数据
        public OctetsStream Begin()
        { 
        	tranpos = pos; 
        	return this;
        }
        
        // 回滚
        public OctetsStream Rollback() 
        { 
        	pos = tranpos; 
        	return this; 
        }
        
        // 提交
        public OctetsStream Commit()
        {
				 if (pos >= MAXSPARE)
				{
                erase(0, pos);
                pos = 0;
				 }
            return this;
        }
    }
}
