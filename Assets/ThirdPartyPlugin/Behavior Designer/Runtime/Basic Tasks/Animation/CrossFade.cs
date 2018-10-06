using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation
{
    [TaskCategory("Basic/Animation")]
    [TaskDescription("Fades the animation over a period of time and fades other animations out. Returns Success.")]
    public class CrossFade : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the animation")]
        public SharedString animationName;
        [Tooltip("The amount of time it takes to blend")]
        public float fadeLength = 0.3f;
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

            targetAnimation.CrossFade(animationName.Value, fadeLength, playMode);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            animationName.Value = "";
            fadeLength = 0.3f;
            playMode = PlayMode.StopSameLayer;
        }
    }
}