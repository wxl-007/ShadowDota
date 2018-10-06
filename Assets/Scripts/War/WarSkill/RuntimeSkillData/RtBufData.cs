using System;
using System.Collections.Generic;
using AW.War;
using AW.Framework;

namespace AW.Data {

	/// <summary>
	/// 实际上是buff和debuff
	/// Buff和Debuff有可能会挂载到各种NPC身上
	/// </summary>
	public class RtBufData {
		//唯一ID
		public readonly int ID;

		//Buff是挂在哪个NPC身上的
		public int HangUpNpcID {
			get;
			private set;
		}
		//Buff释放的NPC
		public int CastorNpcID {
			get;
			private set;
		}

		//Buff挂载的TriggerID
		public int TriggerID {
			get;
			set;
		}

		//可以被打断吗？
		public bool canInterrupt {
			get {
				return BuffCfg.BuffType == BufType.Booting;
			}
		}

		//目前这个是没用的
		public int level;

		/// <summary>
		/// 冷却的时间
		/// </summary>
		private float coolDown = 0;
		// 存活时间
		private float alive   = 0;
		//开始循环了吗？
		private bool Cycle    = false;

		public int Num {
			get {
				return BuffCfg.ID;
			}
		}

		//计时的时常
		private float curDuration = 0;

		//buf当前的层数
		private int curLayers;

		//是无限的？
		private bool isInFinity;

		//释放了多少次了？
		private short curCastTime;

		//立刻释放一次吗？
		private bool castRightNow;

		//Buff的起源
		public readonly OriginOfBuff origin;

		//buff的配置
		public BuffConfigData BuffCfg;

		/// <summary>
		/// 有可能没有，也可能有。
		/// 没有的时候，应该就会挂载一个Trigger
		/// 没有的时候，也可能只是改变NPC状态之类的
		/// </summary>
		public RtSkData onCycleskill;

		public RtSkData OnStartSkill;

		public RtSkData OnEndSkill;


		/// <summary>
		/// 函数指针，所有需要在特定Buff阶段执行的钩子函数
		/// </summary>
		public List<Action<RtBufData>> BeginFunc = null;
		public List<Action<RtBufData>> CycleFunc = null;
		public List<Action<RtBufData>> EndFunc   = null;
		public List<Action<RtBufData>> TriFunc   = null;

		public RtBufData(int bufNum, int bufId, int fromNpcId, int toNpcId, OriginOfBuff o, int initLayer, float duration = -1F, int level = 0) {
			BeginFunc = new List<Action<RtBufData>>();
			CycleFunc = new List<Action<RtBufData>>();
			EndFunc   = new List<Action<RtBufData>>();
			TriFunc   = new List<Action<RtBufData>>(); 

			SkBufferModel bfModel = Core.Data.getIModelConfig<SkBufferModel>();
			BuffCfg = bfModel.get(bufNum);

			Utils.Assert(BuffCfg == null, "RtBufData can't get buff config. buf num = " + bufNum);

			if(BuffCfg.ScriptStart > 0)
				OnStartSkill = new RtSkData(BuffCfg.ScriptStart, -1);

			if(BuffCfg.ScriptCycle > 0)
				onCycleskill = new RtSkData(BuffCfg.ScriptCycle, -1);

			if(BuffCfg.ScriptEnd > 0)
				OnEndSkill = new RtSkData(BuffCfg.ScriptEnd, -1);

			//初始层数
			curLayers = BuffCfg.Stacks > initLayer ? initLayer : BuffCfg.Stacks;

			alive = 0;
			coolDown = BuffCfg.DelayTime;
			castRightNow = BuffCfg.DelayTime <= 0F;

			curCastTime = 0;

			origin = o;
			Cycle = false;

			ID = bufId;

			CastorNpcID = fromNpcId;
			HangUpNpcID = toNpcId;

			if(duration <= 0F )
				curDuration = BuffCfg.Duration;
			else 
				curDuration = duration;

			isInFinity = curDuration == -1;
		}

		/// <summary>
		/// 设定客制化的Duration
		/// </summary>
		/// <param name="dur">Dur.</param>
		public void setCustomeDuration (float dur) {
			curDuration = dur;
		}

		/// <summary>
		/// 设定客制化的Duration
		/// </summary>
		/// <param name="dur">Dur.</param>
		public void setCustomeDurAddtion (float dur) {
			curDuration = curDuration + dur;
		}

		/// <summary>
		/// 设定客制化的层数
		/// </summary>
		/// <param name="Layer">Layer.</param>
		public void setMoreLayer(int Layer) {
			curLayers = curLayers + Layer;
			if(curLayers > BuffCfg.Stacks) curLayers = BuffCfg.Stacks;
		}

		public void RegisterFunc(Action<RtBufData> func, BuffPhase phase) {
			if(func != null) {
				switch(phase) {
				case BuffPhase.Cycle:
					CycleFunc.Add(func);
					break;
				case BuffPhase.Start:
					BeginFunc.Add(func);
					break;
				case BuffPhase.End:
					EndFunc.Add(func);
					break;
				case BuffPhase.Trigger:
					TriFunc.Add(func);
					break;
				}
			}
		}

		#region DOT/HOT 阶段

		public void Hit() {
			if(curCastTime == 0) {
				OnStart();
			} else if(curCastTime > 0) {
				OnTick();
			}
		}

		/// <summary>
		/// Buff开始了
		/// </summary>
		public void OnStart() {

			foreach(Action<RtBufData> act in BeginFunc) {
				act(this);
			}

			curCastTime = 1;
			Cycle = true;
			coolDown = BuffCfg.EffectCycle;
		}

		/// <summary>
		/// 每个循环
		/// </summary>
		public void OnTick() {

			foreach(Action<RtBufData> act in CycleFunc) {
				act(this);
			}

			curCastTime ++;
			Cycle = true;
			coolDown = BuffCfg.EffectCycle;
		}

		/// <summary>
		/// Buff结束
		/// </summary>
		public void OnEnd() {

			foreach(Action<RtBufData> act in EndFunc) {
				act(this);
			}

			Cycle = false;
			curCastTime ++;
		}

		/// <summary>
		/// 被动触发
		/// </summary>
		public void OnTrigger() {
			foreach(Action<RtBufData> act in TriFunc) {
				act(this);
			}
		}

		#endregion


		/// <summary>
		/// 如果结果为false，则代表已经过了生命周期
		/// </summary>
		/// <param name="delTime">Del time.</param>
		public bool Update(float delTime) {
			bool going = true;
			//立刻释放第一次？
			if(castRightNow) {
				castRightNow = false;
				OnStart();
			}

			alive += delTime;
			coolDown -= delTime;

			if(coolDown <= 0F)
				Hit();

			//结束
			if(isInFinity == false) {
				if(curDuration <= alive) {
					OnEnd();
					going = false;
				}
			}

			return going;
		}

	}
}

