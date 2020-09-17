using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmbeddedDictionaryB : MonoBehaviour {
	public List<GameObject> cameraList;

	private Dictionary<GameObject, Dictionary<string, string>> camControls = new Dictionary<GameObject, Dictionary<string, string>>();

	// Use this for initialization
	void Start () {

	}

	public void RegisterCameraTest(GameObject gObj) {
		Debug.Log("EDB: Registering camera: " + gObj);

		if (!cameraList.Contains(gObj)) {
			cameraList.Add(gObj);

			camControls.Add(gObj, gObj.GetComponent<EmbeddedDictionaryA>().GetCamControls());
        }

		Debug.Log(camControls.Count + " | " + camControls[gObj].Keys.Count);
		foreach (string key in camControls[gObj].Keys) {
			Debug.Log("Property found: " + key + " (" + camControls[gObj][key].ToString() + ")...");
		}

		CreateUIControls();
	}

	private void CreateUIControls () {
		Debug.Log("Creating fake UI...");

		for (int i = 0; i < cameraList.Count; i++) {
			foreach (string camProperty in camControls[cameraList[i]].Keys) {
				Debug.Log("Creating UI for: " + camProperty); // + " (" + camControls[cameraList[i]][camProperty].ToString() + ")...");

				//string[] dataString = camProperty.Split('|');
				string[] dataString = camControls[cameraList[i]][camProperty].ToString().Split('|');

				for (int u = 0; u < dataString.Length; u++) {
					Debug.Log("Data String index " + u + ": " + dataString[u]);
				}

				Debug.Log("Control type is: " + dataString[0]);

				if (dataString[0] == "fslider") {
					string[] sliderRange = dataString[1].Split('_');
					Debug.Log(string.Format("Creating a float slider for {0} with a range from {1} to {2}: ", camProperty, sliderRange[0], sliderRange[1]));
					Debug.Log(string.Format("Setting UI float slider to: {0}", dataString[dataString.Length - 1]));
				}
				if (dataString[0] == "bool") {
					Debug.Log(string.Format("Creating an on/off switch for {0}: {1}", camProperty, dataString[1]));
					Debug.Log(string.Format("Setting UI switch to: {0}", dataString[dataString.Length - 1]));
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
