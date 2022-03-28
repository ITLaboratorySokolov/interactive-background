using System.Threading.Tasks;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.VirtualWorld
{
    /// <summary>
    /// Interface for a virtual world that represents a local or server space.
    /// </summary>
    public interface IVirtualWorld
    {
        /// <summary>
        /// Add object to a world.
        /// </summary>
        /// <param name="gameObject">Game object that should be added.</param>
        /// <returns>A task.</returns>
        Task AddObjectAsync(GameObject gameObject);

        /// <summary>
        /// Hides world and all objects within.
        /// </summary>
        void Hide();

        /// <summary>
        /// Loads a world and all objects.
        /// </summary>
        /// <returns>A task.</returns>
        Task LoadAsync();
    }
}