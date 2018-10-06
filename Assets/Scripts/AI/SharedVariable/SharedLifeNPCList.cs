using UnityEngine;
using System.Collections.Generic;
using AW.War;
using BehaviorDesigner.Runtime;

namespace AW.Data 
{
	[System.Serializable]
	public class SharedLifeNPCList : SharedVariable
	{
        public List<ServerLifeNpc> Value { get { return mValue; } set { mValue = value; } }
		[SerializeField]
        private List<ServerLifeNpc> mValue;

		public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (List<ServerLifeNpc>)value; }

        public override string ToString() { return (mValue == null ? "null" : mValue.Count + " ServerLifeNpc"); }
        public static implicit operator SharedLifeNPCList(List<ServerLifeNpc> value) { var sharedVariable = new SharedLifeNPCList(); sharedVariable.SetValue(value); return sharedVariable; }
	}
}
