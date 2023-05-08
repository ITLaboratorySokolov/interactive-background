using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Serialization.Bitmap;
using ZCU.TechnologyLab.Common.Connections.Client.Data;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;
using System.Threading.Tasks;

/// <summary>
/// Class that manages connection to server
/// - connects to server
/// - once per second sends updates of the screen
/// - disconnects from server
/// </summary>
public class ServerConnection : MonoBehaviour
{

    [Header("Server connection")]
    /// <summary> Connection to server </summary>
    // [SerializeField]
    ServerSessionAdapter connection; // ServerSessionConnection 
    ServerDataAdapter dataConnection; // ServerDataConnection 
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session; //SignalRSessionWrapper
    /// <summary> Countdown to next image send </summary>
    private double timeToSnapshot;
    /// <summary> Server url </summary>
    [SerializeField]
    StringVariable url;
    /// <summary> Client name </summary>
    [SerializeField]
    StringVariable clientName;

    [Header("Connection actions")]
    /// <summary> Action performed upon Start </summary>
    [SerializeField]
    UnityEvent actionStart = new UnityEvent();
    /// <summary Action performed upon Destroy </summary>
    [SerializeField]
    UnityEvent actionEnd = new UnityEvent();
    /// <summary> Synchronization call has been finished </summary>
    bool syncCallDone;

    [Header("Data objects")]
    /// <summary> Bitmap serializer </summary>
    RawBitmapSerializer serializer; //  BitmapSerializer 
    /// <summary> World object DTO for screenshot to be sent to server </summary>
    WorldObjectDto wod;
    /// <summary> WOD Properties </summary>
    Dictionary<string, byte[]> properties;
    /// <summary> Scaled screen capture texture </summary>
    Texture2D scaled;
    /// <summary> Object data in byte array </summary>
    byte[] data;


    /// <summary>
    /// Performes once upon start
    /// - creates instances of needed local classes
    /// - calls action actionStart
    /// </summary>
    private void Start()
    {
        serializer = new RawBitmapSerializer();
        connection = new ServerSessionAdapter(session);

        var restClient = new RestDataClient(url.Value);
        dataConnection = new ServerDataAdapter(restClient); // dataSession

        syncCallDone = false;
        actionStart.Invoke();

        // Get "empty" texture
        Texture2D t = new Texture2D(1, 1);
        data = t.GetRawTextureData();
        properties = serializer.Serialize(t.width, t.height, "RGBA", data);

        // Create DTO
        wod = new WorldObjectDto();
        wod.Name = "FlyKiller_" + clientName.Value;
        wod.Position = new RemoteVectorDto();
        wod.Rotation = new RemoteVectorDto();
        wod.Scale = new RemoteVectorDto();
        wod.Scale.X = 1; wod.Scale.Y = 1; wod.Scale.Z = 1;
        wod.Type = "Bitmap";
        wod.Properties = properties;

        Destroy(t);
    }

    /// <summary>
    /// On reconnecting
    /// </summary>
    public void OnReconnecting()
    {
        Debug.Log("Trying to reconnect...");
        syncCallDone = false;
    }

    /// <summary>
    /// On reconnected to server
    /// </summary>
    public void OnReconnected()
    {
        Debug.Log("Reconnected...");

        StartCoroutine(SyncCall());
    }

    /// <summary>
    /// Disconnected from server
    /// </summary>
    public void Disconnected()
    {
        Debug.Log("Server offline!!");
        ResetConnection();
        StartCoroutine(RestartConnection());
    }

    /// <summary>
    /// Reset connection to server
    /// </summary>
    public void ResetConnection()
    {
        syncCallDone = false;
    }

    /// <summary>
    /// Called when automatic connection to server fails
    /// - attempts to restart connection to server
    /// </summary>
    public void ConnectionFailed()
    {
        Debug.Log("Connection to server failed. Launching restart procedure");
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
        StartCoroutine(SyncCall());
    }

    /// <summary>
    /// Starting synchronization call
    /// </summary>
    /// <returns> IEnumerator </returns>
    IEnumerator SyncCall()
    {
        yield return new WaitUntil(() => session.State == SessionState.Connected);
        
        var t = GetObjectAsync();

        while (!t.IsCompleted)
            yield return null;

        syncCallDone = true;
        Debug.Log("Sync call done");
    }

    /// <summary>
    /// Get "FlyKiller" object from server database
    /// - if it is present, do nothing
    /// - if it is not, send initializing object to server
    /// </summary>
    private async Task GetObjectAsync()
    {
        // try to get object from server
        try
        {
            WorldObjectDto d = await dataConnection.GetWorldObjectAsync("FlyKiller_" + clientName.Value);
            wod.Position = d.Position;
            wod.Rotation = d.Rotation;
            wod.Scale = d.Scale;
            wod.Properties = d.Properties;
        }
        // if not located on server - send it to server
        catch
        {
            StartCoroutine(SendForFirstTime());
        }
    }

    /// <summary>
    /// Send world object to server for the first time
    /// </summary>
    /// <returns></returns>
    IEnumerator SendForFirstTime()
    {
        yield return new WaitForEndOfFrame();

        Texture2D t = ScreenCapture.CaptureScreenshotAsTexture();
        var data = t.GetRawTextureData();
        var properties = serializer.Serialize(t.width, t.height, "RGBA", data);

        wod.Properties = properties;

        SendToServer(wod, false);
        Debug.Log("Object " + wod.Name + " was sent to server database.");

        Destroy(t);
    }

    /// <summary>
    /// Records current frame and sends it to server
    /// </summary>
    /// <returns></returns>
    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        
        // No recording until synchronized with server
        if (!syncCallDone)
            yield break;

        if (session.State != SessionState.Connected)
            yield break;

        // Record screenshot - resizing hurts performance
        scaled = ScreenCapture.CaptureScreenshotAsTexture(); 

        // Get pixel data
        data = scaled.GetRawTextureData();

        // Add properties
        properties = serializer.Serialize(scaled.width, scaled.height, "RGBA", data); 

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
        bool contains = await dataConnection.ContainsWorldObjectAsync(worldImage.Name);

        try
        {
            if (contains)
            {
                WorldObjectPropertiesDto props = new WorldObjectPropertiesDto() { Properties = worldImage.Properties };
                await dataConnection.UpdateWorldObjectPropertiesAsync(worldImage.Name, props);
            }
            else
            {
                await dataConnection.AddWorldObjectAsync(worldImage);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to send to server:");
            Debug.Log(e.Message);
        }

        /*
        try
        {
            if (update)
            {
                WorldObjectPropertiesDto props = new WorldObjectPropertiesDto() { Properties = worldImage.Properties };
                await dataConnection.UpdateWorldObjectPropertiesAsync(worldImage.Name, props);
            }
            else
            {
                await dataConnection.AddWorldObjectAsync(worldImage);
            }
            }
            catch (Exception e)
            {
                Debug.LogError("Unable to send to server:");
                Debug.Log(e.Message);

                if (update)
                    await dataConnection.AddWorldObjectAsync(worldImage);
        }
        */
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
