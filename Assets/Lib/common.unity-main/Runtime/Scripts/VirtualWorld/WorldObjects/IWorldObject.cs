using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.VirtualWorld.WorldObjects
{
    /// <summary>
    /// Inteface that describes functionality of a world object.
    /// </summary>
    public interface IWorldObject
    {
        /// <summary>
        /// Gets a type of a world object.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets properties of a world object.
        /// </summary>
        /// <returns>Properties of a world object.</returns>
        Dictionary<string, string> GetProperties();

        /// <summary>
        /// Sets properties of a world object.
        /// </summary>
        /// <param name="properties">Properties of a world object.</param>
        void SetProperties(Dictionary<string, string> properties);

        /// <summary>
        /// Sets value of a single property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">Value of the property.</param>
        void SetProperty(string propertyName, string propertyValue);
    }
}
