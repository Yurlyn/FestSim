using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WebcamDevices : MonoBehaviour {

    public List <string> webcamNames;
    public int webcamId;

    private int currentID;
    private Renderer renderer;
    private WebCamTexture webcamFeedA;
    private WebCamTexture webcamFeedB;
    private bool feedIsA = true;

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

    void SetNewWebcam (int id) {
        Debug.Log("Active webcam set to: id " + webcamId + ", \"" + webcamNames[webcamId] + "\"");
        webcamFeedA = new WebCamTexture(webcamNames[id]);
        renderer.material.mainTexture = webcamFeedA;
        renderer.material.SetTexture("_EmissionMap", webcamFeedA);
        webcamFeedA.Play();

        currentID = id;

        if (!Application.isPlaying) {
            webcamFeedA.Stop();
        }
    }

    void SwitchWebcam (int id) {
        Debug.Log(gameObject.name + ": Switching webcam from " + webcamNames[currentID] + " (Id: " + currentID + ") to " + webcamNames[id] + " (Id: " + id + ")");

        if (feedIsA) {
            webcamFeedB = new WebCamTexture(webcamNames[id]);
            renderer.material.mainTexture = webcamFeedB;
            renderer.material.SetTexture("_EmissionMap", webcamFeedB);
            webcamFeedB.Play();

            if (webcamFeedA != null) {
                webcamFeedA.Stop();
            }
        } else {
            webcamFeedA = new WebCamTexture(webcamNames[id]);
            renderer.material.mainTexture = webcamFeedA;
            webcamFeedA.Play();

            if (webcamFeedB != null) {
                webcamFeedB.Stop();
            }
        }

        feedIsA = !feedIsA;
        currentID = id;

        if (!Application.isPlaying) {
            webcamFeedA.Stop();
            webcamFeedB.Stop();
        }
    }

    void Update() {
        if (currentID != webcamId && Application.isPlaying) {
            //SetNewWebcam(webcamId);
            SwitchWebcam(webcamId);
        }
    }
}