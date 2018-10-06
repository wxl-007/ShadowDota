using UnityEngine;
using System.Collections;

namespace AW.Data
{
    [Modle(type = DataSource.FromLocal)]
    public class Effect3DModel : KVModelBase<int, Effect3DModelConfigData> {
        public override bool loadFromConfig() {
            return base.load(ConfigType.Effect3DModel);
        }
    }

    [System.Serializable]
    public class Effect3DModelConfigData : UniqueBaseData <int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID;

        /// <summary>
        /// 起手
        /// </summary>
        public string Start;

        /// <summary>
        /// 出手
        /// </summary>
        public string Middle;

        /// <summary>
        /// 收手
        /// </summary>
        public string End;

        /// <summary>
        /// 动画片段
        /// </summary>
        public string Anim;

        public override int getKey() {
            return this.ID;
        }
    }

}