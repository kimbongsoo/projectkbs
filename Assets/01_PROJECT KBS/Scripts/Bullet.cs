using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 20;
    void Start()
    {
        Destroy(gameObject, 3.0f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Wall cubeHp = collision.gameObject.GetComponent<Wall>();
        if (cubeHp != null)
        {
            cubeHp.DestoryCube(damage);
        }

        if (collision.gameObject.CompareTag("Item"))
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
