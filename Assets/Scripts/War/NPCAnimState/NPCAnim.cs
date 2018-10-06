using UnityEngine;
using System.Collections;
using AW.Data;
using System.Collections.Generic;
using AW.Framework;

namespace AW.War
{
    public enum NpcAnimState
    {
        Stand,
        Run,
        Attack,
        CastSkill,
        CastSkill_1,
        CastSkill_2,
        CastSkill_3,
        CastSkill_4,
        CastBuff,
        CastTrigger,
        Suffer,
        Killed,
        Respawn,
        Idle,
        Win,
        Lose,
        ManualInput,
        Destroying
    }

    public enum NpcAnimEffect
    {
        Attack_1,
        Attack_1_Start,
        Attack_2,
        Attack_2_Start,
        Attack_3,
        Attack_3_Start,
        Skill_1,
        Skill_1_Start,
        Skill_2,
        Skill_2_Start,
        Skill_3,
        Skill_3_Start,
        Skill_4,
        Skill_4_Start,
        Buff,
        Trigger
    }

    [System.Serializable]
    public class AnimationMsg
    {
        public int index;
        public string animationName;
        public float animationEventTimer;
        public float animationTimer;
        public int targetId;
        public float arg1;
        public float arg2;
        public Effect3DModelConfigData ecd;
        public List<Vec3F> objCrtV;

        public AnimationMsg()
        {

        }
    }

    public class WarUIInfo : CliID
    {
        public int uniqueId;
        public WarCamp camp;
    }

    public class NpcAnimInfo : CliID
    {
        public NpcAnimState nextState;
        public short index;
        public WarUIInfo ui;
        public string data;
    }

    public class NpcEffectInfo : CliID
    {
        public NpcAnimState nextState;
        public int from;
        public int to;
        public int hitAction;
        public int Op;
    }

    public class StatusInfoItem
    {
        public NpcStatus status;
        public bool isAdd;

        public StatusInfoItem(NpcStatus s, bool add)
        {
            status = s;
            isAdd = add;
        }

        public StatusInfoItem()
        {

        }
    }

    public class NpcStatusInfo : CliID
    {
        public List<StatusInfoItem> items = null;

        public void addItem(NpcStatus s, bool add)
        {
            if(items == null)
            {
                items = new List<StatusInfoItem>();
            }
            items.Add(new StatusInfoItem(s, add));
        }
    }
}
