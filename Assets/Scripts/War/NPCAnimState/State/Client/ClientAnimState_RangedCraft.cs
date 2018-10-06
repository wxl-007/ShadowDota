using UnityEngine;
using System.Collections;

namespace AW.War
{
    public class ClientAnimState_RangedCraft : ClientNpcAnimState {

    	// Use this for initialization
        public override void Start () {
            base.Start();
    	}
    	
    	// Update is called once per frame
        public override void Update () {
            base.Update();
    	}

        public override void ResetActiveAnimState()
        {
            animator.SetBool("isAttack_1", false);
        }
    }
}
