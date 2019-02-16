using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    public Color[] colors;
    public bool debug = true;
    // Start is called before the first frame update
    void Start()
    {
        ChangeColor();
    }

    // Update is called once per frame
    void Update()
    {
        if(debug){
            if (Input.GetMouseButtonDown(0)) ChangeColor();
        }
    }

    void ChangeColor(){

        Renderer rend = GetComponent<Renderer>();

        //Set the main Color of the Material to green
        //rend.material.shader = Shader.Find("HDRP/Lit");
        rend.material.SetColor("_BaseColor", colors[Random.Range(0, colors.Length)]); 
    }
}
