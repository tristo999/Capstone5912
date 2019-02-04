using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : MonoBehaviour
{
    public NavMeshAgent nav;
    public GameObject currentPlayer;
    private Vector3 intPosition;
    private GameObject[] players;
    public enum enemyState
    {
        idle, chasing, attacking, returning
    };
    public enemyState currentState;
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

        if (currentPlayer != null)
        {
            //Check range of player and decide to attack or chase
            nav.SetDestination(currentPlayer.transform.position);
        }
        else
        {
            // If not at inital position, begin returning to position. If at current position then idle
        }
    }
}
