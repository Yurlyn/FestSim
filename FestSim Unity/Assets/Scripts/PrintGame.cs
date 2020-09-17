using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintGame : MonoBehaviour {

    public string myGameIdea;

	// Use this for initialization
	void Start () {
        F_PrintGame();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void F_PrintGame() {
        print(myGameIdea);
    }
}
