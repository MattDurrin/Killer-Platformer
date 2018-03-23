using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    public int currentHealth;
    public int maxHealth;


	// Use this for initialization
	void Start () {

        currentHealth = maxHealth;
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void hurtPlayer(int damage)
    {
        currentHealth -= damage;
    }

    public void healPlayer(int heal)
    {
        if(heal + currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += heal;
        }
    }
}
