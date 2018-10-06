using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
    [TaskCategory("Basic/Animation")]
    [TaskDescription("Sets animate physics to the specified value. Returns Success.")]
    public class SetAnimatePhysics : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Are animations executed in the physics loop?")]
        public SharedBool animatePhysics;

        // cache the animation component
        private Animation targetAnimation;

        public override void OnStart()
        {
            targetAnimation = GetDefaultGameObject(targetGameObject.Value).GetComponent<Animation>();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetAnimation == null) {
                Debug.LogWarning("Animation is null");
                return TaskStatus.Failure;
            }

            targetAnimation.animatePhysics = animatePhysics.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            animatePhysics.Value = false;
        }
    }
}