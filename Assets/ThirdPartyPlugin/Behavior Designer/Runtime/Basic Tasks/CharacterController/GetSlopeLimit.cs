using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCharacterController
{
    [TaskCategory("Basic/CharacterController")]
    [TaskDescription("Stores the slope limit of the CharacterController. Returns Success.")]
    public class GetSlopeLimit : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The slope limit of the CharacterController")]
        [RequiredField]
        public SharedFloat storeValue;

        private CharacterController characterController;

        public override void OnStart()
        {
            characterController = GetDefaultGameObject(targetGameObject.Value).GetComponent<CharacterController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (characterController == null) {
                Debug.LogWarning("CharacterController is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = characterController.slopeLimit;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = 0;
        }
    }
}