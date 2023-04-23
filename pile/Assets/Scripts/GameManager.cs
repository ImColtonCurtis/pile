using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;  

public class GameManager : MonoBehaviour
{
    public static bool playerStarted = false, playerWon = false, playerLost = false, spawnNewHouse = false, grounded = false, reloadLevel, inLoading, finishLineTouched;
    bool settled = false, fasterLoad, finishChecker, forceTheWin;

    [SerializeField] GameObject houseObj;
    [SerializeField] Transform buildingsTransform;
    HouseBuilder currentHouse;
    GameObject currentObj;
    float heightTracker = -2.445143f, gapDistance, defaultGap;

    [SerializeField] SpriteRenderer fullSquare;
    [SerializeField] Animator shakerAnim;
    [SerializeField] Transform camTrackerTransform;
    Transform targetTransform;
    Vector3 camVelocity = Vector3.zero;

    [SerializeField] Camera myCam;
    float sizeVelocity = 0;
    float targetSize = 5f;
    bool runningSettleCheck = false, spawnedStartingMechanics = false, spawnedWinningMechanics = false, spawnedLosingMechanics = false, reloadingLevel = false;

    [SerializeField] SpriteRenderer mainTitle, continueTitle, retryTitle;
    [SerializeField] TextMeshPro levelText;

    [SerializeField] GameObject finishLineObj;
    int spawnInt;

    [SerializeField] GameObject confettiObj;

    [SerializeField] SpriteRenderer[] soundIcons;

    [SerializeField] SoundManagerLogic mySoundManager;
    public static SoundManagerLogic staticSoundManager;

    [SerializeField] AudioSource mainMenuMusic;

    public static int perfectTrackerCount;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        staticSoundManager = mySoundManager;

        targetTransform = camTrackerTransform;
        levelText.text = "level " + PlayerPrefs.GetInt("CurrentLevel", 1).ToString();

        // boiler plate lit
        playerStarted = false;
        playerWon = false;
        playerLost = false;
        spawnNewHouse = false;
        grounded = false;

        perfectTrackerCount = 0;

        settled = false;
        fasterLoad = false;
        reloadLevel = false; 
        inLoading = false;

        forceTheWin = false;

        runningSettleCheck = false;
        spawnedStartingMechanics = false;
        spawnedWinningMechanics = false;
        spawnedLosingMechanics = false;
        reloadingLevel = false;
        finishChecker = false;

        finishLineTouched = false;

        confettiObj.SetActive(false);

        spawnInt = 0;

        if (PlayerPrefs.GetInt("LoadNewLevel", 1) == 1)
        {
            DetermineLevelLit();
        }
        else
        {
            finishLineObj.transform.position = new Vector3(0, PlayerPrefs.GetFloat("FinishLineHeight", Random.Range(1.95f, 7f)), 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeSROut());
        SpawnHouse();
        SpawnHouse();
    }

    void DetermineLevelLit() // persist between level loads
    {
        // finish line height (persists between levels)
        // dettermine spawn config (4 block pattern?)
        switch (PlayerPrefs.GetInt("CurrentLevel", 1))
        {
            case 1:
                PlayerPrefs.SetFloat("BaseMoveSpeed", 0.015f);
                PlayerPrefs.SetFloat("FinishLineHeight", 1.33f);
                PlayerPrefs.SetInt("Object1", 0);
                PlayerPrefs.SetInt("Object2", 0);
                PlayerPrefs.SetInt("Object3", 0);
                PlayerPrefs.SetInt("Object4", 0);
                break;
            case 2:
                PlayerPrefs.SetFloat("BaseMoveSpeed", 0.03f);
                PlayerPrefs.SetFloat("FinishLineHeight", 1.95f);
                PlayerPrefs.SetInt("Object1", 0);
                PlayerPrefs.SetInt("Object2", 1);
                PlayerPrefs.SetInt("Object3", 0);
                PlayerPrefs.SetInt("Object4", 0);
                break;
            case 3:
                PlayerPrefs.SetFloat("BaseMoveSpeed", 0.02f);
                PlayerPrefs.SetFloat("FinishLineHeight", Random.Range(1.33f, 3f));
                PlayerPrefs.SetInt("Object1", 2);
                PlayerPrefs.SetInt("Object2", 2);
                PlayerPrefs.SetInt("Object3", 2);
                PlayerPrefs.SetInt("Object4", 2);
                break;
            case 4:
                PlayerPrefs.SetFloat("BaseMoveSpeed", 0.025f);
                PlayerPrefs.SetFloat("FinishLineHeight", Random.Range(1.33f, 3f));
                PlayerPrefs.SetInt("Object1", 7);
                PlayerPrefs.SetInt("Object2", 0);
                PlayerPrefs.SetInt("Object3", 7);
                PlayerPrefs.SetInt("Object4", 0);
                break;
            case 5:
                PlayerPrefs.SetFloat("BaseMoveSpeed", 0.0225f);
                PlayerPrefs.SetFloat("FinishLineHeight", Random.Range(1.33f, 2.5f));
                PlayerPrefs.SetInt("Object1", 0);
                PlayerPrefs.SetInt("Object2", 1);
                PlayerPrefs.SetInt("Object3", 0);
                PlayerPrefs.SetInt("Object4", 1);
                break;
            default:
                PlayerPrefs.SetFloat("BaseMoveSpeed", Random.Range(0.015f, 0.05f));
                int firstObj = Random.Range(0, 9);
                if (firstObj == 1 || firstObj == 5 || firstObj == 6)
                    firstObj = 0;
                PlayerPrefs.SetInt("Object1", firstObj);
                PlayerPrefs.SetInt("Object2", Random.Range(0, 9));
                PlayerPrefs.SetInt("Object3", Random.Range(0, 9));
                PlayerPrefs.SetInt("Object4", Random.Range(0, 9));
                PlayerPrefs.SetFloat("FinishLineHeight", Random.Range(1.33f, 3f));
                break;
        }
        finishLineObj.transform.position = new Vector3(0, PlayerPrefs.GetFloat("FinishLineHeight", Random.Range(1.95f, 7f)), 0);
        // determine base level move speed
        PlayerPrefs.SetInt("LoadNewLevel", 0);
    }

    IEnumerator FinishBackUpCheck()
    {
        for (int i = 0; i < 18; i++)
        {
            if (!finishLineTouched)
                break;
            yield return new WaitForSeconds(0.1f);
        }

        if (finishLineTouched && !playerLost)
        {
            forceTheWin = true;
            staticSoundManager.Play("WinJingle"); // win jingle
            playerWon = true;
        }
        else if (!playerLost)
            finishChecker = false;
    }

    IEnumerator PauseSpawner()
    {
        while (finishChecker && !playerWon)
            yield return new WaitForSeconds(0.1f);
        if (!playerWon)
            SpawnHouse();
    }

    // Update is called once per frame
    void Update()
    {
        if (finishLineTouched && !finishChecker)
        {
            StartCoroutine(FinishBackUpCheck());
            finishChecker = true;
        }

        if (reloadLevel)
        {
            fasterLoad = true;
            inLoading = true;
            StartCoroutine(FadeSRIn());
            reloadLevel = false;
        }

        if (spawnNewHouse && settled && !playerWon)
        {
            settled = false;
            if (!playerLost && !playerWon)
            {                
                currentHouse.isActive = false;
                if (finishLineTouched || finishChecker)
                    StartCoroutine(PauseSpawner());
                else
                    SpawnHouse();
                // cam zoom out
                if (targetSize < 7)
                    targetSize += 0.845818f;
            }
            spawnNewHouse = false;
        }
        
        if (grounded && !runningSettleCheck)
        {
            shakerAnim.SetTrigger("Shake");
            StartCoroutine(WaitToSettle());            
        }

        // player started
        if (playerStarted && !spawnedStartingMechanics)
        {
            foreach (SpriteRenderer sprite in soundIcons)
            {
                StartCoroutine(FadeSROutWhite(sprite));
            }

            StartCoroutine(FadeOutAudio(mainMenuMusic));

            StartCoroutine(FadeOutTMP(levelText));
            StartCoroutine(FadeSROutWhite(mainTitle));
            spawnedStartingMechanics = true;
        }

        if (playerWon && !spawnedWinningMechanics)
        {
            if (settled || forceTheWin)
            {
                PlayerPrefs.SetInt("FailedInARow", 0); //

                confettiObj.SetActive(true);
                PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel", 1) + 1);
                PlayerPrefs.SetInt("LoadNewLevel", 1);
                StartCoroutine(FadeSRInWhite(continueTitle));
                Debug.Log("player won");
                spawnedWinningMechanics = true;
            }
        }

        // player lost
        if (playerLost && !spawnedLosingMechanics && !spawnedWinningMechanics && !playerWon)
        {
            PlayerPrefs.SetInt("FailedInARow", PlayerPrefs.GetInt("FailedInARow", 0) + 1); //

            StartCoroutine(FadeSRInWhite(retryTitle));
            Debug.Log("player lost");
            spawnedLosingMechanics = true;
        }

        // (re)load level
        if ((playerWon || playerLost) && InputManager.playerTapped && !reloadingLevel)
        {
            StartCoroutine(FadeSRIn());
            reloadingLevel = true;
        }
    }

    IEnumerator WaitToSettle()
    {
        runningSettleCheck = true;
        yield return new WaitForSecondsRealtime(0.3f);
        while (grounded && currentHouse.pubRB.velocity.sqrMagnitude > 1)
        {
            yield return new WaitForSecondsRealtime(0.2f);
        }

        if (grounded)
        {
            settled = true;
            grounded = false;
        }

        runningSettleCheck = false;
    }
    IEnumerator FadeOutAudio(AudioSource myAudio)
    {
        float timer = 0, totalTime = 24;
        float startingLevel = myAudio.volume;
        while (timer <= totalTime)
        {
            myAudio.volume = Mathf.Lerp(startingLevel, 0, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    private void LateUpdate()
    {
        // camera tracker
        Vector3 targetPosition = targetTransform.TransformPoint(new Vector3(0, 1, 0));
        if (!playerLost && !playerWon)
        {
            // restrain x
            targetPosition = new Vector3(0, Mathf.Max(targetPosition.y, 0), 0);
            camTrackerTransform.position = Vector3.SmoothDamp(camTrackerTransform.position, targetPosition, ref camVelocity, 0.5f);

            // cam size
            myCam.orthographicSize = Mathf.SmoothDamp(myCam.orthographicSize, targetSize, ref sizeVelocity, 0.5f);
        }
    }

    void SpawnHouse()
    {
        currentObj = Instantiate(houseObj, transform.position, Quaternion.identity, buildingsTransform);
        currentHouse = currentObj.GetComponent<HouseBuilder>();
        currentHouse.myCamTransform = transform;

        targetTransform = currentObj.transform;

        if (spawnInt == 0)
            currentHouse.isBaseHouse = true;

        // set spawn type
        switch (spawnInt%4)
        {
            case 0:
                currentHouse.objectType = PlayerPrefs.GetInt("Object1", 0);
                break;
            case 1:
                currentHouse.objectType = PlayerPrefs.GetInt("Object2", 0);
                break;
            case 2:
                currentHouse.objectType = PlayerPrefs.GetInt("Object3", 0);
                break;
            case 3:
                currentHouse.objectType = PlayerPrefs.GetInt("Object4", 0);
                break;
            default:
                Debug.Log("error");
                break;                
        }
        spawnInt++;

        // determine spawn position
        if (Random.Range(0, 2) == 0) // spawned left, moving right
        {
            currentObj.transform.position = new Vector3(-1.7f, heightTracker, 0);
            currentHouse.movingRight = true;
        }
        else
        {
            currentObj.transform.position = new Vector3(1.7f, heightTracker, 0);
            currentHouse.movingRight = false;
        }
        if (currentHouse.isBaseHouse)
            currentObj.transform.position = new Vector3(0, heightTracker, 0);

        // determine spawn height
        defaultGap = 0.89f;
        switch (currentHouse.objectType)
        {
            case 0: // normal
                gapDistance = defaultGap;
                break;
            case 1: // small
                gapDistance = defaultGap - 0.4f;
                break;
            case 2: // large
                gapDistance = defaultGap + 1.1f;
                break;
            case 3: // normal width: small height
                gapDistance = defaultGap - 0.4f;
                break;
            case 4: // normal width: large heigth
                gapDistance = defaultGap + 1.1f;
                break;
            case 5: // small width: normal height
                gapDistance = defaultGap;
                break;
            case 6: // small width: large height
                gapDistance = defaultGap + 1.1f;
                break;
            case 7: // large width: small height
                gapDistance = defaultGap - 0.4f;
                break;
            case 8: // large width, normal height
                gapDistance = defaultGap;
                break;
            default: // normal
                gapDistance = defaultGap;
                break;
        }
        heightTracker += gapDistance;

        // determine starting house move speed
        currentHouse.moveSpeed = PlayerPrefs.GetFloat("BaseMoveSpeed", 0.015f);
    }

    IEnumerator FadeSROut()
    {
        float timer = 0, totalTime = 15;

        fullSquare.color = Color.black;
        yield return new WaitForSecondsRealtime(0.3f);
        while(timer <= totalTime)
        {
            fullSquare.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeSROutWhite(SpriteRenderer mySprite)
    {
        float timer = 0, totalTime = 15;

        if (mySprite.color.a <= 0)
            mySprite.color = Color.white;

        Color startingColor = mySprite.color;
        while (timer <= totalTime)
        {
            mySprite.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeSRIn()
    {
        float timer = 0, totalTime = 15;

        while (timer <= totalTime)
        {
            fullSquare.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
        if (fasterLoad)
        {
            fasterLoad = false;
            yield return new WaitForSecondsRealtime(0.7f);
            inLoading = false;
        }
        else
            yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene(0, LoadSceneMode.Single); // main game
    }

    IEnumerator FadeSRInWhite(SpriteRenderer mySprite)
    {
        float timer = 0, totalTime = 15;

        while (timer <= totalTime)
        {
            mySprite.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeOutTMP(TextMeshPro myTMP)
    {
        float timer = 0, totalTime = 15;

        myTMP.color = Color.white;
        while (timer <= totalTime)
        {
            myTMP.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }
}
