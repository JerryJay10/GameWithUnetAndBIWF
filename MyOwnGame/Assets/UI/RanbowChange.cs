using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RanbowChange : MonoBehaviour
{
    public Button StartButton;
    private float red = 255;
    private float green = 0;
    private float blue = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (blue == 0 & green != 255)
            green = green + 17;
        if (green == 255 & red != 0)
            red = red - 17;
        if (red == 0 & blue != 255)
            blue = blue + 17;
        if(blue == 255 & green != 0)
            green = green - 17;
        if (green == 0 & red != 255)
            red = red + 17;
        if (red == 255 & blue != 0)
            blue = blue - 17;

        StartButton.GetComponent<Image>().color = new Color(red/255f,green/255f,blue/255f);

    }
}
