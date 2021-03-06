﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObj : MonoBehaviour {

    public List<string> textContents;
    public Material highlightMat;
    public Material normalMat;
    public bool need3dUI = false;
    public bool needTextUI = true;
    public bool needOS = false;
    public GameObject threeDUI;
    public GameObject highlightPart;
    // The object who will listen the instruction and do something.(usually with Key or Door scripts on it)
    public GameObject listenerObj; 



    // flag to mark whether this object is within the interactive range.
    // 0 ------ Too far to interact
    // 1 ------ Near enough, interact to show 3dUI
    // 2 ------ interact to get feed back
    public int status = 0;

    // Whether or not the object has been illuminated by the light ball around player
    public bool hasDetected = false;

    //Display Floating Effects
    [Header("Floating Settings")]
    public bool isFloating = false;
    public float perRadian = 0.03f;
    public float radius = 0.8f;
    float radian = 0;

    // When the message first show up, it is proper to add the message to Top-left hint area,
    // while is it is not the first time, it is improper.
    private bool messageShowed = false;



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isFloating)
        {
            radian += perRadian; 
            float dy = Mathf.Cos(radian) * radius; 
            transform.position = transform.position + new Vector3(0, dy, 0);
        }
            
	}

    // Check and change the obj's material(NEED to deduplicate)
    // isHighlight == true --- use highlight mat; isHighlight == false --- use normal mat.
    public void SetHighlight(bool isHighlight) {
        if(isHighlight == true) {
            if(highlightMat && highlightPart) {
                highlightPart.GetComponent<MeshRenderer>().material = highlightMat;
            }
        }
        else {
            if(normalMat && highlightPart) {
                highlightPart.GetComponent<MeshRenderer>().material = normalMat;
            }
        }
    }

    // InteractiveObjManager will call this function to get text content.
    // idx: one object might have multiple text to store.
    public string GetTextContent(int idx) {
        return textContents[idx];
    }

    // Set current gameObject to be interactive, 
    public void SetInteractive(int statusNum) {
        status = statusNum;
        if(statusNum == 0) {
            if(threeDUI && need3dUI == true) {
                threeDUI.GetComponent<ThreeDUIView>().Deactivate();
            }
        }
    }

    // Obsolete
    public void OnInteract() {
        if(threeDUI && need3dUI == true) {
            threeDUI.GetComponent<ThreeDUIView>().Activate();
        }
    }

    // Obsolete
    public void AfterInteract() {
        if(threeDUI && need3dUI == true) {
            threeDUI.GetComponent<ThreeDUIView>().Deactivate();
        }
    }

    public string Trigger() {
        string messageOnTrigger = "";
        if(listenerObj.GetComponent<Door>() != null) {
            messageOnTrigger = listenerObj.GetComponent<Door>().TryOpen();
        }
        else if(listenerObj.GetComponent<Key>() != null) {
            messageOnTrigger = listenerObj.GetComponent<Key>().TriggerShow();
        }
        return messageOnTrigger;
    }

    public string GetOS() {
        if(textContents.Count >= 2) {
            return textContents[1];
        }
        else {
            return "";
        }
    }

    // which will be displayed on top-left of screen
    public List<string> GetHint() {
        List<string> answer = new List<string>();
        if(textContents.Count <= 1 || messageShowed == true) {
            answer.Add("");
            return answer;
        }

        messageShowed = true;
        // Handle special case of "plate on the table"(offer two hints)
        if(needOS == true && textContents.Count == 4) {
            answer.Add(textContents[2]);
            answer.Add(textContents[3]);
            return answer;
        }
        if(needOS == true && textContents.Count > 2) {
            answer.Add(textContents[2]);
            return answer;
        }
        else {
            answer.Add(textContents[1]);
            return answer;
        }
    }
}
