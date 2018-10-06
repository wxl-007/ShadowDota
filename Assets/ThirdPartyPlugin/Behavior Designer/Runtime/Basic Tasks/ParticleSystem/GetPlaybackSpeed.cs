using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
    [TaskCategory("Basic/ParticleSystem")]
    [TaskDescription("Stores the playback speed of the Particle System.")]
    public class GetPlaybackSpeed : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The playback speed of the ParticleSystem")]
        [RequiredField]
        public SharedFloat storeResult;

        private ParticleSystem targetParticleSystem;

        public override void OnStart()
        {
            targetParticleSystem = GetDefaultGameObject(targetGameObject.Value).GetComponent<ParticleSystem>();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetParticleSystem == null) {
                Debug.LogWarning("ParticleSystem is null");
                return TaskStatus.Failure;
            }

            storeResult.Value = targetParticleSystem.playbackSpeed;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeResult = 0;
        }
    }
}