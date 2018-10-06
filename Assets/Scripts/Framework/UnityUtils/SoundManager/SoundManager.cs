/*
 * Created By Allen Wu. All rights reserved.
 */

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using AW.IO;
using AW.Sound;
using AW.Data;
using AW.Resources;


public class SoundManager : KVModelBase<int, SoundData> {
	private const int AUDIO_EFFECT = 0xFE;
	private const int AUDIO_BMG = 0x01;
	//声效的路径
	private const string AUDIO_ROOT_PATH = "Audio/";

	private UserConfigManager uMgr;
	private bool bMute;
	private bool cached = true;

    private AudioLoader mAudioLoader = null;

    public SoundManager(UserConfigManager u, AudioLoader al) {
		cached = true;
        mAudioLoader = al;
		uMgr = u;
		bMute = uMgr.UserConfig.mute == 0 ? false : true;
		base.load(ConfigType.SoundConfig);

	}

	public override bool loadFromConfig () {
		return true;
	}

	/// <summary>
	/// 在第一次运行Android平台上，因为执行顺序的问题，Audio Data会没加载上
	/// </summary>
	public void reloadConfig() {
		instance.Clear();
		base.load(ConfigType.SoundConfig);
	}


	#region 获取声效文件的名字

	public string getBGM (SceneBGM bgm) {
		string fileName = string.Empty;
		SoundData sound = null;
		if(instance.TryGetValue((int)bgm, out sound)) {
			fileName = sound.name;
		}
		return fileName;
	}

	public string getSoundFx(SoundFx fx) {
		string fileName = string.Empty;
		SoundData sound = null;
		if(instance.TryGetValue((int)fx, out sound)) {
			fileName = sound.name;
		}
		return fileName;
	}

	public string getBtnFX(ButtonType btn) {
		string fileName = string.Empty;
		SoundData sound = null;
		if(instance.TryGetValue((int)btn, out sound)) {
			fileName = sound.name;
		}
		return fileName;
	}

	#endregion


	#region 各种方便的操作


    /// <summary>
    /// 新手引导过程播放的声音,使用完就释放
    /// </summary>
    /// <returns>The fx play.</returns>
    /// <param name="fx">Fx.</param>
    /// <param name="SoundFinished">Sound finished.</param>
    /// <param name="DefaultLayer">Default layer.</param>
    public int GuideFxPlay(SoundFx fx, System.Action SoundFinished = null, int DefaultLayer = 0x02) {
        int layer = -1;
        if(!bMute) {
            string fileName = getSoundFx(fx);
            if(string.IsNullOrEmpty(fileName)) {
                ConsoleEx.DebugLog("Can't find Button Sound Effect. Button Type = " + fx.ToString());
            } else {
                AudioClip clip = null;
				clip = Core.ResEng.getLoader<SoundLoader>().load(AUDIO_ROOT_PATH + fileName, cached);
                layer = Core.SoundEng.PlayClipForce(clip, DefaultLayer, false, 1.0f);

                if(SoundFinished != null) {
                    AsyncTask.QueueOnMainThread(SoundFinished, clip.length);
                }

                clip = null;
            }
        }
        return layer;
    }

    /// <summary>
    /// 特效声音的播放, 需要管理内存, 第三个参数支持循环播放声效
    /// </summary>
    /// <param name="fx">Fx.</param>
	public int SoundFxPlay(SoundFx fx, System.Action SoundFinished = null, bool loop = false) {
        int layer = -1;
		if(!bMute) {
			string fileName = getSoundFx(fx);
			if(string.IsNullOrEmpty(fileName)) {
				ConsoleEx.DebugLog("Can't find Sound Effect. Sound Type = " + fx.ToString());
			} else {
				AudioClip clip = null;
				clip = Core.ResEng.getLoader<SoundLoader>().load(AUDIO_ROOT_PATH + fileName, cached);
				layer = Core.SoundEng.PlayClip(clip, AUDIO_EFFECT, loop);

                if(SoundFinished != null) {
                    AsyncTask.QueueOnMainThread(SoundFinished, clip.length);
                }
			}
		}
        return layer;
	}

	/// <summary>
	/// 播放背景音乐
	/// </summary>
    public void BGMPlay(SceneBGM BGM) {
		if(!bMute) {
			string fileName = getBGM(BGM);
			if(string.IsNullOrEmpty(fileName)) {
				ConsoleEx.DebugLog("Can't find Button Sound Effect. Button Type = " + BGM.ToString());
			} else {
				AudioClip clip = null;
				clip = Core.ResEng.getLoader<SoundLoader>().load(AUDIO_ROOT_PATH + fileName, cached);
                Core.SoundEng.PlayClipForce(clip, AUDIO_BMG, true, 0.8f);
			}
		}
	}

    /// <summary>
    /// 关闭背景音乐
    /// </summary>
    public void BGMStop() {
        if(!bMute) {
            Core.SoundEng.StopChannel(0);
        }
    }  

	/// <summary>
	/// 按钮的音效
	/// </summary>
	/// <param name="type">Type.</param>
	public void BtnPlay(ButtonType type) {
		if(!bMute) {
			AudioClip clip = null;
			switch(type) {
			case ButtonType.Confirm:
				clip = Core.ResEng.getLoader<SoundLoader>().load(AUDIO_ROOT_PATH + "sfx_button", cached);
				break;
			case ButtonType.Cancel:
				clip = Core.ResEng.getLoader<SoundLoader>().load(AUDIO_ROOT_PATH + "sfx_button1", cached);
				break;
			}
			if(clip != null) Core.SoundEng.PlayClip(clip, AUDIO_EFFECT, false);
		}
	}

	public bool SoundMute {
		get {
			return bMute;
		}
	}

	/// <summary>
	/// 这个方法只能提供给GameUI或GameBattle场景使用，
	/// </summary>
	public void SwitchSound(bool usedInBattle = false) {
		bMute = !bMute;

		uMgr.UserConfig.mute = bMute ? (short)1 : (short)0;
		uMgr.save();
        if(bMute) {
            Core.SoundEng.StopChannel(0);
        } else {
			string fileName = getBGM( usedInBattle ? SceneBGM.BGM_BATTLE : SceneBGM.BGM_GAMEUI);
            AudioClip clip = null;
			clip = Core.ResEng.getLoader<SoundLoader>().load(AUDIO_ROOT_PATH + fileName, cached);
            Core.SoundEng.PlayClipForce(clip, AUDIO_BMG, true, 0.8f);
        }
            
	}

	#endregion
}


