#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
    [TaskCategory("Basic/AudioSource")]
    [TaskDescription("Plays the audio clip with a delay specified in seconds. Returns Success.")]
    public class PlayScheduled : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Time in seconds on the absolute time-line that AudioSettings.dspTime refers to for when the sound should start playing")]
        float time = 0;

        private AudioSource audioSource;

        public override void OnStart()
        {
            audioSource = GetDefaultGameObject(targetGameObject.Value).GetComponent<AudioSource>();
        }

        public override TaskStatus OnUpdate()
        {
            if (audioSource == null) {
                Debug.LogWarning("AudioSource is null");
                return TaskStatus.Failure;
            }

            audioSource.PlayScheduled(time);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            time = 0;
        }
    }
}
#endif