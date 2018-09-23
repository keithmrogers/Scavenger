using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    private GameObject mainCameraTitleScreen;
    private GameObject mainCameraServerSelectScreen;
    public GameObject titleScreenUI;
    public GameObject serverScreenUI;
    public GameObject vrTitleScreenUI;
    public GameObject vrServerScreenUI;
    public GameObject vrcontroller;
    public AudioSource backgroundMusic;
    public float rotateSpeed = 5.0f;
    private ScreenSelect currentScreen;
    private bool triggerClicked = false;

    void Start()
    {
        serverScreenUI.SetActive(false);

        currentScreen = ScreenSelect.TitleScreen;

        mainCameraTitleScreen = new GameObject();
        mainCameraTitleScreen.transform.rotation = Quaternion.Euler(36.6029f, 0, 0);
        mainCameraTitleScreen.transform.localScale = new Vector3(1, 1, 1);
        mainCameraTitleScreen.transform.position = new Vector3(306, 5.3f, 102.41f);

        mainCameraServerSelectScreen = new GameObject();
        mainCameraServerSelectScreen.transform.rotation = Quaternion.Euler(36.6029f, 180, 0);
        mainCameraServerSelectScreen.transform.localScale = new Vector3(1, 1, 1);
        mainCameraServerSelectScreen.transform.position = new Vector3(306, 5.3f, 102.41f);

        backgroundMusic.Play();

        if (GameSettings.UseVR && vrcontroller != null)
        {
            RegisterVREvents();
            vrServerScreenUI.SetActive(false);
        }
    }

    private void RegisterVREvents()
    {
        var laser = vrcontroller.GetComponent("SteamVR_LaserPointer");
        ((SteamVR_LaserPointer)laser).PointerIn += TitleScreenManager_PointerIn;
        ((SteamVR_LaserPointer)laser).PointerOut += TitleScreenManager_PointerOut;

        var trackedController = vrcontroller.GetComponent<SteamVR_TrackedController>();

        trackedController.TriggerClicked += TrackedController_TriggerClicked;
        trackedController.TriggerUnclicked += TrackedController_TriggerUnclicked;   
    }

    private void UnregisterVREvents()
    {
        var laser = vrcontroller.GetComponent("SteamVR_LaserPointer");
        ((SteamVR_LaserPointer)laser).PointerIn -= TitleScreenManager_PointerIn;
        ((SteamVR_LaserPointer)laser).PointerOut -= TitleScreenManager_PointerOut;

        var trackedController = vrcontroller.GetComponent<SteamVR_TrackedController>();

        trackedController.TriggerClicked -= TrackedController_TriggerClicked;
        trackedController.TriggerUnclicked -= TrackedController_TriggerUnclicked;
    }

    private void TitleScreenManager_PointerOut(object sender, PointerEventArgs e)
    {
        Debug.Log(e.target.name);
        if (e.target.name == "vrStartButton")
        {
            var eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            eventSystem.SetSelectedGameObject(null);
        }
        else if (e.target.name == "vrExitButton")
        {
            var eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            eventSystem.SetSelectedGameObject(null);
        }
    }

    private void TitleScreenManager_PointerIn(object sender, PointerEventArgs e)
    {
        Debug.Log(e.target.ToString());
        if (e.target.name == "vrStartButton")
        {
            var eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            eventSystem.SetSelectedGameObject(e.target.gameObject);                
        }
        else if (e.target.name == "vrExitButton")
        {
            var eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            eventSystem.SetSelectedGameObject(e.target.gameObject);
        }
    }

    void Update()
    {
        if (currentScreen == ScreenSelect.TitleScreen)
        {
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.RotateTowards(
                Camera.main.transform.rotation,
                mainCameraTitleScreen.transform.rotation,
                180), rotateSpeed * Time.deltaTime);
        } else if (currentScreen == ScreenSelect.ServerSelect)
        {
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.RotateTowards(
                Camera.main.transform.rotation,
                mainCameraServerSelectScreen.transform.rotation,
                180), rotateSpeed * Time.deltaTime);
        }
    }

    private void TrackedController_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        triggerClicked = false;
    }

    private void TrackedController_TriggerClicked(object sender, ClickedEventArgs e)
    {
        if (triggerClicked == false)
        {
            var eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            if (eventSystem.currentSelectedGameObject.name == "vrStartButton")
            {
                OnNewGameClick();
            }
            else if (eventSystem.currentSelectedGameObject.name == "vrExitButton")
            {
                OnExitClick();
            }
        }

        triggerClicked = true;
    }

    private void SwitchToTitleScreen()
    {
        currentScreen = ScreenSelect.TitleScreen;
        if (!GameSettings.UseVR)
        {            
            titleScreenUI.SetActive(true);
            serverScreenUI.SetActive(false);
        } 
        else
        {
            var lobbyManager = GameObject.Find("LobbyGameManager").GetComponent<LobbyGameManager>();

            lobbyManager.UnregisterVREvents();
            vrTitleScreenUI.SetActive(true);
            vrServerScreenUI.SetActive(false);
            GameObject.Find("scavengerlogo3").transform.localScale = new Vector3(1, 1, 1);
            RegisterVREvents();
        }
    }

    private void SwitchToServerScreen()
    {
        currentScreen = ScreenSelect.ServerSelect;
        if (!GameSettings.UseVR)
        {            
            titleScreenUI.SetActive(false);
            serverScreenUI.SetActive(true);
        }
        else
        {
            var lobbyManager = GameObject.Find("LobbyGameManager").GetComponent<LobbyGameManager>();

            UnregisterVREvents();
            vrTitleScreenUI.SetActive(false);
            vrServerScreenUI.SetActive(true);
            GameObject.Find("scavengerlogo3").transform.localScale = new Vector3(0, 0, 0);
            lobbyManager.RegisterVREvents();
        }
    }

    public void OnNewGameClick()
    {
        Debug.Log("New Game Clicked");
        
        SwitchToServerScreen();
        
    }
    public void OnExitClick()
    {
        Debug.Log("Exit Clicked");

        Application.Quit();
    }

    public void OnBackClick()
    {
        Debug.Log("Back Clicked");

        SwitchToTitleScreen();

    }

}

public enum ScreenSelect
{
    TitleScreen,
    ServerSelect
}