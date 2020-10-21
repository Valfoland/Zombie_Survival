using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject imageInCombat;
    public static System.Action<bool> onWin;
    public static bool isWin;
    public static bool isEndGame;
    private void Awake()
    {
        isWin = false;
        isEndGame = false;
        BotManager.onCombat += InCombat;
        PlayerPosition.onGetPosition += RoundWin;
        PlayerHealth.onDead += RoundLose;
    }

    private void OnDestroy()
    {
        BotManager.onCombat -= InCombat;
        PlayerPosition.onGetPosition -= RoundWin;
        PlayerHealth.onDead -= RoundLose;
    }

    private void InCombat(int countCombat)
    {
        if (countCombat != 0)
        {
            imageInCombat.SetActive(true);
        }
        else
        {
            imageInCombat.SetActive(false);
        }

    }

    private void RoundWin(Transform transformPlayer)
    {
        if (transformPlayer.position.x >= MazeSpawner.GoalPosition.x - 2 &&
           transformPlayer.position.z >= MazeSpawner.GoalPosition.z - 2)
        {
            isEndGame = true;
            isWin = true;
            onWin?.Invoke(true);
            PlayerPosition.onGetPosition -= RoundWin;
        }
    }

    private void RoundLose()
    {
        isEndGame = true;
        onWin?.Invoke(false);
    }
}
