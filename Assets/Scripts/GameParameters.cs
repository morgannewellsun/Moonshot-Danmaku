using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParameters : Singleton<GameParameters>
{
    [Header("Movement")]
    public float playerSpeedFocused;
    public float playerSpeedNormal;

    [Header("Dashing")]
    public bool playerDashHoldingAllowed;
    public float playerDashDistance;
    public float playerDashDuration;
    public float playerDashCooldown;

    [Header("Attacking")]
    public List<AttackLockOnStep> playerAttackLockOnSteps;

    [Header("Miscellaneous")]
    public float cameraBoundsOffset;

    [Header("Enemy Fighter")]
    public int enemyFighterMaxLockOns;
}

[Serializable]
public struct AttackLockOnStep
{
    public float time;
    public int count;
}
