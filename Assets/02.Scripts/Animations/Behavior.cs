using Project3D.Controller;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;

namespace Project3D.Animations
{
    public class Behavior : StateMachineBase
    {
        [SerializeField] private CharacterState state;
        protected CharacterController controller;

        public void Init(CharacterController controller)
        {
            this.controller = controller;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (stateInfo.normalizedTime > 1.0f)
            {
                controller.ChangeState(state);
            }
        }
    }
}