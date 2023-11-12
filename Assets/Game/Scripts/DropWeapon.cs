using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapon : MonoBehaviour
{
    public List<GameObject> swords;

    public void DropSwords()
    {
        foreach (GameObject sword in swords)
        {
            sword.AddComponent<Rigidbody>();
            sword.AddComponent<BoxCollider>();
            sword.transform.parent = null;
        }
    }
}
