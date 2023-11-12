using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02_Behaviour : MonoBehaviour
{
    //Shooting
    public GameObject damageOrb;
    public Transform shootingPoint;

    private Character _character;
    private void Awake() {
        _character = GetComponent<Character>();
    }
    void Update()
    {
        _character.RotateToTarget();
    }

    private void Shooting()
    {
        Instantiate(damageOrb, shootingPoint.position, shootingPoint.rotation);
    }
}
