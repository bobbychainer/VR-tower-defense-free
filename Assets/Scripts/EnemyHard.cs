using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHard : EnemyController {
    protected override void Start() {
        base.Start();
        enemyHealth = 5;
        enemyValue = 5;
        enemySpeed = 2;
    }
}
