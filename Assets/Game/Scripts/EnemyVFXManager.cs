using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect footStepBurt;
    public VisualEffect attackSmash;
    public ParticleSystem beingHitVFX;
    public VisualEffect beingHitSplashVFX;

    public void FootStepBurt()
    {
        footStepBurt.SendEvent("OnPlay");
    }

    public void AttackSmash()
    {
        attackSmash.SendEvent("OnPlay");
    }

    public void PlayBeingHit(Vector3 attackPos)
    {
        Vector3 forceForwad = transform.position - attackPos;
        forceForwad.Normalize();
        forceForwad.y = 0f;
        beingHitVFX.transform.rotation = Quaternion.LookRotation(forceForwad);
        beingHitVFX.Play();
        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        VisualEffect splashClone = Instantiate(beingHitSplashVFX, splashPos, Quaternion.identity);
        splashClone.SendEvent("OnPlay");
        Destroy(splashClone.gameObject, 3f);
    }
}
