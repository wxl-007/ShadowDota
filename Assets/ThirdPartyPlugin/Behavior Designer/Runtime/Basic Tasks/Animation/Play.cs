using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
    [TaskCategory("Basic/Animation")]
    [TaskDescription("Plays animation without any blending. Returns Success.")]
    public class Play : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the animation")]
        public SharedString animationName;
        [Tooltip("The play mode of the animation")]
        public PlayMode playMode = PlayMode.StopSameLayer;

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
                targetAnimation.Play();
            } else {
                targetAnimation.Play(animationName.Value, playMode);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            animationName.Value = "";
            playMode = PlayMode.StopSameLayer;
        }
    }
}