using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageZoneControllerABC : MonoBehaviour
{
    public static List<DamageZoneControllerABC> activeDamageZoneControllers = new List<DamageZoneControllerABC>();

    void Awake()
    {
        activeDamageZoneControllers.Add(this);
    }

    void OnDestroy()
    {
        activeDamageZoneControllers.Remove(this);
    }

    public abstract bool CheckPlayerCollision();
}
