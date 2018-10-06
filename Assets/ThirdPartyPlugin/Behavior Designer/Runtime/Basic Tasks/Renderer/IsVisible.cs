using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRenderer
{
    [TaskCategory("Basic/Renderer")]
    [TaskDescription("Returns Success if the Renderer is visible, otherwise Failure.")]
    public class IsVisible : Conditional
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        // cache the renderer component
        private  Renderer targetRenderer;

        public override void OnStart()
        {
            targetRenderer = GetDefaultGameObject(targetGameObject.Value).GetComponent<Renderer>();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetRenderer == null) {
                Debug.LogWarning("Renderer is null");
                return TaskStatus.Failure;
            }

            return targetRenderer.isVisible ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}