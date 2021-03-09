using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using GoogleMobileAds.Api;

public delegate void OperateFigure(bool side);

public class GameScreen : MonoBehaviour
{
    public GameObject grid;
    public Swipe swipeControls;
    public GameObject leftButton, rightButton, downButton, rotateButton, pausePopUp, fadeButton;
    public Text scoreText;
    public float startFastHorizontalMovementDelat;
    public float nativeAspectRatio;
    public float horizontalDragSpeed;
    public float verticalDragSpeed;
    public float superAccelerationDragSpeed;
    public int[] pointsForDestroyingLines = new int[5];

    private TetrisGrid gridScript;
    private float fastHorizontalMovementDelay;
    private bool rightSidePressed, leftSidePressed;
    private Vector3 mouseMoved;
    private Vector3 lastMousePosition;
    private Vector3 downMousePosition;
    private bool mousePressed = false;
    private bool wasFigureMoved = false;
    private Tweener scoreTweener;
    private int score = 0;
    private int scoreForTween = 0;
    private InterstitialAd interstitial;
    private BannerView bannerView;
    string adUnitId;

    // Start is called before the first frame update
    void Start()
    {
        float aspectRatio = (Screen.width * 1.0f) / Screen.height;
        //scale = aspectRatio / nativeAspectRatio;
        Vector3 localScale = this.transform.localScale;
        //this.transform.localScale = new Vector3(scale * localScale.x, scale * localScale.y);
        Vector3 lastMousePosition = Input.mousePosition;
        mouseMoved = new Vector3();

        Save save = new Save();

        gridScript = grid.GetComponent<TetrisGrid>();
        TweenCallback<int> callb = OnLineDestroy;
        TweenCallback OnGameStartCallb = OnGameStart;
        gridScript.SetOnLinesDestroy(callb);
        gridScript.SetOnGameStart(OnGameStartCallb);
        gridScript.SetOnFigureLand(OnFigureLand);
        ResetVariables(true);

        TweenCallback OnMouseUp1 = OnMouseUp;
        TweenCallback OnMouseDown1 = OnMouseDown;
        fadeButton.GetComponent<FadeButton>().Set(OnMouseDown1, OnMouseUp1);

        horizontalDragSpeed = Screen.width * 0.1f;
        verticalDragSpeed = Screen.height * 0.1f;
        superAccelerationDragSpeed = verticalDragSpeed / 6;

        LoadInterstitial();

        string adUnitId;
        if (Application.platform == RuntimePlatform.Android)
            adUnitId = "ca-app-pub-6874205512651144/9313092129";
        else
            adUnitId = "Unexpected Platform";
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        this.bannerView.LoadAd(request);
    }

    private void OnGameStart()
    {
        LoadGame();
    }

    private Save CreateSaveGameObject()
    {
        Save save = new Save();
        save.score = score;

        Cell[,] cells = gridScript.cells;
        save.SaveGridState(cells);

        save.figurePosition.x = gridScript.currentFigure.transform.localPosition.x;
        save.figurePosition.y = gridScript.currentFigure.transform.localPosition.y;
        save.figureIndex = gridScript.figScript.index;
        save.nextFigureIndex = gridScript.nextFigureIndex;
        save.rotationCount = gridScript.figScript.rotationCount;
        save.figureFallDelay = gridScript.figureFallDelay;
        save.figureFallDelayDecreaseCurrentTime = gridScript.figureFallDelayDecreaseCurrentTime;

        return save;
    }

    private void SaveGame()
    {
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            CreateCubeInCell Create = gridScript.CreateBlockInCell;
            save.SetGridState(Create);

            gridScript.LaunchSavedStartFigure(new Vector3(save.figurePosition.x, save.figurePosition.y), save.figureIndex, save.rotationCount,  save.nextFigureIndex);

            gridScript.figureFallDelay = save.figureFallDelay;
            gridScript.figureFallDelayDecreaseCurrentTime = save.figureFallDelayDecreaseCurrentTime;

            scoreText.text = save.score.ToString();
            score = save.score;
            scoreForTween = score;
        }
        else
        {
            gridScript.LaunchStartFigure();
        }
    }


    void LoadInterstitial()
    {
        string adUnitId;
        if (Application.platform == RuntimePlatform.Android)
            adUnitId = "ca-app-pub-6874205512651144/5373847119";
        else
            adUnitId = "Unexpected Platform";

        this.interstitial = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.OnAdOpening += OnInterstitialOpen;
        this.interstitial.OnAdClosed += OnInterstitialClose;

        this.interstitial.LoadAd(request);
    }

    void ResetVariables(bool _)
    {
        fastHorizontalMovementDelay = 0;
        rightSidePressed = false;
        leftSidePressed = false;

        if (scoreTweener != null) scoreTweener.Kill(true);
        scoreText.text = "0";
        score = 0;
        scoreForTween = 0;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            StartPausePopUp();
    }

    private void OnFigureLand()
    {
        mousePressed = false;
    }


    void OnLineDestroy(int lineCount)
    {
        if (lineCount > 0)
        {
            if (scoreTweener != null) scoreTweener.Kill(true);
            score += pointsForDestroyingLines[lineCount];
            scoreTweener = DOTween.To(x => scoreForTween = (int)x, scoreForTween, score, 1.5f)
                                   .SetEase(Ease.OutCubic)
                                   .SetAutoKill(false);
            scoreTweener.OnUpdate(() => scoreText.text = scoreForTween.ToString());
        }
    }

    private void OnMouseDown()
    {
        mousePressed = true;
        downMousePosition = Input.mousePosition;
        lastMousePosition = downMousePosition;
        //print("Down");
    }

    private void OnMouseUp()
    {
        mousePressed = false;
        mouseMoved = new Vector3();

        Vector3 posDiff = Input.mousePosition - downMousePosition;
        if (!wasFigureMoved && posDiff.magnitude < 0.0001f) RotateFigure(true);
        wasFigureMoved = false;
        //print("UPPPP");
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        if (fastHorizontalMovementDelay > 0) { fastHorizontalMovementDelay -= delta; }

        if (rightSidePressed && fastHorizontalMovementDelay < 0)
        {
            gridScript.MoveFigure(true);
        }
        else if (leftSidePressed && fastHorizontalMovementDelay < 0)
        {
            gridScript.MoveFigure(false);
        }

        if (mousePressed)
        {
            Vector3 posDiff = Input.mousePosition - lastMousePosition;
            if (posDiff.magnitude < 1) { posDiff = new Vector3(); }
            mouseMoved += posDiff;

            if (Mathf.Abs(mouseMoved.x) >= horizontalDragSpeed)
            {
                wasFigureMoved = true;
                int turnsCount = 0;
                bool turnSide = true;

                if (mouseMoved.x > 0)
                {
                    turnsCount = (int)Mathf.Round(mouseMoved.x / horizontalDragSpeed);
                    mouseMoved.x -= horizontalDragSpeed * turnsCount;
                }
                else
                {
                    turnSide = false;
                    turnsCount = (int)Mathf.Round(Mathf.Abs(mouseMoved.x) / horizontalDragSpeed);
                    mouseMoved.x += horizontalDragSpeed * turnsCount;
                }

                for (int i = 0; i < turnsCount; i++)
                {
                    gridScript.MoveFigure(turnSide);
                }

                if (gridScript.CanFigureSpeedBeChanged()) 
                {
                    //SetFigureSpeed(1);
                    mouseMoved.y = 0;
                }

            }
            else if (posDiff.y < -superAccelerationDragSpeed)
            {
                gridScript.ApplyFigureSuperFastFall();
            }
            else if (mouseMoved.y <= -verticalDragSpeed)
            {
                mouseMoved.y = 0;
                gridScript.MoveFigureDownOnOneCell();
            }

            lastMousePosition = Input.mousePosition;
        }
    }

    void MoveFigure(bool side)
    {
        gridScript.MoveFigure(side);
        if (side) { rightSidePressed = true; }
        else { leftSidePressed = true; }
        fastHorizontalMovementDelay = startFastHorizontalMovementDelat;
    }

    void RotateFigure(bool _)
    {
        gridScript.RotateFigure();
    }

    void Restart()
    {
        File.Delete(Application.persistentDataPath + "/gamesave.save");
        ResetVariables(true);
        gridScript.Start();
        gridScript.ClearCells();
        gridScript.SetPause(false);

        if (interstitial.IsLoaded())
            interstitial.Show();
        LoadInterstitial();
    }

    public void StartPausePopUp()
    {
        PausePopUp pauseScript = pausePopUp.GetComponent<PausePopUp>();
        TweenCallback OnNewGameButtonClick1 = Restart;
        TweenCallback OnContinueButtonClick1 = OnContinueButtonClick;
        TweenCallback OnMainMenuButtonClick1 = OnMainMenuButtonClick;
        pausePopUp.SetActive(true);
        pauseScript.Start(OnNewGameButtonClick1, OnContinueButtonClick1, OnMainMenuButtonClick1);
        gridScript.SetPause(true);

        SaveGame();
    }

    public void OnContinueButtonClick()
    {
        gridScript.SetPause(false);
    }

    public void OnMainMenuButtonClick()
    {
        this.bannerView.Destroy();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void OnInterstitialOpen(object sender, EventArgs args)
    {
        gridScript.SetPause(true);
    }

    public void OnInterstitialClose(object sender, EventArgs args)
    {
        gridScript.SetPause(false);
    }
}
