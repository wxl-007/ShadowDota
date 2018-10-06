using UnityEngine;
using System.Collections;

namespace AW.War
{
    public class ClientAnimState_3004 : ClientNpcAnimState {

    	// Use this for initialization
        public override void Start () {
            base.Start();
    	}
    	
    	// Update is called once per frame
        public override void Update () {
            base.Update();
    	}

        public override void OnSwitchRun(bool run)
        {

        }

        public override IEnumerator OnSwitchRun(bool run, bool smooth = false)
        {
            yield return null;
        }

        public override void ResetActiveAnimState()
        {

        }

        public override void On_Destroy(WarMsgParam param)
        {

        }
    }
}
