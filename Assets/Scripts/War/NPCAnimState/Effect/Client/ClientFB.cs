using UnityEngine;
using System.Collections;

namespace AW.War
{
    public class ClientFB : MonoBehaviour
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
        /// The child.
        /// </summary>
        public GameObject child;
        /// <summary>
        /// 是否初始化过
        /// </summary>
        private bool inited = false;
        /// <summary>
        /// 计算位置用
        /// </summary>
        Vector3 pos = Vector3.zero;
        float dis = 0f;
        #endregion

        #region  NPC
        private WarClientManager wmMgr;
        private Transform Target;
        #endregion

        #region 组件
        private Transform tran;
        #endregion

        // Use this for initialization
        void Start()
        {
	
        }
	
        // Update is called once per frame
        void Update()
        {
            if (inited)
            {
                Move();
            }
        }

        void Move()
        {
            if (Target != null)
            {
                pos = Target.transform.position;
                pos.y = 2;
                tran.LookAt(pos);
                dis = Vector3.Distance(tran.position, pos);
                if(dis < 1f)
                {
                    EmitChild();
                    return;
                }
            }

            frameDis = speed * Time.deltaTime;
            tran.Translate(Vector3.forward * frameDis);

            if (Target == null)
            {
                maxDis -= frameDis;
                if (maxDis <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Init(AnimationMsg msg)
        {
            wmMgr = WarClientManager.Instance;
            tran = transform;
            if(msg != null)
            {
                speed = msg.arg1;
                maxDis = msg.arg2;
                if(wmMgr != null)
                {
                    BNPC npc = wmMgr.npcMgr.GetNpc(msg.targetId);
                    if(npc != null)
                    {
                        Target = npc.transform;
                    }
                }
            }
            inited = true;
        }

        void EmitChild()
        {
            if(child != null)
            {
                GameObject obj = Instantiate(child, tran.position, tran.rotation) as GameObject;
                Destroy(obj, 1f);
            }
            Destroy(gameObject);
        }
    }
}