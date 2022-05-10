using System;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Connections.Session;

namespace ZCU.TechnologyLab.Common.Unity.Connections.Session
{
    /// <summary>
    /// Abstract wrapper of a session client from ZCU.TechnologyLab.Common.Connections.Session.
    /// The wrapper enables for the session client to be managed from a Unity scene.
    /// </summary>
    public abstract class SessionClientWrapper : MonoBehaviour, ISessionClient
    {
        /// <summary>
        /// Session client.
        /// </summary>
        protected ISessionClient sessionClient;

        /// <inheritdocs/>
        public SessionState SessionState => this.sessionClient.SessionState;

        /// <summary>
        /// Awake method is called when game object is created.
        /// It initializes the session client.
        /// </summary>
        protected abstract void Awake();

        /// <summary>
        /// Starts a session.
        /// This method should be used only for events. It cannot be awaited.
        /// </summary>
        public async void StartSession()
        {
            try
            {
                await this.StartSessionAsync();
                Debug.Log("Session started");
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot start session", this);
                Debug.LogException(ex, this);
            }
        }

        /// <summary>
        /// Stops a session.
        /// This method should be used only for events. It cannot be awaited.
        /// </summary>
        public async void StopSession()
        {
            try
            {
                await this.StopSessionAsync();
                Debug.Log("Session stopped");
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot stop session", this);
                Debug.LogException(ex, this);
            }
        }

        /// <inheritdoc/>
        public Task StartSessionAsync()
        {
            return this.sessionClient.StartSessionAsync();
        }

        /// <inheritdoc/>
        public Task StopSessionAsync()
        {
            return this.sessionClient.StopSessionAsync();
        }

        /// <inheritdoc/>
        public void RegisterCallback<T>(string method, Action<T> callback)
        {
            this.sessionClient.RegisterCallback(method, callback);
        }

        /// <inheritdoc/>
        public void UnregisterCallback(string method)
        {
            this.sessionClient.UnregisterCallback(method);
        }

        /// <inheritdoc/>
        public Task SendMessageAsync<T>(string method, T parameter)
        {
            return this.sessionClient.SendMessageAsync(method, parameter);
        }
    }
}
