using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyControllerABC : MonoBehaviour
{
    public static List<EnemyControllerABC> activeEnemyControllers = new List<EnemyControllerABC>();

    public List<WeakPoint> weakPoints = new List<WeakPoint>();
    public List<DamageZoneControllerABC> damageZoneControllers = new List<DamageZoneControllerABC>();

    void Awake()
    {
        activeEnemyControllers.Add(this);
        InitializeWeakPoints();
    }

    void Start()
    {
        
    }

    void Update()
    {
        UpdateChildSpecific();
        UpdateWeakPointPositions();
    }

    void OnDestroy()
    {
        activeEnemyControllers.Remove(this);
    }

    protected abstract void InitializeWeakPoints();

    protected abstract void UpdateChildSpecific();

    private void UpdateWeakPointPositions()
    {
        foreach (WeakPoint weakPoint in weakPoints)
        {
            weakPoint.position = (
                weakPoint.relativePosition
                + new Vector2(this.transform.position.x, this.transform.position.y));
        }
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

    public void ApplyAttack(WeakPoint weakPoint, int lockOns)
    {
        Debug.Log($"Damaged weak point number {weakPoints.IndexOf(weakPoint)} for {lockOns} damage.");
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
