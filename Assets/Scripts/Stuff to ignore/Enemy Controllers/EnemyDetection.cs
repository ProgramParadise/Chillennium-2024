using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public Transform originPointDown, endPointDown, originPointLeft, endPointLeft, originPointRight, endPointRight, originPointUp, endPointUp;

    public bool wallDetectedDown, wallDetectedLeft, wallDetectedRight, wallDetectedUp;
    private int collisions;

    private void Update()
    {
        collisions = Physics.AllLayers;
        WallDetector();
    }

    void WallDetector()
    {
        Debug.DrawLine(originPointDown.position, endPointDown.position, Color.green);
        Debug.DrawLine(originPointLeft.position, endPointLeft.position, Color.green);
        Debug.DrawLine(originPointRight.position, endPointRight.position, Color.green);
        Debug.DrawLine(originPointUp.position, endPointUp.position, Color.green);
        wallDetectedDown = Physics2D.Linecast(originPointDown.position, endPointDown.position, collisions);
        wallDetectedLeft = Physics2D.Linecast(originPointLeft.position, endPointLeft.position, collisions);
        wallDetectedRight = Physics2D.Linecast(originPointRight.position, endPointRight.position, collisions);
        wallDetectedUp = Physics2D.Linecast(originPointUp.position, endPointUp.position, collisions);
    }
}
