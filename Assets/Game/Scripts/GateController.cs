using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject gateVisual;
    public Collider gateCollider;
    public float openGateDuration;
    public float openTargetY = -1.5f;

    IEnumerator OpenGateAnimation()
    {
        float currentOpenTime = 0;
        Vector3 startPos = gateVisual.transform.position;
        Vector3 targetPos = gateVisual.transform.position + Vector3.up * openTargetY;
        while (currentOpenTime < openGateDuration)
        {
            currentOpenTime += Time.deltaTime;
            gateVisual.transform.position = Vector3.Lerp(startPos, targetPos, currentOpenTime/openGateDuration);
            yield return null;
        }
        gateCollider.enabled = false;
    }

    public void OpenGate()
    {
        StartCoroutine(OpenGateAnimation());
    }

}
