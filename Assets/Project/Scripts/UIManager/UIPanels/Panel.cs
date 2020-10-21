using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Panel
{
    protected GameObject panelObject;

    protected abstract void ShowPanel();
    protected virtual void HidePanel()
    {
        panelObject.SetActive(false);
    }

    public Panel(GameObject panelObject = null)
    {
        this.panelObject = panelObject;
    }
}

public abstract class PanelEndGameDecorator<T> : Panel
{
    protected T dataSet;
    protected DataSetEndGamePanel dataSetendGamePanel;
    protected Panel panel;

    public PanelEndGameDecorator(Panel panel, DataSetEndGamePanel dataSetendGamePanel, T dataSet)
    {
        this.dataSet = dataSet;
        this.panel = panel;
        this.dataSetendGamePanel = dataSetendGamePanel;
    }

    protected virtual void BackToMenu()
    {
        CustomLevelLoader.LoadLevel("Menu");
    }

}
