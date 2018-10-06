using System;
using System.Collections.Generic;
using AW.Data;
using AW.Message;
using System.Linq;

namespace AW.War {
	using UVec3 = UnityEngine.Vector3;

	[Effect(OP = EffectOp.MoveToChild)]
	public class MoveToChildEffect : BaseEffect, ICastEffect {
		public MoveToChildEffect() : base() { }

		#region ICastEffect implementation

		public void Init (EffectConfigData config, SkillConfigData skillcfg) {
			base.cfg   = config;
			base.skCfg = skillcfg;
		}

		/// <summary>
		/// 释放技能效果
		/// </summary>
		/// <param name="src">效果发起者</param>
		/// <param name="skTarget">技能选择的目标, 技能目标忽略</param>
		/// <param name="target">技能先选择目标后，再次Effect选择后的目标,忽略</param>
		/// <param name="skDirectHurt">是否是技能的直接伤害, 这个没有伤害</param>
		/// <param name="container">所有数据展现的容器，本次Effect施法的效果会依次加入</param>
		public void Cast (ServerNPC src, IEnumerable<ServerNPC> skTarget, IEnumerable<ServerNPC> target, bool skDirectHurt, List<MsgParam> container) {
			//获取本地的EffectAttribute
			var efType = GetType();
			var classAttribute = (EffectAttribute)Attribute.GetCustomAttribute(efType, typeof(EffectAttribute));

			WarSrcAnimParam param = new WarSrcAnimParam();
			param.OP = classAttribute.OP;
            param.ShootAction = skCfg.ShootAction;
            param.ShootTime = skCfg.ShootTime;
            param.ShootEventTime = skCfg.ShootEventTime;

			/// 将自己位移到符合<param1>条件的子物体位置，原有子物体是否存在由<param2>决定
			MvToChildCon con = (MvToChildCon) Enum.ToObject(typeof(MvToChildCon), cfg.Param1);
			MvToChildAlive alive = (MvToChildAlive) Enum.ToObject(typeof(MvToChildAlive), cfg.Param2);

			 
			IEnumerable<ServerNPC> reTargets = select(src, ((ServerNPC)src).getChildNpc, con);
			if(reTargets.Any()) {
				ServerNPC final = reTargets.First();
				if(final != null) {

					UVec3 pos = final.transform.position;

					SelfDescribed desc = new SelfDescribed(){
						src    = src.UniqueID,
						target = src.UniqueID,
						act    = Verb.Blink,
						srcEnd = new EndResult() {
							param1 = (int)alive,
							param2 = final.UniqueID,
							param8 = pos.x,
							param9 = pos.y,
							param10 = pos.z,
						},
						targetEnd = null,
					};

					param.described = desc;
				}
			}

			container.Add(param);
		}

		#endregion

		/// <summary>
		/// 根据Param1来选择目标
		/// </summary>
		/// <param name="castor">Castor.</param>
		/// <param name="targets">Targets.</param>
		IEnumerable<ServerNPC> select(ServerNPC castor, IEnumerable<ServerNPC> targets, MvToChildCon condition) {
			List<ServerNPC> chosen = new List<ServerNPC>();
			IEnumerable<ServerNPC> itor = chosen.AsEnumerable<ServerNPC>();

			switch(condition) {
			case MvToChildCon.None:
				itor = targets;
				break;
			case MvToChildCon.Farest:
				UVec3 pp = castor.transform.position;
				ServerNPC farest = targets.OrderBy( n => UVec3.SqrMagnitude( n.transform.position - pp ) ).LastOrDefault();
				if(farest != null) chosen.Add(farest);
				break;
			case MvToChildCon.Nearest:
				UVec3 p = castor.transform.position;
				ServerNPC nearest = targets.OrderBy( n => UVec3.SqrMagnitude( n.transform.position - p ) ).FirstOrDefault();
				if(nearest != null) chosen.Add(nearest);
				break;
			case MvToChildCon.HpHighest:
				ServerNPC highest = targets.OrderByDescending( n => n.data.rtData.CurHpNested ).FirstOrDefault();
				if(highest != null) chosen.Add(highest);
				break;
			case MvToChildCon.HpLowest:
				ServerNPC lowest = targets.OrderBy( n => n.data.rtData.CurHpNested ).FirstOrDefault();
				if(lowest != null) chosen.Add(lowest);
				break;
			}

			return itor;
		}

	}
}
