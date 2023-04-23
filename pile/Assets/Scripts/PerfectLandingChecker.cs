using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectLandingChecker : MonoBehaviour
{
    public bool perfectLanding;

    private void OnEnable()
    {
        perfectLanding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tag == "BotCol" && collision.tag == "TopCol")
        {
            perfectLanding = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (tag == "BotCol" && collision.gameObject.tag == "TopCol")
        {
            perfectLanding = false;
        }
    }

    private void LateUpdate()
    {
        transform.localPosition = Vector3.zero;
    }
}
