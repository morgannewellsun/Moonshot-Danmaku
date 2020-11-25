using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BulletController : DamageZoneControllerABC
{
    [Header("Don't Modify")]

    public static bool initialized = false;
    public static float lengthScale;
    public static Int16Iterator zSortingValues = new Int16Iterator();

    public Collider2D cachedCollider;
    public Vector2 velocity = Vector2.zero;
    public float expiryTime = 0f;

    void Awake()
    {
        cachedCollider = this.GetComponent<Collider2D>();
        GetComponent<SpriteRenderer>().sortingOrder = zSortingValues.Pop();
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

    public static GameObject SpawnAndInitialize(GameObject prefab, Vector2 position, Vector2 velocity)
    {
        GameObject newBullet = Instantiate(
            prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        bool found = newBullet.TryGetComponent(out BulletController newBulletController);
        if (!found)
        {
            newBulletController = newBullet.AddComponent<BulletController>();
        }
        activeDamageZoneControllers.Add(newBulletController);
        newBulletController.velocity = velocity;
        newBulletController.expiryTime = Time.time + lengthScale / velocity.magnitude;
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
