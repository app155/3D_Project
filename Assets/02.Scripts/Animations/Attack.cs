using Project3D.Controller;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterController;

namespace Project3D.Animations
{
    public class Attack : Behavior
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Debug.Log("hi");
            animator.Play("Attack");

        }
    }
}