using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats
{
    public GameStats(DataSetEndGamePanel dataSetEndGamePanel)
    {
        dataSetEndGamePanel.GameStats[0].text = GameTime.Minutes + GameTime.Seconds;
        dataSetEndGamePanel.GameStats[1].text = BotManager.CountKilledBots.ToString();
        dataSetEndGamePanel.GameStats[2].text = AIHealth.CountDamage.ToString();
        dataSetEndGamePanel.GameStats[3].text = PlayerHealth.HealthRepair.ToString();
        dataSetEndGamePanel.GameStats[4].text = WeaponController.CountAmmo.ToString();
    }
}
