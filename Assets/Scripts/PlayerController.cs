using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rect posBounds;

    public int xMovementDir;
    public int yMovementDir;
    public Vector2 normMovementDir;

    public bool isDashing;
    public float dashStartTime;
    public Vector2 dashOrigin;
    public Vector2 dashTarget;

    void Awake()
    {
        CalculatePositionBounds();
    }

    void Update()
    {
        UpdateMovementVariables();
        UpdatePlayerPosition();
        UpdateDashState();
    }

    private void CalculatePositionBounds()
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

}
