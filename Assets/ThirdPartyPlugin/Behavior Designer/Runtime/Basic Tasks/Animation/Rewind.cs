using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
    [TaskCategory("Basic/Animation")]
    [TaskDescription("Rewinds an animation. Rewinds all animations if animationName is blank. Returns Success.")]
    public class Rewind : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the animation")]
        public SharedString animationName;

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

            if (string.IsNullOrEmpty(animationName.Value)) {
                targetAnimation.Rewind();
            } else {
                targetAnimation.Rewind(animationName.Value);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            animationName.Value = "";
        }
    }
}