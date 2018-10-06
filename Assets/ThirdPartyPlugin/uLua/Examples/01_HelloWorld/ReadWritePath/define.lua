luanet.load_assembly("Assembly-CSharp")

NGUITools = luanet.import_type("NGUITools");


--基础类型定义--
UnityEngine		= luanet.UnityEngine;
System 			= luanet.System;
Collections		= System.Collections;
Hashtable		= Collections.Hashtable;
Generic			= Collections.Generic;

Debug			= UnityEngine.Debug;

GameObject		= UnityEngine.GameObject;
Transfrom		= UnityEngine.Transfrom;
Vector2			= UnityEngine.Vector2;
Vector3			= UnityEngine.Vector3;
Time			= UnityEngine.Time;
GUI				= UnityEngine.GUI;
Rect			= UnityEngine.Rect;
Resources		= UnityEngine.Resources;
Font			= UnityEngine.Font;
