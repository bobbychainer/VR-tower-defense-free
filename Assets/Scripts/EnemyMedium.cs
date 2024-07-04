using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMedium : EnemyController {
    protected override void Start() {
        base.Start();
        enemyHealth = 2;
        enemyValue = 2;
        enemySpeed = 3f;
    }
}
