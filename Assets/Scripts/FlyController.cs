using UnityEngine;

/// <summary>
/// Script controling the behaviour of the fly
/// </summary>
public class FlyController : MonoBehaviour
{
    [Header("Position in scene")]
    /// <summary> Minimum allowed position of fly </summary>
    [SerializeField]
    Transform minSceneLoc;
    /// <summary> Maximum allowed position of fly </summary>
    [SerializeField]
    Transform maxSceneLoc;

    [Header("Scripts")]
    /// <summary> Swatter script </summary>
    [SerializeField]
    Swatter swatter;

    [Header("Internal booleans")]
    /// <summary> Did fly move in this frame </summary>
    internal bool oneFrame = false;
    /// <summary> Did fly move </summary>
    internal bool moved = false;

    /// <summary> Random </summary>
    System.Random r;

    /// <summary>
    /// Performes once at the start
    /// </summary>
    private void Start()
    {
        r = new System.Random();
    }

    /// <summary>
    /// Move fly to a new random location
    /// </summary>
    public void MoveToNewLocation()
    {
        float newX = minSceneLoc.position.x + r.Next((int)(maxSceneLoc.position.x - minSceneLoc.position.x));
        float newY = maxSceneLoc.position.y + r.Next((int)(minSceneLoc.position.y - maxSceneLoc.position.y));
        transform.position = new Vector3(newX, newY);
        
        // It was hit this frame
        oneFrame = true;
        // It has moved
        moved = true;
    }
}
