using UnityEngine;
using System.Collections;
using AW.Data;
using AW.Message;

namespace AW.War
{
    public class SkImpFactory
    {
        public static ISkImp getSkImp(EffectOp op)
        {
            ISkImp be = null;
            switch(op)
            {
                case EffectOp.Injury:
                    be = new SkDamage_1();
                    break;
                case EffectOp.MoveTarget:
                    be = new SkMoveTarget_5();
                    break;
                case EffectOp.MoveToChild:
                    be = new SkMoveToChild_6();
                    break;
                case EffectOp.CtorNPC:
                    be = new SkCrtNpc_8();
                    break;
                case EffectOp.Bullet_NPC:
                    be = new SkBulletNpc_16();
                    break;
                default:
                    be = new EmptySkImp();
                    break;
            }
            return be;
        }
    }

    public abstract class BaseSkImp
    {
        /// <summary>
        /// 技能处理的消息
        /// </summary>
        protected WarMsgParam param;
        /// <summary>
        /// 施法者
        /// </summary>
        protected ServerNPC castor;

        protected EffectOp OP;

        protected BaseSkImp()
        {

        }
    }

    public interface ISkImp
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="npc">Npc.</param>
        /// <param name="msg">Parameter.</param>
        void InitSk(ServerNPC npc, WarMsgParam msg);
        /// <summary>
        /// 运行技能逻辑
        /// </summary>
        void CastSk(params Object[] args);
        /// <summary>
        /// 技能处理
        /// </summary>
        void SkHandle();
        /// <summary>
        /// Reset this instance.
        /// </summary>
        void Reset();
        /// <summary>
        /// Sks the op.
        /// </summary>
        /// <returns>The op.</returns>
        EffectOp SkOp();
    }

    public class EmptySkImp : ISkImp
    {
        #region ISkImp

        public void InitSk(ServerNPC npc, WarMsgParam msg)
        {

        }

        public void CastSk(params Object[] args)
        {
           
        }

        public void SkHandle()
        {

        }

        public void Reset()
        {

        }

        public EffectOp SkOp()
        {
            return EffectOp.None;
        }


        #endregion
    }
}
