using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExceptionDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy_PathFind>())
        {
            Destroy(collision.gameObject.transform.parent.gameObject);
        
        }
        else if (collision.GetComponent<Projectile>())
        {
            collision.GetComponent<Projectile>().DestroyMyself();

        }
    }
   
}
