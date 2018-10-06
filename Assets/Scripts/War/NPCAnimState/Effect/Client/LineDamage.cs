using UnityEngine;
using System.Collections;

namespace AW.War
{
    public class LineDamage : SkillEffectBase
    {

        public LineRenderer line;
        public float length;
        public int numOfPoint;

        public GameObject child;
        Vector3 npcPos = Vector3.zero;

        public override void EmitEffect(ClientNPC _from, ClientNPC _target, bool _needUpdate = true)
        {
            npcPos = _target.CachedTran.position;

            length = Vector3.Distance(_from.transform.position, _target.transform.position);

            int i = 0;
            numOfPoint = (int)Mathf.Abs(length/2)+1;
            line.SetVertexCount(numOfPoint);

            float interval =  length/ (numOfPoint-1);
            float pointZ = 0;

            while ( i < numOfPoint) 
            {  
                Vector3 pos = new Vector3 (0.5f*Random.Range(-1f, 1f), 0.5f*Random.Range(-1f, 1f), pointZ);

                line.SetPosition (i, pos);
                i++;
                pointZ =pointZ + interval;
            }
        }

        public override void LifeTime(float lifeTime)
        {

        }

        void OnDestroy()
        {
            if(child != null)
            {
                GameObject obj = Instantiate(child, npcPos, Quaternion.identity) as GameObject;
                Destroy(obj, 0.5f);
            }
        }
    }
}
