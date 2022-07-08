using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Connections.Session;
using ZCU.TechnologyLab.Common.Connections;
using ZCU.TechnologyLab.Common.Unity.Connections.Data;

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
    ServerSessionConnection connection;
    ServerDataConnection dataConnection;
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session;
    [SerializeField]
    RestDataClientWrapper dataSession;

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

    /// <summary> WOD Properties </summary>
    Dictionary<string, byte[]> properties;
    /// <summary> Scaled screen capture texture </summary>
    Texture2D scaled;
    /// <summary> Object data in byte array </summary>
    byte[] data;
    /// <summary> Stopwatch for debug </summary>
    System.Diagnostics.Stopwatch stopWatch;

    /// <summary>
    /// Performes once upon start
    /// - creates instances of needed local classes
    /// - calls action actionStart
    /// </summary>
    private void Start()
    {
        serializer = new BitmapSerializer();
        connection = new ServerSessionConnection(session);
        dataConnection = new ServerDataConnection(dataSession);
        
        actionStart.Invoke();

        Texture2D t = new Texture2D(1, 1);
        data = t.GetRawTextureData();
        properties = serializer.SerializeRawBitmap(t.width, t.height, "RGBA", data);

        // Create DTO
        wod = new WorldObjectDto();
        wod.Name = "FlyKiller";
        wod.Position = new RemoteVectorDto();
        wod.Rotation = new RemoteVectorDto();
        wod.Scale = new RemoteVectorDto();
        wod.Scale.X = 1; wod.Scale.Y = 1; wod.Scale.Z = 1;
        wod.Type = "Bitmap";
        wod.Properties = properties;

        Destroy(t);
    }

    /// <summary>
    /// Called when automatic connection to server fails
    /// - attempts to restart connection to server
    /// </summary>
    public void ConnectionFailed()
    {
        Debug.Log("Launching restart procedure");
        StartCoroutine(RestartConnection());
    }

    /// <summary>
    /// Restarting procedure
    /// - creates a minimum 5s delay
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartConnection()
    {
        yield return new WaitForSeconds(5);
        actionStart.Invoke();
    }

    /// <summary>
    /// Called when successfully connected to server
    /// </summary>
    public void ConnectedToServer()
    {
        Debug.Log("Connected to server");
        StartCoroutine(SyncCall());
    }

    /// <summary>
    /// Starting synchronization call
    /// </summary>
    /// <returns> IEnumerator </returns>
    IEnumerator SyncCall()
    {
        yield return new WaitUntil(() => session.SessionState == SessionState.Connected);
        GetObjectAsync();
    }

    private async void GetObjectAsync()
    {
        try
        {
            WorldObjectDto d = await dataConnection.GetWorldObjectAsync("FlyKiller");
        }
        catch
        {
            SendToServer(wod, false);
            Debug.Log("Init sent to server");

        }
        syncCallDone = true;
        Debug.Log("Sync call done");
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
    /// Records current frame and sends it to server
    /// </summary>
    /// <returns></returns>
    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();

        if (!syncCallDone)
            yield break;

        // Record screenshot - resizing hurts performance
        scaled = ScreenCapture.CaptureScreenshotAsTexture(); // ImageProcessor.ScaleTexture(scrsh, scrsh.width/2, scrsh.height/2);

        // Get pixel data
        data = scaled.GetRawTextureData();

        // Add properties
        properties = serializer.SerializeRawBitmap(scaled.width, scaled.height, "RGBA", data);  // new Dictionary<string, string>();

        // Add properties to DTO and send to server
        wod.Properties = properties;
        SendToServer(wod, true);

        Destroy(scaled);
    }

    /// <summary>
    /// Send transfer object to server
    /// </summary>
    /// <param name="worldImage"> Transfer object </param>
    private async void SendToServer(WorldObjectDto worldImage, bool update)
    {
        try
        {
            stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            if (update)
            {
                WorldObjectPropertiesDto props = new WorldObjectPropertiesDto() { Properties = worldImage.Properties };
                await dataConnection.UpdateWorldObjectPropertiesAsync(worldImage.Name, props);
            }
            else
                await dataConnection.AddWorldObjectAsync(worldImage);

            stopWatch.Stop();
            Debug.Log(stopWatch.ElapsedMilliseconds + " ms");
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to send to server");
            Debug.Log(e.Message);
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
            timeToSnapshot = 1;
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
