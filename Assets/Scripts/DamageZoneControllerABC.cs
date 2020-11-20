using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class DamageZoneControllerABC : MonoBehaviour
{
    [Header("Don't Modify")]

    public static List<DamageZoneControllerABC> activeDamageZoneControllers = new List<DamageZoneControllerABC>();

    public abstract bool IsCollidingWithPlayer();

    public static bool IsAnyCollidingWithPlayer()
    {
        foreach (DamageZoneControllerABC damageZone in activeDamageZoneControllers)
        {
            if (damageZone.IsCollidingWithPlayer())
            {
                return true;
            }
        }
        return false;
    }
}
