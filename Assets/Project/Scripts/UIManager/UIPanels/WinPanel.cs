using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinPanel : PanelEndGameDecorator<object>
{
    public WinPanel(Panel p, DataSetEndGamePanel endGamePanel) : base(p, endGamePanel, null)
    {
        dataSetendGamePanel.BtnBackToMenu.onClick.AddListener(BackToMenu);
        ShowPanel();
    }

    protected override void ShowPanel()
    {
        dataSetendGamePanel.TextEndGame.text = "WIN";
        dataSetendGamePanel.TextEndGame.color = Color.green;
        dataSetendGamePanel.BtnsPanelEndGame[0].SetActive(true);
    }

    protected override void BackToMenu()
    {
        base.BackToMenu();
    }
}

