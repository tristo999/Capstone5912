using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : MonoBehaviour
{
    public NavMeshAgent nav;
    public GameObject currentPlayer;
    public Vector3 intPosition;
    private GameObject[] players;
    public enum enemyState
    {
        idle, chasing, attacking, returning
    };
    public enemyState currentState;
    public float roomWidth = 30;
    // Start is called before the first frame update
    void Start()
    {
        nav = this.GetComponent<NavMeshAgent>();
        players = GameObject.FindGameObjectsWithTag("Player");
        intPosition = transform.position;
        currentState = enemyState.idle;
    }

    // Update is called once per frame
    void Update()
    {

        currentPlayer = null;
        foreach (GameObject x in players)
        {
            if (x.transform.position.x < intPosition.x + (roomWidth / 2) && x.transform.position.x > intPosition.x - (roomWidth / 2) && x.transform.position.z < (intPosition.z + roomWidth) / 2 && x.transform.position.z > intPosition.z - (roomWidth / 2))
            {
                if (currentPlayer == null)
                {
                    currentPlayer = x;
                }
                else
                {

                }
            }
        }
        if (currentPlayer != null)
        {
            //Check range of player and decide to attack or chase

            nav.SetDestination(currentPlayer.transform.position);
        }
        else
        {
            nav.SetDestination(intPosition);
        }
    }
}
