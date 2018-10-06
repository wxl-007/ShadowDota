using UnityEngine;
using System.Collections;

namespace AW.War
{
    public class SimpleMoveForward : SkillEffectBase
    {

        Transform tran;
        bool inited;

        void Update()
        {
            if(inited)
            {
                tran.Translate(Vector3.forward * 25f * Time.deltaTime);
            }
        }

        public override void EmitEffect(ClientNPC _from, ClientNPC _target, bool _needUpdate = true)
        {
            tran = transform;
            inited = true;
        }

        public override void LifeTime(float lifeTime)
        {
            Destroy(gameObject, lifeTime);
        }
    }
}
