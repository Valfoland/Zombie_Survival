using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : BaseAction
{
	public static System.Action<float> OnAttack;

	public override void Act(StateController controller)
	{
		Attack(controller);
	}

	private void Attack(StateController controller)
	{
		controller.BotManager.Animator.SetBool("isAttack", true);
		controller.BotManager.Animator.SetBool("isRun", false);
		controller.BotManager.Animator.SetBool("isWalk", false);
		controller.navMeshAgent.Stop();

		if (!controller.IsChase)
		{
			controller.IsChase = true;
			BotManager.BotInCombat++;
			BotManager.onCombat?.Invoke(BotManager.BotInCombat);
		}

		if (Time.time - controller.LeftTime > controller.enemyStats.AttackRate)
		{
			controller.LeftTime = Time.time;
			OnAttack?.Invoke(controller.enemyStats.AttackDamage);
		}
		OnAttack?.Invoke(0);
	}
}
