using Project3D.Controller;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;

namespace Project3D.Animations
{
    public class Attack : AnimBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Debug.Log("hi");
            animator.Play("Attack");

        }
    }
}