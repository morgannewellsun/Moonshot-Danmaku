using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : DamageZoneControllerABC
{
    public Vector2 velocity;
    public float duration;

    public static int bulletSortingMaxCount = -1;
    public static float bulletSortingDeltaZ = -1f;
    public static float bulletSortingInitialZ = -1f;
    public static float bulletSortingFinalZ = -1f;
    public static float bulletSortingNextZ = -1f;
    public static bool bulletSortingInitialized = false;

    public float expiryTime = 0f;

    public Collider2D cachedCollider = null;

    void Awake()
    {
        expiryTime = Time.time + duration;
    }

    void Start()
    {
        InitializeBulletSortingParamsIfNeeded();
        transform.position = new Vector3(
            this.transform.position.x,
            this.transform.position.y,
            GetAndIncrementBulletSortingNextZ());
    }

    void Update()
    {
        CheckBulletExpiry();
        UpdateBulletPosition();
    }

    public override bool CheckPlayerCollision()
    {
        if (cachedCollider is null)
        {
            cachedCollider = this.GetComponent<Collider2D>();
        }
        return cachedCollider.OverlapPoint(PlayerController.Instance.transform.position);
    }

    private static void InitializeBulletSortingParamsIfNeeded()
    {
        if (!bulletSortingInitialized)
        {
            bulletSortingMaxCount = GameParameters.Instance.bulletSortingMaxCount;
            bulletSortingDeltaZ = (Camera.main.nearClipPlane - Camera.main.farClipPlane) / bulletSortingMaxCount;
            bulletSortingInitialZ = Camera.main.farClipPlane + bulletSortingDeltaZ;
            bulletSortingFinalZ = Camera.main.nearClipPlane - bulletSortingDeltaZ;
            bulletSortingNextZ = bulletSortingInitialZ;
            bulletSortingInitialized = true;
        }
    }

    private static float GetAndIncrementBulletSortingNextZ()
    {
        float returnValue = bulletSortingNextZ;
        bulletSortingNextZ += bulletSortingDeltaZ;
        if (bulletSortingNextZ <= bulletSortingFinalZ)
        {
            bulletSortingNextZ = bulletSortingInitialZ;
        }
        return returnValue;
    }

    private void CheckBulletExpiry()
    {
        if (Time.time >= expiryTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void UpdateBulletPosition()
    {
        transform.position = new Vector3(
            this.transform.position.x + Time.deltaTime * velocity.x,
            this.transform.position.y + Time.deltaTime * velocity.y,
            this.transform.position.z);
    }
}
