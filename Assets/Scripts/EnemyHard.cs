using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHard : EnemyController {
    protected override void Start() {
        base.Start();
        enemyHealth = 5 + healthIncrease;
        enemyValue = 5 + valueIncrease;
        enemySpeed = 2f + speedIncrease;
    }
}
