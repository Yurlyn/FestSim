using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour {
    public void Log (string msg, string debugType) {
        if (debugType.ToLower() == "war") {
            Debug.LogWarning(gameObject.name + ": " + msg);
        } else if (debugType.ToLower() == "err") {
            Debug.LogError(gameObject.name + ": " + msg);
        } else {
            Debug.Log(gameObject.name + ": " + msg);
        }
    }
}