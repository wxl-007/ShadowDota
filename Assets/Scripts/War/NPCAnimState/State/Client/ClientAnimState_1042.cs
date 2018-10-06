using UnityEngine;
using System.Collections;

namespace AW.War
{
    public class ClientAnimState_1042 : ClientNpcAnimState {

    	// Use this for initialization
        public override void Start () {
            cachedTran.localScale = Vector3.zero;
            base.Start();
            LeanTween.scale(gameObject, Vector3.one, 0.2f);
    	}
    	
    	// Update is called once per frame
        public override void Update () {
            base.Update();
    	}

        public override void ResetActiveAnimState()
        {
            animator.SetBool("isAttack_1", false);
            animator.SetBool("isAttack_2", false);
        }
    }
}
