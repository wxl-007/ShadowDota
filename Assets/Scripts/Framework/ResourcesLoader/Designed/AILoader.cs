using UnityEngine;
using System.Collections.Generic;
using System.IO;
using URes = UnityEngine.Resources;
using BehaviorDesigner.Runtime;
using System.Text;

namespace AW.Resources {
	public class AILoader : ILoaderDispose {
		#region ILoaderDispose implementation

		private readonly string AI_PATH = "Pack/AI/Node/";
		private StringBuilder strBld;

		//刷新NPC
		public const string NPC_FRESH = "FreshNPC";
		//普通攻击
		public const string NORMAL_ATTACK = "NormalAttack";
		//寻路攻击
		public const string PATHFIND_ATK = "PathFindAttack";
        //简单寻路攻击
        public const string SIMPLE_PFATK = "SimplePathFindAttack";
		//手动攻击
		public const string MANUALBATTLE = "ManualBattle";
		//跟随队长
		public const string FOLLOW = "FollowCaptain";

		public AILoader()
		{
			strBld = new StringBuilder ();
		}

		public void ClearCache (bool gc) {

		}

		#endregion

		public ExternalBehavior load(string name, bool cached = true) 
		{
			strBld.Remove (0, strBld.Length);
			strBld.Append (AI_PATH);
			strBld.Append (name);
			Object obj = URes.Load(strBld.ToString());
			if (obj != null)
			{
				return (ExternalBehavior)obj;
			}
			ConsoleEx.DebugWarning ("not find ai tree in path :: " + strBld.ToString());
			return null;
		}
	}
}