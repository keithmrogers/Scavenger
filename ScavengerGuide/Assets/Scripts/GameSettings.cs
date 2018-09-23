using UnityEngine;
using System.Collections;

public class GameSettings : MonoBehaviour {
    public static GameSettings instance = null;
    
    public static bool UseVR = false;
    public static int numberOfEggs = 15;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
