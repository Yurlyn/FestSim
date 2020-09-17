using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatCommandsHandler : MonoBehaviour {
    /// <summary>
    /// This script handles the chat commands by filtering the first word of a chat message.
    /// If a command has been found it will trigger a camera switch, movement, etc. or it
    /// will handle the commanding viewer's avatar.
    /// 
    /// Some hardcoded commands:
    /// !blacklist add/remove <username>            : Adds or removes a viewer to/from the blacklist.
    ///                                               A user on the blacklist can not use any commands
    /// !admin add/remove <username>                : Adds or removes a viewer to/from the admin list
    ///                                               A user on the admin list can use admin commands
    /// </summary>

    // Public Declarations
    public List<string> adminCommands;
    public List<string> viewerCommands;
    
    public List<string> viewersBlacklist;       // To add or remove a viewer to/from the blacklist use: !blacklist add/remove <username>

    // Private Declarations
    public List<string> admins;

    private TwitchChat twitchChat;

    // Use this for initialization
    void Start () {
        twitchChat = GetComponent<TwitchChat>();

        // Adding hardcoded commands to the adminCommands
        adminCommands.Add("!admin");
        adminCommands.Add("!blacklist");
    }

    // For adding and removing admins
    public void Admins (string cmd, string user) {
        // Adding admins
        if (!admins.Contains("user") && cmd.ToLower() == "add") {
            admins.Add(user.ToLower());
        }

        // Removing admins
        if (admins.Contains("user") && cmd.ToLower() == "remove") {
            admins.Remove(user.ToLower());
        }
    }

    // Adding a viewer to the blacklist, this will prevent the user from using commands
    private void BlacklistViewer (string cmd, string user) {
        // Adding viewer to the blacklist
        if (cmd == "add") {
            Debug.Log(string.Format("Adding {0} to the blacklist BibleThump", user));
            twitchChat.WriteToChat(string.Format("{0} was added to the blacklist BibleThump", user));
        } else if (cmd == "remove") {  // Removing viewer from the blacklist
            Debug.Log(string.Format("Removing {0} from the blacklist PogChamp", user));
            twitchChat.WriteToChat(string.Format("Removed {0} from the blacklist PogChamp", user));
        }
    }

    // Used by the TwitchChat.cs
    public void ChatInput(string user, string msg) {
        Debug.Log(string.Format("CCH ChatIput triggered by {0} with message: {1}", user, msg));

        string response = "";

        // YMV: Check if a viewerCommand was said by just checking the first word
        string cmd = msg.Split(' ')[0];

        if (adminCommands.Contains(cmd)) {
            Debug.Log(string.Format("Viewer {0} command triggered: {1}", user, msg));
            
            if (!viewersBlacklist.Contains(user)) {
                if (viewerCommands.Contains(cmd)) {
                    // Execute commands here. There will be a finite list of commands to handle
                    Debug.Log("Executing command \"" + cmd + "\"");
                    response = string.Format("{0}, herp derp", user);
                }
            }
        }

        twitchChat.WriteToChat(response); // Sends feedback to the Twitch chat
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}