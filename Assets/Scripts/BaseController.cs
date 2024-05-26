using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseController : MonoBehaviour {

    public float maxHealth = 20f;
    public float currentHealth;
    //public Slider healthBar;

    void Start ()
    {
        currentHealth = maxHealth;
        //healthBar.value = currentHealth / maxHealth;

    }

    void Update ()
    {
        //healthBar.value = currentHealth / maxHealth;

    }

    public void TakeDmg(float dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // sorgt daf�r, dass currHealth immer zwischen 0 und maxHealth ist

        if(currentHealth <= 0 )
        {
            gameOver();
        }

    }

    void gameOver()
    {
        Debug.Log("Base Destroyed, Game Over!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {

            //Debug.Log(other.gameObject + " Entered Base");

            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                int damageToBase = enemyController.getEnemyValue();
                TakeDmg(damageToBase);
                Destroy(other.gameObject);
            }
        }
    }

}
