using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WebcamDevicesYV003 : MonoBehaviour {

    public List <string> webcamNames;
    public int webcamId;

    private int currentID;
    private Renderer renderer;
    private WebCamTexture webcamFeedA;			// First feed
    private WebCamTexture webcamFeedB;			// Second feed
    private bool feedIsA = true;				// Internal check for webcamFeedA

    // Gets the list of devices and prints them to the console.
    void Start() {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++) {
            //Debug.Log("Webcam found: " + devices[i].name);

            if (!webcamNames.Contains(devices[i].name)) {
                webcamNames.Add(devices[i].name);
            }
        }

        renderer = GetComponent<Renderer>();
        Debug.Log("Renderer found: " + renderer);
    }

    void SwitchWebcam (int id) {
        Debug.Log(gameObject.name + ": Switching webcam from " + webcamNames[currentID] + " (Id: " + currentID + ") to " + webcamNames[id] + " (Id: " + id + ")");

        if (feedIsA) { // If current feed is A, set B
            webcamFeedB = new WebCamTexture(webcamNames[id]);
            renderer.material.SetTexture("_BaseColorMap", webcamFeedB);
            renderer.material.SetTexture("_EmissionMap", webcamFeedB);
            webcamFeedB.Play();

            if (webcamFeedA != null) {
                webcamFeedA.Stop(); // Stops the feed if there was one (turns off webcam)
            }
        } else { // If current feed is NOT A, set A
            webcamFeedA = new WebCamTexture(webcamNames[id]);
            renderer.material.SetTexture("_BaseColorMap", webcamFeedA);
			renderer.material.SetTexture("_EmissionMap", webcamFeedA);
            webcamFeedA.Play();

            if (webcamFeedB != null) {
                webcamFeedB.Stop(); // Stops the feed if there was one (turns off webcam)
            }
        }

        feedIsA = !feedIsA; // Flips 
        currentID = id;

        if (!Application.isPlaying) { // If the game is stopped in the editor, kill all feeds
            webcamFeedA.Stop();
            webcamFeedB.Stop();
        }
    }

    void Update() {
        if (currentID != webcamId && Application.isPlaying) { // If current webcamId is not the same as set Id, set feed to set Id
            SwitchWebcam(webcamId);
        }
    }
}