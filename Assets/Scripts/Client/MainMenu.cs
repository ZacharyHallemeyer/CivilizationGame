using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


public class MainMenu : MonoBehaviour
{
    public Text hostUserNameFieldText, joinUserNameFieldText;
    public Text hostUserNamePlaceholderText, joinUserNamePlaceholderText;
    public Text hostPortPlaceholderText, hostPortFieldText;
    public Text joinPortPlaceholderText, joinPortFieldText;
    public Text ipFieldText, ipFieldPlaceholderText;

    public Slider musicSlider, soundEffectSlider;

    // World Gen Options
    public Slider biomeSlider, foodSlider, woodSlider, metalSlider, neutralCitySlider, obstacleSlider, xSizeSlider, zSizeSlider,
                  waterLevelSlider;
    public TextMeshProUGUI biomeText, foodText, woodText, metalText, neutralCityText, obstacleText, xSizeText, zSizeText,
                           waterLevelText;

    private void Start()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        SetUpUserNameField();
        SetUpIPField();
        SetSliderUI();
    }

    #region Set Up

    public void SetUpPortField()
    {
        if (PlayerPrefs.GetInt("Port", -1) == -1)
            PlayerPrefs.SetInt("Port", 26950);
        hostPortPlaceholderText.text = PlayerPrefs.GetInt("Port").ToString();
        joinPortPlaceholderText.text = PlayerPrefs.GetInt("Port").ToString();

    }

    public void SetUpUserNameField()
    {
        // Create random username if no username is found
        if (PlayerPrefs.GetString("Username", "NULLNULL") == "NULLNULL")
            PlayerPrefs.SetString("Username", RandomUsernameGenerator(15));
        hostUserNamePlaceholderText.text = PlayerPrefs.GetString("Username", "NULLNULL");
        joinUserNamePlaceholderText.text = PlayerPrefs.GetString("Username", "NULLNULL");
    }

    public void SetUpIPField()
    {
        // If no previous IP found then use local host IP
        if (PlayerPrefs.GetString("HostIP", "NULL") == "NULL")
            PlayerPrefs.SetString("HostIP", "127.0.0.1");
        ipFieldPlaceholderText.text = PlayerPrefs.GetString("HostIP");
    }

    /// <summary>
    /// Sets option sliders to player prefs values
    /// </summary>
    public virtual void SetSliderUI()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", .75f);
        soundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);

        biomeSlider.value = PlayerPrefs.GetInt("WorleyPoints", 250);
        foodSlider.value = PlayerPrefs.GetInt("FoodTiles", 15);
        woodSlider.value = PlayerPrefs.GetInt("WoodTiles", 15);
        metalSlider.value = PlayerPrefs.GetInt("MetalTiles", 15);
        obstacleSlider.value = PlayerPrefs.GetInt("Obstacles", 15);
        neutralCitySlider.value = PlayerPrefs.GetInt("NeutralCities", 15);
        xSizeSlider.value = PlayerPrefs.GetInt("XSize", 25);
        zSizeSlider.value = PlayerPrefs.GetInt("XZize", 25);
        waterLevelSlider.value = PlayerPrefs.GetFloat("WaterLevel", .25f);

        biomeText.text = PlayerPrefs.GetInt("WorleyPoints", 250).ToString();
        foodText.text = PlayerPrefs.GetInt("FoodTiles", 15).ToString();
        woodText.text = PlayerPrefs.GetInt("WoodTiles", 15).ToString();
        metalText.text = PlayerPrefs.GetInt("MetalTiles", 15).ToString();
        obstacleText.text = PlayerPrefs.GetInt("Obstacles", 15).ToString();
        neutralCityText.text = PlayerPrefs.GetInt("NeutralCities", 15).ToString();
        xSizeText.text = PlayerPrefs.GetInt("XSize", 25).ToString();
        zSizeText.text = PlayerPrefs.GetInt("XZize", 25).ToString();
        waterLevelText.text = PlayerPrefs.GetFloat("WaterLevel", .25f).ToString("n2");
    }

    #endregion

    #region Actions

    /// <summary>
    /// Changes the port to connect to
    /// </summary>
    /// <param name="_portString"> Port to connect to </param>
    public void ChangePort(string _portString)
    {
        int _portNum = int.Parse(_portString);
        PlayerPrefs.SetInt("Port", _portNum);
        hostPortPlaceholderText.text = _portString;
        joinPortPlaceholderText.text = _portString;
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    /// <summary>
    /// Changes player username
    /// </summary>
    /// <param name="_username"> new username </param>
    public void ChangeUsername(string _username)
    {
        PlayerPrefs.SetString("Username", _username);
        hostUserNameFieldText.text = _username;
        joinUserNameFieldText.text = _username;
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    /// <summary>
    /// Changes the ip to connect to
    /// </summary>
    /// <param name="_ip"></param>
    public void ChangeHostIP(string _ip)
    {
        PlayerPrefs.SetString("HostIP", _ip);
        SetUpIPField();
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    /// <summary>
    /// Switch game mode on server side
    /// </summary>
    /// <param name="_gameModeName"></param>
    public void ChangeServerGameMode(string _gameModeName)
    {
        AsyncOperation _asyncOperation = SceneManager.LoadSceneAsync(_gameModeName, LoadSceneMode.Additive);
        if (!_asyncOperation.isDone)
        {
            StartCoroutine(WaitAndSetActiveScene(_asyncOperation, _gameModeName));
        }
        else
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_gameModeName));
            ChangeClientScene("Client" + _gameModeName.Substring(6));
        }
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    /// <summary>
    /// When the scene is loaded, set it to active scene
    /// </summary>
    /// <param name="_asyncOperation"> the operation being completed </param>
    /// <param name="_gameModeName"> game mode to switch client side to </param>
    /// <returns></returns>
    public IEnumerator WaitAndSetActiveScene(AsyncOperation _asyncOperation, string _gameModeName)
    {
        yield return new WaitForEndOfFrame();
        if(_asyncOperation.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_gameModeName));
            ChangeClientScene("Client" + _gameModeName.Substring(6));
        }
        else
            StartCoroutine(WaitAndSetActiveScene(_asyncOperation, _gameModeName));
    }

    public void ChangeClientScene(string _gameModeName)
    {
        AsyncOperation _asyncOperation = SceneManager.LoadSceneAsync(_gameModeName, LoadSceneMode.Additive);

        if (!_asyncOperation.isDone)
            StartCoroutine(WaitAndUnloadScene(_asyncOperation, "ClientMainMenu"));
        else
            UnloadScene("ClientMainMenu");
    }

    private IEnumerator WaitAndUnloadScene(AsyncOperation __asyncOperation, string _sceneToUnload)
    {
        yield return new WaitForEndOfFrame();
        if (__asyncOperation.isDone)
        {
            if (SceneManager.GetActiveScene().name.Substring(6) == "Domination")
            {
                if (WorldGeneratorSS.instance != null)
                    WorldGeneratorSS.instance.GenerateWorld();
            }
            UnloadScene(_sceneToUnload);
        }
        else
            StartCoroutine(WaitAndUnloadScene(__asyncOperation, _sceneToUnload));
    }

    public void UnloadScene(string _sceneName)
    {
        SceneManager.UnloadSceneAsync(_sceneName, UnloadSceneOptions.None);
    }

    public void Quit()
    {
        AudioManager.instance.Play(Constants.uiClickAudio);
        Application.Quit();
    }

    #endregion

    #region Sliders

    /// <summary>
    /// Sets player prefs and audio volume in regard to general volume
    /// </summary>
    public virtual void SetMusicVolumePreference(float _volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", _volume);
        if (AudioManager.instance == null)
            AudioManager.instance = FindObjectOfType<AudioManager>();
        AudioManager.instance.SetMusicVolume();
    }

    /// <summary>
    /// Sets player prefs and audio volume in regard to general volume
    /// </summary>
    public virtual void SetSoundEffectsPreference(float _volume)
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", _volume);
        if (AudioManager.instance == null)
            AudioManager.instance = FindObjectOfType<AudioManager>();
        AudioManager.instance.SetSoundEffectVolume();
    }

    public virtual void SetBiome(float _value)
    {
        PlayerPrefs.SetInt("WorleyPoints", (int) _value);
        biomeText.text = ((int)_value).ToString();
    }

    public virtual void SetFood(float _value)
    {
        PlayerPrefs.SetInt("FoodTiles", (int) _value);
        foodText.text = ((int)_value).ToString();
    }

    public virtual void SetWood(float _value)
    {
        PlayerPrefs.SetInt("WoodTiles", (int) _value);
        woodText.text = ((int)_value).ToString();
    }

    public virtual void SetMetal(float _value)
    {
        PlayerPrefs.SetInt("MetalTiles", (int) _value);
        metalText.text = ((int)_value).ToString();
    }

    public virtual void SetObstacles(float _value)
    {
        PlayerPrefs.SetInt("ObstacleTiles", (int) _value);
        obstacleText.text = ((int)_value).ToString();
    }

    public virtual void SetNeutralCities(float _value)
    {
        PlayerPrefs.SetInt("NeutralCities", (int) _value);
        neutralCityText.text = ((int)_value).ToString();
    }

    public virtual void SetXSize(float _value)
    {
        PlayerPrefs.SetInt("XSize", (int) _value);
        xSizeText.text = ((int)_value).ToString();
    }

    public virtual void SetZSize(float _value)
    {
        PlayerPrefs.SetInt("ZSize", (int) _value);
        zSizeText.text = ((int)_value).ToString();
    }

    public virtual void SetWaterLevel(float _value)
    {
        PlayerPrefs.SetFloat("WaterLevel", _value);
        waterLevelText.text = _value.ToString("n2");
    }


    #endregion

    #region Tools

    /// <summary>
    /// Generates random username
    /// </summary>
    /// <param name="_characterCount"> username character count </param>
    /// <returns></returns>
    public string RandomUsernameGenerator(int _characterCount)
    {
        string _username = "";
        for(int i = 0; i < _characterCount; i++)
        {
            _username += (char)Random.Range(97, 123);
        }
        return _username;
    }

    /// <summary>
    /// Plays ui click sound
    /// </summary>
    public void PlayUISound()
    {
        AudioManager.instance.Play(Constants.uiClickAudio);
    }

    #endregion
}
