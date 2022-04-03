using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization;

namespace ZCU.TechnologyLab.Common.Unity.VirtualWorld.WorldObjects
{
    /// <summary>
    /// A world object that describes a bitmap.
    /// </summary>
    public class BitmapWorldObject : MonoBehaviour, IWorldObject
    {
        /// <summary>
        /// Description of a type of this world object.
        /// </summary>
        public const string TypeDescription = "Bitmap";

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
        /// Texture that holds data of a bitmap.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Child game object that holds a bitmap.
        /// </summary>
        private GameObject bitmapObject;

        /// <summary>
        /// Bitmap serializer.
        /// </summary>
        public BitmapWorldObjectSerializer bitmapSerializer;

        /// <inheritdoc/>
        public string Type => TypeDescription;

        /// <summary>
        /// Initializes texture, mesh filter and mesh renderer.
        /// </summary>
        private void Awake()
        {
            this.texture = new Texture2D(1, 1, TextureFormat.ARGB32, true);
            this.bitmapObject = new GameObject("bitmap", typeof(MeshFilter), typeof(MeshRenderer));
            this.bitmapObject.transform.parent = this.transform;

            this.meshFilter = bitmapObject.GetComponent<MeshFilter>();
            this.meshFilter.mesh = this.GeneratePlane();

            this.meshRenderer = bitmapObject.GetComponent<MeshRenderer>();
            if (this.meshRenderer.material == null)
            {
                this.meshRenderer.material = new Material(Shader.Find("Diffuse"));
            }

            this.meshRenderer.material.mainTexture = this.texture;

            this.bitmapSerializer = new BitmapWorldObjectSerializer();
        }

        /// <summary>
        /// Creates a bitmap world object from a file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        public void FromFile(string path)
        {
            var data = File.ReadAllBytes(path);
            this.texture.LoadImage(data);
            this.SetScale(this.texture.width, this.texture.height);
        }

        /// <inheritdoc/>
        public Dictionary<string, string> GetProperties()
        {
            return this.bitmapSerializer.SerializeProperties(this.texture.width, this.texture.height, "RGB", this.texture.GetRawTextureData());
        }

        /// <inheritdoc/>
        public void SetProperties(Dictionary<string, string> properties)
        {
            this.texture.width = this.bitmapSerializer.DeserializeWidth(properties);
            this.texture.height = this.bitmapSerializer.DeserializeHeight(properties);

            this.SetScale(this.texture.width, this.texture.height);

            this.texture.SetPixelData(this.bitmapSerializer.DeserializePixels(properties), 0);
            this.texture.Apply(); // TODO: Might be neccessary to update changes
        }

        /// <inheritdoc/>
        public void SetProperty(string propertyName, string propertyValue)
        {
            switch (propertyName)
            {
                case BitmapWorldObjectSerializer.WidthKey:
                    {
                        this.texture.width = this.bitmapSerializer.DeserializeWidth(propertyValue);
                    }
                    break;
                case BitmapWorldObjectSerializer.HeightKey:
                    {
                        this.texture.height = this.bitmapSerializer.DeserializeHeight(propertyValue);
                    }
                    break;
                case BitmapWorldObjectSerializer.PixelsKey:
                    {
                        this.texture.SetPixelData(this.bitmapSerializer.DeserializePixels(propertyValue), 0);
                    }
                    break;
            }

            this.SetScale(this.texture.width, this.texture.height);
            //this.texture.Apply(); TODO: Might be neccessary to update changes
        }

        /// <summary>
        /// Generates a mesh of a plane.
        /// </summary>
        /// <returns>The mesh.</returns>
        private Mesh GeneratePlane()
        {
            var mesh = new Mesh();
            mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0) };
            mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
            mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            mesh.RecalculateNormals();
            return mesh;
        }

        /// <summary>
        /// Sets scale according to width and height of a bitmap.
        /// </summary>
        /// <param name="width">The width of the bitmap.</param>
        /// <param name="height">The height of the bitmap.</param>
        private void SetScale(int width, int height)
        {
            if (width > height)
            {
                this.bitmapObject.transform.localScale = new Vector3(1, (float)height / width, 1);
            }
            else
            {
                this.bitmapObject.transform.localScale = new Vector3((float)width / height, 1, 1);
            }
        }
    }
}
