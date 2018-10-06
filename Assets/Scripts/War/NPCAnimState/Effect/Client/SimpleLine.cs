using UnityEngine;
using System.Collections;
using AW.War;
using AW.Message;
using AW.Data;

namespace AW.War
{
    public class SimpleLine : SkillEffectBase
    {
        private BNPC From;
        private BNPC Target;
        public LineRenderer line;
        private Vector3 p1 = Vector3.zero, p2 = Vector3.zero;

        // Use this for initialization
        void Awake()
        {
       
        }
	
        // Update is called once per frame
        void Update()
        {
            UpdateLine();
        }

        void FixedUpdate()
        {
            if (From == null)
            {
                Destroy(gameObject);
            }
        }

        public override void EmitEffect(ClientNPC _from, ClientNPC _target, bool _needUpdate = true)
        {
            From = _from;
            Target = _target;
        }

        public override void LifeTime(float lifeTime)
        {

        }

        private void UpdateLine()
        {
            if (From != null && Target != null)
            {
                p1 = From.transform.position;
                p1.y = 2;
                p2 = Target.transform.position;
                p2.y = 2;
                line.SetPosition(0, p1);
                line.SetPosition(1, p2);
            }
        }
    }
}