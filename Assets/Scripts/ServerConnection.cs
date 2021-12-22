using Microsoft.AspNetCore.SignalR.Client;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZCU.TechnologyLab.Common.Models;

public class ServerConnection : MonoBehaviour
{
    internal string url = "https://localhost:49159/virtualWorldHub";

    System.DateTime now;
    System.DateTime last;

    bool connected = false;
    double timeToRetry;

    double timeToSnapshot;


    /// <summary> Hub connection </summary>
    private HubConnection hubConnection;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(url);
        // try to connect to server
        this.hubConnection = new HubConnectionBuilder().WithUrl(url).Build();
        TryToConnect();
        List<WorldObject> l = new List<WorldObject>();
        this.hubConnection.On<List<WorldObject>>("GetAllWorldObjects", (list) => { Debug.Log("recieved " + list.Count + " world objects"); });
        hubConnection.On<WorldObject>("UpdateWorldObject", (obj) => { Debug.Log(obj.Name); });
    }

    // TODO how do i kill you on app end
    private async void TryToConnect()
    {
        if (this.connected || hubConnection.State != HubConnectionState.Disconnected) return;

        connected = false;
        try
        {
            await this.hubConnection.StartAsync();
            connected = true;
        }
        catch (System.Exception e)
        {
            connected = false;
            timeToRetry = 2;
            Debug.LogError("Cannot connect to a server: " + e.Message);
        }

    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
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

    }

    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        var t = ScreenCapture.CaptureScreenshotAsTexture();
        t = ScaleTexture(t, 30, 15);
        var b = t.EncodeToPNG();
        Debug.Log(b.Length);

        File.WriteAllBytes("D:/moje/school/05/PRJ/Projects/ScreenshotTest.png", b);
        Debug.Log("Saved to image");

        WorldObject worldImage = new WorldObject();
        worldImage.Name = "RealsenseProjection";
        worldImage.Position = new RemoteVector();
        worldImage.Rotation = new RemoteVector();
        worldImage.Type = "Bitmap";
        worldImage.Properties = new System.Collections.Generic.Dictionary<string, string>();

        // data
        string pixelStr = "";
        byte[] data = t.GetRawTextureData();
        Debug.Log(data.Length);
        pixelStr = System.Text.Encoding.ASCII.GetString(data);

        /*
        Color[] pixels = t.GetPixels();
        Debug.Log(pixels.Length);
        AddToString(pixels[0].r, pixelStr);

        // TODO do this in another thread? how long it takes -> measure time
        for (int i = 0; i < pixels.Length; i++)
        {
            // yield return new WaitForEndOfFrame();
            pixelStr = AddToString(pixels[i].r, pixelStr);
            pixelStr = AddToString(pixels[i].g, pixelStr);
            pixelStr = AddToString(pixels[i].b, pixelStr);
            pixelStr = AddToString(pixels[i].a, pixelStr);
        }
        */

        worldImage.Properties.Add("Format", "RGBA");
        worldImage.Properties.Add("Widht", $"{t.width}");
        worldImage.Properties.Add("Height", $"{t.height}");
        worldImage.Properties.Add("Pixels", pixelStr);
        Debug.Log("Converted to string");

        Debug.Log(worldImage.Properties["Pixels"].Length);
        Debug.Log(" \" " +worldImage.Properties["Pixels"] + "\"");

        // TODO - musim update když už tam je? jak poznam že se fakt přidal a tak?
        SendToServer(worldImage);

        Object.Destroy(t);
    }

    private async void SendToServer(WorldObject worldImage)
    {
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

    }

    private string AddToString(float f, string str)
    {
        byte[] byteArr = System.BitConverter.GetBytes(f);
        str += System.BitConverter.ToString(byteArr);
        return str;
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
        // if not connected try to connect every 2s   
        if (!connected)
        {
            timeToRetry -= Time.deltaTime;

            if (timeToRetry < 0.01)
            {
                TryToConnect();
            }
        }

        timeToSnapshot -= Time.deltaTime;
    }


    public void OnDestroy()
    {
         hubConnection.DisposeAsync();
    }
}
