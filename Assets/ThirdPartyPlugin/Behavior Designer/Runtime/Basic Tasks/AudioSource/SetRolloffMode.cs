using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
    [TaskCategory("Basic/AudioSource")]
    [TaskDescription("Sets the rolloff mode of the AudioSource. Returns Success.")]
    public class SetRolloffMode : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The rolloff mode of the AudioSource")]
        public AudioRolloffMode rolloffMode;

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

            audioSource.rolloffMode = rolloffMode;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            rolloffMode = AudioRolloffMode.Logarithmic;
        }
    }
}