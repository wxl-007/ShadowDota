using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Reflection;


public static class LuaBinding
{
    public class BindType
    {
        public string name;
        public Type type;
        public bool IsStatic;
        public string baseName = null;
        public string wrapName = "";
        public string libName = "";

        string GetTypeStr(string str)
        {
            if (str.Contains("`"))
            {
                string regStr = @"^(?<s0>.*?)\.?(?<s1>\w*)`[1-9]\[(?<s2>.*?)\]$";
                Regex r = new Regex(regStr, RegexOptions.None);
                Match mc = r.Match(str);
                bool beMember = false;

                if (!mc.Success)
                {
                    regStr = @"^(?<s0>.*?)\.?(?<s1>\w*)`[1-9]\+(?<s3>.*?)\[(?<s2>.*?)\]$";
                    r = new Regex(regStr, RegexOptions.None);
                    mc = r.Match(str);
                    beMember = true;
                }

                string s0 = mc.Groups["s0"].Value;
                string s1 = mc.Groups["s1"].Value;
                string s2 = mc.Groups["s2"].Value;
                string[] ss = s2.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                s2 = string.Empty;

                for (int i = 0; i < ss.Length; i++)
                {
                    ss[i] = ToLua._TC(ss[i]);
                }

                for (int i = 0; i < ss.Length - 1; i++)
                {
                    s2 += ss[i];
                    s2 += ",";
                }

                s2 += ss[ss.Length - 1];

                string s4 = string.Format("{0}<{1}>", s1, s2);

                if (beMember)
                {
                    s4 += ".";
                    s4 += mc.Groups["s3"].Value;
                }

                str = s0 + "." + s4;
            }
            else if (str.Contains("+"))
            {
                str = str.Replace('+', '.');
            }

            return str;
        }

        public BindType(Type t)
        {
            string str = t.ToString();
            //str = GetTypeStr(str);
            libName = GetTypeStr(str);
            type = t;

            if (t.BaseType != null)
            {
                baseName = t.BaseType.ToString();

                if (baseName == "System.ValueType")
                {
                    baseName = null;
                }
            }

            if (t.GetConstructor(Type.EmptyTypes) == null && t.IsAbstract && t.IsSealed)
            {
                baseName = null;
                IsStatic = true;
            }

            int index = str.LastIndexOf('.');

            if (index > 0)
            {
                name = str.Substring(index + 1);
                name = name.Replace('+', '.');
                wrapName = name.Replace(".", "");
            }
            else
            {
                name = str.Replace('+', '.');
                wrapName = name.Replace(".", "");
            }
        }

        public BindType SetBaseName(string str)
        {
            baseName = str;
            return this;
        }

        public BindType SetClassName(string str)
        {
            name = str;
            wrapName = GetWrapName();
            return this;
        }

        public BindType SetWrapName(string str)
        {
            wrapName = str;
            return this;
        }

        public BindType SetLibName(string str)
        {
            libName = str;
            return this;
        }

        string GetWrapName()
        {
            string[] ss = name.Split(new char[] { '.' });
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < ss.Length; i++)
            {
                sb.Append(ss[i]);
            }

            return sb.ToString();
        }
    }

    static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    //注意必须保持基类在其派生类前面声明，否则自动生成的注册顺序是错误的
    static BindType[] binds = new BindType[]
    {	
        //object 由于跟 Object 文件重名就不加入了
		//系统自带
		_GT(typeof(System.Type)),
		_GT(typeof(System.Math)),
		//_GT(typeof(CallLuaFunction)),
		//		_GT(typeof(System.IO.File)),             预注册
		//		_GT(typeof(System.IO.Directory)),    预注册

		//U3D
		_GT(typeof(Vector2)),
		_GT(typeof(Vector3)),
		_GT(typeof(Object)),
		_GT(typeof(GameObject)),
		_GT(typeof(Transform)),
		_GT(typeof(Component)),
		_GT(typeof(Behaviour)),
		_GT(typeof(MonoBehaviour)),
		_GT(typeof(Time)),
		_GT(typeof(Application)),
		_GT(typeof(Animation)),
		_GT(typeof(Animator)),
		_GT(typeof(AssetBundle)),
		_GT(typeof(Debug)),
		_GT(typeof(Resources)),
		_GT(typeof(Color)),
        //ngui
		#region BaseClass
		_GT(typeof(UIRect)),
		//     _GT(typeof(UIWidget)),                        预注册
		_GT(typeof(UIBasicSprite)),
		_GT(typeof(UIWidgetContainer)),
		_GT(typeof(UIButtonColor)),
		_GT(typeof(UIProgressBar)),
		#endregion

         _GT(typeof(UICamera)),
		 _GT(typeof(UITexture)),
		 _GT(typeof(UILabel)),
		 _GT(typeof(UISprite)),
		 _GT(typeof(UIButton)),
		 _GT(typeof(UISlider)),
		 _GT(typeof(UIPanel)),
		 _GT(typeof(NGUIDebug)),

		_GT(typeof(UITweener)),
		_GT(typeof(TweenPosition)),
		 _GT(typeof(TweenScale)),
		 _GT(typeof(TweenAlpha)),
	
		//自定义
		_GT(typeof(LuaScriptMgr)),
		_GT(typeof(BaseLua)),
		_GT(typeof(LuaLinker)),

		_GT(typeof(UIEventCenter.EventListener)),
		_GT(typeof(UIEventCenter.EventSender)),
		_GT(typeof(UIEventCenter.UIEventManager)),

		_GT(typeof(Core)),
		_GT(typeof(DownLoadFromWeb)),
		_GT(typeof(DeviceInfo)),
		_GT(typeof(LuaManager)),
		_GT(typeof(LuaTools)),
		_GT(typeof(Jumper)),
    };

	//自动生成包装(绑定)类
	[MenuItem("toLua/1.Generate CustomClass's WarpFile", false, 11)]
    public static void Binding()
    {
        if (!Application.isPlaying)
		{
            EditorApplication.isPlaying = true;
        }

        for (int i = 0; i < binds.Length; i++)
        {
            ToLua.Clear();
            ToLua.className = binds[i].name;
            ToLua.type = binds[i].type;
            ToLua.isStaticClass = binds[i].IsStatic;
            ToLua.baseClassName = binds[i].baseName;
            ToLua.wrapClassName = binds[i].wrapName;
            ToLua.libClassName = binds[i].libName;
            ToLua.Generate(null);
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < binds.Length; i++)
        {
            sb.AppendFormat("\t\t{0}Wrap.Register();\r\n", binds[i].wrapName);
        }

        EditorApplication.isPlaying = false;
        StringBuilder sb1 = new StringBuilder();

        for (int i = 0; i < binds.Length; i++)
        {
            sb1.AppendFormat("\t\t{0}Wrap.Register(L);\r\n", binds[i].wrapName);
        }

		AutoRegiste();
        AssetDatabase.Refresh();
    }




	//预注册
	static string[] pre_registration = new string[]
	{
		"UIWidgetWrap",
		"DirectoryWrap",
		"FileWrap",
		"yieldWrap",
	};
	//自动注册自定义类到LuaBinder文件中
	[MenuItem("toLua/2.Auto Registe into LuaBinder", false, 12)]
	static void AutoRegiste()
	{
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("public static class LuaBinder");
        sb.AppendLine("{");
        sb.AppendLine("\tpublic static void Bind(IntPtr L)");
        sb.AppendLine("\t{");

		string[] files = Directory.GetFiles("Assets/LuaWrap/Wrap/", "*.cs", SearchOption.TopDirectoryOnly);

		//自动读取生成的文件注册到LuaBinder中
        for (int i = 0; i < files.Length; i++)
        {
            string wrapName = Path.GetFileName(files[i]);
            int pos = wrapName.LastIndexOf(".");
            wrapName = wrapName.Substring(0, pos);
            sb.AppendFormat("\t\t{0}.Register(L);\r\n", wrapName);
        }

		//预注册
		for(int i= 0;i < pre_registration.Length ;i++)
		{
			sb.AppendFormat("\t\t{0}.Register(L);\r\n", pre_registration[i]);
		}


        sb.AppendLine("\t}");
        sb.AppendLine("}");

		string file = Application.dataPath + "/LuaWrap/Base/LuaBinder.cs";

        using (StreamWriter textWriter = new StreamWriter(file, false, Encoding.UTF8))
        {
            textWriter.Write(sb.ToString());
            textWriter.Flush();
            textWriter.Close();
        }

		AssetDatabase.Refresh();
    }


	//清除在LuaBinder类中的绑定
    [MenuItem("toLua/3.Clear LuaBinder File And Delete Warp File", false, 13)]
    static void ClearLuaBinder()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("public static class LuaBinder");
        sb.AppendLine("{");
        sb.AppendLine("\tpublic static void Bind(IntPtr L)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

		string file = Application.dataPath + "/LuaWrap/Base/LuaBinder.cs";

        using (StreamWriter textWriter = new StreamWriter(file, false, Encoding.UTF8))
        {
            textWriter.Write(sb.ToString());
            textWriter.Flush();
            textWriter.Close();
        }

		//把生成的文件也删除掉
		string[] files = Directory.GetFiles("Assets/LuaWrap/Wrap/", "*.cs", SearchOption.TopDirectoryOnly);
		for (int i = 0; i < files.Length; i++)
		{
			File.Delete(files[i]);
		}

        AssetDatabase.Refresh();
    }
	

    static string GetOS()
    {
#if UNITY_STANDALONE
        return "Win";
#elif UNITY_ANDROID
        return "Android";
#elif UNITY_IPHONE
        return "IOS";
#endif
    }

    //[MenuItem("Lua/Build Lua with luajit", false, 1)]
    public static void BuildLua()
    {
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;

        System.Diagnostics.Process proc = System.Diagnostics.Process.Start(Application.dataPath + "/Lua/Build.bat");
        proc.WaitForExit();
        AssetDatabase.Refresh();
        string[] files = Directory.GetFiles("Assets/Lua/Out", "*.lua.bytes");
        List<Object> list = new List<Object>();

        for (int i = 0; i < files.Length; i++)
        {
            Object obj = AssetDatabase.LoadMainAssetAtPath(files[i]);
            list.Add(obj);
        }

        if (files.Length > 0)
        {
            string output = string.Format("{0}/Bundle/Lua.unity3d", Application.dataPath);
            BuildPipeline.BuildAssetBundle(null, list.ToArray(), output, options, EditorUserBuildSettings.activeBuildTarget);
            string output1 = string.Format("{0}/{1}/Lua.unity3d", Application.persistentDataPath, GetOS());
            FileUtil.DeleteFileOrDirectory(output1);
            Directory.CreateDirectory(Path.GetDirectoryName(output1));
            FileUtil.CopyFileOrDirectory(output, output1);
            AssetDatabase.Refresh();
        }

        UnityEngine.Debug.Log("编译lua文件结束");
    }

    //[MenuItem("Lua/Build Lua without jit", false, 2)]
    public static void BuildLuaNoJit()
    {
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;

        string[] files = Directory.GetFiles("Assets/Lua/Out", "*.lua.bytes");

        for (int i = 0; i < files.Length; i++)
        {            
            FileUtil.DeleteFileOrDirectory(files[i]);
        }

        files = Directory.GetFiles(Application.dataPath + "/Lua/", "*.lua", SearchOption.TopDirectoryOnly);

        for (int i = 0; i < files.Length; i++)
        {
            string fname = Path.GetFileName(files[i]);
            FileUtil.CopyFileOrDirectory(files[i], Application.dataPath + "/Lua/Out/" + fname + ".bytes");
        }

        AssetDatabase.Refresh();

        files = Directory.GetFiles("Assets/Lua/Out", "*.lua.bytes");
        List<Object> list = new List<Object>();

        for (int i = 0; i < files.Length; i++)
        {
            Object obj = AssetDatabase.LoadMainAssetAtPath(files[i]);
            list.Add(obj);
        }

        if (files.Length > 0)
        {
            string output = string.Format("{0}/Bundle/Lua.unity3d", Application.dataPath);
            BuildPipeline.BuildAssetBundle(null, list.ToArray(), output, options, EditorUserBuildSettings.activeBuildTarget);
            string output1 = string.Format("{0}/{1}/Lua.unity3d", Application.persistentDataPath, GetOS());
            FileUtil.DeleteFileOrDirectory(output1);
            Directory.CreateDirectory(Path.GetDirectoryName(output1));
            FileUtil.CopyFileOrDirectory(output, output1);
            AssetDatabase.Refresh();
        }

        UnityEngine.Debug.Log("编译lua文件结束");
    }
}
