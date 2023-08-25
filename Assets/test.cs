using System;
using System.Collections;
using UnityEngine;

public class test : MonoBehaviour
{
    private IEnumerator Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            StartCoroutine(LerpGrow(transform.GetChild(i).GetChild(0)));
            yield return new WaitForSeconds(.04f);
        }
    }

    private int avgFrameRate;

    private void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
    }

    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 25;
        GUI.Label(new Rect(55, 55, 100, 25), avgFrameRate.ToString(), style);
    }

    private IEnumerator LerpGrow(Transform growTransform)
    {
        var matIns = growTransform.GetComponent<Renderer>().material;
        while (true)
        {
            float elapsedTime = 0F;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
                matIns.SetFloat("_Grow", elapsedTime);
                Debug.Log("AAA "  + elapsedTime);
            }
        }
    }
}
