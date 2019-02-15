using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    public enum RoomState { active, warning, crumbling, inactive };
    public RoomState currentState;
    private GameObject[] players;
    private GameObject nearbyPlayer;
    private ParticleSystem rocks;
    private float stateTime, timeThreshhold;
    // Start is called before the first frame update
    void Start()
    {
        currentState = RoomState.active;
        players = GameObject.FindGameObjectsWithTag("Player");
        stateTime = 0f;
        timeThreshhold = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        stateTime += Time.deltaTime;
        if(nearbyPlayer != null && stateTime > timeThreshhold)
        {
            if(currentState == RoomState.active)
            {
                currentState = RoomState.warning;
            }
            else if(currentState == RoomState.warning)
            {
                //particles of small rocks 
                currentState = RoomState.crumbling;
            }
            else if (currentState == RoomState.crumbling)
            {
                //particles of large rocks for threshhold
                currentState = RoomState.inactive;
            }
            stateTime = 0;
        }
    }
}
