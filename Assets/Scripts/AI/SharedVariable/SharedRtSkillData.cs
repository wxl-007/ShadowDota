using UnityEngine;
using System.Collections;
using AW.War;
using AW.Data;

//共享NPC数据变量
namespace BehaviorDesigner.Runtime.Tasks
{
	[System.Serializable]
	public class SharedRtSkillData : SharedVariable
	{
		public RtSkData Value { get { return mValue; } set { mValue = value; } }
		[SerializeField]
		private RtSkData mValue;

		public override object GetValue() { return mValue; }
		public override void SetValue(object value) { mValue = (RtSkData)value; }

		public override string ToString() { return (mValue == null ? "null" : mValue.ToString()); }
	}
}