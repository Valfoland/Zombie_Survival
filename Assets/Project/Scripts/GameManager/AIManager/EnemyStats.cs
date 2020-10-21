using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public float MoveSpeed = 1;
    public float LookRange = 40f;
    public float LookOutRange = 40f;
    public float AttackRange = 40f;
    public float Angle = 360;
    public int CountRays = 6;

    public float AttackRate = 1f;
    public float ChangeWayRate = 5f;
    public float AttackForce = 15f;
    public int AttackDamage = 50;

    public float SearchDuration = 4f;
    public float SearchingTurnSpeed = 120f;

    public const float POSITION_MIN = 1;
    public const float POSITION_MAX = 20;
}