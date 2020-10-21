using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DataSetLosePanel
{
    public GameObject DiedPanel;
    public Button Restart;
}

public class LosePanel : PanelEndGameDecorator<DataSetLosePanel>
{
    public LosePanel(Panel p, DataSetEndGamePanel endGamePanel, DataSetLosePanel panel) : base(p, endGamePanel, panel)
    {
        dataSetendGamePanel.BtnBackToMenu.onClick.AddListener(BackToMenu);
        dataSet.Restart.onClick.AddListener(Restart);
        ShowPanel();
    }

    protected override void ShowPanel()
    {
        dataSet.DiedPanel.SetActive(true);
        dataSetendGamePanel.TextEndGame.text = "FAIL!";
        dataSetendGamePanel.TextEndGame.color = new Color32(253, 0, 79, 255);
        dataSetendGamePanel.BtnsPanelEndGame[0].SetActive(true);
        dataSetendGamePanel.BtnsPanelEndGame[1].SetActive(true);
    }

    protected override void BackToMenu()
    {
        base.BackToMenu();
    }

    private void Restart()
    {
        CustomLevelLoader.LoadLevel("Game");
    }
}
