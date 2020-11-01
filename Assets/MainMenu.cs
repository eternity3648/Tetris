using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public GameObject newGameButton;
    public GameObject continueGameButton;
    public GameObject camera;
    public Text text;
    private InterstitialAd interstitial;

    // Start is called before the first frame update
    void Start()
    {

        MobileAds.Initialize(initStatus => { });

        List<string> deviceIds = new List<string>();
        deviceIds.Add("14A008DCDEBB64EFE258EE71E0835FA1");
        RequestConfiguration requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(deviceIds)
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        text.text = "Raul";

        string adUnitId;
        if (Application.platform == RuntimePlatform.Android)
            adUnitId = "ca-app-pub-6874205512651144/5373847119";
        else
            adUnitId = "Unexpected Platform";

        this.interstitial = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.

        this.interstitial.OnAdClosed += OnInterstitialClose;
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;

        this.interstitial.LoadAd(request);

        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
            continueGameButton.GetComponent<Button>().interactable = true;

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowInterstitialAndStartNewGame()
    {
        if (this.interstitial.IsLoaded() && Application.platform == RuntimePlatform.Android)
            this.interstitial.Show();
        else
            StartNewGame();
    }

    public void ShowInterstitialAndLoadSavedGame()
    {
        if (this.interstitial.IsLoaded() && Application.platform == RuntimePlatform.Android)
            this.interstitial.Show();
        else
            LoadGame();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void StartNewGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
            File.Delete(Application.persistentDataPath + "/gamesave.save");

        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        LoadGame();
    }
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        text.GetComponent<Text>().text = "HandleAdLoaded event received";
    }

    public void OnInterstitialClose(object sender, EventArgs args)
    {
        LoadGame();
    }


}
