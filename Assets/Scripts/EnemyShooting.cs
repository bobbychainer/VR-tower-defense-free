using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : EnemyController
{
    protected override void Start()
    {
        base.Start();
        enemyHealth = 3;
        enemyValue = 3;
        enemySpeed = 3.5f;
    }
}
