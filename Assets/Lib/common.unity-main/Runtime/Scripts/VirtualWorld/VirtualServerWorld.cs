using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Connections;
using ZCU.TechnologyLab.Common.Unity.VirtualWorld.WorldObjects;

namespace ZCU.TechnologyLab.Common.Unity.VirtualWorld
{
    /// <summary>
    /// Virtual world space on a server.
    /// </summary>
    public class VirtualServerWorld : MonoBehaviour, IVirtualWorld
    {
        /// <summary>
        /// Dictionary of all objects that are placed in this space.
        /// </summary>
        private readonly Dictionary<string, GameObject> worldObjects = new Dictionary<string, GameObject>();

        /// <summary>
        /// Supported world objects.
        /// </summary>
        private readonly Dictionary<string, Type> supportedTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Connection to a server.
        /// </summary>
        [SerializeField]
        private VirtualWorldServerConnectionWrapper serverConnection;

        /// <summary>
        /// Event that is called when a new object is received from a server.
        /// </summary>
        [SerializeField]
        private UnityEvent<GameObject> OnWorldObjectReceived = new UnityEvent<GameObject>();

        /// <summary>
        /// Inicialize private members.
        /// </summary>
        private void Awake()
        {
            this.supportedTypes.Add(MeshWorldObject.TypeDescription, typeof(MeshWorldObject));
            this.supportedTypes.Add(BitmapWorldObject.TypeDescription, typeof(BitmapWorldObject));
        }

        /// <summary>
        /// Assigns action to callbacks on start.
        /// </summary>
        private void Start()
        {
            this.serverConnection.OnGetAllWorldObjects(LoadWorldObjects);
            this.serverConnection.OnAddWorldObject(AddWorldObject);
            this.serverConnection.OnRemoveWorldObject(RemoveWorldObject);
            this.serverConnection.OnSetWorldObjectProperties(SetWorldObjectProperties);
            this.serverConnection.OnSetWorldObjectProperty(SetWorldObjectProperty);
            this.serverConnection.OnUpdateWorldObject(UpdateWorldObject);
            this.serverConnection.OnTransformWorldObject(TransformWorldObject);
        }

        /// <inheritdoc/>
        public async Task AddObjectAsync(GameObject gameObject)
        {
            var worldObject = gameObject.GetComponent<IWorldObject>();
            if (worldObject == null)
            {
                throw new ArgumentException($"GameObject does not contain component of type {nameof(IWorldObject)}");
            }

            gameObject.transform.parent = this.transform;
            this.worldObjects.Add(gameObject.name, gameObject);

            var worldObjectDto = new WorldObjectDto
            {
                Name = gameObject.name,
                Position = new RemoteVectorDto
                {
                    X = gameObject.transform.position.x,
                    Y = gameObject.transform.position.y,
                    Z = gameObject.transform.position.z
                },
                Rotation = new RemoteVectorDto
                {
                    X = gameObject.transform.rotation.eulerAngles.x,
                    Y = gameObject.transform.rotation.eulerAngles.y,
                    Z = gameObject.transform.rotation.eulerAngles.z
                },
                Scale = new RemoteVectorDto
                {
                    X = gameObject.transform.localScale.x,
                    Y = gameObject.transform.localScale.y,
                    Z = gameObject.transform.localScale.z
                },
                Type = worldObject.Type,
                Properties = worldObject.GetProperties()
            };

            await this.serverConnection.AddWorldObjectAsync(worldObjectDto);
        }

        /// <inheritdoc/>
        public async Task LoadAsync()
        {
            this.Clear();
            this.gameObject.SetActive(true);
            await this.serverConnection.GetAllWorldObjectsAsync();
        }

        /// <inheritdoc/>
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Deletes all objects.
        /// </summary>
        public void Clear()
        {
            this.worldObjects.Clear();
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Adds world object from server on the machine.
        /// </summary>
        /// <param name="worldObjectDto">World object that should be added.</param>
        private void AddWorldObject(WorldObjectDto worldObjectDto)
        {
            try
            {
                Debug.Log("Add world object");
                Debug.Log($"Name: {worldObjectDto.Name}");
                Debug.Log($"Properties count: {worldObjectDto.Properties.Count}");
                foreach (var property in worldObjectDto.Properties)
                {
                    Debug.Log($"Property key: {property.Key}; Property value: {property.Value}");
                }

                if (this.supportedTypes.TryGetValue(worldObjectDto.Type, out Type type))
                {
                    var gameObject = new GameObject(worldObjectDto.Name);
                    gameObject.transform.parent = this.transform;

                    this.SetTransform(gameObject, worldObjectDto.Position, worldObjectDto.Rotation, worldObjectDto.Scale);

                    IWorldObject worldObject = (IWorldObject)gameObject.AddComponent(type);
                    worldObject.SetProperties(worldObjectDto.Properties);
                    this.worldObjects.Add(worldObjectDto.Name, gameObject);

                    this.OnWorldObjectReceived.Invoke(gameObject);
                }                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

        }

        /// <summary>
        /// Removes a world object when it was deleted from a server.
        /// </summary>
        /// <param name="name">Name of the world object.</param>
        private void RemoveWorldObject(string name)
        {
            try
            {
                if (this.worldObjects.TryGetValue(name, out var worldObject))
                {
                    GameObject.Destroy(worldObject);
                    this.worldObjects.Remove(name);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Loads all objects from server.
        /// </summary>
        /// <param name="worldObjectDtos">World objects that are sent form server.</param>
        private void LoadWorldObjects(List<WorldObjectDto> worldObjectDtos)
        {
            try
            {
                Debug.Log("Load world objects");

                foreach (var worldObjectDto in worldObjectDtos)
                {
                    this.AddWorldObject(worldObjectDto);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

        }

        /// <summary>
        /// Sets propeties of a world object.
        /// </summary>
        /// <param name="propertiesDto">Properties received from a server.</param>
        private void SetWorldObjectProperties(WorldObjectPropertiesDto propertiesDto)
        {
            try
            {
                if (this.worldObjects.TryGetValue(propertiesDto.ObjectName, out var worldObject))
                {
                    var worldObjectType = worldObject.GetComponent<IWorldObject>();
                    if (worldObjectType != null)
                    {
                        worldObjectType.SetProperties(propertiesDto.Properties);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Sets a single property of a world object.
        /// </summary>
        /// <param name="propertyDto">Property received from a server.</param>
        private void SetWorldObjectProperty(WorldObjectPropertyDto propertyDto)
        {
            try 
            { 
                if (this.worldObjects.TryGetValue(propertyDto.ObjectName, out var worldObject))
                {
                    var worldObjectType = worldObject.GetComponent<IWorldObject>();
                    if (worldObjectType != null)
                    {
                        worldObjectType.SetProperty(propertyDto.PropertyName, propertyDto.PropertyValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Transforms an object.
        /// </summary>
        /// <param name="worldObjectTransformDto">Transform of an object.</param>
        private void TransformWorldObject(WorldObjectTransformDto worldObjectTransformDto)
        {
            try
            {
                Debug.Log("Transform world object");

                if (this.worldObjects.TryGetValue(worldObjectTransformDto.ObjectName, out var gameObject))
                {
                    this.SetTransform(gameObject, worldObjectTransformDto.Position, worldObjectTransformDto.Rotation, worldObjectTransformDto.Scale);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Updates a world object.
        /// World object must be already added.
        /// </summary>
        /// <param name="worldObjectDto">Updated world object.</param>
        private void UpdateWorldObject(WorldObjectDto worldObjectDto)
        {
            try
            {
                if(this.worldObjects.TryGetValue(worldObjectDto.Name, out GameObject gameObject))
                {
                    var worldObject = gameObject.GetComponent<IWorldObject>();
                    if (worldObject == null)
                    {
                        throw new ArgumentException($"GameObject does not contain component of type {nameof(IWorldObject)}");
                    }

                    this.SetTransform(gameObject, worldObjectDto.Position, worldObjectDto.Rotation, worldObjectDto.Scale);
                    worldObject.SetProperties(worldObjectDto.Properties);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Sets transform to a game object.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        private void SetTransform(GameObject gameObject, RemoteVectorDto position, RemoteVectorDto rotation, RemoteVectorDto scale)
        {
            gameObject.transform.SetPositionAndRotation(
                new Vector3(position.X, position.Y, position.Z),
                Quaternion.Euler(rotation.X, rotation.Y, rotation.Z));

            gameObject.transform.localScale = new Vector3(scale.X, scale.Y, scale.Z);
        }
    }
}