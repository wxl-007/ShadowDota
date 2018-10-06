using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AW.Data;
using AW.War;

public static class AITools 
{		
	//取得两点间的距离的平方，
	public static float GetSqrDis(Vector3 from, Vector3 to)
	{
		return Vector3.SqrMagnitude (to - from);
	}

	//是否在范围内
	public static bool IsInRange(Vector3 from, float range, params Transform[] to)
	{
		for (int i = 0; i < to.Length; i++)
		{
			float distance = Vector3.SqrMagnitude (to [i].position - from);
			if (distance <= range * range)
				return true;
		}
		return false;
	}

	//是否在范围内
	public static bool IsInRange(Vector3 from, float range, params Vector3[] to)
	{
		for (int i = 0; i < to.Length; i++)
		{
			float distance = Vector3.SqrMagnitude (to [i] - from);
			if (distance <= range * range)
				return true;
		}
		return false;
	}

	//得到在范围内距离自己最近的npc
    public static ServerNPC GetNearNPCInRange(Vector3 from, float range, ServerNPC[] npcs)
	{
		float dis = Mathf.Infinity;
        ServerNPC near = null;
		for (int i = 0; i < npcs.Length; i++)
		{
			if (IsInRange(from, range, npcs[i].transform))
			{
				float distance = GetSqrDis (from, npcs [i].transform.position);
				if(distance < dis * dis)
				{
					dis = distance;
					near = npcs[i];
				}
			}
		}
		return near;
	}

	//得到范围内所有敌人
    public static List<ServerLifeNpc> GetAllNPCInRange(Vector3 from, float range, ServerLifeNpc[] npcs)
	{
        List<ServerLifeNpc> list = new List<ServerLifeNpc> ();
		for (int i = 0; i < npcs.Length; i++)
		{
            if (IsInRange(from, range + npcs[i].data.configData.radius, npcs[i].transform))
			{
				list.Add (npcs [i]);
			}
		}
		return list;
	}

	//得到距离自己最近的npc
    public static ServerLifeNpc GetNeareastNPC(Vector3 from, ServerLifeNpc[] npcs)
	{
		float dis = Mathf.Infinity;
        ServerLifeNpc near = null;
		for (int i = 0; i < npcs.Length; i++)
		{
			float distance = GetSqrDis (from, npcs [i].transform.position);
			if(distance < dis * dis)
			{
				dis = distance;
				near = npcs[i];
			}
		}
		return near;
	}
}
