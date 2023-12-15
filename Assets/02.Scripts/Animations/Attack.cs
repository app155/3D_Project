using UnityEngine;
using CharacterController = Project3D.Controller.CharacterController;

namespace Project3D.Animations
{
    public class Attack : AnimState
    {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Debug.Log("hi");


        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

        }
    }
}