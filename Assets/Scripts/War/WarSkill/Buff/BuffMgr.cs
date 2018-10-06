using System;
using System.Collections.Generic;
using AW.Data;
using System.Threading;
using AW.Framework;
using System.Linq;

namespace AW.War {

	/// <summary>
	/// 统一的将Buff、Debuff都简化的叫做Buff
	/// 
	/// Buff的容器，虽然每个Buff都是在NPC身上，但实际上NPC只有一个ID的引用
	/// 所有的Buff都在BuffMgr里面
	/// </summary>
	public class BuffMgr {
		//分配给Buff的唯一ID
		static int allocIdx = 0;

		private BuffCastor bfCastor;

		//Key is Unique ID
		private Dictionary<int, RtBufData> OnWork = null;

		//工作的容器，因为OnWork 在Update里foreach的时候，可能会递归的创建Buff,
		//导致经常出现Out of Sync.
		private List<RtBufData> working = null;
		//依赖触发器
		private TriggerMgr trigMgr = null;
		//npc结构
		private WarServerNpcMgr NpcMgr;

		private BuffMgr() {
			allocIdx = 0;
			OnWork = new Dictionary<int, RtBufData>();
			working = new List<RtBufData>();
			bfCastor = BuffCastor.instance;
		}

		public static BuffMgr Instance {
			get {
				return GenericSingleton<BuffMgr>.Instance;
			}
		}

		public void init(WarServerNpcMgr npcMgr, TriggerMgr tgMgr) {
			NpcMgr = npcMgr;
			bfCastor.init(npcMgr);
			trigMgr = tgMgr;
		}

		#region 创建BUff

		/// <summary>
		/// 创建NPC身上的buff, 最简单的添加逻辑, 只适合在给泉水添加buff等简单情况下的逻辑
		/// </summary>
		/// <returns>The buff.</returns>
		/// <param name="bufNum">Buffer number.</param>
		/// <param name="npcId">Npc identifier.</param>
		/// <param name="level">Level.</param>
		public RtBufData createBuff(BuffCtorParam param) {
			allocIdx ++;
			RtBufData buf = new RtBufData(param.bufNum, allocIdx, param.fromNpcId, 
				param.toNpcId, param.origin, param.initLayer, param.duration, param.level);

			//加入管理
			OnWork[allocIdx] = buf;

			//挂在Npc上
			ServerNPC npc = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(param.toNpcId);
			npc.addBuff(allocIdx);

			///
			/// ---- 注册释放方法 ----
			///
			buf.RegisterFunc(bfCastor.FirstCast,      BuffPhase.Start);
			buf.RegisterFunc(bfCastor.cast,           BuffPhase.Cycle);
			buf.RegisterFunc(bfCastor.EndCast,        BuffPhase.End);
			buf.RegisterFunc(bfCastor.EndCastStep2,   BuffPhase.End);

			///
			/// ---- 需要创建Trigger ------
			///

			int[] triIds = buf.BuffCfg.TriggerIDList;
			if(triIds != null) {
				int cnt = triIds.Length;
				if(cnt > 0) {
					for(int i = 0; i < cnt; ++ i) 
						trigMgr.CreateTrigger(param.toNpcId, triIds[i], buf);
				}
			}

			return buf;
		}

		/// <summary>
		/// 创建buff，更加通用的处理逻辑
		/// 也许并不创建Buff
		/// </summary>
		/// <returns>The buff.</returns>
		/// <param name="param">Parameter.</param>
		public RtBufData createBuff(BuffCtorParamEx param) {
			#if DEBUG
			Utils.Assert(param.cfg == null, "param.cfg == null");
			#endif

			int casterID    = param.fromNpcId;
			int tarID       = param.toNpcId;
			int buffID      = param.cfg.Param1;
			float customDur = param.cfg.Param2 * Consts.OneThousand;
			DotHotDurType durType = (DotHotDurType) Enum.ToObject(typeof(DotHotDurType), param.cfg.Param3);
			int Layers      = param.cfg.Param4;

			SkBufferModel bfModel  = Core.Data.getIModelConfig<SkBufferModel>();
			BuffConfigData BuffCfg = bfModel.get(buffID);


			List<RtBufData> TarBuff = new List<RtBufData>();
			foreach(RtBufData buf in OnWork.Values) {
				if(buf != null && buf.HangUpNpcID == tarID) {
					TarBuff.Add(buf);
				}
			}

			///
			/// 1. 判定要创建的buff是否有Group ID 作为主键 的冲突, 
			///    但是释放者的ID必须不相同
			///

			//buff的分组
			List<RtBufData> Conflict = new List<RtBufData>();
			int groupId     = BuffCfg.BuffGroup;
			foreach(RtBufData buf in TarBuff) {

				if(buf != null) {
					if(buf.CastorNpcID != casterID && buf.BuffCfg.BuffGroup == groupId) {
						Conflict.Add(buf);
					}
				}

			}

			//删除冲突
			if(Conflict.Count > 0) {
				int cnt = Conflict.Count;
				for(int i = 0; i < cnt; ++ i) {
					RtBufData d = Conflict[i];
					rmBuff(d.ID, d.HangUpNpcID);
				}
			}


			///
			/// 2. 判定Buff ID和CasterID 作为主键来决定是否有冲突
			///
			Conflict.Clear();
			foreach(RtBufData buf in TarBuff) {

				if(buf != null) {
					if(buf.CastorNpcID == casterID && buf.BuffCfg.ID == buffID) {
						Conflict.Add(buf);
					}
				}

			}

			int confcnt = Conflict.Count;
			bool hasConflict = confcnt > 0;
			RtBufData createOne = null;

			if(hasConflict) {
				//不删除冲突
				for(int i = 0; i < confcnt; ++ i) {
					RtBufData d = Conflict[i];
					//冲突的buff时间处理，
					switch(durType) {
					//刷新Buff的时间
					case DotHotDurType.Fixture:
						d.setCustomeDuration(d.BuffCfg.Duration);
						break;
					case DotHotDurType.Increase:
						d.setCustomeDurAddtion(customDur);
						break;
					case DotHotDurType.Leaveit:
						break;
					}

					//冲突的Buff的层数处理
					d.setMoreLayer(Layers);
				}

				createOne = Conflict[0];

			} else {
				//创建Buff
				BuffCtorParam ctor = new BuffCtorParam() {
					bufNum    = buffID,
					fromNpcId = casterID,
					toNpcId   = param.toNpcId,
					//buff的起源
					origin    = OriginOfBuff.BornWithSkill,
					//初始的层数（默认1）
					initLayer = 1,
					//等级(忽略）
					level     = -1,
					//自定义duration（默认填-1F)
					duration  = Consts.USE_BUFF_CONFIG_DURATION,
				};

				createOne = createBuff(ctor);
			}


			return createOne;
		}

		#endregion


		#region 删除Buff

		//第一个是唯一ID，第二个是挂载的NPC ID, 第三个是是否已经执行过结束逻辑
		public RtBufData rmBuff(int bufId, int npcId, bool endIsExe = false) {
			RtBufData buf = null;
			OnWork.TryGetValue(bufId, out buf);
			//去除管理
			OnWork.Remove(bufId);
			//去除Npc上
			ServerNPC npc = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(npcId);
			npc.rmBuff(bufId);

			if(buf != null && endIsExe == false) buf.OnEnd();
			return buf;
		}

		/// <summary>
		/// 删除所有NPC身上的BUFF
		/// </summary>
		/// <param name="npcId">Npc identifier.</param>
		public void rmAllBuff(int npcId) {
			//去除Npc上
			ServerNPC npc = WarServerManager.Instance.npcMgr.GetNPCByUniqueID(npcId);
			List<int> buffArray = npc.getBuffList;
			int cnt = buffArray.Count;

			for(int i = 0; i < cnt; ++ i) {
				int bufId = buffArray[i];

				RtBufData buf = null;
				if(OnWork.TryGetValue(bufId, out buf)) {
					buf.OnEnd();
				}

				//去除管理
				OnWork.Remove(bufId);
			}

			npc.rmAllBuff();
		}

		#endregion

		/// <summary>
		/// 查找出一个Buff释放是挂在NPC身上
		/// </summary>
		/// <returns>The buff.</returns>
		/// <param name="bufNum">Buffer number.</param>
		/// <param name="fromNpcId">From npc identifier.</param>
		/// <param name="toNpcId">To npc identifier.</param>
		public RtBufData findBuff(int bufNum, int fromNpcId, int toNpcId) {
			RtBufData theOne = null;
			foreach(RtBufData buf in OnWork.Values) {
				if(buf != null) {
					bool found = buf.HangUpNpcID == toNpcId & buf.CastorNpcID == fromNpcId;
					found = found & bufNum == buf.Num;

					if(found) {
						theOne = buf;
						break;
					}
				}
			}
			return theOne;
		}

		/// <summary>
		/// 查找出一个Buff是否是挂在NPC身上
		/// </summary>
		/// <returns>The buff.</returns>
		/// <param name="bufNum">Buffer number.</param>
		public List<RtBufData> findBuff(int bufNum, int toNpcId) {
			List<RtBufData> theOne = null;
			theOne = OnWork.Values.Where(buf => (buf.Num == bufNum & buf.HangUpNpcID == toNpcId)).ToList();
			return theOne;
		}

		/// <summary>
		/// 挑选出, 某个NPC可以被删除的NpcStatus
		/// </summary>
		/// <returns><c>true</c>, if buff status was found, <c>false</c> otherwise.</returns>
		/// <param name="toTest">To test.</param>
		public NpcStatus SiftOutStatus(NpcStatus toBeRmed, int buffId, int npcId) {

			NpcStatus S1 = NpcStatus.None;

			///
			/// 找出当前所有buff的(除了buffId）
			///
			foreach(RtBufData buf in OnWork.Values) {
				if(buf != null && buf.ID != buffId && npcId == buf.HangUpNpcID) {
					S1 = S1.set(buf.BuffCfg.Status);
				}
			}

			NpcStatus S2 = S1.pickUp(toBeRmed);

			return S2;
		}

		/// <summary>
		/// 根据Key找到RtBufData
		/// </summary>
		/// <param name="key">Key.</param>
		public RtBufData get(int key) {
			RtBufData theOne = null;
			bool found = OnWork.TryGetValue(key, out theOne);
			if(found) return theOne;
			else return null;
		}

		public void Update(float delTime) {

			foreach(RtBufData buf in OnWork.Values) {
				if(buf != null) {
					working.Add(buf);
				}
			}

			int workCnt = working.Count;
			if(workCnt > 0) {
				for(int i = 0; i < workCnt; ++ i) {
					RtBufData buf = working[i];
					bool alive = buf.Update(delTime);
					if(!alive) {
						rmBuff(buf.ID, buf.HangUpNpcID, true);
					}

					///
					/// 判定当前的是否是引导技能
					///
					if(buf.canInterrupt) {
						//检测是否已被打断
						ServerNPC npc = NpcMgr.GetNPCByUniqueID(buf.HangUpNpcID);
						ServerLifeNpc life = npc as ServerLifeNpc;
						bool interrupt = life.curStatus.AnySame(NpcStatus.Interrupt);
						if(interrupt) {
							rmBuff(buf.ID, buf.HangUpNpcID);
						}
					}

				}
				working.Clear();
			}
		}

	}

	//创建Buff的参数列表
	public struct BuffCtorParam {

		public int bufNum;		
		public int fromNpcId;
		public int toNpcId;
		//buff的起源
		public OriginOfBuff origin;
		//初始的层数（默认1）
		public int initLayer;
		//等级(忽略）
		public int level;
		//自定义duration（默认填-3F)
		public float duration;

	}

	//创建Buff的参数更加复杂的列表
	public struct BuffCtorParamEx {
		public int fromNpcId;
		public int toNpcId;
		//等级(忽略）
		public int level;
		//效果的配置数据
		public EffectConfigData cfg;
	}

}

