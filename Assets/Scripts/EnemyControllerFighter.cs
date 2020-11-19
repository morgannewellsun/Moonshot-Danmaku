using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerFighter : EnemyControllerABC
{
    public Vector2 movementDirection;
    public List<ConstAccelSegment> constAccelSegments;

    public float distanceMoved = 0f;

    protected override void InitializeWeakPoints()
    {
        AddWeakPoint(
            new Vector2(this.transform.position.x, this.transform.position.y), 
            GameParameters.Instance.enemyFighterMaxLockOns);
    }

    protected override void UpdateChildSpecific()
    {
        UpdateLinearMovement();
        UpdateTrackWeakPointPosition();
        UpdateShootBullets();
    }

    private void UpdateLinearMovement()
    {
        ConstAccelSegment currentSegment = new ConstAccelSegment();
        bool found = false;
        foreach(ConstAccelSegment segment in constAccelSegments)
        {
            if (distanceMoved >= segment.startDistance && distanceMoved < segment.endDistance)
            {
                currentSegment = segment;
                found = true;
                break;
            }
        }
        if (!found) {
            Debug.Log("Appropriate movement segment not found, deleting enemy fighter.");
            Destroy(this.gameObject);
        }
        else
        {
            float deltaDistance = Time.deltaTime * Mathf.Lerp(
                currentSegment.startSpeed, 
                currentSegment.endSpeed, 
                (distanceMoved - currentSegment.startDistance) / (currentSegment.endDistance - currentSegment.startDistance));
            distanceMoved += deltaDistance;
            Vector2 deltaPosition = movementDirection * deltaDistance;
            transform.position = new Vector3(
                this.transform.position.x + deltaPosition.x,
                this.transform.position.y + deltaPosition.y,
                this.transform.position.z);
        }
    }

    private void UpdateTrackWeakPointPosition()
    {
        weakPoints[0].relativePosition = new Vector2(this.transform.position.x, this.transform.position.y);
    }

    private void UpdateShootBullets()
    {
        return;  // TODO
    }
}

[Serializable]
public struct ConstAccelSegment
{
    public float startDistance;
    public float startSpeed;
    public float endDistance;
    public float endSpeed;
}