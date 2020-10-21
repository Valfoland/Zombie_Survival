using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DataSetEndGamePanel
{
    public GameObject AudioGame;
    public GameObject EndGamePanelObject;
    public Text TextEndGame;
    public Text[] GameStats;
    public GameObject[] BtnsPanelEndGame;
    public Button BtnBackToMenu;
}

public class EndGamePanel : Panel
{
    public EndGamePanel(DataSetEndGamePanel dataSetEndGamePanel) : base(dataSetEndGamePanel.EndGamePanelObject)
    {
        _ = new GameStats(dataSetEndGamePanel);
        dataSetEndGamePanel.AudioGame.SetActive(false);
        ShowPanel();
    }

    protected override void ShowPanel()
    {
        panelObject.SetActive(true);
    }
}
