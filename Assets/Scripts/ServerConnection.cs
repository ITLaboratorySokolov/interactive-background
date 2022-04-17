﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Connections.Session;

// TODO retry connection
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
    ZCU.TechnologyLab.Common.Connections.ServerConnection connection;
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
    BitmapSerializer serializer;
    /// <summary> World object DTO for screenshot to be sent to server </summary>
    WorldObjectDto wod;
    /// <summary> Synchronization call has been finished </summary>
    bool syncCallDone;

    /// <summary>
    /// Performes once upon start
    /// - creates instances of needed local classes
    /// - calls action actionStart
    /// </summary>
    private void Start()
    {
        serializer = new BitmapSerializer();
        connection = new ZCU.TechnologyLab.Common.Connections.ServerConnection(session);
        //session.StartSession();
        actionStart.Invoke();

        // Create DTO
        wod = new WorldObjectDto();
        wod.Name = "FlyKiller";
        wod.Position = new RemoteVectorDto();
        wod.Rotation = new RemoteVectorDto();
        wod.Scale = new RemoteVectorDto();
        wod.Scale.X = 1; wod.Scale.Y = 1; wod.Scale.Z = 1;
        wod.Type = "Bitmap";

        StartCoroutine(SyncCall());
    }

    /// <summary>
    /// Starting synchronization call
    /// </summary>
    /// <returns> IEnumerator </returns>
    IEnumerator SyncCall()
    {
        yield return new WaitUntil(() => session.SessionState == SessionState.Connected);
        connection.AllWorldObjectsReceived += ProcessObjects;
        GetAllObjects();
    }

    /// <summary>
    /// Process incoming objects from server
    /// </summary>
    /// <param name="l"> List of objects </param>
    private void ProcessObjects(List<WorldObjectDto> l)
    {
        bool present = false;

        // Look through l for "FlyKiller"
        for (int i = 0; i < l.Count; i++)
            if (l[i].Name == wod.Name)
                present = true;

        Debug.Log("Present? " + present);

        // If not present - send
        if (!present)
            SendToServer(wod, false);

        syncCallDone = true;
    }

    /// <summary>
    /// Get all objects from server
    /// </summary>
    private async void GetAllObjects()
    {
        Debug.Log("Start");

        // Is it already on server
        try
        {
            await connection.GetAllWorldObjectsAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        Debug.Log("End");
    }

    /// <summary>
    /// Records current frame and sends it to server
    /// </summary>
    /// <returns></returns>
    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();

        if (!syncCallDone)
            yield break;

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
        properties.Add(BitmapSerializer.WidthKey, $"{scaled.width}");
        properties.Add(BitmapSerializer.HeightKey, $"{scaled.height}");
        properties.Add(BitmapSerializer.FormatKey, $"{scaled.format}");
        properties.Add(BitmapSerializer.PixelsKey, $"{pxs}");

        // Add properties to DTO and send to server
        wod.Properties = properties;
        SendToServer(wod, true);
    }

    /// <summary>
    /// Send transfer object to server
    /// </summary>
    /// <param name="worldImage"> Transfer object </param>
    private async void SendToServer(WorldObjectDto worldImage, bool update)
    {
        try
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            if (update)
                await connection.UpdateWorldObjectAsync(worldImage);
            else 
                await connection.AddWorldObjectAsync(worldImage);

            stopWatch.Stop();
            Debug.Log(stopWatch.ElapsedMilliseconds + " ms");

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

        // Debug send on key pressed
        /*
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
