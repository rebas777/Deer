﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class MainPanelManager : MonoBehaviour {

    public Transform mainPanel;
    public Transform escapePanel;
    public Transform mainPanelTarget;
    public Transform mainPanelHome;
    public Transform escPanelTarget;
    public Transform escPanelHome;
    public GameObject canvasLogo;
    public GameObject canvasCamera;
    public GameObject cheatSheet;

    private bool cheatSheetOpen = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void ShowMainPanel() {
        canvasLogo.SetActive(true);
        canvasCamera.SetActive(true);
        mainPanel.gameObject.SetActive(true);
        mainPanel.DOMove(mainPanelTarget.position, 0.3f);
    }

    public void HideMainPanel(bool needDisableCam) {
        //mainPanel.DOMove(mainPanelHome.position, 0.3f);
        //Debug.Log("MainPanel Move to Home Position");
        mainPanel.gameObject.SetActive(false);
        canvasLogo.SetActive(false);
        if(needDisableCam == true) {
            canvasCamera.SetActive(false);
        }
        else {
            canvasCamera.SetActive(true);
        }
    }

    //public void ShowStartPanel() {
    //    HideMainPanel();
    //    startPanel.DOMove(startPanelTarget.position, 0.3f);
    //}

    //public void HideStartPanel() {
    //    ShowMainPanel();
    //    startPanel.DOMove(startPanelHome.position, 0.3f);
    //}

    public void ShowEscPanel() {
        escapePanel.DOMove(escPanelTarget.position, 0.3f);
    }

    public void HideEscPanel() {
        escapePanel.DOMove(escPanelHome.position, 0.3f);
        if(cheatSheetOpen == true) {
            cheatSheetOpen = false;
            cheatSheet.SetActive(false);
        }
    }

    public void Reset() {
        Debug.Log("Reset MainPanelManager");
        HideMainPanel(false);
    }

    public void ShowHideCheatSheet() {
        if(cheatSheetOpen == false) {
            cheatSheetOpen = true;
            cheatSheet.SetActive(true);
        }
        else {
            cheatSheetOpen = false;
            cheatSheet.SetActive(false);
        }
    }

    public bool GetCheatSheet()
    {
        return cheatSheetOpen;
    }
}
