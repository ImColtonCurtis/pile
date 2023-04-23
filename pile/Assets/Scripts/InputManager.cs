using System.Collections;
using System.Collections.Generic;
using Unity.Services.Mediation.Samples;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static bool playerTapped = false;

    [SerializeField] GameObject noIcon;

    [SerializeField] Animator soundAnim;

    private void Awake()
    {
        playerTapped = false;

        if (PlayerPrefs.GetInt("SoundStatus", 1) == 1)
        {
            noIcon.SetActive(false);
            AudioListener.volume = 1;
        }
        else
        {
            noIcon.SetActive(true);
            AudioListener.volume = 0;
        }
    }

    void OnTouchDown(Vector3 point)
    {
        if (!GameManager.inLoading)
        {
            if (ShowAds.poppedUp)
            {
                if (point.x <= 0)
                    ShowAds.shouldShowRewardedAd = true;
                else
                    ShowAds.dontShow = true;
            }
            else if (ShowAds.skipPoppedUp)
            {
                if (point.x <= 0)
                    ShowAds.shouldShowRewardedAd = true;
                else
                    ShowAds.dontShow = true;
            }
            else
            {
                if (!GameManager.playerStarted && point.x <= -0.85f && point.y <= -3.4f) // bottom left button clicked
                {
                    if (PlayerPrefs.GetInt("SoundStatus", 1) == 1)
                    {
                        PlayerPrefs.SetInt("SoundStatus", 0);
                        noIcon.SetActive(true);
                        AudioListener.volume = 0;
                    }
                    else
                    {
                        PlayerPrefs.SetInt("SoundStatus", 1);
                        noIcon.SetActive(false);
                        AudioListener.volume = 1;
                    }
                    soundAnim.SetTrigger("Blob");
                }
                else
                {
                    if (!GameManager.playerLost && !GameManager.playerWon)
                        GameManager.staticSoundManager.Play("Plop" + Random.Range(1, 5)); // plop sound

                    if (!GameManager.playerStarted)
                        GameManager.playerStarted = true;
                    playerTapped = true;
                }
            }
        }
    }
}
