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
    public int health;

    protected virtual void Awake()
    {
        health = GetMaxHealth();
    }

    protected virtual void Update()
    {
        UpdateWeakPointPositions();
    }

    protected abstract int GetMaxHealth();

    protected abstract void HandleWeakPointDamage(WeakPoint weakPoint, int lockOns);

    public void ApplyAttack(WeakPoint weakPoint, int lockOns)
    {
        Debug.Log($"Damaged weak point number {weakPoints.IndexOf(weakPoint)} for {lockOns} damage.");
        HandleWeakPointDamage(weakPoint, lockOns);
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
        newWeakPoint.distanceFromPlayer = 
            (newWeakPoint.position - (Vector2)PlayerController.Instance.transform.position).magnitude;
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
            weakPoint.distanceFromPlayer = 
                (weakPoint.position - (Vector2)PlayerController.Instance.transform.position).magnitude;
        }
    }
}

[Serializable]
public class WeakPoint
{
    public Vector2 relativePosition;
    public Vector2 position;
    public float distanceFromPlayer;
    public int maxLockOns;
    public bool isActive;
    public EnemyControllerABC enemyController;
}
