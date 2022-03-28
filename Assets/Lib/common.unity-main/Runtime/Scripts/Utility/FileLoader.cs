using SimpleFileBrowser;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityMeshImporter;
using ZCU.TechnologyLab.Common.Unity.VirtualWorld.WorldObjects;

namespace ZCU.TechnologyLab.Common.Unity.Utility
{
    /// <summary>
    /// Loads files from file system with a file dialog.
    /// When files are selected an object with corresponding world object type is created.
    /// Created object is reported afterwards by an event.
    /// </summary>
    public class FileLoader : MonoBehaviour
    {
        /// <summary>
        /// Event called when a file is loaded and game object is created according to a type of a file.
        /// </summary>
        [SerializeField]
        private UnityEvent<GameObject> OnFileLoaded = new UnityEvent<GameObject> ();

        /// <summary>
        /// Opens a file dialog.
        /// </summary>
        public void OpenLocalFile()
        {
            StartCoroutine(ShowLoadDialogCoroutine());
        }

        /// <summary>
        /// Opens a file dialog and waits for user input. Loaded file is then parsed and object is added to an active world space.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator ShowLoadDialogCoroutine()
        {
            FileBrowser.SetFilters(false, 
                new FileBrowser.Filter("Obrázek (*.jpg; *.png)", ".jpg", ".png"),
                new FileBrowser.Filter("Mesh (*.obj; *.fbx; *.gltf; *.ply; *.stl)", ".obj", ".fbx", ".gltf", ".ply", ".stl"));

            // Show a load file dialog and wait for a response from user
            // Load file/folder: both, Allow multiple selection: true
            // Initial path: default (Documents), Initial filename: empty
            // Title: "Load File", Submit button text: "Load"
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Load Files and Folders", "Load");

            // Dialog is closed
            // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
            Debug.Log(FileBrowser.Success);

            if (FileBrowser.Success)
            {
                this.ReadFiles(FileBrowser.Result);
            }
        }

        /// <summary>
        /// Reads selected files.
        /// </summary>
        /// <param name="paths">Paths of files.</param>
        private void ReadFiles(string[] paths)
        {
            foreach (var path in paths)
            {
                string extension = Path.GetExtension(path);
                GameObject obj;

                switch(extension)
                {
                    case ".jpg":
                    case ".png":
                        {
                            obj = new GameObject(Path.GetFileNameWithoutExtension(path));
                            var bitmap = obj.AddComponent<BitmapWorldObject>();
                            bitmap.FromFile(path);
                        }
                        break;
                    case ".obj":
                    case ".fbx":
                    case ".gltf":
                    case ".ply":
                    case ".stl":
                        {
                            obj = MeshImporter.Load(path);
                            if (obj != null)
                            {
                                obj.AddComponent<MeshWorldObject>();
                            }
                        }
                        break;
                    default:
                        throw new FileLoadException("Unknown file extension.");

                }

                this.OnFileLoaded.Invoke(obj);
            }
        }
    }
}