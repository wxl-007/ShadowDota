using System;
using System.Reflection;

/// <summary>
/// Generic singleton. 支持无参数的泛型单例模式
/// </summary>
public class GenericSingleton<T> where T : class  {

    private static volatile T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                if (_instance == null)
                {
                    Type type = typeof(T);
                    ConstructorInfo ctor;
                    ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);
                    _instance = (T)ctor.Invoke(new object[0]);
                }
                
            }

            return _instance;
        }
    }

}


#region 使用方法

/*public sealed class ZoomTool 
{
    #region Constructor

    private ZoomTool()
    {
    }

    #endregion

    #region Properties

        ......

    public static ZoomTool Instance
    {
        get { return GenericSingleton<ZoomTool>.Instance; }
    }

    #endregion
}


*/
#endregion