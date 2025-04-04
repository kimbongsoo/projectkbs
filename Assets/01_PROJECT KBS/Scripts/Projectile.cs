using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100f;
    public float lifeTime = 3f;
    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log(collision.gameObject.name);
        //총알 충돌처리에 대한 구현
        //TODO 1. Effect를 출력.
        //TODO 2. Damage 처리하기 -> 캐릭터, 배경에 맞았는지
        string[] materials = { "Wood", "Rock", "Dirt", "Metal" };

        string material = materials.FirstOrDefault(type => collision.collider.material.name.Contains(type));

        if (EffectManager.Instance.GetEffect(material, out GameObject effect))
        {
            effect.transform.position = collision.contacts[0].point;
            effect.transform.forward = collision.contacts[0].normal;
        }
        // if(collision.collider.material.name.Contains("Wood"))
        // {
        //     if(EffectManager.Instance.GetEffect("Wood", out GameObject woodEffect))
        //     {
        //         woodEffect.transform.position = collision.contacts[0].point;
        //         woodEffect.transform.forward = collision.contacts[0].normal;
        //     }
        // }
        // else if(collision.collider.material.name.Contains("Rock"))
        // {
        //     if(EffectManager.Instance.GetEffect("Rock", out GameObject rockEffect))
        //     {
        //         rockEffect.transform.position = collision.contacts[0].point;
        //         rockEffect.transform.forward = collision.contacts[0].normal;
        //     }
        // }
        // else if(collision.collider.material.name.Contains("Dirt"))
        // {
        //     if(EffectManager.Instance.GetEffect("Dirt", out GameObject dirtEffect))
        //     {
        //         dirtEffect.transform.position = collision.contacts[0].point;
        //         dirtEffect.transform.forward = collision.contacts[0].normal;
        //     }
        // }
        // else if(collision.collider.material.name.Contains("Metal"))
        // {
        //     if(EffectManager.Instance.GetEffect("Metal", out GameObject metalEffect))
        //     {
        //         metalEffect.transform.position = collision.contacts[0].point;
        //         metalEffect.transform.forward = collision.contacts[0].normal;
        //     }
        // }
        //총알 파괴
        Destroy(gameObject);  

    }

}
