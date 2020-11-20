using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyControllerABC : MonoBehaviour
{
    [Header("Don't Modify")]

    public static List<EnemyControllerABC> activeEnemyControllers = new List<EnemyControllerABC>();

    public static Int16 enemySortingNextZ = Int16.MinValue;

    public List<WeakPoint> weakPoints = new List<WeakPoint>();
    public List<DamageZoneControllerABC> damageZoneControllers = new List<DamageZoneControllerABC>();

    protected virtual void Update()
    {
        UpdateWeakPointPositions();
    }

    public void ApplyAttack(WeakPoint weakPoint, int lockOns)
    {
        Debug.Log($"Damaged weak point number {weakPoints.IndexOf(weakPoint)} for {lockOns} damage.");
    }

    protected static Int16 GetAndIncrementEnemySortingNextZ()
    {
        Int16 returnValue = enemySortingNextZ;
        enemySortingNextZ += 1;
        if (enemySortingNextZ == Int16.MaxValue)
        {
            enemySortingNextZ = Int16.MinValue;
        }
        return returnValue;
    }

    protected void AddWeakPoint(Vector2 relativePosition, int maxLockOns, bool isActive = true)
    {
        WeakPoint newWeakPoint = new WeakPoint();
        newWeakPoint.relativePosition = relativePosition;
        newWeakPoint.position = relativePosition + new Vector2(this.transform.position.x, this.transform.position.y);
        newWeakPoint.maxLockOns = maxLockOns;
        newWeakPoint.isActive = isActive;
        newWeakPoint.enemyController = this;
        weakPoints.Add(newWeakPoint);
    }

    private void UpdateWeakPointPositions()
    {
        foreach (WeakPoint weakPoint in weakPoints)
        {
            weakPoint.position = (
                weakPoint.relativePosition
                + new Vector2(this.transform.position.x, this.transform.position.y));
        }
    }
}

[Serializable]
public class WeakPoint
{
    public Vector2 relativePosition;
    public Vector2 position;
    public int maxLockOns;
    public bool isActive;
    public EnemyControllerABC enemyController;
}
