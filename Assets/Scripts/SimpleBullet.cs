using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    public string shoter;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        transform.position += transform.right;
        GetComponent<Rigidbody2D>().velocity = transform.right * 40f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            PlayerMove pm = collision.transform.GetComponent<PlayerMove>();
            if (pm.localPlayer)
            {
                if (pm.user != shoter)
                {
                    pm.takeDamage(damage, shoter);
                    Destroy(gameObject);
                }

            }
            else
            {
                if (pm.user != shoter)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
