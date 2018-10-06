using UnityEngine;
using System.Collections;

namespace AW.War
{
    public class ServerFB : MonoBehaviour
    {
        #region 参数
        /// <summary>
        /// 最大移动距离
        /// </summary>
        private float maxDis = 0f;
        /// <summary>
        /// 速度
        /// </summary>
        private float speed = 0f;
        /// <summary>
        /// 单帧内的移动距离
        /// </summary>
        private float frameDis = 0f;
        /// <summary>
        /// 是否初始化过
        /// </summary>
        private bool inited = false;
        /// <summary>
        /// 攻击的序号
        /// </summary>
        public int attackIndex = -1;
        /// <summary>
        /// 计算位置用
        /// </summary>
        Vector3 pos = Vector3.zero;
        float dis = 0f;
        public bool followTarget;
        #endregion

        #region  NPC
        private WarServerManager wmMgr;
        private ServerNPC owner;
        private ServerNPC target;
        private WarSrcAnimParam sphereParam;
        public int npcId;
        #endregion

        #region 组件
        private Transform tran;
        #endregion

        // Use this for initialization
        void Start()
        {
	
        }
	
        // Update is called once per frame
        void Update () {
            if (inited)
            {
                Move();
            }
        }

        void Move()
        {
            if (target != null)
            {
                pos = target.transform.position;
                pos.y = tran.position.y;
                tran.LookAt(pos);
                dis = Vector3.Distance(tran.position, pos);
                if(dis < 1f)
                {
                    WarTarAnimParam[] tars = sphereParam.InjureTar;
                    for(int i = 0; i < tars.Length; i++)
                    {
                        wmMgr.npcMgr.SendMessageAsync(owner.UniqueID, tars[i].described.target, tars[i]);
                    }
                    Destroy(gameObject);
                    return;
                }
            }

            frameDis = speed * Time.deltaTime;
            tran.Translate(Vector3.forward * frameDis);

            if (target == null)
            {
                maxDis -= frameDis;
                if (maxDis <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Init(ServerNPC parent, WarMsgParam param)
        {
            owner = parent;
            wmMgr = WarServerManager.Instance;
            tran = transform;
            if(param is WarSrcAnimParam)
            {
                WarSrcAnimParam wp = param as WarSrcAnimParam;
                sphereParam = wp;
                WarTarAnimParam[] tars = wp.InjureTar;
                speed = wp.described.targetEnd.param8;
                maxDis = wp.described.targetEnd.param9;
                if (tars != null && tars.Length > 0)
                {
                    SelfDescribed sd = tars[0].described;
                    npcId = sd.targetEnd.param5;
                    if (wmMgr != null)
                    {
                        target = wmMgr.npcMgr.GetNPCByUniqueID(npcId);
                        if (target != null && parent.outLog)
                        {
                            Debug.Log("Sphere target : " + target.name);
                        }
                    }
                }
                else
                {
                    target = null;
                }
            }
            inited = true;
        }
    }
}
