using Microsoft.AspNetCore.SignalR.Client;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Unity.VirtualWorld;
using ZCU.TechnologyLab.Common.Unity.VirtualWorld.WorldObjects;

public class ServerConnection : MonoBehaviour
{
    /// <summary>
    /// Server virtual world.
    /// </summary>
    [SerializeField]
    public VirtualServerWorld serverSpace;

    internal string url = "https://localhost:49155/virtualWorldHub";

    System.DateTime now;
    System.DateTime last;

    bool connected = false;
    double timeToRetry;

    double timeToSnapshot;
    
    public BitmapWorldObject wo;
    public GameObject carrier;

    /// <summary> Hub connection </summary>
    // private HubConnection hubConnection;
    // private VirtualWorldServerConnection vwsc;

    [SerializeField]
    private UnityEvent actionStart = new UnityEvent();
    [SerializeField]
    private UnityEvent actionEnd = new UnityEvent();


    /// <summary>
    /// Invokes action on start.
    /// </summary>
    private void Start()
    {
        actionStart.Invoke();
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        return source;

        /*
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        Color[] pixels = result.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color newColor = source.GetPixelBilinear((float)(i % result.width) / (float)result.width, (float)(i/result.width) / (float)result.height);
            pixels[i] = newColor;
        }
        result.SetPixels(pixels);
        result.Apply();
        return result;
        */
    }

    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();

        // carrier = new GameObject();
        // carrier.AddComponent<BitmapWorldObject>();
        // wo = carrier.GetComponent<BitmapWorldObject>();

        Texture2D scaled = ScreenCapture.CaptureScreenshotAsTexture();
        scaled = ScaleTexture(scaled, 80, 40); // 30, 15);

        Texture2D t = new Texture2D(80, 40);//30, 15);
        Debug.Log("length of new " + t.GetRawTextureData().Length);
        t.SetPixels(scaled.GetPixels());

        var b = t.EncodeToPNG();
        File.WriteAllBytes("D:/moje/school/05/PRJ/Projects/ScreenshotTest.png", b);
        Debug.Log("Saved to image");

        // WorldObjectDto worldImage = new WorldObjectDto();
        // worldImage.Position = new RemoteVectorDto();
        // worldImage.Rotation = new RemoteVectorDto();
        // worldImage.Type = "Bitmap";
        // worldImage.Properties = new System.Collections.Generic.Dictionary<string, string>();

        wo.name = "RealsenseProjection";

        int w = wo.bitmapSerializer.DeserializeWidth($"{t.width}");
        Debug.Log(w);

        // data
        byte[] data = t.GetRawTextureData();
        // Dictionary<string, string> properties;
        // properties = serializer.SerializeProperties(t.width, t.height, "RGBA", data);
        wo.SetProperty(BitmapWorldObjectSerializer.WidthKey, $"{t.width}");
        wo.SetProperty(BitmapWorldObjectSerializer.HeightKey, $"{t.height}");
        wo.SetProperty(BitmapWorldObjectSerializer.FormatKey, "RGBA");
        wo.SetProperty(BitmapWorldObjectSerializer.PixelsKey, $"{data}");



        /*
        string pixelStr = "";
        Debug.Log("length of raw data " + data.Length);
        pixelStr += System.Text.Encoding.Unicode.GetString(data);

        File.WriteAllBytes("D:/moje/school/05/PRJ/Projects/outgoing.txt", data);
        File.WriteAllText("D:/moje/school/05/PRJ/Projects/outgoing_str.txt", pixelStr);

        Dictionary<string, string> properties = new Dictionary<string, string>();
        properties.Add("Height", $"{t.height}");
        properties.Add("Width", $"{t.width}");
        properties.Add("Format", "RGBA");
        properties.Add("Pixels", pixelStr);
        Debug.Log("Converted to string");
        */

        // int w = serializer.DeserializeWidth(properties);
        // wo.SetProperties(properties);

        // Debug.Log(properties["Pixels"].Length);
        // Debug.Log(" \" " + properties["Pixels"] + "\"");

        // TODO - musim update když už tam je, jak poznam že se fakt přidal a tak
        try
        {
            SendToServer(carrier);
        } catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }

        Object.Destroy(t);
    }

    private async void SendToServer(GameObject worldImage)
    {
        await serverSpace.AddObjectAsync(worldImage);

        /*
        Debug.Log(hubConnection.State);


        try
        {
            // await hubConnection.SendAsync("RemoveWorldObject", "Name");
            await hubConnection.SendAsync("AddWorldObject", worldImage);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not send  to the server: " + e.Message);
        }

        Debug.Log("Sent to server");
        */
    }


    public void LateUpdate()
    {
        /*
        if (timeToSnapshot < 0.01)
        {
            // 4 times per second
            timeToSnapshot = 0.25;
            if (connected)
            {
                Debug.Log(connected);
                StartCoroutine(RecordFrame());
            }
        }
        */

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(RecordFrame());

        }
    }

    // Update is called once per frame
    void Update()
    {

        /*
        // if not connected try to connect every 2s   
        if (!connected)
        {
            timeToRetry -= Time.deltaTime;

            if (timeToRetry < 0.01)
            {
                TryToConnect();
            }
        }
        */

        timeToSnapshot -= Time.deltaTime;
    }


    public void OnDestroy()
    {
        actionEnd.Invoke();
    }
}
