using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOverTarget : MonoBehaviour
{

    public Transform target;
    private string textToDisplay;
 
    public bool displayName = true;
    public bool displayTAG = false;
	public string uname;
	
	GameObject PartyPeople;
 
    // Use this for initialization
    void Start () {
		PartyPeople = GameObject.Find("PartyPeople");
    }
 
    // Update is called once per frame
    void Update () {
        nameDisplayer();
        tagDisplayer();
    }
 
    void LateUpdate (){
        //Make the text allways face the camera
        transform.rotation = Camera.main.transform.rotation;
    }
 
    //displays the name of the parent
    void nameDisplayer(){
        if(displayName){
            displayTAG = false;
            //textToDisplay = PartyPeople.GetComponent<FollowPeople>().uname;
            //changes the text to the Name
            changeTextColor();
        }
    }
 
    //displays the TAG of the parent
    void tagDisplayer(){
        if(displayTAG){
            displayName = false;
            //changes the text to the TAG
            textToDisplay = (string)this.transform.parent.tag;
            changeTextColor();
        }
    }
 
    /* # Exibe o texto armazenado na variavel publica "textToDisplay",
       # possibilitando que outros scripts alterem a variavel e, consequentemente, o texto em si */
   
    //Changes the color
    public void changeTextColor() {
 /*
        //Enemy = red
        if(this.transform.parent.tag == "Enemy"){
            renderer.material.color = Color.red;
        }
       
        //Player = Green
        if(this.transform.parent.tag == "Player"){
            renderer.material.color = Color.green;
        }
 
        //Neutral = yellow
        if(this.transform.parent.tag == "Neutral"){
            renderer.material.color = Color.yellow;
        }
 */
        // Access the TextMesh component and change it for "textToDisplay" value
        // Modo de acessar o component TextMesh do Texto3d e mudá-lo para "textToDisplay"  
        TextMesh tm = GetComponent<TextMesh>();
        tm.text = textToDisplay;
    }
}
