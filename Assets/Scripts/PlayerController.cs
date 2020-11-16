using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public Rect posBounds;

    public int xMovementDir = 0;
    public int yMovementDir = 0;
    public Vector2 normMovementDir = Vector2.zero;

    public bool isDashing = false;
    public float dashStartTime = 0f;
    public Vector2 dashOrigin = Vector2.zero;
    public Vector2 dashTarget = Vector2.zero;

    public bool isAttacking = false;
    public float attackStartTime = 0f;
    public int totalLockOnCount = 0;
    public Dictionary<WeakPoint, int> weakPointLockOnCounts = new Dictionary<WeakPoint, int>();

    void Awake()
    {
        InitializePosBounds();
    }

    void Update()
    {
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

    private void UpdateMovementVariables()
    {
        xMovementDir = (Input.GetButton("Right") ? 1 : 0) - (Input.GetButton("Left") ? 1 : 0);
        yMovementDir = (Input.GetButton("Up") ? 1 : 0) - (Input.GetButton("Down") ? 1 : 0);
        normMovementDir = new Vector2(xMovementDir, yMovementDir).normalized;
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
            Vector2 deltaPosition = normMovementDir * movementSpeed * Time.deltaTime;
            SetAndClampPlayerPosition(transform.position.x + deltaPosition.x, transform.position.y + deltaPosition.y);
        }
    }

    private void UpdateDashState()
    {
        if (isDashing)
        {
            float dashInterpolationFactor = (Time.time - dashStartTime) / GameParameters.Instance.dashDuration;
            if (dashInterpolationFactor >= 1)
            {
                isDashing = false;
                SetAndClampPlayerPosition(dashTarget.x, dashTarget.y);
            }
            else
            {
                Vector2 dashInterpolatedPosition = Vector2.Lerp(dashOrigin, dashTarget, dashInterpolationFactor);
                SetAndClampPlayerPosition(dashInterpolatedPosition.x, dashInterpolatedPosition.y);
            }
        }
        else if (GameParameters.Instance.dashHoldingAllowed ? Input.GetButton("Dash") : Input.GetButtonDown("Dash"))
        {
            if (Time.time - dashStartTime < GameParameters.Instance.dashCooldown)
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
                dashOrigin = new Vector2(this.transform.position.x, this.transform.position.y);
                dashTarget = dashOrigin + normMovementDir * GameParameters.Instance.dashDistance;
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
                pair.Key.ApplyAttack(pair.Value);
            }
            totalLockOnCount = 0;
            weakPointLockOnCounts = new Dictionary<WeakPoint, int>();
        }
        else if (isAttacking)
        {
            int maximumLockOns = 0;
            float lockOnTime = Time.time - attackStartTime;
            for(int i = 0; i < GameParameters.Instance.attackLockOnTimes.Count; i++)
            {
                if (lockOnTime >= GameParameters.Instance.attackLockOnTimes[i])
                {
                    maximumLockOns = Mathf.Max(maximumLockOns, GameParameters.Instance.attackLockOnMaxCounts[i]);
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
                collectedWeakPoints.Sort();
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
