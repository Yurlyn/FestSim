using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TwitchLib.Api.Models.Helix.Games.GetGames;
using UnityEditorInternal;
using UnityEngine;

[ExecuteInEditMode]
public class CameraType : MonoBehaviour {
	/// <summary>
	/// This script is for setting up camera's and saving their controls in bookmarks.
	/// 
	/// The bookmark lists for each camera type set the sliders, not the base settings!!
	/// Which means that changing the base settings such as height the slider(s) do NOT change with it.
	/// The effect however DOES change. Example: a fixedCamHeight of 20% of a maxHeight of 100 is an effect of 20.
	/// Changing the maxHeight to 200 keeps the fixedCamHeight on 20% but the effect will be 40.
	/// The director user has to take that into consideration when adding to the settings below.
	/// Of course, the director user cana easily just change the values of the bookmarks below
	/// </summary>

	public enum cameraTypes {
		Crane,
		Dolly,
		Drone,
		Fixed
	};

	[Header("Choose camera type")]
	public cameraTypes cameraType;

	// These settings are the center of all camera types.
	// For the fixed and crane cams this will be where they are virtually standing on the floor or platform
	// For the dolly this is the center of the rails
	// For the drone this will be its central starting point
	//[Header("Camera type's main postition and rotation")]
	//public Vector3 mainPosition;
	[Tooltip("X = Pitch, Y = Yaw, Z = Roll")]
	public Vector3 cameraRotation;

	public bool isLocked;

	//////////////////////////////
	// Fixed Cam Anchor Settings
	//////////////////////////////
	[Header("Settings apply to fixed camera type only")]
	public float fixedMaxHeight;
	[Space]
	[Range(0, 100)]
	[Tooltip("In percentage of its max equivalent")]
	public float fixedHeight;

	[Space]
	[Tooltip("From 0% to 100% as this sets the slider, not the actual height")]
	public List<float> fixedCamBookmarks;

	//////////////////////////////
	// Dolly Cam Anchor Settings
	//////////////////////////////
	[Header("Settings apply to dolly camera type only")]
	public float dollyMaxHeight;
	public float dollyMaxMovement = 3;
	[Space]
	[Range(0, 100)]
	[Tooltip("In percentage of its max equivalent")]
	public float dollyHeight;
	[Range(-100, 100)]
	[Tooltip("In percentage of its max equivalent")]
	public float dollyMovement;

	[Space]
	public List<Vector2> dollyBookmarks;

	//////////////////////////////
	// Crane Cam Anchor Settings
	//////////////////////////////
	[Header("Settings apply to crane camera type only")]
	public float craneMaxOrbitHeight;
	private float craneMinOrbitRange = 5;
	[Tooltip("Minimum is 5")]
	public float craneMaxOrbitRange = 3;
	public float craneMinPitch;
	public float craneMaxPitch;

	[Space]
	[Range(0, 100)]
	public float craneOrbitHeight;
	[Range(0, 100)]
	public float craneOrbitRange;
	[Range(0, 100)]
	public float cranePitch;
	public float craneYaw;
	[Space]
	[Tooltip("HRPY = Heights (x), Ranges (y), Pitches (z) and Yaws (w)")]
	public List<Vector4> craneHRPYBookmarks;

	[Space]
	public float craneCamSuspensionHeight;
	[Tooltip("When enabled the cam automatically compensates for crane movement/rotation if no camera target was set")]
	public bool craneCamUseGyro;
	public bool craneCamPitchLimit;
	public Vector2 craneCamMinMaxPitch;
	public bool craneCamYawLimit;
	public Vector2 craneCamMinMaxYaw;
	public bool craneCamRollLimit;
	public Vector2 craneCamMinMaxRoll;

	[Space]
	[Range(0, 100)]
	public float craneCamPitch;
	[Range(0, 100)]
	public float craneCamYaw;
	[Range(0, 100)]
	public float craneCamRoll;
	[Space]
	public List<Vector3> craneCamRotationBookmarks;

	//////////////////////////////
	// Drone Cam Anchor Settings
	//////////////////////////////
	[Header("Settings apply to drone camera type only")]
	[Tooltip("Only use positive numbers. The main position is the middle of a virtual floor")]
	public Vector3 droneMaxRange;

	[Space]
	public float droneMaxSpeed;
	[Range(0, 100)]
	public float droneSpeed;
	public float droneMaxAcceleration;
	[Range(0, 100)]
	public float droneAcceleration;
	public float droneMaxBrake;
	[Range(0, 100)]
	public float droneBrake;

	[Space]
	[Range(-100, 100)]
	public float dronePositionX;
	[Range(0, 100)]
	public float dronePositionY;
	[Range(-100, 100)]
	public float dronePositionZ;

	[Space]
	public List<Vector3> droneCamWaypoints;

	////////////////////////////////////////////////////////////
	// Actual camera settings such as focal length, zoom, etc.
	////////////////////////////////////////////////////////////
	[Header("Settings apply to the actual camera only")]
	[Tooltip("This is a requirement!")]
	public GameObject cameraObject;

	// public float camMaxZoom;
	public float camMinFOV = 50;
	public float camMaxFOV = 90;
	[Tooltip("Controls the amount of zoom between 0% (=camMinFOV) and 100% (=camMaxFOV).")]
	[Range(0, 100)]
	public float camZoom;
	//[Range(0, 100)]
	//public float camFOV;
	[Space]
	public List<float> camZoomBookmarks;

	[Space]
	public bool useCamTargetVisual;
	public List<GameObject> camTargetVisuals;       // Objects to look at. This should not be an actual object of the scene but a dedicated empty 3D object! Otherwise it may not capture what it needs to capture
	public bool useCamTargetVirtual;
	public List<GameObject> camTargetVirtuals;     // Coordinates to look at

	//[Header("Exposed privates... Kappa")]
	/// <summary>
	/// Extra required stuff
	/// </summary>
	//[SerializeField]
	private GameObject anchorCrane;
	//[SerializeField]
	private GameObject anchorCam;

	private string prevCameraType;

	private Dictionary<string, string> cameraTypeControls = new Dictionary<string, string>();
	private Dictionary<string, string> cameraTypeSettings;

	private CameraDirector cameraDirector;

	// Use this for initialization
	void Start() {
		anchorCrane = null;
		anchorCam = null;

		CheckCamera();
		CreateAnchors();

		prevCameraType = cameraType.ToString();

		cameraDirector = GameObject.FindGameObjectWithTag("Director").GetComponent<CameraDirector>();
		cameraDirector.RegisterCamera(gameObject);
	}

	private void CheckCamera() {
		if (cameraObject == null || cameraObject.GetComponent<Camera>() == null) {
			//Debug.LogError("No camera object set or missing Camera component on set camera object for " + gameObject.name + "!");
		}
	}

	private void CreateAnchors() {
		if (transform.childCount == 0) {
			//Debug.Log("No children objects found. Creating Crane and Cam Anchors...");

			anchorCrane = new GameObject();
			anchorCrane.name = "Crane Anchor";
			anchorCrane.transform.parent = gameObject.transform;
			anchorCam = new GameObject();
			anchorCam.name = "Cam Anchor";
			anchorCam.transform.parent = anchorCrane.transform;
		} else {
			//Debug.Log("Children objects found. Searching for Crane Anchor...");
			GameObject tempGO = null;

			// Look for the crane anchor point
			for (int i = 0; i < transform.childCount; i++) {
				tempGO = transform.GetChild(i).gameObject;

				if (tempGO.name == "Crane Anchor") {
					//Debug.Log("Crane Anchor found. Setting to anchorCrane...");
					anchorCrane = tempGO;

					if (anchorCrane.transform.childCount > 0) {
						tempGO = null;
						//Debug.Log("Children found in Crane Anchor. Searching for Cam Anchor...");
						for (int u = 0; u < anchorCrane.transform.childCount; u++) {
							tempGO = anchorCrane.transform.GetChild(u).gameObject;
							//Debug.Log("Currently checking: " + tempGO.name);

							if (tempGO.name == "Cam Anchor") {
								//Debug.Log("Cam Anchor found. Setting to anchorCam...");
								anchorCam = tempGO;
								break;
							}
						}
						if (anchorCam == null) {
							//Debug.Log("Cam Anchor not found. Creating Cam Anchor...");
							anchorCam = new GameObject();
							anchorCam.name = "Cam Anchor";
							anchorCam.transform.parent = anchorCrane.transform;
						}

					} else {
						//Debug.Log("No children found. Creating Cam Anchor...");
						anchorCam = new GameObject();
						anchorCam.name = "Cam Anchor";
						anchorCam.transform.parent = anchorCrane.transform;
					}
					break;
				}
			}
			if (anchorCrane == null) {
				//Debug.Log("Crane Anchor not found. Creating Crane and Cam Anchors");
				anchorCrane = new GameObject();
				anchorCrane.name = "Crane Anchor";
				anchorCrane.transform.parent = gameObject.transform;
				anchorCam = new GameObject();
				anchorCam.name = "Cam Anchor";
				anchorCam.transform.parent = anchorCrane.transform;
			}
		}

		UpdateCameraProperties();
	}

	// Update camera properties
	private void UpdateCameraProperties() {
		Vector3 fixedBasePosition = transform.position;

		// Applying fixed camera properties
		if (cameraType == cameraTypes.Fixed) {
			// Setting base
			anchorCrane.transform.localPosition = new Vector3(0, 0, 0);
			anchorCrane.transform.localEulerAngles = new Vector3(0, 0, 0);

			// Setting camera height
			float fixedHeightCalc = fixedMaxHeight * (fixedHeight / 100);
			anchorCam.transform.localPosition = new Vector3(0, fixedHeightCalc, 0);

			// Setting camera angle
			anchorCam.transform.localEulerAngles = cameraRotation;
		}

		// Applying dolly camera properties
		if (cameraType == cameraTypes.Dolly) {
			// Setting base
			anchorCrane.transform.localPosition = new Vector3(0, 0, 0);
			anchorCrane.transform.localEulerAngles = new Vector3(0, 0, 0);

			// Setting camera height and dolly position
			float dollyHeightCalc = dollyMaxHeight * (dollyHeight / 100);
			float dollyMovementCalc = dollyMaxMovement * (dollyMovement / 100);

			anchorCam.transform.localPosition = new Vector3(dollyMovementCalc, dollyHeightCalc, 0);

			// Setting camera angle
			anchorCam.transform.localEulerAngles = cameraRotation;
		}

		// Applying crane camera properties
		if (cameraType == cameraTypes.Crane) {
			// Setting the camera's suspension height
			// cameraObject.transform.localPosition = new Vector3(0, craneCamSuspensionHeight, 0);

			// Setting crane anchor's height
			float craneHeightCalc = craneMaxOrbitHeight * (craneOrbitHeight / 100);
			anchorCrane.transform.localPosition = new Vector3(0, craneHeightCalc, 0);

			// Setting crane anchor's rotation
			// Making sure craneMinPitch is less or equal to craneMaxPitch
			if (craneMinPitch > craneMaxPitch) {
				craneMinPitch = craneMaxPitch;
			}

			// Calculating how much the crane can actually pitch
			float cranePitchSize;
			if (craneMinPitch < 0) {
				cranePitchSize = craneMinPitch * -1 + craneMaxPitch;
			} else {
				cranePitchSize = craneMaxPitch - craneMinPitch;
			}

			float cranePitchCalc = -1 * (cranePitchSize * (cranePitch / 100) + craneMinPitch);

			// Setting full crane arm rotation
			anchorCrane.transform.localEulerAngles = new Vector3(cranePitchCalc, craneYaw, 0);

			// Setting the camera anchor rotation properties
			// Establishing the orbit ring
			if (craneMaxOrbitRange < craneMinOrbitRange) {
				craneMaxOrbitRange = craneMinOrbitRange;
			}
			float anchorCamPosCalc = craneMaxOrbitRange * (craneOrbitRange / 100);

			// Calculating the camera's rotation
			Vector3 craneCamRotCalc;

			// First the pitch values
			if (!craneCamPitchLimit) {
				//craneCamRotCalc.x = cameraRotation.x + anchorCrane.transform.eulerAngles.x;
				craneCamRotCalc.x = anchorCrane.transform.rotation.x + cameraRotation.x;
			} else {
				float craneCamPitchSize;

				if (craneCamMinMaxPitch.x < 0) {
					craneCamPitchSize = craneCamMinMaxPitch.x * -1 + craneCamMinMaxPitch.y;
				} else {
					craneCamPitchSize = craneCamMinMaxPitch.y - craneCamMinMaxPitch.x;
				}
				//float cranePitchCalc = -1 * (cranePitchSize * (cranePitch / 100) + craneMinPitch);
				craneCamRotCalc.x = craneCamPitchSize * (craneCamPitch / 100) + craneCamMinMaxPitch.x;

				// Making sure it doesn't go into the negative so 100% is not the lowest number but the highest
				if (craneCamPitchSize < 0) {
					craneCamPitchSize = craneCamPitchSize * -1;
				}
			}

			// Second the yaw values
			if (!craneCamYawLimit) {
				// craneCamRotCalc.y = cameraRotation.y;
				craneCamRotCalc.y = anchorCrane.transform.rotation.y + cameraRotation.y;
			} else {
				float craneCamYawRange;

				if (craneCamMinMaxYaw.x < 0) {
					craneCamYawRange = craneCamMinMaxYaw.x * -1 + craneCamMinMaxYaw.y;
				} else {
					craneCamYawRange = craneCamMinMaxYaw.y - craneCamMinMaxYaw.x;
				}

				/*if (craneCamYawRange < 0) {
					craneCamYawRange = craneCamYawRange * -1;
				}*/

				craneCamRotCalc.y = craneCamYawRange * (craneCamYaw / 100) - (craneCamYawRange * 0.5f); // 1653.60
			}

			// And third the roll values
			if (!craneCamRollLimit) {
				craneCamRotCalc.z = anchorCrane.transform.rotation.z + cameraRotation.z;
			} else {
				float craneCamRollRange;

				if (craneCamMinMaxYaw.x < 0) {
					craneCamRollRange = craneCamMinMaxRoll.x * -1 + craneCamMinMaxRoll.y;
				} else {
					craneCamRollRange = craneCamMinMaxRoll.y - craneCamMinMaxRoll.x;
				}

				if (craneCamRollRange < 0) {
					craneCamRollRange = craneCamRollRange * -1;
				}

				craneCamRotCalc.z = craneCamRollRange * (craneCamRoll / 100) - (craneCamRollRange * 0.5f);
			}

			// Actually setting camera the rotation
			//cameraRotation.x = craneCamRotCalc.x * -1;

			// Setting camera angle
			if (craneCamUseGyro) {
				anchorCam.transform.eulerAngles = new Vector3(0, anchorCrane.transform.rotation.y + craneCamRotCalc.y, 0);
				cameraObject.transform.localEulerAngles = new Vector3(anchorCrane.transform.rotation.x + craneCamRotCalc.x, 0, anchorCrane.transform.rotation.z + craneCamRotCalc.z);
			} else {
				anchorCam.transform.localEulerAngles = new Vector3(0, anchorCrane.transform.rotation.y + craneCamRotCalc.y, 0);
				cameraObject.transform.localEulerAngles = new Vector3(anchorCrane.transform.rotation.x + craneCamRotCalc.x, 0, anchorCrane.transform.rotation.z + craneCamRotCalc.z);
			}
			// Actually setting the position (done after all the anchors' rotations)
			//anchorCam.transform.localPosition = new Vector3(0, craneCamSuspensionHeight, anchorCamPosCalc);
		}

		// Applying drone camera properties
		if (cameraType == cameraTypes.Drone) {
			anchorCrane.transform.localPosition = new Vector3(0, 0, 0);
			anchorCrane.transform.localEulerAngles = new Vector3(0, 0, 0);
			anchorCam.transform.localEulerAngles = new Vector3(0, 0, 0);

			Vector3 droneCamPosCalc;

			droneCamPosCalc.x = droneMaxRange.x * (dronePositionX / 100);
			droneCamPosCalc.y = droneMaxRange.y * (dronePositionY / 100);
			droneCamPosCalc.z = droneMaxRange.z * (dronePositionZ / 100);


			anchorCam.transform.localPosition = new Vector3(droneCamPosCalc.x, droneCamPosCalc.y, droneCamPosCalc.z);

			// Setting camera angle
			anchorCam.transform.localEulerAngles = cameraRotation;
		}
	}

	/// <summary>
	/// Director Control
	/// 
	/// The key is: the controls/settings section name|the name of the control/setting, eg: Camera Controller|Rotation
	/// If the key's section name contains an underscore it means that the controls/settings with the same number behind the underscore
	/// are grouped together in a sub section underneath each other, eg: controller_0|zoom, controller_0|focus
	/// The value is the control type (+ type attributes) + value(s), eg: fslider|-100_100|value / vector3_x_y_z / bool|true
	/// 
	/// Type of controls on GUI:
	/// bool											= A checkmark or on/off-switch
	/// ftext											= float input text area, also draggable
	/// fknob_fmin_fmax									= float knob with limits, fN = minimum value, fX is maximum value for the knob (not the controlled)
	/// fknobinf_fSense									= float knob without limits, fSense is sensitivity. Maybe make this 
	/// iknob_fmin_fmax									= int knob
	/// fslider_fmin_fmax								= float slider, fN = minimum value, fX is maximum value for the slider (not the controlled)
	/// islider_imin_imax								= int slider
	/// v2input											= Vector2 input text, also draggable
	/// v2fpad_fXmin_fXmax_fYmin_fYmax					= Vector2 finite pad
	/// v2ipad_fXsense_fYsense							= Vector2 infinite touch(/drag) pad, X-sensitivity and Y-sensitivity
	/// v3input											= Vector3 input text, also draggable
	/// v3fpad_fXmin_fXmax_fYmin_fYmax_fZmin_fZmax		= Vector3 finite pad, scroll the mousewheel to control the Z value
	/// v3ipad_fXsense_fYsense_fZsense					= Vector3 infinite pad
	/// v3fball_fXmin_fXmax_fYmin_fYmax_fZmin_fZmax		= Vector3 finite ball
	/// v3fball_fXsense_fYsense_fZsense					= Vector3 infinite ball
	/// v3list											= Vector3 list
	/// v3list_name										= Vector3 list with name per entry. A pseudo dictionary for naming bookmarks
	/// 
	/// Note: The names of the controls and settings properties could be split up as well by a | so UI sections can be made for the controls/settings to separate them.
	/// Example: "Crane|Max Orbit Height", "Crane|Max Pitch", "Camera|Min Max Pitch", Camera|Min Max Yaw".
	/// </summary>
	public Dictionary<string, string> GetDirectorControls () { // (string type) { // THESE ARE THE MAIN CONTROLS, VISIBLE FOR THE DIRECTORS MAIN SCREEN
		Debug.Log("Setting camera controls for Director");
		cameraTypeControls.Clear();

		// Adding base camera controls
		cameraTypeControls.Add("Camera Controls|Rotation", "v3input|" + cameraRotation);
		cameraTypeControls.Add("Camera Controls|Zoom", "fslider|0_100|" + camZoom);
		cameraTypeControls.Add("Camera Controls_0|Use Virtual Target", "bool|" + useCamTargetVirtual);
		cameraTypeControls.Add("Camera Controls_1|Use Visual Target", "bool|" + useCamTargetVisual);

		/*cameraTypeControls.Add("Camera Bookmarks|Zoom Bookmarks", "v3list_name|" + camZoomBookmarks);
		cameraTypeControls.Add("Camera Bookmarks|Zoom Bookmarks", "v3list_name|" + camZoomBookmarks);
		cameraTypeControls.Add("Camera Bookmarks|Zoom Bookmarks", "v3list_name|" + camZoomBookmarks);*/

		//if (type == "fixed") {
		if (cameraType == cameraTypes.Fixed) {
			cameraTypeControls.Add("Controller|Height", "fslider|0_100|" + fixedMaxHeight);
		}

		//if (type == "dolly") {
		if (cameraType == cameraTypes.Dolly) {
			cameraTypeControls.Add("Controller|Height", "fslider|0_100|" + dollyHeight);
			cameraTypeControls.Add("Controller|Movement", "fslider|-100_100|" + dollyMovement);
		}

		//if (type == "crane") {
		if (cameraType == cameraTypes.Crane) {
			cameraTypeControls.Add("Crane Controller|Orbit Height", "fslider|0_100|" + craneOrbitHeight);
			cameraTypeControls.Add("Crane Controller|Orbit Range", "fslider|0_100|" + craneOrbitRange);
			cameraTypeControls.Add("Crane Controller|Pitch", "fslider|0_100|" + cranePitch);
			cameraTypeControls.Add("Crane Controller|Yaw", "ftext|" + craneYaw);

			cameraTypeControls.Add("Switches_0|Use Camera Gyro", "bool|" + craneCamUseGyro);
			cameraTypeControls.Add("Switches_0|Camera Limit Pitch", "bool|" + craneCamPitchLimit);
			cameraTypeControls.Add("Switches_0|Camera Limit Yaw", "bool|" + craneCamYawLimit);
			cameraTypeControls.Add("Switches_0|Camera Limit Roll", "bool|" + craneCamRollLimit);

			cameraTypeControls.Add("Camera Controller|Pitch", "fslider|0_100|" + craneCamPitch);
			cameraTypeControls.Add("Camera Controller|Yaw", "fslider|0_100|" + craneCamYaw);
			cameraTypeControls.Add("Camera Controller|Roll", "fslider|0_100|" + craneCamRoll);
		}

		//if (type == "drone") {
		if (cameraType == cameraTypes.Drone) {
			cameraTypeControls.Add("Speed", "fslider|0_100|" + droneSpeed);
			cameraTypeControls.Add("Acceleration", "fslider|0_100|" + droneAcceleration);
			cameraTypeControls.Add("Brake", "fslider|0_100|" + droneBrake);

			cameraTypeControls.Add("Position X", "fslider|-100_100|" + dronePositionX);
			cameraTypeControls.Add("Position Y", "fslider|0_100|" + dronePositionY);
			cameraTypeControls.Add("Position Z", "fslider|-100_100|" + dronePositionZ);
		}

		return cameraTypeControls;
	}

	public Dictionary<string, string> GetDirectorSettings() { //(string type) { // THESES ARE THE SETTINGS, THE FIXED NUMBERS AND LIMITS THAT THE CONTROLS ABOVE USE
		Debug.Log("Setting camera settings for Director");

		cameraTypeSettings.Clear();
		cameraTypeSettings.Add("Camera Type", cameraType.ToString());

		//if (type == "fixed") {
		if (cameraType == cameraTypes.Fixed) {
			cameraTypeSettings.Add("Max Height", "ftext|" + fixedMaxHeight);
		}

		//if (type == "dolly") {
		if (cameraType == cameraTypes.Dolly) {
			cameraTypeSettings.Add("Max Height", "ftext|" + dollyMaxHeight);
			cameraTypeSettings.Add("Max Movement", "ftext|" + dollyMaxMovement);
		}

		//if (type == "crane") {
		if (cameraType == cameraTypes.Crane) {
			cameraTypeSettings.Add("Max Orbit Height", "ftext|" + craneMaxOrbitHeight);
			cameraTypeSettings.Add("Max Orbit Range", "ftext|" + craneMaxOrbitRange);
			cameraTypeSettings.Add("Min Pitch", "ftext|" + craneMinPitch);
			cameraTypeSettings.Add("Max Pitch", "ftext|" + craneMaxPitch);

			cameraTypeSettings.Add("Cam Min Max Pitch", "v2input|" + craneCamMinMaxPitch);
			cameraTypeSettings.Add("Cam Min Max Yaw", "v2input|" + craneCamMinMaxYaw);
			cameraTypeSettings.Add("Cam Min Max Roll", "v2input|" + craneCamMinMaxRoll);
		}

		//if (type == "drone") {
		if (cameraType == cameraTypes.Drone) {
			cameraTypeSettings.Add("Max Range", "v3input|" + droneMaxRange);
			cameraTypeSettings.Add("Max Speed", "ftext|" + droneMaxSpeed);
			cameraTypeSettings.Add("Max Acceleration", "ftext|" + droneMaxAcceleration);
			cameraTypeSettings.Add("Max Brake", "ftext|" + droneMaxBrake);
		}

		return cameraTypeSettings;
	}

	// Actually controlling the current camera type and camera settings from the director
	public void SystemsControl(int cameraTypeId) {

	}

	// Update is called once per frame
	void Update () {
		if (prevCameraType != cameraType.ToString()) {
			Debug.Log(prevCameraType + " ---- " + cameraType.ToString());
			// SetDirectorControls();

			cameraDirector.RegisterCamera(gameObject);
			prevCameraType = cameraType.ToString();
		}

		//UpdateCameraProperties();
	}
}