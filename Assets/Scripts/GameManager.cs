using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public int currentItems;
    public Text itemText;

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
    }
}
