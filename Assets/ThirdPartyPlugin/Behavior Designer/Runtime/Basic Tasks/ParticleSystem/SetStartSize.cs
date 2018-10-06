using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
    [TaskCategory("Basic/ParticleSystem")]
    [TaskDescription("Sets the start size of the Particle System.")]
    public class SetStartSize : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The start size of the ParticleSystem")]
        public SharedFloat startSize;

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

            targetParticleSystem.startSize = startSize.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            startSize = 0;
        }
    }
}