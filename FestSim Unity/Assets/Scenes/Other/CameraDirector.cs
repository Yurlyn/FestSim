using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : MonoBehaviour {
	// Camera's and their controls and settings
	// The cameraList stores all the camera gameobjects whil the two dictionaries save the camera gameobjects' settings and controls
	// The camera type will also be stored in cameraSettings along with the universal settings
	public List<GameObject> cameraList;
	private Dictionary<GameObject, Dictionary<string, string>> cameraControls = new Dictionary<GameObject, Dictionary<string, string>>();
	private Dictionary<GameObject, Dictionary<string, string>> cameraSettings = new Dictionary<GameObject, Dictionary<string, string>>(); // Also includes the basic universal settings

	// Also, the firstCamRegistered may no longer be needed.

	private bool firstCamRegistered = false;

	public bool TestButton;
	public bool TestButton2;

	// Use this for initialization
	void Start () {
		// Deleting empty entries created in Unity's editor
		for (int i = 0; i < cameraList.Count; i++) {
			if (cameraList[i] == null) {
				cameraList.RemoveAt(i);
            }
        }
	}

	public void RegisterCamera (GameObject camObj) {
		Debug.Log("Director is registering camera: " + camObj.name);
		
		if (!cameraList.Contains(camObj)) {
			// Camera listing
			cameraList.Add(camObj);

			// Camera controls and settings
			cameraControls.Add(camObj, camObj.GetComponent<CameraType>().GetDirectorControls());
			cameraSettings.Add(camObj, camObj.GetComponent<CameraType>().GetDirectorSettings());
        } else {
			int camIndex = cameraList.IndexOf(camObj);

			// Camera controls and settings
			Debug.Log(string.Format("Debugging {0}: ", cameraList[camIndex]));
			if (cameraControls.Count != cameraList.Count) {
				Debug.Log(string.Format("Camera controls size not the same as camera list size: {0} vs {1}", cameraControls.Count, cameraList.Count));
				cameraControls.Add(camObj, camObj.GetComponent<CameraType>().GetDirectorControls());

				Debug.Log(string.Format("CameraControlsName count: {0}", cameraControls.Count));

				// Can find camObj's director controls?
			} else {
				Debug.Log(string.Format("Overwriting camera controls..."));

				cameraControls.Remove(camObj);
				cameraControls.Add(camObj, camObj.GetComponent<CameraType>().GetDirectorControls());

				Debug.Log(string.Format("CameraControlsName count: {0}", cameraControls.Count));
			}
			
			//cameraControls[camIndex] = cameraList[camIndex].GetComponent<CameraType>().GetDirectorControls();
			//cameraSettings[camIndex] = cameraList[camIndex].GetComponent<CameraType>().GetDirectorSettings();
		}

		/*if (!firstCamRegistered && cameraList.Count < 1) {
			Debug.Log("Cam Fixed Controls entries: " + camFixedControls.Count);

			camFixedControls = cameraList[0].GetComponent<CameraType>().GetDirectorControls("fixed");
			camFixedSettings = cameraList[0].GetComponent<CameraType>().GetDirectorSettings("fixed");

			Debug.Log("Cam Fixed Controls entries: " + camFixedControls.Count);
		}*/
	}

	private void CreateControls () {
		
    }

	// Update is called once per frame
	void Update() {
		if (TestButton) {
			TestButton = false;

			for (int i = 0; i < cameraList.Count; i++) {
				Debug.Log("Camera check for: " + cameraList[i].name);
				Debug.Log(string.Format("{0}'s controls are: ", cameraList[i].name));
				/*for (int u = 0; u < cameraControls.Count; u++) {
					Debug.Log(string.Format("Control {0}: {1} ({2})", u, cameraControls[u].ToString(), cameraControls[u]));
                }*/

				//for (int u = 0; u < cameraControlsName[cameraList[i]].Count; u++) {
				Debug.Log(string.Format("Control {0}: {1} ({2})", cameraControls[cameraList[i]], "value"));
				//}
			}
		}

		if (TestButton2) {
			TestButton2 = false;

			foreach (GameObject listedCamera in cameraList)
				Debug.Log(string.Format("Camera \"{0}\" is a {1} camera...", listedCamera.name, listedCamera.GetComponent<CameraType>().cameraType));
		}
	}
}