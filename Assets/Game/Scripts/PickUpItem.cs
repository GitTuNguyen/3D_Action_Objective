using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PickUpItem : MonoBehaviour
{
    public enum ItemType
    {
        Health,
        Coin
    }

    public ItemType type;
    public int value;
    public ParticleSystem pickUpVFX;
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            other.GetComponent<Character>().PickUp(this);
            if(pickUpVFX != null)
            {
                Instantiate(pickUpVFX, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
