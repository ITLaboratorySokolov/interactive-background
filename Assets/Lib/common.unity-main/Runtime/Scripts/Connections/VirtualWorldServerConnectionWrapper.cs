using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Connections;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;

namespace ZCU.TechnologyLab.Common.Unity.Connections
{
    /// <summary>
    /// A wrapper of VirtualWorldServerConnection from ZCU.TechnologyLab.Common.Connections.
    /// The wrapper enables for the connection to be managed from a Unity scene.
    /// </summary>
    public sealed class VirtualWorldServerConnectionWrapper : MonoBehaviour
    {
        /// <summary>
        /// Session client wrapper.
        /// </summary>
        [SerializeField]
        private SessionClientWrapper sessionClient;

        /// <summary>
        /// Original VirtualWrodlServerConnection.
        /// </summary>
        private VirtualWorldServerConnection serverConnection;

        /// <summary>
        /// Initializes connection to a server.
        /// </summary>
        private void Awake()
        {
            this.serverConnection = new VirtualWorldServerConnection(this.sessionClient);
        }

        #region Callbacks from a server

        /// <summary>
        /// Triggers provided action when <see cref="GetAllWorldObjects"/> method is called on a sever.
        /// </summary>
        /// <param name="callback">Triggered action.</param>
        public void OnGetAllWorldObjects(Action<List<WorldObjectDto>> callback)
        {
            this.serverConnection.OnGetAllWorldObjects(callback);
        }

        /// <summary>
        /// Triggers provided action when <see cref="AddWorldObject"/> method is called on a server.
        /// </summary>
        /// <param name="callback">Triggered callback.</param>
        public void OnAddWorldObject(Action<WorldObjectDto> callback)
        {
            this.serverConnection.OnAddWorldObject(callback);
        }

        /// <summary>
        /// Triggers provided action when <see cref="SetWorldObjectProperties"/> method is called on a sever.
        /// </summary>
        /// <param name="callback">Triggered action.</param>
        public void OnSetWorldObjectProperties(Action<WorldObjectPropertiesDto> callback)
        {
            this.serverConnection.OnSetWorldObjectProperties(callback);
        }

        /// <summary>
        /// Triggers provided action when <see cref="SetWorldObjectProperty"/> method is called on a sever.
        /// </summary>
        /// <param name="callback">Triggered action.</param>
        public void OnSetWorldObjectProperty(Action<WorldObjectPropertyDto> callback)
        {
            this.serverConnection.OnSetWorldObjectProperty(callback);
        }

        /// <summary>
        /// Triggers provided action when <see cref="RemoveWorldObject"/> method is called on a sever.
        /// </summary>
        /// <param name="callback">Triggered action.</param>
        public void OnRemoveWorldObject(Action<string> callback)
        {
            this.serverConnection.OnRemoveWorldObject(callback);
        }

        /// <summary>
        /// Triggers provided action when <see cref="TransformWorldObject"/> method is called on a sever.
        /// </summary>
        /// <param name="callback">Triggered action.</param>
        public void OnTransformWorldObject(Action<WorldObjectTransformDto> callback)
        {
            this.serverConnection.OnTransformWorldObject(callback);
        }

        /// <summary>
        /// Triggers provided action when <see cref="UpdateWorldObject"/> method is called on a sever.
        /// </summary>
        /// <param name="callback">Triggered action.</param>
        public void OnUpdateWorldObject(Action<WorldObjectDto> callback)
        {
            this.serverConnection.OnUpdateWorldObject(callback);
        }
        #endregion

        #region Methods called on a server

        /// <summary>
        /// Calls <see cref="GetAllWorldObjects"/> method on a server.
        /// Method will return all world objects via <see cref="OnGetAllWorldObjects"/> callback.
        /// </summary>
        /// <returns>A task</returns>
        public Task GetAllWorldObjectsAsync()
        {
            return this.serverConnection.GetAllWorldObjectsAsync();
        }

        /// <summary>
        /// Calls <see cref="AddWorldObject"/> method on a server.
        /// Method adds a world object on a server and the server tells other clients about the change.
        /// </summary>
        /// <param name="worldObject">Object that should be added.</param>
        /// <returns>A task.</returns>
        public Task AddWorldObjectAsync(WorldObjectDto worldObject)
        {
            return this.serverConnection.AddWorldObjectAsync(worldObject);
        }

        /// <summary>
        /// Calls <see cref="RemoveWorldObject"/> method on a server.
        /// Method removes a world object on a server and the server tells other clients about the change.
        /// </summary>
        /// <param name="worldObject">Object that should be removed.</param>
        /// <returns>A task.</returns>
        public Task RemoveWorldObjectAsync(string objectName)
        {
            return this.serverConnection.RemoveWorldObjectAsync(objectName);
        }

        /// <summary>
        /// Calls <see cref="SetWorldObjectProperties"/> method on a server.
        /// Method changes properties of a world object on a server and the server tells other clients about the change.
        /// </summary>
        /// <param name="objectName">Name of an object that should have its properties updated.</param>
        /// <param name="properties">New properties.</param>
        /// <returns>A task.</returns>
        public Task SetWorldObjectPropertiesAsync(string objectName, Dictionary<string, string> properties)
        {
            return this.serverConnection.SetWorldObjectPropertiesAsync(objectName, properties);
        }

        /// <summary>
        /// Calls <see cref="SetWorldObjectProperties"/> method on a server.
        /// Method changes properties of a world object on a server and the server tells other clients about the change.
        /// </summary>
        /// <param name="worldObjectProperties">DTO that contains informations about object and properties.</param>
        /// <returns>A task.</returns>
        public Task SetWorldObjectPropertiesAsync(WorldObjectPropertiesDto worldObjectProperties)
        {
            return this.serverConnection.SetWorldObjectPropertiesAsync(worldObjectProperties);
        }

        /// <summary>
        /// Calls <see cref="SetWorldObjectProperty"/> method on a server.
        /// Method changes value of a property of a world object on a server and the server tells other clients about the change.
        /// </summary>
        /// <param name="objectName">Name of an object that should have a property updated.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="property">Value of the property.</param>
        /// <returns>A task.</returns>
        public Task SetWorldObjectPropertyAsync(string objectName, string propertyName, string property)
        {
            return this.serverConnection.SetWorldObjectPropertyAsync(objectName, propertyName, property);
        }

        /// <summary>
        /// Calls <see cref="SetWorldObjectProperty"/> method on a server.
        /// Method changes value of a property of a world object on a server and the server tells other clients about the change.
        /// </summary>
        /// <param name="worldObjectProperty">DTO that contains informations about object and property.</param>
        /// <returns>A task.</returns>
        public Task SetWorldObjectPropertyAsync(WorldObjectPropertyDto worldObjectProperty)
        {
            return this.serverConnection.SetWorldObjectPropertyAsync(worldObjectProperty);
        }

        /// <summary>
        /// Calls <see cref="TransformWorldObject"/> method on a server.
        /// Method transforms a world object on a server and the server tells other clients about the change.
        /// </summary>
        /// <param name="objectName">Name of an object that should be transformed.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Euler angles rotation around axis.</param>
        /// <param name="scale">Scale.</param>
        /// <returns>A task.</returns>
        public Task TransformWorldObjectAsync(string objectName, RemoteVectorDto position, RemoteVectorDto rotation, RemoteVectorDto scale)
        {
            return this.serverConnection.TransformWorldObjectAsync(objectName, position, rotation, scale);
        }

        /// <summary>
        /// Calls <see cref="TransformWorldObject"/> method on a server.
        /// Method transforms a world object on a server and the server tells other clients about the change.
        /// </summary>
        /// <param name="worldObjectTransform">DTO that contains informations about object and transform.</param>
        /// <returns>A task.</returns>
        public Task TransformWorldObjectAsync(WorldObjectTransformDto worldObjectTransform)
        {
            return this.serverConnection.TransformWorldObjectAsync(worldObjectTransform);
        }

        /// <summary>
        /// Calls <see cref="UpdateWorldObject"/> method on a server.
        /// </summary>
        /// <param name="worldObject">World object that should be updated.</param>
        /// <returns>A task.</returns>
        public Task UpdateWorldObjectAsync(WorldObjectDto worldObject)
        {
            return this.serverConnection.UpdateWorldObjectAsync(worldObject);
        }
        #endregion
    }
}
