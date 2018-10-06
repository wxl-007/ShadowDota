using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
    [TaskCategory("Basic/Animation")]
    [TaskDescription("Sets the wrap mode to the specified value. Returns Success.")]
    public class SetWrapMode : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("How should time beyond the playback range of the clip be treated?")]
        public WrapMode wrapMode = WrapMode.Default;

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

            targetAnimation.wrapMode = wrapMode;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            wrapMode = WrapMode.Default;
        }
    }
}