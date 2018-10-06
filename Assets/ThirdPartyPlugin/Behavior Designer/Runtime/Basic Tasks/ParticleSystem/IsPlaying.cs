using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
    [TaskCategory("Basic/ParticleSystem")]
    [TaskDescription("Is the Particle System playing?")]
    public class IsPlaying : Conditional
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

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

            return targetParticleSystem.isPlaying ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}