using System;

namespace AW.Sound {
	//背景音效的枚举
	public enum SceneBGM {
		BGM_Login  = 0x01,
		BGM_GAMEUI = 0x02,
		BGM_BATTLE = 0x03,
		BGM_BOSS   = 0x04,
		BGM_PROLOG = 0x05,
		BGM_14YEAR = 0x06,
		BGM_CG_BiKe= 0x07,
	}

	//声效的枚举
	public enum SoundFx {
		FX_Searching = 0xA0,
	}

	public enum ButtonType {
		Confirm = 0x01,
		Cancel  = 0x02, 
	}

}

