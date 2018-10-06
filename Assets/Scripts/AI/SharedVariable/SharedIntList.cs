using UnityEngine;
using System.Collections.Generic;
using AW.War;
using BehaviorDesigner.Runtime;

namespace AW.Data 
{
	[System.Serializable]
	public class SharedIntList : SharedVariable
	{
		public List<int> Value { get { return mValue; } set { mValue = value; } }
		[SerializeField]
		private List<int> mValue;

		public override object GetValue() { return mValue; }
		public override void SetValue(object value) { mValue = (List<int>)value; }

		public override string ToString() { return (mValue == null ? "null" : mValue.Count + " int"); }
		public static implicit operator SharedIntList(List<int> value) { var sharedVariable = new SharedIntList(); sharedVariable.SetValue(value); return sharedVariable; }
	}
}
