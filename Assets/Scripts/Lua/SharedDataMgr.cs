using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SharedData 
{
	Dictionary<string,object> data = new Dictionary<string, object>();

	public int Count { get{return data.Count;} }

	public object this[string key]
	{
		get
		{
			if(data.ContainsKey(key))
				return data[key];
			return null;
		}
		set
		{
			if(!data.ContainsKey(key))
				data.Add(key,value);
			else
				data[key] = value;
		}
	}

	public object Get(params string[] keys)
	{
		if(keys == null) 
			return null;
		int length =  keys.Length;
		if(length == 1)
			return data[keys[0]];

		SharedData returnData = this;
		object Value = null;
		for(int i=0; i < length ; i ++)
		{
			if(i == length - 1)
				Value = returnData[keys[i]];
			else
			{
				if( returnData[keys[i]] is SharedData )
				   returnData = (SharedData)returnData[keys[i]];
				else
				{
					Debug.LogError( keys[i] +" is not a table!");
					return null;
				}
				if(returnData == null)
					return null;
			}
		}
		return Value;
	}


	public void Print()
	{
		foreach(string key in data.Keys)
			Debug.LogError(key+"    "+data[key].ToString());
	}


}


public class SharedListData
{
	List<object> dataList = new List<object>();
	public object this[int key]
	{
		get
		{
			if ( key < dataList.Count )
				return dataList[key];
			else
				return null;
		}
		set
		{
			dataList[key] = value;
		}
	}
	
	public object first
	{
		get
		{
			if(dataList.Count > 0)
				return dataList[0];
			else
				return null;
		}
		set
		{
			//if(dataList.Count > 0)
			dataList[0] = value;
		}
	}
	
	public object last
	{
		get
		{
			if(dataList.Count > 0)
				return dataList[dataList.Count - 1];
			else
				return null;
		}
		set
		{
			//if(dataList.Count > 0)
			dataList[dataList.Count - 1] = value;
		}
	}
	
	public int Count
	{
		get
		{
			return dataList.Count;
		}
	}
	
	public bool  Remove(object item)
	{
		return dataList.Remove(item);
	}
	
	public void RemoveAt(int index)
	{
		dataList.RemoveAt(index);
	}
	
	public void Add(object item)
	{
		dataList.Add(item);
	}
	
	public List<object> List
	{
		get
		{
			return dataList;
		}
	}

	#region 只有存放的value值是一个Lua函数,才可以使用这个方法
	public bool CallFunction(int index)
	{
		if(index < dataList.Count)
		{
			if( dataList[index].GetType() == typeof(LuaInterface.LuaFunction)  )
			{
				LuaInterface.LuaFunction f= (LuaInterface.LuaFunction)dataList[index];
				f.Call();
				return true;
			}
		}
		return false;
	}
	#endregion

	
}





public class SharedDataMgr {

	static Dictionary<string,SharedData> dataCenter = new Dictionary<string, SharedData>();
	public static SharedData getSharedData(string dataName)
	{
		if(dataCenter.ContainsKey(dataName))
			return dataCenter[dataName];
		return null;
	}

	public static SharedData newClass(string className)
	{
		SharedData shared = null;
		if( !dataCenter.TryGetValue(className,out shared) )
		{
			shared = new SharedData();
			dataCenter.Add(className,shared);
		}
		return shared;
	}


	#region 将table存成一个List
	static Dictionary<string,SharedListData> dataList = new Dictionary<string, SharedListData>();
	public static SharedListData getList(string ListName)
	{
		SharedListData _list = null;
		dataList.TryGetValue(ListName,out _list);
		return _list;
	}

	public static SharedListData newList(string ListName)
	{
		SharedListData _list = new SharedListData();
		if(dataList.ContainsKey(ListName))
			dataList[ListName] = _list;
		else
			dataList.Add(ListName,_list);
		return _list;
	}

	#endregion

}




