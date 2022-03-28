using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Unity.Utility;

namespace ZCU.TechnologyLab.Common.Unity.VirtualWorld.WorldObjects
{
    /// <summary>
    /// A world object that describes a mesh.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshWorldObject : MonoBehaviour, IWorldObject
    {
        /// <summary>
        /// Description of a type of this world object.
        /// </summary>
        public const string TypeDescription = "Mesh";

        /// <summary>
        /// Supported mesh primitives.
        /// </summary>
        private static readonly string[] SupportedPrimitives = { "Triangle" };

        /// <summary>
        /// Mesh filter.
        /// </summary>
        /// <remarks>
        /// Provides informations about a mesh.
        /// </remarks>
        private MeshFilter meshFilter;

        /// <summary>
        /// Mesh renderer.
        /// </summary>
        /// <remarks>
        /// Provides informations about material of a mesh.
        /// </remarks>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// Mesh serializer.
        /// </summary>
        private MeshWorldObjectSerializer meshSerializer;

        /// <inheritdoc/>
        public string Type => TypeDescription;

        /// <summary>
        /// Initializes mesh filter and mesh renderer.
        /// </summary>
        private void Awake()
        {
            this.meshFilter = GetComponent<MeshFilter>();
            if(this.meshFilter.mesh == null)
            {
                this.meshFilter.mesh = new Mesh();
            }
            
            this.meshRenderer = GetComponent<MeshRenderer>();
            if(this.meshRenderer.material == null)
            {
                this.meshRenderer.material = new Material(Shader.Find("Diffuse"));
            }

            this.meshSerializer = new MeshWorldObjectSerializer();
        }

        /// <inheritdoc/>
        public Dictionary<string, string> GetProperties()
        {
            return this.meshSerializer.SerializeProperties(PointConverter.Point3DToFloat(this.meshFilter.mesh.vertices), this.meshFilter.mesh.triangles, SupportedPrimitives[0]);
        }

        /// <inheritdoc/>
        public void SetProperties(Dictionary<string, string> properties)
        {
            if (this.meshSerializer.SupportPrimitive(properties, SupportedPrimitives))
            {
                this.meshFilter.mesh.vertices = PointConverter.FloatToPoint3D(this.meshSerializer.DeserializeVertices(properties));
                this.meshFilter.mesh.triangles = this.meshSerializer.DeserializeIndices(properties);
                this.meshFilter.mesh.RecalculateNormals();
            }
        }

        /// <inheritdoc/>
        public void SetProperty(string propertyName, string propertyValue)
        {
            switch (propertyName)
            {
                case MeshWorldObjectSerializer.PointsKey:
                    {
                        this.meshFilter.mesh.vertices = PointConverter.FloatToPoint3D(this.meshSerializer.DeserializeVertices(propertyValue));
                    }
                    break;
                case MeshWorldObjectSerializer.IndicesKey:
                    {
                        this.meshFilter.mesh.triangles = this.meshSerializer.DeserializeIndices(propertyValue);
                    }
                    break;
            }
        }
    }
}
