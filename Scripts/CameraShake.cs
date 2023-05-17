using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator DoCameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;

        float timePassed = 0f;

        while(timePassed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            timePassed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPos;
        originalPos = Vector3.zero;
    }



}
