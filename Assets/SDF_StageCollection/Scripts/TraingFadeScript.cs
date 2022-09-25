using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TraingFadeScript : MonoBehaviour
{

    [Header("Fade Option")]
    [Tooltip("Neon Fade should be set to true to enable fade effect")]
    public bool neonFading;

    [Tooltip("The higher the fade speed the slower the fade will be")]
    [Range (0f, .1f)]
    public float fadeSpeed;

    float alphaValue = 256f;

    bool isfadeIn;
    bool isfadeOut;

    [Header("Material Color")]
    [Tooltip("Material color will change on game start")]
    public Color32 neoncolor;

    public GameObject fallingLeavesLeft;
    public GameObject fallingLeavesRight;

    [Header("Fadeing GameObjects")]
    [Tooltip("Neon Obj Amount should match the Neon Objs size")]
    public int neonObjAmount;
    public GameObject[] neonObjs = new GameObject[4];




    void Start()
    {
        //neoncolor = new Color32(85, 255, 73, (byte) (alphaValue * 255));
        for (int j = 0; j < neonObjAmount; j++)
        {
            neonObjs[j].GetComponent<Renderer>().material.SetColor("_TintColor", neoncolor);
        }

        if (neonFading)
        {
            alphaValue = 256;
            //StartCoroutine("FadeOut");
        }
    }

    public void LateUpdate()
    {
        if (neonFading)
        {
            neoncolor.a = (byte)(alphaValue * 255);

            if (alphaValue == 150)
            {
                StopCoroutine("FadeOut");
                StartCoroutine("FadeIn");
            }
            else if (alphaValue == 256)
            {
                StopCoroutine("FadeIn");
                StartCoroutine("FadeOut");

            }
        }        
    }




    IEnumerator FadeIn()
    {

        isfadeIn = true;
        isfadeOut = false;

        for (float i = alphaValue; 257 > i; i++)
            {
                alphaValue = i;

                for (int j = 0; j < neonObjAmount; j++)
                {
                    neonObjs[j].GetComponent<Renderer>().material.SetColor("_TintColor", neoncolor);
                }
                yield return new WaitForSeconds(fadeSpeed);
            }

        yield return null;
    }


    IEnumerator FadeOut()   {

        isfadeIn = false;
        isfadeOut = true;

        if(fallingLeavesLeft != null && fallingLeavesRight != null)
        {
            fallingLeavesLeft.SetActive(false);
            fallingLeavesRight.SetActive(false);
        }

        yield return new WaitForSeconds(0f);

        if (fallingLeavesLeft != null && fallingLeavesRight != null)
        {
            fallingLeavesLeft.SetActive(true);
            fallingLeavesRight.SetActive(true);
        }        

        for (float i = alphaValue; 149 < i ; i--)
        {

            alphaValue = i;

            for (int j = 0; j < neonObjAmount; j++)
            {
                neonObjs[j].GetComponent<Renderer>().material.SetColor("_TintColor", neoncolor);
            }
            yield return new WaitForSeconds(fadeSpeed);
        }

        yield return null;
    }
}
