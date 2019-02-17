using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDestroy : MonoBehaviour
{
    [Tooltip("Probability of object existing")]
    [Range(0, 100)]
    public int probability;

    // Start is called before the first frame update
    void Start()
    {
        if(Random.Range(0,100) > probability){
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
