using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Scavenger.Server;

public class GameManager : MonoBehaviour {
    public GameObject egg;
    public GameObject user;
    public int numberOfEggs;
    public int eggHeight;
    public int eggsCollected = 0;
    public GameObject txtEggsCollected;
    public Vector3 userPosition;
    public GameObject txtDirectionValue;
    public GameObject txtPositionValue;
    public List<Material> materialList = new List<Material>();
    public AudioClip eggFind;
    public AudioSource backgroundMusic;
    public GameObject vrCamera;
    private bool deleteEggMode = false;
    private IList<GameObject> listOfEggs;
    private GameObject eggToDestroy;
    private readonly Queue<Position> _points = new Queue<Position>();
    private Vector3 updatedPosition;
    private float? newDegrees = null;
    private List<Animator> animationsPlaying = new List<Animator>();
    private AudioSource audioSource;
    private int minX = -25;
    private int maxX = 25;
    private int minZ = -25;
    private int maxZ = 25;

    // Use this for initialization
    void Start() {
        numberOfEggs = GameSettings.numberOfEggs;
        listOfEggs = new List<GameObject>();

        PlaceEggs();

        userPosition = UserCoordinates();
        user.transform.position = userPosition;
        updatedPosition = userPosition;

        audioSource = GetComponent<AudioSource>();
        backgroundMusic.Play();

        ServerControls.instance.StartTracking(_guideClient_OnScavengerMoved, _guideClient_OnScavengerChangedDirection);
    }

    // Update is called once per frame
    void Update() {
        CheckFoundEgg();
        UpdateLocationAndDirection();
        CheckEggClicked();
    }

    public void DeleteEggClicked()
    {
        if (!deleteEggMode)
        {
            deleteEggMode = true;
            GameObject.Find("DeleteEggText").GetComponent<Text>().text = "Delete Egg (On)";
        }
        else
        {
            deleteEggMode = false;
            GameObject.Find("DeleteEggText").GetComponent<Text>().text = "Delete Egg (Off)";
        }
    }

    public void ResetGame()
    {
        foreach (var item in listOfEggs)
        {
            Destroy(item);
        }
        listOfEggs.Clear();

        PlaceEggs();
    }

    private void CheckEggClicked()
    {
        if (Input.GetMouseButtonDown(0) && deleteEggMode == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2000))
            {
                if (hit.transform.gameObject.tag == "Egg")
                {
                    Debug.Log("Egg Clicked!");

                    listOfEggs.Remove(hit.transform.gameObject);
                    Destroy(hit.transform.gameObject);
                }                
            }
        }
    }
    
    private void CheckFoundEgg()
    {
        foreach (var egg in listOfEggs)
        {
            if (user.GetComponent<Collider>().bounds.Intersects(egg.GetComponentInChildren<BoxCollider>().bounds))
            {
                eggToDestroy = egg;
            }
        }

        if (eggToDestroy != null)
        {
            var animation = eggToDestroy.GetComponent<Animator>();
            animationsPlaying.Add(animation);
            animation.Play("Break");
            audioSource.clip = eggFind;
            audioSource.Play();
            ServerControls.instance.FoundEgg();

            listOfEggs.Remove(eggToDestroy);
            eggsCollected++;
            eggToDestroy = null;
        }

        for (int i = 0; i < animationsPlaying.Count; i++)
        {
            var animation = animationsPlaying[i];

            if (animation.GetBool("Break"))
            {
                Destroy(animation.transform.gameObject);
                animationsPlaying.RemoveAt(i);
                break;
            }
        }
    }

    private void UpdateLocationAndDirection()
    {
        var text = txtEggsCollected.GetComponent<UnityEngine.UI.Text>();
        text.text = eggsCollected.ToString();

        if (_points != null && _points.Count > 0)
        {
            var point = _points.Dequeue();

            if (point != null)
            {
                updatedPosition = new Vector3();
                updatedPosition.x = (float)point.X;
                updatedPosition.z = (float)point.Y;
                updatedPosition.y = userPosition.y;
            }
        }

        user.transform.position = Vector3.Lerp(user.transform.position, updatedPosition, 1f / 150);
        vrCamera.transform.position = new Vector3(user.transform.position.x, 2.5f, user.transform.position.z);

        var positionVal = txtPositionValue.GetComponent<UnityEngine.UI.Text>();
        positionVal.text = user.transform.position.x.ToString() + ", " + user.transform.position.z.ToString();

        if (newDegrees != null)
        {
            //user.transform.eulerAngles = new Vector3(0, newDegrees.GetValueOrDefault(), 0);
            user.transform.rotation = Quaternion.Slerp(user.transform.rotation,
                Quaternion.Euler(new Vector3(0, newDegrees.GetValueOrDefault(), 0)),
                2.5f * Time.deltaTime);

            var directionVal = txtDirectionValue.GetComponent<UnityEngine.UI.Text>();
            directionVal.text = user.transform.rotation.ToString();
        }
    }

    private void PlaceEggs()
    {
        //Test Eggs For Keith
        //var newEgg = Instantiate(egg);
        //newEgg.transform.position = new Vector3(75.65835f, 15f, -55.374437f);
        //listOfEggs.Add(newEgg);

        //newEgg = Instantiate(egg);
        //newEgg.transform.position = new Vector3(82.866586f, 15f, 10.0408964f);
        //listOfEggs.Add(newEgg);

        //newEgg = Instantiate(egg);
        //newEgg.transform.position = new Vector3(-100.866971f, 15f, 100.39304f);
        //listOfEggs.Add(newEgg);

        //newEgg = Instantiate(egg);
        //newEgg.transform.position = new Vector3(-99.402094f, 15f, 15.442051f);
        //listOfEggs.Add(newEgg);

        int numberForEachQuadrant = numberOfEggs / 4;
        int numberInCurrentQuadrant = 0;
        int currentQuadrant = 0;

        for (int i = 0; i < numberOfEggs; i++)
        {
            var newEgg = Instantiate(egg);
            newEgg.transform.position = GetRandomEggLocation(currentQuadrant);
            var material = materialList[Random.Range(0, materialList.Count)];
            newEgg.transform.Find("BottomEgg").GetComponent<Renderer>().material = material;
            newEgg.transform.Find("TopEgg").GetComponent<Renderer>().material = material;
            listOfEggs.Add(newEgg);

            numberInCurrentQuadrant++;

            if (numberInCurrentQuadrant == numberForEachQuadrant)
            {
                currentQuadrant++;
                numberInCurrentQuadrant = 0;
            }
        }

    }

    private void _guideClient_OnScavengerChangedDirection(double direction)
    {
        newDegrees = (float)direction * Mathf.Rad2Deg;
        newDegrees += 90;
    }

    private void _guideClient_OnScavengerMoved(Position position)
    {
        _points.Enqueue(position);
    }

    //interpret this from gps coordinates, step tracking, etc.  maybe the server should just send this in?
    public Vector3 UserCoordinates()
	{
		return new Vector3 (0, 1, 0);
	}

    #region Egg Location
    public Vector3 GetRandomEggLocation(int quadrant)
	{        
        var xValue = GetRandomXValue(quadrant);
        var zValue = GetRandomZValue(quadrant);

        int tries = 0;
        //minimum distance from start area
        while ((xValue <= maxX && xValue >= minX &&
            zValue <= maxZ && zValue >= minZ) || tries > 5)
        {
            xValue = GetRandomXValue(quadrant);
            zValue = GetRandomZValue(quadrant);

            //give up after a while, don't get stuck, not likely... but...
            tries++;
        }

        tries = 0;
        //minimum distance from each other
        while(IsTooCloseToEgg(xValue, zValue) || tries > 5)
        {
            xValue = GetRandomXValue(quadrant);
            zValue = GetRandomZValue(quadrant);

            tries++;
        }

        return new Vector3 (xValue, eggHeight, zValue);
	}

    private int GetRandomXValue(int quadrant)
    {
        if (quadrant == 0)
        {
            return Random.Range(-250, 0);
        }
        else if (quadrant == 1)
        {
            return Random.Range(0, 250);
        }
        else if (quadrant == 2)
        {
            return Random.Range(-250, 0);
        }
        else
        {
            return Random.Range(0, 250);
        }
    }

    private int GetRandomZValue(int quadrant)
    {
        if (quadrant == 0 || quadrant == 1)
        {
            return Random.Range(0, 250);
        }
        else
        {
            return Random.Range(-250, 0);
        }
    }

    private bool IsTooCloseToEgg(int xValue, int zValue)
    {
        foreach(var item in listOfEggs)
        {
            var eggDeltaX = item.transform.localPosition.x - xValue;
            var eggDeltaY = item.transform.localPosition.z - zValue;

            if (eggDeltaX <= maxX && eggDeltaX >= minX &&
            eggDeltaY <= maxZ && eggDeltaY >= minZ)
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}
