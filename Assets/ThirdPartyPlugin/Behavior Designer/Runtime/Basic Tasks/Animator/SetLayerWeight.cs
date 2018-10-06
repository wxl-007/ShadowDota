using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
    [TaskCategory("Basic/Animator")]
    [TaskDescription("Sets the layer's current weight. Returns Success.")]
    public class SetLayerWeight : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The layer's index")]
        public SharedInt index;
        [Tooltip("The weight of the layer")]
        public SharedFloat weight;

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

            animator.SetLayerWeight(index.Value, weight.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            index = 0;
            weight = 0;
        }
    }
}