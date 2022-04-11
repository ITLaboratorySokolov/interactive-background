using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Unity.Connections;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;

// TODO retry connection
// TODO update vs 1st send
// TODO test if connected to server
// TODO yells on close that screen capture cannot be done outside of playmode!

/// <summary>
/// Class that manages connection to server
/// - connects to server
/// - 4 times per second sends updates of the screen
/// - disconnects from server
/// </summary>
public class ServerConnection : MonoBehaviour
{
    /// <summary> Countdown to next image send </summary>
    private double timeToSnapshot;

    /// <summary> Connection to server </summary>
    [SerializeField]
    VirtualWorldServerConnectionWrapper connection;
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session;
    
    /// <summary> Action performed upon Start </summary>
    [SerializeField]
    UnityEvent actionStart = new UnityEvent();
    /// <summary Action performed upon Destroy </summary>
    [SerializeField]
    UnityEvent actionEnd = new UnityEvent();

    /// <summary> Bitmap serializer </summary>
    BitmapWorldObjectSerializer serializer;

    /// <summary>
    /// Performes once upon start
    /// - creates instances of needed local classes
    /// - calls action actionStart
    /// </summary>
    private void Start()
    {
        serializer = new BitmapWorldObjectSerializer();
        //session.StartSession();
        actionStart.Invoke();
    }

    /// <summary>
    /// Records current frame and sends it to server
    /// </summary>
    /// <returns></returns>
    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();

        // Record screenshot - resizing hurts performance
        // Texture2D scrsh = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D scaled = ScreenCapture.CaptureScreenshotAsTexture(); // ImageProcessor.ScaleTexture(scrsh, scrsh.width/2, scrsh.height/2);
        // scaled = ImageProcessor.ChangeFormat(scaled, TextureFormat.ARGB32);
        // Debug.Log(scaled.format);

        // Get pixel data
        var data = scaled.GetRawTextureData();
        var pxs = serializer.SerializePixels(data);

        /* Debug output
        var data2 = serializer.DeserializePixels(pxs);
        scaled.SetPixelData(data2, 0);
        scaled.Apply();
        
        byte[] bytes = scaled.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);

        string s = ""; string s2 = ""; string s3 = "";
        for (int i = 0; i < 100; i++)
        {
            s += data[i]; s2 += data2[i]; s3 += pxs[i]; 
        }
        Debug.Log(s); Debug.Log(s2); Debug.Log(s3);
        */

        // Add properties
        Dictionary<string, string> properties = new Dictionary<string, string>();
        properties.Add(BitmapWorldObjectSerializer.WidthKey, $"{scaled.width}");
        properties.Add(BitmapWorldObjectSerializer.HeightKey, $"{scaled.height}");
        properties.Add(BitmapWorldObjectSerializer.FormatKey, $"{scaled.format}");
        properties.Add(BitmapWorldObjectSerializer.PixelsKey, $"{pxs}");

        // Create data transfer object
        WorldObjectDto wod = new WorldObjectDto();
        wod.Name = "FlyKiller";
        wod.Position = new RemoteVectorDto();
        wod.Rotation = new RemoteVectorDto();
        wod.Scale = new RemoteVectorDto();
        wod.Scale.X = 1; wod.Scale.Y = 1; wod.Scale.Z = 1;
        wod.Type = "Bitmap";
        wod.Properties = properties;

        SendToServer(wod);
    }

    /// <summary>
    /// Send transfer object to server
    /// </summary>
    /// <param name="worldImage"> Transfer object </param>
    private void SendToServer(WorldObjectDto worldImage)
    {
        // TODO - musim update když už tam je, jak poznam že se fakt přidal a tak
        try
        {
            connection.AddWorldObjectAsync(worldImage);
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to send to server");
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// Late update called once per frame
    /// </summary>
    public void LateUpdate()
    {
        // Send frame to server if countdown low enough
        if (timeToSnapshot < 0.01)
        {
            // Reset timer
            timeToSnapshot = 0.25;
            StartCoroutine(RecordFrame());
        }

        /* Debug send on key pressed
        if (Input.GetKeyDown(KeyCode.U))
            StartCoroutine(RecordFrame());
        */
    }

    /// <summary>
    /// Update called once per frame
    /// </summary>
    void Update()
    {
        // Decrease countdown
        timeToSnapshot -= Time.deltaTime;
    }

    /// <summary>
    /// Called once upon destroying the object
    /// </summary>
    public void OnDestroy()
    {
        //session.StopSession();
        actionEnd.Invoke();
    }
}
