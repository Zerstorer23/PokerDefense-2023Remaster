using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision enter at " + transform.position);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Collision exit at " + transform.position);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Collision stay at " +transform.position);
    }
}
