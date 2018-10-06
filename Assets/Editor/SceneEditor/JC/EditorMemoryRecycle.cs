using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//场景编译器的内存回收

[InitializeOnLoad]
public class EditorMemoryRecycle 
{
	static List<Object> memoryList = new List<Object>();

	static EditorMemoryRecycle ()
	{
		EditorApplication.update += Update;
	}
	
	static void Update ()
	{
		if(memoryList.Count == 0) return;
		foreach(Object o in memoryList)
		{
			//Debug.LogError("RemoveMemory:"+o.name);
			MonoBehaviour.DestroyImmediate(o);
		}
		memoryList.Remove(null);



	}

	public static void RemoveMemory(Object  _Object)
	{
		if(_Object != null)
			memoryList.Add(_Object);

	}

}
