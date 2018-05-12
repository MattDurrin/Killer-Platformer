using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    public int currentItems;
    public Text itemText;
    public Text pHealth;
    public int playersHealth;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addItems(int itemsToAdd)
    {
        currentItems += itemsToAdd;
        itemText.text = "Pages: " + currentItems;
           /* + "\nHealth: " + player.playerHealth;*/


    }

    public void addPlayerHealth(int playerHealth)
    {
        playersHealth = playerHealth;
        pHealth.text = "Health: " + playersHealth;
    }

    public int getItems()
    {
        return currentItems;
    }
}
