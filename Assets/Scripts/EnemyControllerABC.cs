using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyControllerABC : MonoBehaviour
{
    public static List<EnemyControllerABC> activeEnemyControllers = new List<EnemyControllerABC>();

    public const int bulletSortingMaxCount = 100000;

    public static float bulletSortingDeltaZ = -1f;
    public static float bulletSortingInitialZ = -1f;
    public static float bulletSortingNextZ = -1f;

    public List<WeakPoint> weakPoints = new List<WeakPoint>();

    public List<GameObject> simpleBulletGameObjects = new List<GameObject>();
    public List<Vector2> simpleBulletVelocities = new List<Vector2>();
    public List<float> simpleBulletExpiryTimes = new List<float>();

    void Awake()
    {
        activeEnemyControllers.Add(this);
        InitializeWeakPoints();
    }

    void Start()
    {
        InitializeBulletSortingParams();
    }

    void Update()
    {
        UpdateChildSpecific();
        UpdateWeakPointPositions();
        UpdateSimpleBullets();
    }

    protected abstract void InitializeWeakPoints();

    protected abstract void UpdateChildSpecific();

    private void InitializeBulletSortingParams()
    {
        bulletSortingDeltaZ = (Camera.main.nearClipPlane - Camera.main.farClipPlane) / bulletSortingMaxCount;
        bulletSortingInitialZ = Camera.main.farClipPlane + bulletSortingDeltaZ;
        bulletSortingNextZ = bulletSortingInitialZ;
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

    public void HandleAttack(WeakPoint weakPoint, int lockOns)
    {
        Debug.Log($"Damaged weak point number {weakPoints.IndexOf(weakPoint)} for {lockOns} damage.");
    }

    public float GetAndIncrementBulletSortingNextZ()
    {
        float returnValue = bulletSortingNextZ;
        bulletSortingNextZ += bulletSortingDeltaZ;
        return returnValue;
    }

    public void ResetBullets()
    {
        bulletSortingNextZ = bulletSortingInitialZ;
        simpleBulletGameObjects = new List<GameObject>();
        simpleBulletVelocities = new List<Vector2>();
        simpleBulletExpiryTimes = new List<float>();
    }

    public void FireSimpleBullet(
        GameObject bullet, 
        Vector2 relativePosition, 
        Vector2 velocity, 
        float timeLimit = -1f, float distanceLimit = -1f)
    {
        if (distanceLimit == -1f)
        {
            distanceLimit = Mathf.Max(
                PlayerController.Instance.posBounds.width, 
                PlayerController.Instance.posBounds.height);
        }
        if (timeLimit == -1f)
        {
            timeLimit = distanceLimit / velocity.magnitude;
        }
        else
        {
            timeLimit = Mathf.Min(distanceLimit / velocity.magnitude, timeLimit);
        }
        GameObject newBullet = GameObject.Instantiate(
            bullet,
            new Vector3(
                this.transform.position.x + relativePosition.x,
                this.transform.position.y + relativePosition.y,
                GetAndIncrementBulletSortingNextZ()),
            Quaternion.identity);
        simpleBulletGameObjects.Add(newBullet);
        simpleBulletVelocities.Add(velocity);
        simpleBulletExpiryTimes.Add(Time.time + timeLimit);
    }

    private void UpdateSimpleBullets()
    {
        List<int> indicesToRemove = new List<int>();
        for (int i = simpleBulletGameObjects.Count - 1; i >= 0; i--)
        {
            if (Time.time > simpleBulletExpiryTimes[i])
            {
                indicesToRemove.Add(i);
            }
            else
            {
                simpleBulletGameObjects[i].transform.position = new Vector3(
                    simpleBulletGameObjects[i].transform.position.x + Time.deltaTime * simpleBulletVelocities[i].x,
                    simpleBulletGameObjects[i].transform.position.y + Time.deltaTime * simpleBulletVelocities[i].y,
                    simpleBulletGameObjects[i].transform.position.z);
            }
        }
        foreach (int indexToRemove in indicesToRemove)
        {
            simpleBulletGameObjects.RemoveAt(indexToRemove);
            simpleBulletVelocities.RemoveAt(indexToRemove);
            simpleBulletExpiryTimes.RemoveAt(indexToRemove);
        }
    }

    void OnDestroy()
    {
        activeEnemyControllers.Remove(this);
    }
}

public class WeakPoint : IComparable<WeakPoint>
{
    public Vector2 relativePosition;
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
