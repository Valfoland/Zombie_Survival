using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
public class ChaseAction : BaseAction
{
    public override void Act(StateController controller)
    {
        Chase(controller);
    }

    private void Chase(StateController controller)
    {
        controller.BotManager.Animator.SetBool("isRun", true);
        controller.BotManager.Animator.SetBool("isWalk", false);
        controller.BotManager.Animator.SetBool("isAttack", false);

        controller.navMeshAgent.speed = controller.BotManager.MoreSpeed;

        if (!controller.IsChase)
        {
            controller.IsChase = true;
            BotManager.BotInCombat++;
            BotManager.onCombat?.Invoke(BotManager.BotInCombat);
        }

        controller.navMeshAgent.destination = controller.chaseTarget.position;
        controller.navMeshAgent.Resume();

    }
}