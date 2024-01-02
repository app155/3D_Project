using Project3D.Controller;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;

namespace Project3D.Animations
{
    public class AnimBehaviour : StateMachineBase
    {
        [SerializeField] private CharacterState state;
        protected CharacterController controller;

        public void Init(CharacterController controller)
        {
            this.controller = controller;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (stateInfo.normalizedTime >= 1.0f)
            {
                
                controller.ChangeState(state);
            }
        }
    }
}