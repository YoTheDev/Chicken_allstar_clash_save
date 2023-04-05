using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_title : MonoBehaviour {
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject grayOpacity;
    [SerializeField] private List<GameObject> pressStartText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startMissionAnyway;
    [SerializeField] private Button noMissionAnyway;

    public List<GameObject> ui = new List<GameObject>();
    public List<GameObject> playerui = new List<GameObject>();
    public List<GameObject> player = new List<GameObject>();
    public List<GameObject> playerSpawner = new List<GameObject>();
    public List<string> missionScene = new List<string>();
    public int playerConnected;
    public int playerReadyCount;

    private bool _attackPressed;
    private bool _jumpPressed;
    private bool _playOneShot;
    private bool _playerReadyBool;
    private int _playerIndex;
    private PlayerInputManager _inputManager;
    private PlayerInput _playerInput;

    void OnAttackHold() { _attackPressed = !_attackPressed; }
    void OnJumpHold() { _jumpPressed = !_jumpPressed; }
    
    private void Start() {
        startButton.interactable = false;
        optionButton.interactable = false;
        startMissionAnyway.interactable = false;
        noMissionAnyway.interactable = false;
        _playOneShot = true;
        titleScreen.SetActive(true);
        startButton.onClick.AddListener(() => PlayerJoining());
        readyButton.onClick.AddListener(() => MorePlayer());
        startMissionAnyway.onClick.AddListener(() => InvokeStartMission());
        readyButton.interactable = false;
        _inputManager = GetComponent<PlayerInputManager>();
        _playerInput = GetComponent<PlayerInput>();
        for (int i = 0; i < ui.Count; i++) ui[i].SetActive(false);
        for (int i = 0; i < playerui.Count; i++) playerui[i].SetActive(false);
    }

    private void Update() {
        if (titleScreen.activeSelf && _jumpPressed && _attackPressed) {
            titleScreen.SetActive(false);
            ui[0].SetActive(true);
            Invoke(nameof(ButtonCooldown),0.4f);
            _playerInput.enabled = false;
        }
        if (!ui[1].activeSelf || ui[2].activeSelf) return;
        if (playerReadyCount == playerConnected && _playerReadyBool) {
            readyButton.interactable = true;
            readyButton.Select();
        }
        else readyButton.interactable = false;
    }
    
    public void PlayerJoining() {
        if (_playOneShot) {
            _inputManager.EnableJoining();
            _playOneShot = false;
        }
    }

    public void MorePlayer() {
        if (_playerIndex != player.Count) {
            _inputManager.DisableJoining();
            ui[2].SetActive(true);
            Invoke(nameof(ButtonCooldown),0.4f);
            grayOpacity.SetActive(true);
        }
        else {
            InvokeStartMission();
            ui[2].SetActive(false); grayOpacity.SetActive(false);
        }
    }

    public void InvokeStartMission() {
        readyButton.interactable = false;
        Invoke(nameof(StartMission),1);
    }
    public void StartMission() { SceneManager.LoadScene(missionScene[0]); }

    void JoinedPlayer() {
        if(_playerIndex > playerSpawner.Count || _playerIndex > player.Count) return;
        player[_playerIndex].transform.position = playerSpawner[_playerIndex].transform.position;
        playerui[_playerIndex].SetActive(true);
        pressStartText[_playerIndex].SetActive(false);
        _playerReadyBool = true;
        _playerIndex++;
        if (_playerIndex > 3) return;
        _inputManager.playerPrefab = player[_playerIndex];
    }

    void OnPlayerJoined() {
        if (!ui[1].activeSelf) return;
        JoinedPlayer();
    }

    void ButtonCooldown()
    {
        if (ui[0].activeSelf)
        {
            startButton.interactable = true;
            optionButton.interactable = true;
        }
        if (ui[2].activeSelf)
        {
            startMissionAnyway.interactable = true;
            noMissionAnyway.interactable = true;
        }
    }
}
