using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StaticDamageZoneController : DamageZoneControllerABC
{
    [Header("Don't Modify")]

    public static Int16Iterator zSortingValues = new Int16Iterator();

    public Collider2D cachedCollider;

    void Awake()
    {
        cachedCollider = this.GetComponent<Collider2D>();
        GetComponent<SpriteRenderer>().sortingOrder = zSortingValues.Pop();
    }

    public static GameObject SpawnAndInitialize(GameObject prefab, Vector2 position)
    {
        GameObject newDamageZone = Instantiate(
            prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        bool found = newDamageZone.TryGetComponent(out StaticDamageZoneController newController);
        if (!found)
        {
            newController = newDamageZone.AddComponent<StaticDamageZoneController>();
        }
        activeDamageZoneControllers.Add(newController);
        return newDamageZone;
    }

    public override bool IsCollidingWithPlayer()
    {
        return cachedCollider.OverlapPoint(PlayerController.Instance.transform.position);
    }
}
