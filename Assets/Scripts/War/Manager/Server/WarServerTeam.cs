using System;
using System.Collections.Generic;
using System.Linq;
using AW.Data;

namespace AW.War
{
    /// <summary>
    /// 是在哪个地图上的战斗
    /// </summary>
    public class MapInfo
    {
        //配表中的ID
        public int ID;
        //读取何种配置
        public ConfigType type;

        public bool IsSame(MapInfo two)
        {
            return this.ID == two.ID && this.type == two.type;
        }
    }

    /// <summary>
    /// 加入房间的角色信息
    /// </summary>
    public class RoomCharactor
    {

        public string Name;
        //唯一ID
        public string UID;

        public int Lv;

        //网络传输的时候是基类Team，
        //实际使用的时候是WrapperTeam
        public Team team;

        /// <summary>
        /// 在整个玩家信息列表列的位置信息
        /// </summary>
        public int pos;

        public WarCamp camp;
        //TODO : add more

        //是否自动战斗
        public bool autoBattle = true;

        public RoomCharactor DeepCopy() {

			RoomCharactor rc = (RoomCharactor)this.MemberwiseClone();
			rc.team = new Team();
			rc.team.team = new List<RoomNpc>();

			int cnt = team.team.Count;
			for(int i = 0; i < cnt; ++ i) {
				rc.team.team.Add(team.team[i].shallowCopy());
			}

			return rc;
        }
    }

    /// <summary>
    /// 加入房间的NPC信息
    /// </summary>
    public class RoomNpc
    {
        public int NpcNum;
        public int NpcStar;
        //ignore
        public int NpcLv;

        //TODO : add Equipment or something else

		public RoomNpc shallowCopy() {
			return (RoomNpc) this.MemberwiseClone();
		}

    }

    public class Team
    {
        // Lua 提供
        //进入战斗前的数据准备
        public List<RoomNpc> team;

    }

    //将Lua过来的数据转化为更加合理的数据
    public class WrapperTeam : Team
    {
        //真实的队员信息
        public List<NPCData> dataTeam;

        //进入战斗后的真实队员信息
        public List<ServerLifeNpc> realTeam;

        //手动控制激活状态的NPC
        public ServerLifeNpc activeNpc;

		public void AddNpcMember(ServerLifeNpc npc, int index) {
			if (realTeam == null)
				realTeam = new List<ServerLifeNpc>();
			realTeam.Add(npc);

			if(index == 0)
				activeNpc = npc;
		}
    }


    /// <summary>
    /// 服务器---进入房间前应该准备好的数据
    /// </summary>
    public class WarServerCharactor
    {
        private NPCModel Model;
        //所有阵营的玩家信息
        public readonly Dictionary<WarCamp, List<RoomCharactor>> allCharactors;

        public readonly Dictionary<WarCamp, List<ServerLifeNpc>> allHeros;

        public WarServerCharactor()
        {
            allCharactors = new Dictionary<WarCamp, List<RoomCharactor>>();
            allHeros = new Dictionary<WarCamp, List<ServerLifeNpc>>();

            Model = Core.Data.getIModelConfig<NPCModel>();
        }

        //设置英雄队列
        public void SetHeroList(WarCamp camp, List<ServerLifeNpc> list)
        {
            allHeros.Add(camp, list);
        }

        //得到英雄队伍
        public List<ServerLifeNpc> GetHeroList(CAMP camp)
        {
            List<ServerLifeNpc> list = null;
            if (allHeros.TryGetValue(WarCamp2Camp.toWarCamp(camp), out list))
                return list;

            ConsoleEx.DebugError("hero list is null ::  " + camp.ToString());
            return null;
        }

        //加入房间
        public void JoinRoom(RoomCharactor charactor)
        {
            if (charactor != null)
            {

                string UID = charactor.UID;
                WarCamp camp = charactor.camp;

                List<RoomCharactor> CharactorList = null;
                bool found = allCharactors.TryGetValue(camp, out CharactorList);
                if (found)
                {

                    RoomCharactor existC = CharactorList.Find(c => c.UID == UID);
                    if (existC == null)
                    {
                        int cnt = CharactorList.Count;
                        charactor.pos = cnt;

                        CharactorList.Add(charactor);
                    }

                }
                else
                {
                    //没有其他的玩家，so一定是0
                    charactor.pos = 0;
                    CharactorList = new List<RoomCharactor>();
                    CharactorList.Add(charactor);

                    allCharactors[camp] = CharactorList;
                }

            }
        }

        //退出房间
        public void QuitRoom(RoomCharactor charactor)
        {
            if (charactor != null)
            {

                string UID = charactor.UID;
                WarCamp camp = charactor.camp;

                List<RoomCharactor> CharactorList = null;
                bool found = allCharactors.TryGetValue(camp, out CharactorList);
                if (found)
                {

                    RoomCharactor existC = CharactorList.Find(c => c.UID == UID);
                    if (existC != null)
                    {
                        CharactorList.RemoveAt(existC.pos);

                        //resize pos
                        int cnt = CharactorList.Count;
                        for (int i = 0; i < cnt; ++i)
                        {
                            CharactorList[i].pos = i;
                        }

                    }

                }

            }
        }

        //主机退出了
        public void HostQuit()
        {
            allCharactors.Clear();
        }

        //切换英雄
        public bool SwitchActiveHero(SwitchInfo si)
        {

            bool ok = false;
            if (si != null)
            {
                List<RoomCharactor> CharactorList = null;
                bool found = allCharactors.TryGetValue(si.camp, out CharactorList);
                if (found)
                {
                    RoomCharactor existC = CharactorList.Find(c => c.UID == si.ClientID);
                    if (existC != null)
                    {
                        WrapperTeam wTeam = (WrapperTeam)existC.team;
                        ok = true;

						ServerLifeNpc npc = wTeam.realTeam.Find(s => s.UniqueID == si.UniqueID);
						wTeam.activeNpc = npc;

                        AsyncTask.QueueOnMainThread(
                            () =>
                            {
                                if (npc != null)
                                {
                                    //前一个主英雄英雄切换为自动战斗
                                    if (WarServerManager.Instance.battleStart)
                                        wTeam.activeNpc.SwitchAutoBattle(true);
                                
                                    if (WarServerManager.Instance.battleStart)
                                        wTeam.activeNpc.SwitchAutoBattle(existC.autoBattle);
                                }
							
                            }
                        );
                    }
                }
            }
            return ok;
        }

        //    切换手动和自动
        //-- 主要是切换为自动 --
        public bool SwitchManulOrAuto(ManualOrAuto ma, ref bool isAuto)
        {
            bool exist = false;

            if (ma != null)
            {
                WarCamp camp = ma.camp;
                string UID = ma.ClientID;
                int UniqueID = ma.UniqueID;

                List<RoomCharactor> CharactorList = null;
                bool found = allCharactors.TryGetValue(camp, out CharactorList);
                if (found)
                {
                    RoomCharactor existC = CharactorList.Find(c => c.UID == UID);
                    if (existC != null)
                    {
                        WrapperTeam wrapperTeam = (WrapperTeam)existC.team;
                        if (wrapperTeam.activeNpc.UniqueID == UniqueID)
                        {

                            existC.autoBattle = ma.auto == 1;
                            exist = true;
                            isAuto = existC.autoBattle;
                            AsyncTask.QueueOnMainThread(
                                () =>
                                {
                                    wrapperTeam.activeNpc.SwitchAutoBattle(existC.autoBattle);
                                }
                            );

                        }

                    }
                }
            }

            return exist;

        }

        //进入战斗场景
        public void EnterWarScene()
        {
            makeUpData(WarCamp.FirstCamp);
            makeUpData(WarCamp.SecondCamp);
        }

        void makeUpData(WarCamp camp)  {

            if (camp == WarCamp.FirstCamp || camp == WarCamp.SecondCamp)
            {
                List<RoomCharactor> chalist = null;
                bool exist = allCharactors.TryGetValue(camp, out chalist);
                if (exist)  {
                    foreach (RoomCharactor charactor in chalist) {
						if(charactor.team is WrapperTeam) continue;

                        WrapperTeam wTeam = new WrapperTeam()
                        {
                            dataTeam = new List<NPCData>(),
						};

                        foreach (RoomNpc npc in charactor.team.team)  {
                            NPCConfigData config = Model.get(npc.NpcNum);
                            NPCData data = new NPCData()
                            {
                                configData = config,
                                rtData = new NPCRuntimeData(config),
                            };
                            wTeam.dataTeam.Add(data);
                        }
						wTeam.team = charactor.team.team;

                        charactor.team = wTeam;
                    }
                }
			}

        }

        public List<RoomCharactor> get(WarCamp camp)
        {
            List<RoomCharactor> chalist = null;
            bool exist = allCharactors.TryGetValue(camp, out chalist);
            return exist ? chalist : null;
        }

        //NPC是否控制的主英雄
        public bool IsHeroActive(ServerLifeNpc npc)
        {
            WarCamp warCamp = WarCamp2Camp.toWarCamp(npc.Camp);
            List<RoomCharactor> chaList = get(warCamp);
            if (chaList != null)
            {
                int len = chaList.Count;
                for (int i = 0; i < len; i++)
                {
                    WrapperTeam team = chaList[i].team as WrapperTeam;
                    if (team.activeNpc.UniqueID == npc.UniqueID)
                        return true;
                }
            }

            return false;
        }

        //是否队伍中的英雄
        public bool IsTeamHero(int uniqueID)
        {
            foreach (KeyValuePair<WarCamp, List<RoomCharactor>> itor in allCharactors)
            {
                List<RoomCharactor> list = itor.Value;
                int len = list.Count;
                for (int i = 0; i < len; i++)
                {
                    WrapperTeam team = list[i].team as WrapperTeam;
                    List<ServerLifeNpc> npcList = team.realTeam;
                    int npcCnt = npcList.Count;
                    for (int j = 0; j < npcCnt; j++)
                    {
                        if (npcList[j].UniqueID == uniqueID)
                            return true;
                    }
                }
            }

            return false;
        }

       
      


        /// <summary>
        /// 判定某个UniqueID是否处于激活状态
        /// </summary>
        /// <returns><c>true</c>, if under active status was ised, <c>false</c> otherwise.</returns>
        /// <param name="camp">Camp.</param>
        /// <param name="UniqueID">Unique I.</param>
        public ServerLifeNpc findActionNpc(WarUIInfo switchInfo)
        {
            ServerLifeNpc activeOne = null;

            if (switchInfo != null)
            {
                WarCamp camp = switchInfo.camp;
                string UID = switchInfo.ClientID;
                int UniqueID = switchInfo.uniqueId;

                List<RoomCharactor> CharactorList = null;
                bool found = allCharactors.TryGetValue(camp, out CharactorList);
                if (found)
                {
                    RoomCharactor existC = CharactorList.Find(c => c.UID == UID);
                    if (existC != null)
                    {
                        WrapperTeam wrapperTeam = (WrapperTeam)existC.team;
                        if (wrapperTeam.activeNpc.UniqueID == UniqueID)
                        {

                            activeOne = wrapperTeam.activeNpc;

                        }

                    }
                }
            }

            return activeOne;
        }
    }

}