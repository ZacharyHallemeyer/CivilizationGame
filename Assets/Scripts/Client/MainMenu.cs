﻿using System.Collections;
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

    public GameObject mainMenuFirstButton, gameSelectionHostFirstButton, gameSelectionJoinFirstButton;
    public GameObject optionsMenuFirstButton, statsMenuFirstButton, aboutMenuFirstButton;

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
        SetUpUserNameField();
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
    /// Sets option sliders to player prefs values
    /// </summary>
    public virtual void SetSliderUI()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", .75f);
        soundEffectSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
    }

    /// <summary>
    /// Sets player prefs and audio volume in regard to general volume
    /// </summary>
    public virtual void SetMusicVolumePreference(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        if (AudioManager.instance == null)
            AudioManager.instance = FindObjectOfType<AudioManager>();
        AudioManager.instance.SetMusicVolume();
    }

    /// <summary>
    /// Sets player prefs and audio volume in regard to general volume
    /// </summary>
    public virtual void SetSoundEffectsPreference(float volume)
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        if (AudioManager.instance == null)
            AudioManager.instance = FindObjectOfType<AudioManager>();
        AudioManager.instance.SetSoundEffectVolume();
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
