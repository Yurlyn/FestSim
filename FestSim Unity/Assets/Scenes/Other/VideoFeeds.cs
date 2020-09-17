using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VideoFeeds : MonoBehaviour {
    /// <summary>
    /// This script is to get all feeds currently available to the computer
    /// It will also keep track of what Video Feed Controller is currently using what feed
    /// </summary>

    public List <string> feedNames;                 // A list of names for video feeds such as webcams or capture cards
    public List <WebCamTexture> activeFeeds;        // A list of the id's from feedNames that need to be active
    public List <GameObject> activeProjectors;      // A list of "projector" game objects that are using a feed

    public List <WebCamTexture> feeds;              // Stores all feeds

    //public List <WebCamTexture> activeFeeds;

    // Gets the list of devices and prints them to the console.
    void Start() {
        WebCamDevice[] hardwareDevices = WebCamTexture.devices;
        for (int i = 0; i < hardwareDevices.Length; i++) {
            //Debug.Log("Feed found: " + devices[i].name);

            if (!feedNames.Contains(hardwareDevices[i].name)) {
                feedNames.Add(hardwareDevices[i].name);
            }
        }
    }

    public int GetVideoFeedsCount () {
        return feedNames.Count;
    }

    /*public void CheckFeeds(WebCamTexture wct) {
        for (int i = 0; i < feedNames.Count; i++) {
            if (wct.isPlaying) {
                activeFeeds.Add(wct.name);
            }
        }
    }*/

    public WebCamTexture RequestVideoFeed (GameObject requester, int id) {
        //Debug.Log("Video feed requested by \"" + requester.name + "\" for feed \"" + feedNames[id] + "\" (ID: " + id + ")");

        // Put the feeds list here!


        // Debug.Log("Feed \"" + feedNames[id] + "\" playing: " + WebCamTexture(feedNames[id]).isPlaying);

        WebCamTexture requestedFeed = new WebCamTexture(feedNames[id]);
        requestedFeed = new WebCamTexture(feedNames[id]);

        //Debug.Log("Active feeds: " + activeFeeds.Count);

        if (activeFeeds.Count != 0) {
            //Debug.Log("Active feeds is not 0: " + activeFeeds.Count);
            for (int i = 0; i < activeFeeds.Count; i++) {
                //Debug.Log("For loop: " + i);
                if (activeFeeds[i].deviceName == requestedFeed.deviceName && activeFeeds[i].isPlaying) {
                    //Debug.Log("Feed already exists. \"" + activeFeeds[i].deviceName + "\" is playing: " + activeFeeds[i].isPlaying);
                } else if (activeFeeds[i].deviceName == requestedFeed.deviceName && !activeFeeds[i].isPlaying) {
                    //Debug.Log("Feed already exists but isn't playing. \"" + activeFeeds[i].deviceName + "\" is playing: " + activeFeeds[i].isPlaying);
                    
                    requestedFeed.Play();

                    //Debug.Log("Playing: \"" + requestedFeed.deviceName + "\" = " + requestedFeed.isPlaying);
                } else {
                    //Debug.Log("Feed doesn't exist. \"" + feedNames[i] + "\": " + requestedFeed.isPlaying);

                    activeFeeds.Add(requestedFeed);
                    requestedFeed.Play();

                    //Debug.Log("New feed added: \"" + requestedFeed.deviceName + "\"");
                }
            }
        } else {
            activeFeeds.Add(requestedFeed);
            requestedFeed.Play();

            //Debug.Log("Feed added: \"" + requestedFeed.deviceName + "\"");
        }

        //Debug.Log("===================================================================================================");

        /*if (!activeFeeds.Contains()) {
            activeFeeds.Add(requestedFeed);
            requestedFeed.Play();
            Debug.Log("Feed added: \"" + activeFeeds[id].deviceName + "\"");
        }*/

        /*if (!activeFeeds[id].isPlaying) {
            Debug.Log("Requested feed is not playing");
            requestedFeed.Play();
            activeFeeds[id] = requestedFeed;
            //Debug.Log("Playing requested feed: " + requestedFeed.isPlaying + ", " + requestedFeeds[id].isPlaying);
        }*/

        /*for (int i = 0; i < feedNames.Count; i++) {
            Debug.Log("Checking currently playing feeds: \"" + feeds[id] + "\": " + requestedFeeds[id].isPlaying);
        }*/

        //Debug.Log("Feed \"" + feedNames[id] + "\" is set to playing: " + WebCamTexture(feedNames[id]).isPlaying);

        //CheckFeeds(requestedFeed);

        // Adding requesting game object to activeProjectors list
        if (!activeProjectors.Contains(requester)) {
            activeProjectors.Add(requester);
        }
        // Adding requested feed activeFeeds list
        /*if (!activeFeeds.Contains(feedNames[id])) {
            activeFeeds.Add(feedNames[id]);
        }*/

        return requestedFeed;
    }
}