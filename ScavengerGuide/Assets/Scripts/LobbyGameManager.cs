using UnityEngine;
using System.Collections;
using Scavenger.Server;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyGameManager : MonoBehaviour {
    public bool switchToMainScene;
    public GameObject serverControls;
    public GameObject statusPanel;
    public GameObject vrStatusPanel;
    public GameObject vrcontroller;
    private bool triggerClicked;
    private GameObject selectedButton;

    void Awake()
    {
        if (ServerControls.instance == null)
        {
            Instantiate(serverControls);
        }
    }

    void Start()
    {
        if (!GameSettings.UseVR)
        {
            statusPanel.SetActive(false);
        }
        else
        {
            vrStatusPanel.SetActive(false);
            RegisterVREvents();
        }
    }
    
    void Update()
    {
        if (switchToMainScene)
        {
            if (GameObject.Find("NumberOfEggs") != null)
            {
                var value = GameObject.Find("NumberOfEggs").GetComponent<Dropdown>().value;
                switch(value)
                {
                    case 0:
                        GameSettings.numberOfEggs = 5;
                        break;
                    case 1:
                        GameSettings.numberOfEggs = 10;
                        break;
                    case 2:
                        GameSettings.numberOfEggs = 15;
                        break;
                    case 3:
                        GameSettings.numberOfEggs = 20;
                        break;
                    case 4:
                        GameSettings.numberOfEggs = 50;
                        break;
                    default:
                        GameSettings.numberOfEggs = 15;
                        break;

                }
            }

            SceneManager.LoadScene("MainScene");
            switchToMainScene = false;
        }
    }
    public void RegisterVREvents()
    {
        var laser = vrcontroller.GetComponent("SteamVR_LaserPointer");
        ((SteamVR_LaserPointer)laser).PointerIn += LobbyGameManager_PointerIn;
        ((SteamVR_LaserPointer)laser).PointerOut += LobbyGameManager_PointerOut;

        var trackedController = vrcontroller.GetComponent<SteamVR_TrackedController>();

        trackedController.TriggerClicked += LobbyGameManager_TriggerClicked;
        trackedController.TriggerUnclicked += LobbyGameManager_TriggerUnclicked;
    }

    public void UnregisterVREvents()
    { 
        var laser = vrcontroller.GetComponent("SteamVR_LaserPointer");
        ((SteamVR_LaserPointer)laser).PointerIn -= LobbyGameManager_PointerIn;
        ((SteamVR_LaserPointer)laser).PointerOut -= LobbyGameManager_PointerOut;

        var trackedController = vrcontroller.GetComponent<SteamVR_TrackedController>();

        trackedController.TriggerClicked -= LobbyGameManager_TriggerClicked;
        trackedController.TriggerUnclicked -= LobbyGameManager_TriggerUnclicked;
    }

    private void LobbyGameManager_PointerOut(object sender, PointerEventArgs e)
    {
        selectedButton = null;
    }

    private void LobbyGameManager_PointerIn(object sender, PointerEventArgs e)
    {
        Debug.Log(e.target.ToString());
        selectedButton = e.target.gameObject;
    }

    private void LobbyGameManager_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        triggerClicked = false;
    }

    private void LobbyGameManager_TriggerClicked(object sender, ClickedEventArgs e)
    {
        if (triggerClicked == false)
        {
            if (selectedButton.name == "VR_FindScavengerButton")
            {
                StartGuide();
            }
            else if (selectedButton.name == "VR_BackButton")
            {
                var titleScreen = GameObject.Find("LobbyGameManager").GetComponent<TitleScreenManager>();
                titleScreen.OnBackClick();
            }
        }

        triggerClicked = true;
    }

    public void StartGuide()
    {
        if (!GameSettings.UseVR)
        {
            statusPanel.SetActive(true);
        }
        else
        {
            vrStatusPanel.SetActive(true);
        }

        ServerControls.instance.StartGuide((_guideClient_OnLobbyReady));
    }
    
    private void _guideClient_OnLobbyReady(System.Guid obj)
    {
        ServerControls.instance.ScavengerId = obj;
        //go to scavenger screen
        switchToMainScene = true;
    }
}
