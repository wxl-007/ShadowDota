1.速度非常快，不适用System.Xml library
  如果想序列化 DataTable, 就必须使用 ADONET Macro
  如果想序列化 XML， 就必须使用USE_XML Macro
2.请注意 SILVERLIGHT marco,具体看代码
3.源代码非常注意类型匹配，我改写了部分代码JSON.cs第570行, 
    改写后的：
    case myPropInfoType.Int: oset = Convert.ToInt32(v); break;
    case myPropInfoType.Long: oset = Convert.ToInt64(v); break;
    case myPropInfoType.String: oset = Convert.ToString(v); break;
    case myPropInfoType.Bool: oset = (bool)v; break;
    case myPropInfoType.DateTime: oset = CreateDateTime((string)v); break;
    case myPropInfoType.Enum: oset = CreateEnum(pi.pt, (string)v); break;
    case myPropInfoType.Guid: oset = CreateGuid((string)v); break;
    原来的：
    case myPropInfoType.Int: oset = (int)((long)v); break;
    case myPropInfoType.Long: oset = (long)v; break;
    case myPropInfoType.String: oset = (string)v; break;
    case myPropInfoType.Bool: oset = (bool)v; break;
    case myPropInfoType.DateTime: oset = CreateDateTime((string)v); break;
    case myPropInfoType.Enum: oset = CreateEnum(pi.pt, (string)v); break;
    case myPropInfoType.Guid: oset = CreateGuid((string)v); break;

4.因为AOT的关系，Ios设备必须使用AOT macro
  新增加更多的测试，分别采用了不同的方法构造对象
  1. AOT0 使用Activator.CreateInstance（）
  2. AOT1 使用ConstructorInfo.Invoke()
  3. AOT2 使用lambda compile
  4. 非AOT，使用了System.Reflection.Emit

  4最快但是仅限于PC和MAC上使用。3.第一次使用效率非常低，但是此后性能仅次于4，适用场合仅在于大量（超过10K）对象创建的时候。
  2和3效率差不多，适合在IOS和ANDROID平台上使用


5.增加了新的解析类型，比如二维数组T[][] test;
  我们的反序列化的代码就可以定义为
  class data{
	  public List<T[]> test;
  }
  -- marked by Allen --
 
  
