using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilder : MonoBehaviour
{
    [SerializeField] GameObject[] homeObjects;
    public bool isBaseHouse, isFrozen, isActive, movingRight;
    public Rigidbody2D pubRB;
    [SerializeField] BoxCollider2D myCol;
    [SerializeField] Rigidbody2D myRB;
    public float moveSpeed = 0.015f;
    public int objectType = 0;
    int speedAdded = 0;
    bool checkWinCon;

    [SerializeField] AudioSource[] landSounds, perfectSounds;
    [SerializeField] PerfectLandingChecker myLandingChecker;

    [SerializeField] GameObject perfectLineObj;

    [SerializeField] GameObject[] perfectTitleObjs;

    public Transform myCamTransform;

    private void OnEnable()
    {
        checkWinCon = false;
        StartCoroutine(WaitAFrame());
    }

    IEnumerator WaitAFrame()
    {
        yield return new WaitForFixedUpdate();
        BuildHouse();
    }

    void BuildHouse()
    {
        if (!isBaseHouse)
        {
            pubRB = myRB;
            isActive = true;
            isFrozen = true;
        }
        if (!isBaseHouse)
        {
            // set rigidbody freeze posiion and rotation on
            myRB.constraints = RigidbodyConstraints2D.FreezeAll;
            // set box colider off
            myCol.enabled = false;
        }
        else
            myCol.enabled = true;

        speedAdded = 0;
        float normal = 1f;
        float small = 0.5f;
        float large = 2f;

        // determine house size
        // set house object mass
        switch (objectType)
        {
            case 0: // normal
                myRB.mass = 1;
                transform.localScale = new Vector3(normal, normal, normal);
                break;
            case 1: // small
                myRB.mass = 0.25f;
                transform.localScale = new Vector3(small, small, normal);
                break;
            case 2: // large
                myRB.mass = 4;
                transform.localScale = new Vector3(large, large, normal);
                break;
            case 3: // normal width: small height
                myRB.mass = 0.5f;
                transform.localScale = new Vector3(normal, small, normal);
                break;
            case 4: // normal width: large heigth
                myRB.mass = 2;
                transform.localScale = new Vector3(normal, large, normal);
                break;
            case 5: // small width: normal height
                myRB.mass = 0.5f;
                transform.localScale = new Vector3(small, normal, normal);
                break;
            case 6: // small width: large height
                myRB.mass = 1;
                transform.localScale = new Vector3(small, large, normal);
                break;
            case 7: // large width: small height
                myRB.mass = 1;
                transform.localScale = new Vector3(large, small, normal);
                break;
            case 8: // large width, normal height
                myRB.mass = 2;
                transform.localScale = new Vector3(large, normal, normal);
                break;
            default: // normal
                myRB.mass = 1;
                transform.localScale = new Vector3(normal, normal, normal);
                break;
        }

        // turn material on
        homeObjects[0].SetActive(true);
        // determine left object (window, sign, clock, or none)
        int leftObj = Random.Range(0, 4);
        switch (leftObj)
        {
            case 0:
                homeObjects[1].SetActive(true);
                break;
            case 1:
                homeObjects[3].SetActive(true);
                break;
            case 2:
                homeObjects[5].SetActive(true);
                break;
            default:
                break;

        }
        // determine right object (window, sign, clock, or none)
        int rightObj = Random.Range(0, 4);
        switch (rightObj)
        {
            case 0:
                homeObjects[2].SetActive(true);
                break;
            case 1:
                homeObjects[4].SetActive(true);
                break;
            case 2:
                homeObjects[6].SetActive(true);
                break;
            default:
                break;

        }
        // determine left roof object (chimney, acnemometer, or none)
        int leftRoofObj = Random.Range(0, 3);
        switch (rightObj)
        {
            case 0:
                homeObjects[7].SetActive(true);
                break;
            case 1:
                homeObjects[9].SetActive(true);
                break;
            default:
                break;

        }
        // determine right roof object (chimney, acnemometer, or none)
        int rightRoofObj = Random.Range(0, 3);
        switch (rightObj)
        {
            case 0:
                homeObjects[8].SetActive(true);
                break;
            case 1:
                homeObjects[10].SetActive(true);
                break;
            default:
                break;

        }
        // determine left fence
        if (Random.Range(0, 2) == 0)
            homeObjects[11].SetActive(true);
        // determine right fence
        if (Random.Range(0, 2) == 0)
            homeObjects[12].SetActive(true);
        // turn door on
        homeObjects[13].SetActive(true);
        // turn door top on
        homeObjects[14].SetActive(true);
        // determine awning
        if (Random.Range(0, 2) == 0)
            homeObjects[15].SetActive(true);
    }

    private void Update()
    {
        if (InputManager.playerTapped && isFrozen && !GameManager.playerLost && !GameManager.playerWon)
        {
           myCol.enabled = true;
           myRB.constraints = RigidbodyConstraints2D.None;

           transform.localPosition += new Vector3(0, 0.01f, 0);

            StartCoroutine(LandingCheck());

           isFrozen = false;
           InputManager.playerTapped = false;
        }
    }

    IEnumerator LandingCheck()
    {
        yield return new WaitForSeconds(0.05f);
        while (Mathf.Abs(myRB.velocity.y) > 0.01f)
            yield return new WaitForSeconds(0.05f);

        checkWinCon = true;
        GameManager.spawnNewHouse = true;
        if (myLandingChecker.perfectLanding && !GameManager.playerLost)
        {
            Debug.Log("Perfect Landing");

            GameObject tempObj = Instantiate(perfectTitleObjs[GameManager.perfectTrackerCount],
                Vector3.zero, Quaternion.identity, myCamTransform);
            tempObj.transform.position = new Vector3(0, 0, 3);
            tempObj.transform.position = transform.position;

            float objScaler;
            switch (GameManager.perfectTrackerCount)
            {
                case 0:
                    objScaler = 0.3f;
                    break;
                case 1:
                    objScaler = 0.7f;
                    break;
                case 2:
                    objScaler = 1.3f;
                    break;
                case 3:
                    objScaler = 1.5f;
                    break;
                case 4:
                    objScaler = 1.5f;
                    break;
                default:
                    objScaler = 1f;
                    break;
            }
            perfectSounds[GameManager.perfectTrackerCount].Play();

            GameManager.perfectTrackerCount++;

            tempObj.transform.localScale = new Vector3(objScaler, objScaler, objScaler); 
            if (GameManager.perfectTrackerCount > perfectTitleObjs.Length - 1)
                GameManager.perfectTrackerCount = perfectTitleObjs.Length - 1;
            perfectLineObj.SetActive(true);
        }
        else
        {
            GameManager.perfectTrackerCount = 0;
        }
    }

    private void FixedUpdate()
    {
        if (isFrozen)
        {
            // move left and right
            if (movingRight)
            {
                transform.position += new Vector3(moveSpeed, 0, 0);
                if (transform.position.x > 1.7f)
                {
                    if (speedAdded < 3)
                    {
                        Mathf.Min(0.15f, moveSpeed * 1.075f);
                        speedAdded++;
                    }
                    movingRight = false;
                }
            }
            else
            {
                transform.position -= new Vector3(moveSpeed, 0, 0);
                if (transform.position.x < -1.7f)
                {
                    if (speedAdded < 3)
                    {
                        Mathf.Min(0.15f, moveSpeed * 1.075f);
                        speedAdded++;
                    }
                    movingRight = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
            GameManager.finishLineTouched = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish" && myCol.enabled)
            GameManager.finishLineTouched = true;

        if (collision.tag == "Finish" && GameManager.grounded && !GameManager.playerWon && checkWinCon)
        {
            GameManager.staticSoundManager.Play("WinJingle"); // win jingle
            GameManager.playerWon = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && !isBaseHouse && !GameManager.playerLost && !GameManager.playerWon)
        {
            GameManager.staticSoundManager.Play("LoseJingle"); // lose jingle
            GameManager.playerLost = true;
        }
        if (collision.gameObject.tag == "House" && !isBaseHouse && isActive && !GameManager.playerWon)
        {
            PlayLandSound();
            GameManager.grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "House" && !isBaseHouse && isActive && !GameManager.playerWon)
        {
            PlayLandSound();
            GameManager.grounded = false;
        }
    }

    void PlayLandSound()
    {
        int landSoundInt = Random.Range(0, landSounds.Length);

        landSounds[landSoundInt].volume = myRB.mass/4f; // 0.25 = 0.25, 4 = 1
        landSounds[landSoundInt].Play();
    }
}
