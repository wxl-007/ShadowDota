using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using AW.AI;
using AW.Data;
using System.Collections.Generic;

namespace AW.AI
{
	[TaskDescription("npc是否按照时间刷新")]
	[TaskCategory("Hero")]
	public class IsFreshType : Conditional
	{
		public SharedInt curGroupIndex;		//当前刷新波数索引
		public SharedInt curGroupCnt;		//当前刷的是第几波怪

		public Wait startDelayTime;			//开始多少秒以后刷新
		public Repeater freshCnt;			//刷新次数
		public Wait timePerCnt;				//每波次之间的时间间隔
		public Wait timePerNPC;				//每个npc之间的创建时间间隔
		public Repeater npcCnt;				//每刷新的npc个数

		protected BNPC myHero;
		protected FreshPoolModel freshPoolModel;
		protected FreshGroupModel freshGroupModel;

		public override void OnStart()
		{
			myHero = GetComponent<BNPC>();
			freshPoolModel = Core.Data.getIModelConfig<FreshPoolModel>();
			freshGroupModel = Core.Data.getIModelConfig<FreshGroupModel>();
		}

		public override TaskStatus OnUpdate()
		{
			ConsoleEx.DebugError ("must override");
			return TaskStatus.Success;
		}

		public void InitData()
		{
			//开始刷新延迟
			startDelayTime.waitTime.Value = myHero.dataInScene.freshParam.starDelayTime;

			//刷新的波数
			if (myHero.dataInScene.freshParam.freshCount > 0)
			{
				freshCnt.count.Value = myHero.dataInScene.freshParam.freshCount;
				freshCnt.repeatForever.Value = false;
			}
			else
			{
				freshCnt.repeatForever.Value = true;
			}

			//每波怪之间的时间间隔
			timePerCnt.waitTime.Value = myHero.dataInScene.freshParam.timePerCount;

			//每个npc刷新之间的间隔时间
			timePerNPC.waitTime.Value = myHero.dataInScene.freshParam.timePerNPC;

			//当前波刷新的npc个数
			curGroupIndex.Value = 0;
			curGroupCnt.Value = 0;
		}

		public override void OnEnd()
		{

		}
	}
}