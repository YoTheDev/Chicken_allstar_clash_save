using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PatternSystem;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "new Game manager",menuName = "ChickenAllStarsClash/InGame/Game_manager")]
public class Game_management : ScriptableObject {
    [SerializeField] private List<GameObject> playerClass;

    public List<ColorList> ListOfColorMaterial;
    public List<ChoosenColorList> ListOfColorChoosen;
    public List<bool> playerAlive;
    public List<GameObject> playerClassChoosen;
    public List<int> ControllerOrder;
    public int _aliveIndex;
    public int _classIndex;
    public int colorIndex;
    public List<string> Controll;
    public bool victory;
    public bool gameOver;
    public int countPlayer;

    public void PlayerCount() {
        playerClassChoosen[_aliveIndex] = playerClass[_classIndex];
        ListOfColorChoosen[_aliveIndex].MaterialOne = ListOfColorMaterial[colorIndex].MaterialOne;
        ListOfColorChoosen[_aliveIndex].MaterialTwo = ListOfColorMaterial[colorIndex].MaterialTwo;
        countPlayer++;
    }
    
    public void PlayerLeft() {
        playerClassChoosen[_aliveIndex] = null;
        ListOfColorChoosen[_aliveIndex].MaterialOne = null;
        ListOfColorChoosen[_aliveIndex].MaterialTwo = null;
        countPlayer--;
    }
    
    public void PlayerDead() {
        playerAlive[_aliveIndex] = false;
        foreach (bool currentPlayer in playerAlive) {
            if (currentPlayer) return;
        }
        GameOver();
    }

    public void GameOver() { gameOver = true; }
    public void Victory() { victory = true; }

    public void OnEnable() {
        colorIndex = 0;
        for (int i = 0; i < playerClassChoosen.Count; i++) {
            playerClassChoosen[i] = null;
        }
        for (int i = 0; i < playerAlive.Count; i++) {
            playerAlive[i] = false;
        }
        for (int i = 0; i < ControllerOrder.Count; i++) {
            ControllerOrder[i] = "\0"[0];
        }
        for (int i = 0; i < Controll.Count; i++) {
            Controll[i] = null;
        }
        countPlayer = 0;
        for (int i = 0; i < ListOfColorChoosen.Count; i++)
        {
            ListOfColorChoosen[i].MaterialOne = null;
            ListOfColorChoosen[i].MaterialTwo = null;
        }
    }

    public void ResetPlayerAlive() {
        for (int i = 0; i < playerAlive.Count; i++) {
            playerAlive[i] = false;
        }
    }
}

[Serializable]
public class ColorList {
    public Material MaterialOne;
    public Material MaterialTwo;
}

[Serializable]
public class ChoosenColorList {
    public Material MaterialOne;
    public Material MaterialTwo;
}
