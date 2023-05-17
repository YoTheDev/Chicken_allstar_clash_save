using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PatternSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Player_management : MonoBehaviour {
    public Game_management GameManagement;
    public Enemy enemy;
    public List<Transform> playerSpawnerArena;
    public float startTimeRemain;
    public PlayerInputManager inputManager;
    public List<GameObject> life;
    public GameObject ready;
    public GameObject go;
    public GameObject victoryUI;
    public GameObject defeatUI;
    public GameObject pauseUI;
    public Button retryButton;
    public Button backButton;
    public Button defeatRetryButton;
    public Button defeatBackButton;
    public Button PauseResumeButton;
    public List<Player_class> playerClass;
    public TextMeshProUGUI score;
    public int timeBonus;
    
    [HideInInspector] public bool ActivateInput;
    [HideInInspector] public float scoreEarned;
    
    private float time;
    private int secondUpdate = 1;
    private bool playOneShot;

    private void Start() {
        Physics.gravity = new Vector3(0, -180f, 0);
        GameManagement.victory = false;
        GameManagement.gameOver = false;
        retryButton.onClick.AddListener(() => Retry());
        backButton.onClick.AddListener(() => BackToTitle());
        defeatRetryButton.onClick.AddListener(() => Retry());
        defeatBackButton.onClick.AddListener(() => BackToTitle());
        PauseResumeButton.onClick.AddListener(() => ResumeGame());
        for (int i = 0; i < life.Count; i++) {
            life[i].SetActive(false);
        }
        for (int i = 0; i < GameManagement.playerClassChoosen.Count; i++) {
            GameObject thisPlayer = GameManagement.playerClassChoosen[i];
            string controller = GameManagement.Controll[i];
            int controllerOrder = GameManagement.ControllerOrder[i];
            InputDevice inputDeviceId = InputSystem.GetDeviceById(controllerOrder);
            if(thisPlayer == null) continue;
            playerClass[i] = thisPlayer.GetComponent<Player_class>();
            thisPlayer.name = "Player_" + i;
            inputManager.playerPrefab = thisPlayer;
            inputManager.JoinPlayer(i,i,controller,inputDeviceId);
            thisPlayer = GameObject.Find("Player_" + i + "(Clone)");
            thisPlayer.transform.position = playerSpawnerArena[i].transform.position;
            life[i].SetActive(true);
            Material[] RoosterBase =
            {
                GameManagement.ListOfColorChoosen[i].MaterialTwo, GameManagement.ListOfColorChoosen[i].MaterialOne,
                GameManagement.ListOfColorChoosen[i].AnimaPlaceHolder, GameManagement.ListOfColorChoosen[i].PirateShaderMaterial
            };
            thisPlayer.GetComponentInChildren<SkinnedMeshRenderer>().materials = RoosterBase;
        }
        switch (GameManagement.countPlayer) {
            case 1:
                enemy.maxHealth = 15000;
                break;
            case 2:
                enemy.maxHealth = 20000;
                break;
            case 3:
                enemy.maxHealth = 23000;
                break;
            case 4:
                enemy.maxHealth = 25000;
                break;
        }
    }

    void Update() {
        if (GameManagement.victory && !playOneShot) {
            victoryUI.SetActive(true);
            switch (GameManagement.countPlayer) {
                case 1:
                    scoreEarned += 500;
                    break;
                case 2:
                    scoreEarned += 375;
                    break;
                case 3:
                    scoreEarned += 200;
                    break;
                case 4:
                    scoreEarned += 100;
                    break;
            }
            scoreEarned += timeBonus;
            Invoke(nameof(ShowScore),1);
            backButton.Select();
            playOneShot = true;
        }
        if (startTimeRemain >= -1) {
            startTimeRemain -= Time.deltaTime;
        }
        else {
            time += Time.deltaTime;
            if (time >= secondUpdate && !GameManagement.victory && !GameManagement.gameOver) {
                secondUpdate++;
                timeBonus -= 100;
                playOneShot = false;
            }
            go.SetActive(false);
        }

        if (GameManagement.gameOver && !playOneShot) {
            defeatUI.SetActive(true);
            defeatRetryButton.Select();
            playOneShot = true;
        }
        if (!(startTimeRemain <= 0) || !(startTimeRemain > -1) || playOneShot) return;
        for (int i = 0; i < playerClass.Count; i++) {
            if(playerClass[i] == null) continue;
            ActivateInput = true;
        }
        go.SetActive(true);
        ready.SetActive(false);
        enemy.EnemyStart();
        playOneShot = true;
    }

    private void ShowScore() {
        score.text = scoreEarned.ToString();
    }
    public void Retry() {
        Scene thisScene = SceneManager.GetActiveScene();
        GameManagement.ResetPlayerAlive();
        Time.timeScale = 1;
        SceneManager.LoadScene(thisScene.name);
    }

    public void BackToTitle() {
        SceneManager.LoadScene("Island_outside");
        Time.timeScale = 1;
        GameManagement.OnEnable();
    }

    public void ResumeGame()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }
}
