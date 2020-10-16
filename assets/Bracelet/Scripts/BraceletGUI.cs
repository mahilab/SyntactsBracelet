using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BraceletGUI : MonoBehaviour
{

    [Tooltip("Color to set lerp button too on vibration.")]
    public Color vibrationColor;

    [Header("References")]
    public Bracelet bracelet;
    public UnityEngine.UI.Button[] tactorButtons;
    public UnityEngine.UI.Image[] tactorButtonsBg;

    float[] lastLevel = new float[8];

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 8; ++i)
        {
            float thisLevel = bracelet.tactors[i].Level();
            tactorButtonsBg[i].color = Color.Lerp(Color.white, vibrationColor, Mathf.Max(lastLevel[i],thisLevel));
            lastLevel[i] = thisLevel;
        }
    }

}
