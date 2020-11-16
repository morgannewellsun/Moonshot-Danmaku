using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerTest : EnemyControllerABC
{
    protected override void InitializeWeakPoints()
    {
        AddWeakPoint(new Vector2(1.919f, 0), 20);
        AddWeakPoint(new Vector2(-1.919f, 0), 10);
    }

    protected override void UpdateWeakPoints() 
    { 
        return;
    }

    public override void HandleAttack(WeakPoint weakPoint, int lockOns)
    {
        Debug.Log($"Damaged weak point number {weakPoints.IndexOf(weakPoint)} for {lockOns} damage.");
    }
}