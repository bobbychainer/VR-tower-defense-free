using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMedium : EnemyController {
    protected override void Start() {
        base.Start();
        enemyHealth = 3;
        enemyValue = 3;
        enemySpeed = 5f;
    }
}
