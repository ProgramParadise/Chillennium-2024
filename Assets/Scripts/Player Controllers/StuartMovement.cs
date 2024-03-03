using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuartMovement : MonoBehaviour
{
    public float step;
    public float yDistance;
    private float yStart;
    private bool movingUp = true;

    void Start()
    {
        yStart = gameObject.transform.position.y;
    }

    void Update()
    {
        if (movingUp && (gameObject.transform.position.y + step <= yStart + yDistance))
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + step, gameObject.transform.position.z);
            if (gameObject.transform.position.y + step > yStart + yDistance)
            {
                movingUp = false;
            }
        } else if (!movingUp && (gameObject.transform.position.y - step >= yStart))
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - step, gameObject.transform.position.z);
            if (gameObject.transform.position.y - step < yStart)
            {
                movingUp = true;
            }
        }
    }
}
