using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] sprites;
    [SerializeField] HouseBuilder myHouse;

    bool switched = false, fadedOut = false, active = false;
    int chosenSprite;

    private void Awake()
    {
        switched = false;
        fadedOut = false;
        active = false;
    }

    private void OnEnable()
    {
        chosenSprite = Random.Range(0, sprites.Length);
        for (int i = 0; i < sprites.Length; i++)
        {
            if (i != chosenSprite)
                sprites[i].enabled = false;
            else
            {
                sprites[i].enabled = true;
                if (!myHouse.isBaseHouse)
                {
                    switched = false;
                    active = true;
                    sprites[i].color = new Color(1, 1, 1, 0.7f);
                }
                else
                {
                    switched = true;
                    active = false;
                    sprites[i].color = new Color(1, 1, 1, 1);
                }
            }
        }
    }

    private void Update()
    {
        if (!myHouse.isFrozen && !switched && !myHouse.isBaseHouse)
        {
            active = false;
            sprites[chosenSprite].color = new Color(1, 1, 1, 1);
            switched = true;
        }
        if ((GameManager.playerLost || GameManager.playerWon) && !fadedOut && active)
        {
            StartCoroutine(FadeOut(sprites[chosenSprite]));
            fadedOut = true;
        }
    }

    IEnumerator FadeOut(SpriteRenderer mySR)
    {
        float timer = 0, totalTime = 15;

        while (timer <= totalTime)
        {
            mySR.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }
}
