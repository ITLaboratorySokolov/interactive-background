using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangeSetup : MonoBehaviour
{
    public RawImage toRed;

    // Start is called before the first frame update
    void Start()
    {
        Texture2D rx = new Texture2D(640,480); //, TextureFormat.RGBA32, false);

        int count = 0;
        for (int w = 0; w < rx.width; w++)
        {
            for (int h = 0; h < rx.height; h++)
            {
                Color res = new Color(0, 0, 0, 0);
                if (((double)w)/rx.width > 0.8 && ((double)h) / rx.height < 0.2)
                {
                    res = new Color(1, 0, 0, 1);
                    count++;
                }

                rx.SetPixel(w, h, res);
            }
        }
        rx.Apply();
        toRed.texture = rx;

        Debug.Log(count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
