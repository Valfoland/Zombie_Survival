using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Attack")]
public class AttackDecision : Decision
{
    private Vector3 tempPosition;
    public override bool Decide(StateController controller)
    {
        bool targetAttack = Attack(controller);
        return targetAttack;
    }

    private bool Attack(StateController controller)
    {
        if (controller.chaseTarget != null)
        {
            if (Mathf.Abs(Vector3.Distance(controller.BotManager.transform.position, controller.chaseTarget.position)) <= controller.enemyStats.AttackRange && !GameManager.isWin)
            {
                return true;
            }
        }
        return false;
    }
}