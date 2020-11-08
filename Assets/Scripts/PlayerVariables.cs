using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVariables : MonoBehaviour
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
    }

    private void CalculatePositionBounds()
    {
        Camera mainCamera = Camera.main;
        float halfWidth = mainCamera.orthographicSize * Screen.width / Screen.height;
        posBounds = Rect.MinMaxRect(
            xmin: -1 * halfWidth + GameParameters.cameraBoundsOffset,
            ymin: -1 * mainCamera.orthographicSize + GameParameters.cameraBoundsOffset,
            xmax: halfWidth - GameParameters.cameraBoundsOffset,
            ymax: mainCamera.orthographicSize - GameParameters.cameraBoundsOffset);
    }

    private void UpdateMovementVariables()
    {
        xMovementDir = (Input.GetButton("Right") ? 1 : 0) - (Input.GetButton("Left") ? 1 : 0);
        yMovementDir = (Input.GetButton("Up") ? 1 : 0) - (Input.GetButton("Down") ? 1 : 0);
        normMovementDir = new Vector2(xMovementDir, yMovementDir).normalized;
    }


}
