using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZCU.TechnologyLab.Common.Unity.Utility.Events
{
    /// <summary>
    /// Arguments of <see cref="ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.IPropertiesManager.PropertiesChanged"/> event.
    /// </summary>
    public class PropertiesChangedEventArgs
    {
        /// <summary>
        /// Name of the object that triggered the event.
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Changed properties.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
    }
}
