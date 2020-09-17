using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour {

    public float scaleFactor = 1.2f;
    public float maxScale = 3f;
    private bool scaleFactorIsProper;

	// Use this for initialization
	void Start () {
		if (scaleFactor <= 1) {
            print("The size of the scaleFactor is too small. Increase it to something above 1.0");
        }
	}

    // Mouse click detection
    void OnMouseDown() {
        print("Mouse pressed...");

        if (scaleFactor > 1) {
            scaleFactorIsProper = true;
        } else {
            print("The size of the scaleFactor is too small. Increase it to something above 1.0");
        }

        if(scaleFactorIsProper) {
            transform.localScale *= scaleFactor;

           if (transform.localScale.x >= maxScale) {
               Destroy(gameObject);
               print("Le pop!");
           }
        }
    }
}
