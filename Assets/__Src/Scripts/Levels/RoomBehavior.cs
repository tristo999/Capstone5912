using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomBehavior : MonoBehaviour
{
    public enum RoomState { active, warning, crumbling, inactive };
    public RoomState currentState;
    private GameObject[] players;
    public GameObject nearbyPlayer;
    public GameObject smallRocksPrefab, bigRocksPrefab;
    public TextMeshProUGUI deathText;
    private ParticleSystem smallRocks, bigRocks;
    private float stateTime, timeThreshhold;
    // Start is called before the first frame update
    void Start()
    {
        currentState = RoomState.active;
        players = GameObject.FindGameObjectsWithTag("Player");

        smallRocks = Instantiate(smallRocksPrefab).GetComponent<ParticleSystem>();
        bigRocks = Instantiate(bigRocksPrefab).GetComponent<ParticleSystem>();
        deathText.text = "";

        smallRocks.transform.position = new Vector3(transform.position.x, transform.position.y + 9, transform.position.z);
        bigRocks.transform.position = new Vector3(transform.position.x / 2, transform.position.y + 9, transform.position.z / 2);

        stateTime = 0f;
        timeThreshhold = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        stateTime += Time.deltaTime;
        if(players[0] != null && stateTime > timeThreshhold)
        {
            switch(currentState)
            {
                case RoomState.active:
                    currentState = RoomState.warning;
                    break;
                case RoomState.warning:
                    //particles of small rocks 
                    smallRocks.Play();
                    currentState = RoomState.crumbling;
                    break;
                case RoomState.crumbling:
                    //particles of large rocks for threshhold
                    bigRocks.Play();
                    currentState = RoomState.inactive;
                    break;
                case RoomState.inactive:
                    deathText.text = "Death has Taken You";
                    break;
            }
            stateTime = 0;
        }
    }
}
