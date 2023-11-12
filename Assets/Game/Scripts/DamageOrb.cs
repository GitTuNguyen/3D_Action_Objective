using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public float speed = 2f;
    public int damage = 10;
    public ParticleSystem hitVFX;
    private Rigidbody _rb;
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate() {
        _rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        Character character = other.GetComponent<Character>();
        if (character != null && other.tag == "Player")
        {
            print("shoot");
            character.ApplyDamage(damage, transform.position);
        }
        Instantiate(hitVFX, transform.position, Quaternion.identity);
        Destroy(gameObject );
    }

}
