using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Death")]
public class DeathAction : BaseAction
{
	private const float TIME_TO_DESTROY = 2f;
	public override void Act(StateController controller)
	{
		Death(controller);
	}

	private void Death(StateController controller)
	{
		controller.navMeshAgent.Stop();
		controller.BotManager.Animator.SetBool("isDeath", true);

		controller.BotManager.BotCollider.enabled = false;
		Destroy(controller.BotManager.BotBody, TIME_TO_DESTROY);
		Destroy(controller.BotManager.BotSlider, TIME_TO_DESTROY);

		if (controller.BotManager.BotBody == null && !controller.IsDeath)
		{
			BotManager.CountKilledBots++;
            controller.BotManager.isOnAttack = false;
			controller.IsDeath = true;
			if (BotManager.BotInCombat > 0)
			{
				BotManager.BotInCombat--;
			}
			BotManager.onCombat?.Invoke(BotManager.BotInCombat);

			int randomValue = Random.Range(1, 11);
			if (randomValue == 5)
			{
				controller.BotManager.Pack[0]?.SetActive(true);
			}
			if (randomValue == 1 || randomValue == 4 || randomValue == 7)
			{
				controller.BotManager.Pack[1]?.SetActive(true);
			}
		}
	}
}