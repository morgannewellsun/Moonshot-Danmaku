using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : IComparable<WeakPoint>
{
    public Vector2 position;
    public int maxLockOns;
    public bool isActive;
    public EnemyControllerABC enemyController;

    public void ApplyAttack(int lockOns)
    {
        enemyController.HandleAttack(this, lockOns);
    }

    public int CompareTo(WeakPoint other)
    {
        Vector2 playerPosition = new Vector2(
            PlayerController.Instance.transform.position.x, 
            PlayerController.Instance.transform.position.y);
        float thisDistance = (position - playerPosition).magnitude;
        float otherDistance = (other.position - playerPosition).magnitude;
        return thisDistance.CompareTo(otherDistance);
    }
}

public abstract class EnemyControllerABC : MonoBehaviour
{
    public static List<EnemyControllerABC> activeEnemyControllers = new List<EnemyControllerABC>();

    public List<WeakPoint> weakPoints = new List<WeakPoint>();

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
        UpdateWeakPoints();
    }

    protected void AddWeakPoint(Vector2 position, int maxLockOns, bool isActive = true)
    {
        WeakPoint newWeakPoint = new WeakPoint();
        newWeakPoint.position = position;
        newWeakPoint.maxLockOns = maxLockOns;
        newWeakPoint.isActive = isActive;
        newWeakPoint.enemyController = this;
        weakPoints.Add(newWeakPoint);
    }

    protected abstract void InitializeWeakPoints();

    protected abstract void UpdateWeakPoints();

    public abstract void HandleAttack(WeakPoint weakPoint, int lockOns);
}
