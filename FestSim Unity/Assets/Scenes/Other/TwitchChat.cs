// Source: https://tutorials.twitchlayout.stream/integrate-twitch-into-a-unity-game-unity-c-tutorial/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;

public class TwitchChat : MonoBehaviour {

    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    public string username, password, channelName; //Get the password from https://twitchapps.com/tmi

    public Text chatBox;
    public Rigidbody player;
    public int speed;

    // TwitchInteractive is of de functie uberhaubt aan staat, en canTwitchControl zijn een aantal mensen die uiteindelijk de controls echt kunnen gebruiken
    public bool TwitchInteractive = true;
    public bool canTwitchControl = false;

    // Losse dingen voor de commands van de chat!
    public GameObject MainCam;
    public GameObject KeyBindScript;
    public GameObject PartyPerson;
    public GameObject partyPeopleSpawner;

    //private FollowPeople followPeople;
    public List<string> authNames;

    public List<string> viewerNames;

    public int timer = 60;
    public float count;

    //test voor controle
    public string uname;

    public string chatText;
    public bool send = false;
    public float newData;
    public float totalData;

    private ChatCommandsHandler chatCmdsHndlr;
    private float reconnectTime = 2f;
    private float reconnectTryTime;

    void Start() {
        MainCam = GameObject.Find("MainCamera");
        KeyBindScript = GameObject.Find("KeyBindingsManager");
        PartyPerson = GameObject.Find("PartyPerson");
        partyPeopleSpawner = GameObject.Find("PartyPeopleSpawner");

        // YMV: Setting the proper ChatCommandHandler so we can use the one in the Unity Inspector
        chatCmdsHndlr = GetComponent<ChatCommandsHandler>();
        // YMV: Adding the main admin to the admins list just in case this hasn't been set in the Inspector
        chatCmdsHndlr.Admins("add", username);

        //followPeople = PartyPerson.GetComponent<FollowPeople>();

        Connect();
        //StateConnection();
    }

    IEnumerator StateConnection () {
        while (!twitchClient.Connected) {
            yield return null;
        }
        WriteToChat("Project connected...");
    }

    private void Connect() {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();
    }

    private void ReadChat() {
        //Debug.Log("Reading chat...");

        //if (twitchClient.Available > 0) {
            var message = reader.ReadLine(); //Read in the current message
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            //Debug.Log("Read message: " + message);

            if (message.Contains("PING")) {
                //Debug.Log("Recieved PING from Twitch. Ponging...");
                writer.WriteLine("PONG :tmi.twitch.tv");
                writer.Flush();
            }

            if (message.Contains("PRIVMSG")) {
                //Debug.Log("Message contains a chat line");
                //Get the users name by splitting it from the string
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint);
                chatName = chatName.Substring(1);

                //Get the users message by splitting it from the string
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                //print(String.Format("{0}: {1}", chatName, message));
                chatBox.text = chatBox.text + "\n" + String.Format("{0}: {1}", chatName, message);

                // YMV: Sending the chat to the ChatCommandsHandler for handling commands
                chatCmdsHndlr.ChatInput(chatName, message);

                // als de feestganger nog niet in het veld staat, spawn hem!!
                if (!viewerNames.Contains(chatName)) {
                    viewerNames.Add(chatName);

                    //followPeople.uname = chatName;
                    //partyPeopleSpawner.GetComponent<PartyPeopleSpawner>().SpawnPartyTwitch(chatName);
                }

                uname = chatName;

                //Run the instructions to control the game!
                GameInputs(message);
            }
        //}
    }

    // YMV: Write feedback to the chat from the stage bot (while in development this is done under the user's name instead of a dedicated bot Twitch account)
    public void WriteToChat (string msg) {
        //Debug.Log("Writing to chat: " + msg);
        writer.WriteLine(String.Format(":{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #{1} : {2}", username.ToLower(), channelName.ToLower(), msg));
        writer.WriteLine("PONG :tmi.twitch.tv");
        writer.Flush();
    }

    private void GameInputs(string ChatInputs) {

        // Enable and disable control for defined users

        // IDEA MKG
        // maybe make this a loop so you can insert all the names that CAN use the control, without changing the code.
        //if (ChatInputs.ToLower() == "enable" && username == "mickeygmusic"){ canTwitchControl = true; }	
        //if (ChatInputs.ToLower() == "disable" && username == "mickeygmusic"){ canTwitchControl = false; }	

        for (int i = 0; i < authNames.Count; i++) {
            if (ChatInputs.ToLower() == "enable" && username == authNames[i]) {
                canTwitchControl = true;
            } else if (ChatInputs.ToLower() == "disable" && username == authNames[i]) {
                canTwitchControl = false;
            }
        }

        if (canTwitchControl) {
            // Set the effect of the commands here. 
            /*if(ChatInputs.ToLower() == "0"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 0; }
	        if(ChatInputs.ToLower() == "1"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 1; }
	        if(ChatInputs.ToLower() == "2"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 2; }
	        if(ChatInputs.ToLower() == "3"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 3; }
	        if(ChatInputs.ToLower() == "4"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 4; }
	        if(ChatInputs.ToLower() == "5"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 5; }
	        if(ChatInputs.ToLower() == "6"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 6; }
	        if(ChatInputs.ToLower() == "7"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 7; }
	        if(ChatInputs.ToLower() == "8"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 8; }
	        if(ChatInputs.ToLower() == "9"){ MainCam.GetComponent<MoveCamera>().TwitchIndex = 9; }*/
        }

        if (ChatInputs.ToLower() == "jump") {
            // DIT MOET NAAR FOLLOWPEOPLE.CS GAAN ONDERAAN
            //Debug.Log("got here: JUMP");
            //followPeople.TestChatMovement(uname);

        }

        /*
        if(ChatInputs.ToLower() == "left")
        {
            player.AddForce(Vector3.left * (speed * 1000));
        }

        if (ChatInputs.ToLower() == "right")
        {
            player.AddForce(Vector3.right * (speed * 1000));
        }

        if (ChatInputs.ToLower() == "forward")
        {
            player.AddForce(Vector3.forward * (speed * 1000));
        }*/
    }

    void Update() {
        if (!twitchClient.Connected)
        {
            //print("Disconnected from Twitch. Reconnecting in " + reconnectTime + " seconds...");
            reconnectTryTime += Time.deltaTime;
            if (reconnectTryTime >= reconnectTime)
            {
                reconnectTryTime = 0;
                if (reconnectTime < 10)
                {
                    reconnectTime *= 2;
                }
                else
                {
                    reconnectTime = 10;
                }

                Connect();
            }
        }

        if (twitchClient.Available > 0)
        {
            newData = twitchClient.Available;
            totalData += newData;
            writer.WriteLine("PONG :tmi.twitch.tv");
            writer.Flush();
            //Debug.Log("Bytes received: " + twitchClient.Available);
            ReadChat();
        }

        // Chat test
        if (send) {
            send = false;
            WriteToChat(chatText);
            chatText = "";
        }

        count += Time.deltaTime;
        if (count >= timer) {
            count = 0;
            writer.WriteLine("PONG :tmi.twitch.tv");
            writer.Flush();
        }
    }
}