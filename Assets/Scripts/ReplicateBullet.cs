using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicateBullet : SimpleBullet
{
    public GameObject bulletPref;
    public float amplitude = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        var b1 = Instantiate(bulletPref, transform.position, Quaternion.identity);
        b1.transform.right = Vector3.Lerp(transform.right, transform.up, amplitude);
        b1.GetComponent<SimpleBullet>().shoter = shoter;
        b1.GetComponent<SimpleBullet>().damage = damage;
        var b2 = Instantiate(bulletPref, transform.position, Quaternion.identity);
        b2.transform.right = Vector3.Lerp(transform.right, -transform.up, amplitude);
        b2.GetComponent<SimpleBullet>().shoter = shoter;
        b2.GetComponent<SimpleBullet>().damage = damage;
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
