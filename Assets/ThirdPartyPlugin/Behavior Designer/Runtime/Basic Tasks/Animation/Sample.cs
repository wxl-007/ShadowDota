using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
    [TaskCategory("Basic/Animation")]
    [TaskDescription("Samples animations at the current state. Returns Success.")]
    public class Sample : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
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

            targetAnimation.Sample();

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}