using UnityEngine;
using System.Collections;
using AW.FSM;

public static class UnityUtils {

	static public void AddChild (GameObject child, GameObject parent, Vector3? LocalPostion = null) {

		if (child !=null && parent != null) {
			Transform t = child.transform;
			t.parent = parent.transform;
			if(LocalPostion == null) LocalPostion = Vector3.zero;
			t.localPosition = (Vector3)LocalPostion;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			child.layer = parent.layer;
		}
	}

	static public void AddChild_Reverse(GameObject child, GameObject parent) {
		if (child != null && parent != null) {
			Transform t = child.transform;
			t.parent = parent.transform;
			t.localScale = Vector3.one;
		}
	}

	static public void JumpToScene(GamePlayFSM gameplay, string target) {
		if( !string.IsNullOrEmpty(target) ) {
			Application.LoadLevel(target);

			if(gameplay != null) {
				StateParam<GameState> param = new StateParam<GameState>();
				param.NowGameState = GameState.LevelChanged;
				param.obj = new LevelChanged() {
					curLevel    = Application.loadedLevelName,
					targetLevel = target,
				};
				gameplay.handleStateChg(param, GameState.LevelChanged);
			}
		}
	}
}
