using UnityEngine;

public class FlyController : MonoBehaviour
{
    internal bool oneFrame = false;
    internal bool moved = false;
    internal bool hit = false;
    public Transform minSceneLoc;
    public Transform maxSceneLoc;
    public Swatter swatter;

    System.Random r = new System.Random();

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            MoveToNewLocation();
        }        
    }

    public void MoveToNewLocation()
    {
        float newX = minSceneLoc.position.x + r.Next((int)(maxSceneLoc.position.x - minSceneLoc.position.x));
        float newY = maxSceneLoc.position.y + r.Next((int)(minSceneLoc.position.y - maxSceneLoc.position.y));

        transform.position = new Vector3(newX, newY);
        hit = false;
        oneFrame = true;
        moved = true;
    }
}
