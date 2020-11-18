using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerTest : EnemyControllerABC
{
    protected override void InitializeWeakPoints()
    {
        AddWeakPoint(new Vector2(1.919f, 0), 6);
        AddWeakPoint(new Vector2(-1.919f, 0), 6);
    }

    protected override void UpdateChildSpecific() 
    { 
        return;
    }
}