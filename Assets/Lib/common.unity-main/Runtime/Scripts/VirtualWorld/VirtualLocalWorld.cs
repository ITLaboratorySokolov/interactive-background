using System.Threading.Tasks;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.VirtualWorld
{
    /// <summary>
    /// Virtual world space on a local machine.
    /// </summary>
    public class VirtualLocalWorld : MonoBehaviour, IVirtualWorld
    {
        /// <inheritdoc/>
        public Task AddObjectAsync(GameObject gameObject)
        {
            gameObject.transform.parent = this.gameObject.transform;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public Task LoadAsync()
        {
            this.gameObject.SetActive(true);
            return Task.CompletedTask;
        }
    }
}
