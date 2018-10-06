using UnityEngine;
using System.Collections;
using AW.War;

//共享NPC数据变量
namespace BehaviorDesigner.Runtime.Tasks
{
	[System.Serializable]
	public class SharedLifeNPC : SharedVariable
	{
        public ServerLifeNpc Value { get { return mValue; } set { mValue = value; } }
		[SerializeField]
        private ServerLifeNpc mValue;

		public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (ServerLifeNpc)value; }

		public override string ToString() { return (mValue == null ? "null" : mValue.ToString()); }
	}
}