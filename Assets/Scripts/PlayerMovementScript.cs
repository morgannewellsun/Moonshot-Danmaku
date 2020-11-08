using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    private PlayerVariables playerVariables;

    void Start()
    {
        playerVariables = this.GetComponent<PlayerVariables>();
    }

    void Update()
    {
        UpdatePlayerPosition();
    }

    private void UpdatePlayerPosition()
    {
        if (playerVariables.isDashing)
        {
            return;
        }
        else
        {
            float movementSpeed = (
                Input.GetButton("Focus") ? GameParameters.playerSpeedFocused : GameParameters.playerSpeedNormal);
            Vector2 deltaPosition = playerVariables.normMovementDir * movementSpeed * Time.deltaTime;
            transform.position = new Vector3(
                Mathf.Clamp(
                    value: this.transform.position.x + deltaPosition.x, 
                    min: playerVariables.posBounds.xMin,
                    max: playerVariables.posBounds.xMax),
                Mathf.Clamp(
                    value: this.transform.position.y + deltaPosition.y,
                    min: playerVariables.posBounds.yMin,
                    max: playerVariables.posBounds.yMax),
                this.transform.position.z);
        }
    }
}
