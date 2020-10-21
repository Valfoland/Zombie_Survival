using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public VisibilityCompute visibilityCompute;
    public static System.Action<int> onCombat;
    public int MoreSpeed;
    public bool isOnAttack;
    public static int BotInCombat;
    public Collider BotCollider;
    public GameObject BotBody;
    public GameObject BotSlider;
    public GameObject[] Pack;
    public GameObject DestroyObject;
    public AIHealth AIHealth;
    public Animator Animator;
    public static int CountKilledBots;
    [SerializeField] private EnemyStats enemyStats;
    private const string TARGET_TAG = "Player";

    private void Awake()
    {
        BotInCombat = 0;
        CountKilledBots = 0;
        visibilityCompute = new VisibilityCompute(TARGET_TAG, enemyStats.CountRays, enemyStats.Angle, false);
    }
}
