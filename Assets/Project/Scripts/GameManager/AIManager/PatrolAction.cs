using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
public class PatrolAction : BaseAction
{
	public override void Act(StateController controller)
	{
		Patrol(controller);
	}

	private void Patrol(StateController controller)
	{
		if (Time.time - controller.LeftTime > controller.enemyStats.ChangeWayRate)
		{
			controller.BotManager.Animator.SetBool("isWalk", true);
			controller.BotManager.Animator.SetBool("isRun", false);
			controller.LeftTime = Time.time;
			controller.navMeshAgent.destination = new Vector3(controller.TargetPos + controller.mainPosition.x, controller.mainPosition.y, controller.TargetPos + controller.mainPosition.z);
			controller.navMeshAgent.Resume();
			controller.TargetPos = -controller.TargetPos;
		}
	}
}