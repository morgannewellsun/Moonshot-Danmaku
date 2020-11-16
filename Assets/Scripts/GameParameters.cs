using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GameParameters : Singleton<GameParameters>
{
    [Header("Movement")]
    public float playerSpeedFocused;
    public float playerSpeedNormal;

    [Header("Dashing")]
    public bool dashHoldingAllowed;
    public float dashDistance;
    public float dashDuration;
    public float dashCooldown;

    [Header("Attacking")]
    public List<float> attackLockOnTimes;
    public List<int> attackLockOnMaxCounts;

    [Header("Miscellaneous")]
    public float cameraBoundsOffset;
}
