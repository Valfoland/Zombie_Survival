using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private DataSetEndGamePanel dataEndGamePanel;
    [SerializeField] private DataSetLosePanel dataLose;
    [SerializeField] private DataSetSettingsPanel dataSettings;

    public static System.Action<bool> onEndGame;
    public static System.Action<bool> onChangePage;

    private static UIManager Instance;

    private void OnEnable()
    {
        GameManager.onWin += ShowEndGamePanel;
    }

    private void OnDisable()
    {
        GameManager.onWin -= ShowEndGamePanel;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            _ = new SettingsPanel(dataSettings);
        }
    }

    public void ShowEndGamePanel(bool isWin)
    {
        Panel endGamePanel = new EndGamePanel(dataEndGamePanel);

        if (isWin)
        {
            _ = new WinPanel(endGamePanel, dataEndGamePanel);
        }
        else
        {
            _ = new LosePanel(endGamePanel, dataEndGamePanel, dataLose);
        }
    }

    public void PlayGame()
    {
        CustomLevelLoader.LoadLevel("Game");
    }

    public void PrevLvl()
    {
        onChangePage?.Invoke(false);
    }

    public void NextLvl()
    {
        onChangePage?.Invoke(true);
    }


}

