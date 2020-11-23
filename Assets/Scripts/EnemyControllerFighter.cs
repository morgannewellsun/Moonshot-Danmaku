using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyControllerFighter : EnemyControllerABC
{
    public enum TargetingMethod
    {
        TowardsPlayer,
        Right,
        Up,
        Left,
        Down
    }

    [Header("Specify in Editor")]

    public Vector2 movementDirection;
    public List<ConstAccelSegment> constAccelSegments;
    public List<float> shootingTimeLoopTimes;
    public TargetingMethod shootingTargetingMethod;
    public GameObject shootingBulletPrefab;
    public float shootingBulletSpeed;

    [Header("Don't Modify")]

    public float distanceMoved = 0f;
    public int shootingTimeLoopBulletsShot = 0;
    public float shootingTimeLoopStartTime = -1f;

    public SpriteHighlightController highlightController;

    protected override void Awake()
    {
        highlightController = this.GetComponent<SpriteHighlightController>();
        base.Awake();
    }

    protected override void Update()
    {
        UpdateLinearMovement();
        UpdateShootBullets();
        base.Update();
    }

    public static GameObject SpawnAndInstantiate(
        GameObject prefab,
        Vector2 position,
        Vector2 movementDirection,
        float shootingDelay = 0)
    {
        GameObject newFighter = Instantiate(prefab);
        newFighter.transform.position = new Vector3(position.x, position.y, 0);
        bool found = newFighter.TryGetComponent(out EnemyControllerFighter newFighterController);
        if (!found)
        {
            throw new ArgumentException("Fighter prefab must have an EnemyControllerFighter script attached.");
        }
        activeEnemyControllers.Add(newFighterController);
        newFighterController.InitializeWeakPoints();
        newFighterController.movementDirection = movementDirection;
        newFighterController.shootingTimeLoopStartTime = Time.time + shootingDelay;
        found = newFighter.TryGetComponent(out SpriteRenderer newFighterSpriteRenderer);
        if (!found)
        {
            throw new ArgumentException("Fighter prefab must have a sprite renderer attached.");
        }
        newFighterSpriteRenderer.sortingOrder = GetAndIncrementEnemySortingNextZ();
        return newFighter;
    }

    protected override int GetMaxHealth()
    {
        return GameParameters.Instance.enemyFighterMaxHealth;
    }

    protected override void HandleWeakPointDamage(WeakPoint weakPoint, int lockOns)
    {
        health -= lockOns;
        highlightController.highlight2(0.2f);
        if (health <= 0)
        {
            activeEnemyControllers.Remove(this);
            this.movementDirection = Vector2.zero;
            Destroy(this.gameObject, 0.2f);
        }
    }

    private void InitializeWeakPoints()
    {
        AddWeakPoint(new Vector2(0, 0), GameParameters.Instance.enemyFighterMaxLockOns);
    }

    private void UpdateLinearMovement()
    {
        ConstAccelSegment currentSegment = new ConstAccelSegment();
        bool found = false;
        float prevSegmentEndDistance = 0f;
        foreach(ConstAccelSegment segment in constAccelSegments)
        {
            if (distanceMoved < segment.endDistance)
            {
                currentSegment = segment;
                found = true;
                break;
            }
            prevSegmentEndDistance = segment.endDistance;
        }
        if (!found) {
            Debug.Log("Appropriate movement segment not found, deleting enemy fighter.");
            activeEnemyControllers.Remove(this);
            Destroy(this.gameObject);
        }
        else
        {
            float deltaDistance = Time.deltaTime * Mathf.Lerp(
                currentSegment.startSpeed, 
                currentSegment.endSpeed, 
                (distanceMoved - prevSegmentEndDistance) / (currentSegment.endDistance - prevSegmentEndDistance));
            distanceMoved += deltaDistance;
            Vector2 deltaPosition = movementDirection.normalized * deltaDistance;
            transform.position = new Vector3(
                this.transform.position.x + deltaPosition.x,
                this.transform.position.y + deltaPosition.y,
                this.transform.position.z);
        }
    }

    private void UpdateShootBullets()
    {
        if (shootingTimeLoopTimes.Count == 0)
        {
            return;
        }
        if (Time.time >= shootingTimeLoopStartTime + shootingTimeLoopTimes[shootingTimeLoopBulletsShot])
        {
            shootingTimeLoopBulletsShot++;
            ShootBullet();
            if (shootingTimeLoopBulletsShot == shootingTimeLoopTimes.Count)
            {
                shootingTimeLoopStartTime = Time.time;
                shootingTimeLoopBulletsShot = 0;
            }
        }
    }

    private void ShootBullet()
    {
        Vector2 velocity;
        switch(shootingTargetingMethod)
        {
            case TargetingMethod.TowardsPlayer:
                velocity = (PlayerController.Instance.transform.position - this.transform.position).normalized;
                break;
            case TargetingMethod.Right:
                velocity = Vector2.right;
                break;
            case TargetingMethod.Up:
                velocity = Vector2.up;
                break;
            case TargetingMethod.Left:
                velocity = Vector2.left;
                break;
            case TargetingMethod.Down:
                velocity = Vector2.down;
                break;
            default:
                throw new ArgumentException("Invalid targeting method specified.");
        }
        velocity *= shootingBulletSpeed;
        BulletController.SpawnAndInitialize(shootingBulletPrefab, this.transform.position, velocity);
    }
}

[Serializable]
public struct ConstAccelSegment
{
    public float startSpeed;
    public float endSpeed;
    public float endDistance;
}