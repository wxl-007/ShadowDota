using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem
{
    [TaskCategory("Basic/ParticleSystem")]
    [TaskDescription("Enables or disables the Particle System emission.")]
    public class SetEnableEmission : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Enable the ParticleSystem emissions?")]
        public SharedBool enable;

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

            targetParticleSystem.enableEmission = enable.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            enable = false;
        }
    }
}