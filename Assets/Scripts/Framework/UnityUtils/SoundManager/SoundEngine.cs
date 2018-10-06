/*
 * Created By Allen Wu. All rights reserved.
 */

using UnityEngine;
using System.Collections;
using System.IO;

///目前我开放了8个音效Layer， 其中第0Layer作为BGM，后面7个作为声效文件
public class SoundEngine : MonoBehaviour {
	private const int AUDIO_SOURCE_MAX_COUNT = 8;
	/// <summary>
	/// Returns (creating if necessary) the SoundManager singleton.
	/// </summary>
	/// <returns>
	/// The SoundManager singleton.
	/// </returns>
	public static SoundEngine GetSingleton () {

		if (sGameObj == null) {
			sGameObj = new GameObject();
			sGameObj.name = "AudioManager";
			return (SoundEngine) sGameObj.AddComponent(typeof(SoundEngine));
		}
		return (SoundEngine) sGameObj.GetComponent(typeof(SoundEngine));

	}

	/// <summary>
	/// Immediately sets volume of the specified channel.
	/// </summary>
	/// <param name="i">Channel number</param>
	/// <param name="newVol">New volume setting, 0.0f to 1.0f</param>
	public void SetVolume(int i, float newVol) {
		oldVolume[i] = newVol;
		newVolume[i] = newVol;
		sources[i].volume = newVol;
	}

	/// <summary>
	/// Linearly interpolates volume of the specified channel
	/// from current value to the new value during the specified time.
	/// </summary>
	/// <param name="i">Channel number</param>
	/// <param name="newVol">New volume setting, 0.0f to 1.0f</param>
	/// <param name="time">Time in seconds</param>
	public void SetVolume(int i, float newVol, float time) {
		oldVolume[i] = sources[i].volume;
		newVolume[i] = newVol;
		transitionStart[i] = Time.time;
		transitionTime[i] = time;
	}


	/// <summary>
	/// Immediately sets volume of the specified clip. The difference
	/// between this method and SetVolume() taking channel number as
	/// parameter is that this method will effect the setting for all
	/// channels playing the given clip.
	/// </summary>
	/// <param name="c">Audio clip</param>
	/// <param name="newVol">New volume setting, 0.0f to 1.0f</param>
	public void SetVolume(AudioClip c, float newVol) {
		for (int i = 0; i < sources.Length; i++) {
			AudioSource s = sources[i];
			if (s.clip == c) {
				oldVolume[i] = newVol;
				newVolume[i] = newVol;
				s.volume = newVol;
			}
		}
	}

	/// <summary>
	/// Linearly interpolates volume of the specified clip
	/// from current value to the new value during the specified time.
	/// The difference between this method and SetVolume() taking channel
	/// number as parameter is that this method will effect the setting for all
	/// channels playing the given clip.
	/// </summary>
	/// <param name="c">Audio clip</param>
	/// <param name="newVol">New volume setting, 0.0f to 1.0f</param>
	/// <param name="time">Time in seconds</param>
	public void SetVolume(AudioClip c, float newVol, float time) {
		for (int i = 0; i < sources.Length; i++) {
			AudioSource s = sources[i];
			if (s.clip == c) {
				oldVolume[i] = s.volume;
				newVolume[i] = newVol;
				transitionStart[i] = Time.time;
				transitionTime[i] = time;
			}
		}
	}


	/// <summary>
	/// Plays given audio clip on any free channel.
	/// </summary>
	/// <param name="c">Audio clip</param>
	/// <param name="loop">Loop setting</param>
	/// <returns>Number of the assigned channel</returns>
	public int PlayClip(AudioClip c, bool loop) {
		for (int i = 0; i < sources.Length; i++) {
			AudioSource s = sources[i];
			if (!s.isPlaying) {
				s.clip = c;
				s.loop = loop;
				s.Play();
				SetVolume(i, 1.0f);
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Plays given audio clip on any free channel included in the mask.
	/// </summary>
	/// <param name="c">Audio clip</param>
	/// <param name="mask">Channel mask, e.g. to specify 0th, 3rd and 11th channel, use 0x0809</param>
	/// <param name="loop">Loop setting</param>
	/// <returns>Number of the assigned channel</returns>
	public int PlayClip(AudioClip c, int mask, bool loop) {
		for (int i = 0; i < sources.Length; i++) {
			if ((mask & (1 << i)) > 0 && !sources[i].isPlaying) {
				sources[i].clip = c;
				sources[i].loop = loop;
				sources[i].Play();
				SetVolume(i, 1.0f);
				return i;
			}
		}
		return -1;
	}

    public int PlayClipForce(AudioClip c, int mask, bool loop, float volumn) {
		for (int i = 0; i < sources.Length; i++) {
			if ((mask & (1 << i)) > 0) {
				sources[i].clip = c;
				sources[i].loop = loop;
				sources[i].Play();
                SetVolume(i, volumn);
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Stops all channels playing the given clip.
	/// </summary>
	/// <param name="c">Audio clip</param>
	public void StopClip(AudioClip c) {
		foreach (AudioSource s in sources) {
			if (s.clip == c && s.isPlaying) {
				s.Stop();
			}
		}
	}

	/// <summary>
	/// Stops the given channel.
	/// </summary>
	/// <param name="i">Channel number</param>
	public void StopChannel(int i) {
		if(i >= 0 && i < AUDIO_SOURCE_MAX_COUNT)
			sources[i].Stop();
	}

	/// <summary>
	/// Utility function for changing music between levels.
	/// Has to be called using StartCoroutine(). Requires System.IO, therefore
	/// for maximum portability you might want to remove it along with the
	/// System.IO dependency.
	///
	/// The function lists all .ogg files in the music/ subdirectory
	/// picks one on random and starts playing it, fading out any previous music.
	/// Calling it once at the beginning of a level should achieve what people
	/// usually want.
	///
	/// Example:
	/// SoundManager sm = SoundManager.GetSingleton();
	/// StartCoroutine(sm.ShuffleMusic());
	/// </summary>
	public IEnumerator ShuffleMusic() {
		// Music
		DirectoryInfo di = new DirectoryInfo("music");
		FileInfo[] fi = di.GetFiles("*.ogg");
		WWW www = new WWW("file://" + fi[musicChoice++ % fi.Length].FullName);

		yield return www;

		int oldChan = musicChan;
		if (oldChan >= 0) {
			SetVolume(oldChan, 0, 5);
		}
		musicChan = PlayClip(www.GetAudioClip(false, true), true);
		if (oldChan >= 0) {
			SetVolume(musicChan, 0);
			SetVolume(musicChan, 0.5f, 5);
		} else {
			SetVolume (musicChan, 0.5f);
		}
	}


	// --------------
	// PRIVATE
	// --------------

	static GameObject sGameObj;
	int musicChan = -1;
	int musicChoice = (int)(Random.value * int.MaxValue);

	AudioSource[] sources;

	float[] oldVolume;
	float[] newVolume;
	float[] transitionStart;
	float[] transitionTime;
	Transform cam;

	private SoundEngine() { }

	void OnLevelWasLoaded(int level) {
		GameObject cam_object = GameObject.FindGameObjectWithTag("MainCamera");
		if(cam_object != null){
			cam = cam_object.transform;
			for (int i = 0; i < sources.Length; i++) {
				if (!sources[i].loop) {
					sources[i].Stop();
				}
			}
		}
	}

	void Awake() {
		DontDestroyOnLoad(sGameObj);

		sources = new AudioSource[AUDIO_SOURCE_MAX_COUNT];

		for (int i = 0; i < sources.Length; i++) {
			sources[i] = (AudioSource) sGameObj.AddComponent(typeof(AudioSource));
		}

		var gobal = GameObject.FindGameObjectWithTag("Global");
		if(gobal != null) transform.parent = gobal.transform;

		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		transform.position = cam.position;

		oldVolume = new float[sources.Length];
		newVolume = new float[sources.Length];
		transitionStart = new float[sources.Length];
		transitionTime = new float[sources.Length];

		for (int i = 0; i < AUDIO_SOURCE_MAX_COUNT; i++) {
			oldVolume[i] = 1.0f;
			newVolume[i] = 1.0f;
			transitionStart[i] = 0.0f;
			transitionTime[i] = 0.00001f;
		}
	}

	void Update() {
		for (int i = 0; i < AUDIO_SOURCE_MAX_COUNT; i++) {
			sources[i].volume = Mathf.Lerp(oldVolume[i], newVolume[i], Mathf.Min(1.0f, (Time.time - transitionStart[i]) / transitionTime[i]));
			if (newVolume[i] <= 0 && sources[i].volume <= 0 && sources[i].isPlaying) {
				sources[i].Stop();
			}
		}
	}
}


