using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{

    public override bool Decide(StateController controller)
    {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    private bool Look(StateController controller)
    {
        RaycastHit hit;

        if (controller.BotManager.isOnAttack || controller.BotManager.visibilityCompute.RayToScan(controller.eyes, controller.enemyStats.LookRange) ||
           (Physics.SphereCast(controller.eyes.position, controller.enemyStats.LookOutRange, controller.eyes.forward * -1, out hit, controller.enemyStats.LookOutRange) &&
             hit.collider.CompareTag("Player")))
        {
            controller.chaseTarget = controller.BotManager.visibilityCompute.Target;
            return true;
        }
        return false;
    }
}