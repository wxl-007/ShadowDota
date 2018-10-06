using UnityEngine;
using System.Collections;
using AW.Data;

/* 
 * 为了编译器后期扩展增加属性，特地与ElementInSceneData区分出来 ,生成单独的编译器配置文件
 */

namespace SceneEditorSttingData
{
	[System.Serializable]
	public class SceneEditorSettingBaseData 
	{
		//类型
		public SceneElementType Type;

		//资源路径
		public string sourcePath;

	}

	//地图在场景中的基本信息
	[System.Serializable]
	public class MapSettingData:SceneEditorSettingBaseData
	{
		//属性
		public MapInSceneData Attribute;

		//显示网格
		public bool showGrid;

		//地图网格大小
		public float[] GridSize;

		//网格颜色
		public float[] GridColor;
	}

	[System.Serializable]
	public class NPCSettingData:SceneEditorSettingBaseData
	{
		//属性
		public NPCInSceneData Attribute;

		//是否跟随地图
		public bool isSeletctedFllowMouse;

		//是否吸附到地图
		public bool isAtMap;

		//是否打开Transform的设置
		public bool isSetTransform;

		//是否合并资源为一体(防止鼠标点击到子物体)
		public bool isMergeResource;

		public NPCSettingData()
		{
			isMergeResource = true;
		}

	}

	[System.Serializable]
	public class SpecialAreaSettingData : SceneEditorSettingBaseData
	{
		//属性
		public SpecialAreaInSceneData Attribute;
		
		//是否跟随地图
		public bool isSeletctedFllowMouse;
		
		//是否吸附到地图
		public bool isAtMap;

		//是否打开Transform的设置
		public bool isSetTransform;
	}
}
