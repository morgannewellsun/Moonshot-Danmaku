using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : DamageZoneControllerABC
{
    [Header("Don't Modify")]

    public static Int16 bulletSortingNextZ = Int16.MinValue;
    public static bool initialized = false;
    public static float lengthScale;

    public Collider2D cachedCollider;
    public Vector2 velocity = Vector2.zero;
    public float expiryTime = 0f;

    void Awake()
    {
        cachedCollider = this.GetComponent<Collider2D>();
    }

    void Start()
    {
        InitializeOnce();
    }

    void Update()
    {
        UpdateCheckBulletExpiry();
        UpdateBulletPosition();
    }

    public static GameObject SpawnAndInitialize(GameObject bulletPrefab, Vector2 position, Vector2 velocity)
    {
        GameObject newBullet = Instantiate(
            bulletPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        bool found = newBullet.TryGetComponent(out BulletController newBulletController);
        if (!found)
        {
            newBulletController = newBullet.AddComponent<BulletController>();
        }
        activeDamageZoneControllers.Add(newBulletController);
        newBulletController.velocity = velocity;
        newBulletController.expiryTime = Time.time + lengthScale / velocity.magnitude;
        found = newBullet.TryGetComponent(out SpriteRenderer newBulletSpriteRenderer);
        if (!found)
        {
            throw new ArgumentException("Bullet prefab must have a sprite renderer attached.");
        }
        newBulletSpriteRenderer.sortingOrder = GetAndIncrementBulletSortingNextZ();
        return newBullet;
    }

    public override bool IsCollidingWithPlayer()
    {
        return cachedCollider.OverlapPoint(PlayerController.Instance.transform.position);
    }

    private static void InitializeOnce()
    {
        if (!initialized)
        {
            lengthScale = 2 * Camera.main.orthographicSize * Mathf.Sqrt(1 + Mathf.Pow(Screen.width / Screen.height, 2));
            initialized = true;
        }
    }

    private static Int16 GetAndIncrementBulletSortingNextZ()
    {
        Int16 returnValue = bulletSortingNextZ;
        bulletSortingNextZ += 1;
        if (bulletSortingNextZ == Int16.MaxValue)
        {
            bulletSortingNextZ = Int16.MinValue;
        }
        return returnValue;
    }

    private void UpdateCheckBulletExpiry()
    {
        if (Time.time >= expiryTime)
        {
            activeDamageZoneControllers.Remove(this);
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
