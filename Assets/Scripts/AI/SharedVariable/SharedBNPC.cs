using UnityEngine;
using System.Collections;
using AW.War;

//共享英雄NPC数据变量
namespace BehaviorDesigner.Runtime.Tasks
{
	[System.Serializable]
	public class SharedBNPC : SharedVariable
	{
		public BNPC Value { get { return mValue; } set { mValue = value; } }
		[SerializeField]
		private BNPC mValue;

		public override object GetValue() { return mValue; }
		public override void SetValue(object value) { mValue = (BNPC)value; }

		public override string ToString() { return (mValue == null ? "null" : mValue.ToString()); }
	}
}
