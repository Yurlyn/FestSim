using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmbeddedDictionaryA : MonoBehaviour {
	public GameObject directorObj;

	string pretendingEnum = "crane";

	public Dictionary<string, string> cameraTypeDict = new Dictionary<string, string>();

	public Dictionary<string, string> craneCameraControls = new Dictionary<string, string>();

	[Space]
	public bool camYawLimit;
	public float craneOrbitHeight;

	// Use this for initialization
	void Start () {
		craneCameraControls.Add("Cam Yaw Limit", "bool|" + camYawLimit); // Last part of data string is only for read out purposes by the director (EmbeddedDictionaryB)
		craneCameraControls.Add("Orbit Height", "fslider|0_100|" + craneOrbitHeight); // Last part of data string is only for read out purposes by the director (EmbeddedDictionaryB)

		directorObj.GetComponent<EmbeddedDictionaryB>().RegisterCameraTest(gameObject);
	}

	public Dictionary<string, string> GetCamControls() {
		//if (pretendingEnum == "crane") {
			return craneCameraControls;
		/*} else {
			return null;
        }*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
