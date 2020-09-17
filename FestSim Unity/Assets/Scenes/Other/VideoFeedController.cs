using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class VideoFeedController : MonoBehaviour {
    /// <summary>
    /// This script is for the canvas/monitor/screen/whatever object.
    /// This will eventually play a feed from the videoFeedsObject's VideoFeeds component onto the object this script is set to
    /// </summary>

    public GameObject videoFeedsObject;                     // The game object that contains the VideoFeeds script. Only 1 game object should have that script!!
    private VideoFeeds videoFeeds;                          // Used as a shortcut for the VideoFeeds script in the videoFeedsObject

    public int videoFeedId;                                 // Used to play this id's feed onto the canvas
    [SerializeField] protected string videoFeedIdName;      // Stores the name of videoFeedId

    public string currentFeedName;                          // The name of the current feed displayed

    private int videoFeedsCount;
    private int currentFeedId;
    private Renderer canvas;

    private WebCamTexture feedA;
    private WebCamTexture feedB;

    // Gets the list of devices and prints them to the console.
    void Start() {
        if (videoFeedsObject == null) {
            //Debug.LogError(gameObject.name + ": No Video Feeds Object was set!");
        } else {
            //Debug.Log("Video Feeds Object was set to " + videoFeedsObject.name);

            if (!videoFeedsObject.GetComponent<VideoFeeds>()) {
                //Debug.Log(videoFeedsObject.name + " does not contain the VideoFeeds script! " + videoFeedsObject.GetComponent<VideoFeeds>());
            } else {
                videoFeeds = videoFeedsObject.GetComponent<VideoFeeds>();   // Sets a shortcut for videoFeedsObject's VideoFeed script

                videoFeedsCount = videoFeeds.GetVideoFeedsCount();
                // Debug.Log("VideoFeedsCount is " + videoFeedsCount);
            }
        }

        canvas = GetComponent<Renderer>();
    }

    public void FeedCheck () {

    }

    void SwitchFeed () {
        //Debug.Log("Switching feeds...");

        //await new WaitUntil(() => videoFeeds.activeFeeds.Contains();
        WebCamTexture requestedFeed = videoFeeds.RequestVideoFeed(gameObject, videoFeedId);

        // Setting the canvas to the feed
        canvas.material.SetTexture("_BaseColorMap", requestedFeed);      // Used for Unity version 2018 or up
        canvas.material.mainTexture = requestedFeed;                        // Used for Unity version 2017

        // Setting the canvas' emission channel
        canvas.material.SetTexture("_EmissionMap", requestedFeed);

        currentFeedId = videoFeedId;
    }

    void Update() {
        if (currentFeedId != videoFeedId &&
            videoFeedId <= videoFeedsCount &&
            videoFeedsCount >= 0 &&
            Application.isPlaying) { // If currentFeedId is not the same as set videoFeedId, set currentFeedId as videoFeedId
            SwitchFeed();           
        }
    }
}