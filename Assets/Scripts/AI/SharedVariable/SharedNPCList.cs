using UnityEngine;
using System.Collections.Generic;
using AW.War;
using BehaviorDesigner.Runtime;

namespace AW.Data 
{
	[System.Serializable]
	public class SharedNPCList : SharedVariable
	{
		public List<BNPC> Value { get { return mValue; } set { mValue = value; } }
		[SerializeField]
		private List<BNPC> mValue;

		public override object GetValue() { return mValue; }
		public override void SetValue(object value) { mValue = (List<BNPC>)value; }

		public override string ToString() { return (mValue == null ? "null" : mValue.Count + " BNPC"); }
		public static implicit operator SharedNPCList(List<BNPC> value) { var sharedVariable = new SharedNPCList(); sharedVariable.SetValue(value); return sharedVariable; }
	}
}
