using UnityEngine;
using System.Collections;

namespace AW.War
{
    public class AttachToTarget : SkillEffectBase
    {

        ClientNPC Target;

        ClientNPC Parent;

        public float height;

        EffectDef ed;

        public override void EmitEffect(ClientNPC _from, ClientNPC _target, bool _needUpdate = true)
        {
            Target = _target;
            Parent = _from;
            transform.parent = Target.transform;
            transform.localPosition = Vector3.zero;
            ClientNpcAnimState cna = Target.animState;
            if (cna == null)
            {
                transform.localPosition = Vector3.up * 2f;
            }
            else
            {
                ed = GetComponent<EffectDef>();
                Transform cm = cna.getModelPart(ed.End);
                if (cm != null)
                {
                    transform.parent = cm;
                    transform.localPosition = Vector3.up * 0.5f;
                }
                else
                {
                    transform.localPosition = Vector3.up * 2f;
                }
            }
//            transform.parent = Target.transform;
        }

        public override void LifeTime(float lifeTime)
        {

        }
    }
}
