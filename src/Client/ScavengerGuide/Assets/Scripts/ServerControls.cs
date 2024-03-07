using UnityEngine;
using System.Collections;
using Scavenger.Server;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class ServerControls : MonoBehaviour {
    //public GameObject lobbyGameManager;
    public static ServerControls instance = null;
    
    IGuideService _guideService;
    TestGuideClient _guideClient;

    public Guid? ScavengerId { get; set; }

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

    void Start()
    {
        _guideClient = new TestGuideClient();

        _guideService = new GuideService();
    }

    public void StartGuide(Action<Guid> onLobbyReady)
    {
        _guideClient.OnLobbyReady += onLobbyReady;

        _guideService.Start(_guideClient);
    }

    public void FindScavenger()
    {
        Debug.Log("Finding Scavenger");
    }

    public void StartTracking(Action<Position> scavengerMoved, Action<double> changedDirection)
    {
        _guideClient.OnScavengerMoved += scavengerMoved;
        _guideClient.OnScavengerChangedDirection += changedDirection;
    }

    public void FoundEgg()
    {
        _guideService.FoundEgg(ScavengerId.GetValueOrDefault());
    }
}
