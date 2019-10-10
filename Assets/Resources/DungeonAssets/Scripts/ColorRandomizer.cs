using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : NetworkBehaviour
{
    public Color[] colors;
    public bool debug = true;
    
    /*
    public void Awake()
    {
        state.AddCallback("Color", ChangeColor);
        if (entity.isOwner) {
            state.Color = colors[Random.Range(0, colors.Length)];
        }
    }

    /*
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
        rend.material.color = state.Color;
    } Todo: EUGH REWRITE THIS YOU ABSOLUTE MAD LAD */
}
