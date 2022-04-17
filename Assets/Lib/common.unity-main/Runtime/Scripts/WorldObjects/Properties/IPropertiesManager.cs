using System;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Utility.Events;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties
{
    /// <summary>
    /// Inteface that prescribes how properties of a world object should be managed.
    /// 
    /// Properties setters in this interface should be used only when the world object is changed from a server. 
    /// Because of this, methods in the interface do not affect events.
    /// 
    /// Events are affected only when a property is set directly with Unity classes. For example when a UnityEngine.Mesh
    /// or verticies and triangles as arrays are set to MeshPropertiesManager via <see cref="MeshPropertiesManager.SetMesh(Mesh)"/>,
    /// <see cref="MeshPropertiesManager.SetVertices(Vector3[])"/>, <see cref="MeshPropertiesManager.SetTriangles(int[])"/> and so on.
    /// Triggered events then result in an update which is sent to a server.
    /// </summary>
    public interface IPropertiesManager
    {
        /// <summary>
        /// Event called when multiple properties change at once.
        /// </summary>
        event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;

        /// <summary>
        /// Event called when a single property is changed.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Gets a type of a managed world object.
        /// </summary>
        string ManagedType { get; }

        /// <summary>
        /// Gets properties of a world object.
        /// </summary>
        /// <returns>Properties of a world object.</returns>
        Dictionary<string, string> GetProperties();

        /// <summary>
        /// Sets properties of a world object.
        /// 
        /// This method does not cause events <see cref="PropertiesChanged"/> and <see cref="PropertyChanged"/> to be called.
        /// </summary>
        /// <param name="properties">Properties of a world object.</param>
        void SetProperties(Dictionary<string, string> properties);

        /// <summary>
        /// Sets value of a single property.
        /// 
        /// This method does not cause events <see cref="PropertiesChanged"/> and <see cref="PropertyChanged"/> to be called.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">Value of the property.</param>
        void SetProperty(string propertyName, string propertyValue);
    }
}