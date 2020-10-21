using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Death")]
public class DeathDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        bool targetVisible = Death(controller);
        return targetVisible;
    }

    private bool Death(StateController controller)
    {
        if (controller.BotManager.AIHealth.Dead)
        {
            return true;
        }
        return false;
    }
}