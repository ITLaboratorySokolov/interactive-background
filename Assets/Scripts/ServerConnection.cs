using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Unity.Connections;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;
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

    [SerializeField]
    VirtualWorldServerConnectionWrapper connection;
    [SerializeField]
    SignalRSessionWrapper session;

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
        //session.StartSession();
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

        Texture2D scaled = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] data = scaled.GetRawTextureData();

        byte[] bytes = scaled.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);

        Dictionary<string, string> properties = new Dictionary<string, string>();
        properties.Add(BitmapWorldObjectSerializer.WidthKey, $"{scaled.width}");
        properties.Add(BitmapWorldObjectSerializer.HeightKey, $"{scaled.height}");
        properties.Add(BitmapWorldObjectSerializer.FormatKey, "RGBA");
        properties.Add(BitmapWorldObjectSerializer.PixelsKey, $"{data}");

        WorldObjectDto wod = new WorldObjectDto();
        wod.Name = "FlyKiller";
        wod.Position = new RemoteVectorDto();
        wod.Rotation = new RemoteVectorDto();
        wod.Scale = new RemoteVectorDto();
        wod.Scale.X = 1; wod.Scale.Y = 1; wod.Scale.Z = 1;
        wod.Type = "Bitmap";
        wod.Properties = properties;

        // TODO - musim update když už tam je, jak poznam že se fakt přidal a tak
        try
        {
            connection.AddWorldObjectAsync(wod);
        } catch (Exception e)
        {
            Debug.LogError("Unable to send to server");
            Debug.LogError(e.Message);
        }

        // Object.Destroy(t);
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
        //session.StopSession();
        actionEnd.Invoke();
    }
}
