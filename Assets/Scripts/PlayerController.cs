using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [Header("Don't Modify")]

    public Rect posBounds = Rect.zero;
    public int xMovementDir = 0;
    public int yMovementDir = 0;
    public Vector2 normVelocity = Vector2.zero;
    public bool isDashing = false;
    public float dashStartTime = 0f;
    public Vector2 dashOriginPos = Vector2.zero;
    public Vector2 dashTargetPos = Vector2.zero;
    public bool isAttacking = false;
    public float attackStartTime = 0f;
    public int totalLockOnCount = 0;
    public Dictionary<WeakPoint, int> weakPointLockOnCounts = new Dictionary<WeakPoint, int>();

    void Awake()
    {
        
    }

    void Start()
    {
        InitializePosBounds();
    }

    void Update()
    {
        UpdateCheckPlayerDamage();
        UpdateMovementVariables();
        UpdatePlayerPosition();
        UpdateDashState();
        UpdateAttackState();
    }

    private void InitializePosBounds()
    {
        Camera mainCamera = Camera.main;
        float halfWidth = mainCamera.orthographicSize * Screen.width / Screen.height;
        posBounds = Rect.MinMaxRect(
            xmin: -1 * halfWidth + GameParameters.Instance.cameraBoundsOffset,
            ymin: -1 * mainCamera.orthographicSize + GameParameters.Instance.cameraBoundsOffset,
            xmax: halfWidth - GameParameters.Instance.cameraBoundsOffset,
            ymax: mainCamera.orthographicSize - GameParameters.Instance.cameraBoundsOffset);
    }

    private void UpdateCheckPlayerDamage()
    {
        if (DamageZoneControllerABC.IsAnyCollidingWithPlayer())
        {
            Debug.Log("Player was hit !!!");

        }
    }

    private void UpdateMovementVariables()
    {
        xMovementDir = (Input.GetButton("Right") ? 1 : 0) - (Input.GetButton("Left") ? 1 : 0);
        yMovementDir = (Input.GetButton("Up") ? 1 : 0) - (Input.GetButton("Down") ? 1 : 0);
        normVelocity = new Vector2(xMovementDir, yMovementDir).normalized;
    }

    private void SetAndClampPlayerPosition(float x, float y)
    {
        transform.position = new Vector3(
            Mathf.Clamp(x, posBounds.xMin, posBounds.xMax),
            Mathf.Clamp(y, posBounds.yMin, posBounds.yMax),
            this.transform.position.z);
    }

    private void UpdatePlayerPosition()
    {
        if (isDashing)
        {
            return;
        }
        else
        {
            float movementSpeed = (
                Input.GetButton("Focus") 
                ? GameParameters.Instance.playerSpeedFocused 
                : GameParameters.Instance.playerSpeedNormal);
            Vector2 deltaPosition = normVelocity * movementSpeed * Time.deltaTime;
            SetAndClampPlayerPosition(transform.position.x + deltaPosition.x, transform.position.y + deltaPosition.y);
        }
    }

    private void UpdateDashState()
    {
        if (isDashing)
        {
            float dashInterpolationFactor = (Time.time - dashStartTime) / GameParameters.Instance.playerDashDuration;
            if (dashInterpolationFactor >= 1)
            {
                isDashing = false;
                SetAndClampPlayerPosition(dashTargetPos.x, dashTargetPos.y);
            }
            else
            {
                Vector2 dashInterpolatedPosition = Vector2.Lerp(dashOriginPos, dashTargetPos, dashInterpolationFactor);
                SetAndClampPlayerPosition(dashInterpolatedPosition.x, dashInterpolatedPosition.y);
            }
        }
        else if (GameParameters.Instance.playerDashHoldingAllowed ? Input.GetButton("Dash") : Input.GetButtonDown("Dash"))
        {
            if (Time.time - dashStartTime < GameParameters.Instance.playerDashCooldown)
            {
                Debug.Log("Dash on cooldown");
            }
            if (xMovementDir == 0 && yMovementDir == 0)
            {
                Debug.Log("No dash direction");
            }
            else
            {
                isDashing = true;
                dashStartTime = Time.time;
                dashOriginPos = new Vector2(this.transform.position.x, this.transform.position.y);
                dashTargetPos = dashOriginPos + normVelocity * GameParameters.Instance.playerDashDistance;
            }
        }
    }

    private void UpdateAttackState()
    {
        if (Input.GetButtonDown("Attack"))
        {
            isAttacking = true;
            attackStartTime = Time.time;
        }
        else if (Input.GetButtonUp("Attack"))
        {
            isAttacking = false;
            foreach (KeyValuePair<WeakPoint, int> pair in weakPointLockOnCounts)
            {
                pair.Key.enemyController.ApplyAttack(pair.Key, pair.Value);
            }
            totalLockOnCount = 0;
            weakPointLockOnCounts = new Dictionary<WeakPoint, int>();
        }
        else if (isAttacking)
        {
            int maximumLockOns = 0;
            float lockOnTime = Time.time - attackStartTime;
            foreach(AttackLockOnStep step in GameParameters.Instance.playerAttackLockOnSteps)
            {
                if (lockOnTime >= step.time)
                {
                    maximumLockOns = Mathf.Max(maximumLockOns, step.count);
                }
            }
            if (totalLockOnCount < maximumLockOns)
            {
                List<WeakPoint> collectedWeakPoints = new List<WeakPoint>();

                foreach(EnemyControllerABC enemyController in EnemyControllerABC.activeEnemyControllers)
                {
                    foreach(WeakPoint weakPoint in enemyController.weakPoints)
                    {
                        collectedWeakPoints.Add(weakPoint);
                    }
                }
                collectedWeakPoints.Sort((a, b) => 
                    (a.position - (Vector2)this.transform.position).magnitude.CompareTo((b.position - (Vector2)this.transform.position).magnitude));
                foreach (WeakPoint weakPoint in collectedWeakPoints)
                {
                    if (!weakPoint.isActive)
                    {
                        continue;
                    }
                    else
                    {
                        bool isNotNew = weakPointLockOnCounts.TryGetValue(weakPoint, out int weakPointLockOnCount);
                        if (!isNotNew)
                        {
                            weakPointLockOnCounts[weakPoint] = 0;
                        }
                        int weakPointRemainingLockOns = weakPoint.maxLockOns - weakPointLockOnCount;
                        int totalRemainingLockOns = maximumLockOns - totalLockOnCount;
                        if (totalRemainingLockOns <= weakPointRemainingLockOns)
                        {
                            weakPointLockOnCounts[weakPoint] += totalRemainingLockOns;
                            totalLockOnCount += totalRemainingLockOns;

                            if (totalRemainingLockOns != 0)
                                Debug.Log($"Locking on {totalRemainingLockOns} times.");

                            break;
                        }
                        else
                        {
                            weakPointLockOnCounts[weakPoint] += weakPointRemainingLockOns;
                            totalLockOnCount += weakPointRemainingLockOns;

                            if (weakPointRemainingLockOns != 0)
                                Debug.Log($"Locking on {weakPointRemainingLockOns} times.");
                        }
                    }
                }
            }
        }
    }
}
