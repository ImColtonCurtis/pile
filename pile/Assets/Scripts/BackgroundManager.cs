using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] Transform bgTransform;
    [SerializeField] GameObject[] backgrounds;

    int themeType = 0;
    float moveSpeed = 0;

    GameObject bg1, bg2;
    bool bg2InBack = true;

    // Start is called before the first frame update
    void Awake()
    {
        themeType = Random.Range(0, 8);
        bg1 = Instantiate(backgrounds[themeType], transform.position, Quaternion.identity, bgTransform);
        bg2 = Instantiate(backgrounds[themeType], transform.position, Quaternion.identity, bgTransform);

        // bg movespeed
        moveSpeed = Random.Range(-0.01f, 0.01f);

        if (moveSpeed < 0)
            bg2.transform.localPosition = bg1.transform.localPosition + new Vector3(30.72f, 0, 0);
        else
            bg2.transform.localPosition = bg1.transform.localPosition - new Vector3(30.72f, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // move backgrounds
        bgTransform.transform.localPosition += new Vector3(moveSpeed, 0, 0);

        if (moveSpeed < 0)
        {
            if (bg2InBack)
            {
                if (bg2.transform.position.x <= 0)
                {
                    bg1.transform.localPosition = bg2.transform.localPosition + new Vector3(30.72f, 0, 0);
                    bg2InBack = false;
                }
            }
            else
            {
                if (bg1.transform.position.x <= 0)
                {
                    bg2.transform.localPosition = bg1.transform.localPosition + new Vector3(30.72f, 0, 0);
                    bg2InBack = true;
                }
            }
        }
        else
        {
            if (bg2InBack)
            {
                if (bg2.transform.position.x >= 0)
                {
                    bg1.transform.localPosition = bg2.transform.localPosition - new Vector3(30.72f, 0, 0);
                    bg2InBack = false;
                }
            }
            else
            {
                if (bg1.transform.position.x >= 0)
                {
                    bg2.transform.localPosition = bg1.transform.localPosition - new Vector3(30.72f, 0, 0);
                    bg2InBack = true;
                }
            }
        }
    }
}
