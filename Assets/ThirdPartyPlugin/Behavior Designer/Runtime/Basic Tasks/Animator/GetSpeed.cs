using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
    [TaskCategory("Basic/Animator")]
    [TaskDescription("Stores the playback speed of the animator. 1 is normal playback speed. Returns Success.")]
    public class GetSpeed : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The playback speed of the Animator")]
        [RequiredField]
        public SharedFloat storeValue;

        private Animator animator;

        public override void OnStart()
        {
            animator = GetDefaultGameObject(targetGameObject.Value).GetComponent<Animator>();
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = animator.speed;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = 0;
        }
    }
}