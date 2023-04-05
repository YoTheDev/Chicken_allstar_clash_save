using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class Player_select : MonoBehaviour
{
    [SerializeField] private GameObject crochet;
    [SerializeField] private Button readyButton;
    [SerializeField] private List<GameObject> _class = new List<GameObject>();
    [SerializeField] private Game_management playerData;
    [SerializeField] private string playerTag;
    [SerializeField] private string playerUI;

    private float xAxis;
    private int uiIndex;
    private bool validate;
    private PlayerInput playerInput;
    private UI_title ui;

    public List<GameObject> _classObject = new List<GameObject>();

    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        playerData.colorIndex = playerInput.playerIndex;
        playerData.ControllerOrder[playerInput.playerIndex] = playerInput.currentControlScheme[playerInput.playerIndex];
        playerData.Controll[playerInput.playerIndex] = playerInput.currentControlScheme;
        readyButton = GameObject.Find("Ready").GetComponent<Button>();
        ui = FindObjectOfType<UI_title>();
        ui.playerConnected++;
        if (gameObject.CompareTag(playerTag)) {
            _class.Add(GameObject.Find(playerUI+"/Class/Pirate")); 
            _class.Add(GameObject.Find(playerUI+"/Class/Mage"));
            _class.Add(GameObject.Find(playerUI+"/Class/Science")); 
            _class.Add(GameObject.Find(playerUI+"/Class/Thief"));
            crochet = GameObject.Find(playerUI+"/Validation");
            ButtonAdd();
        }
    }

    void ButtonAdd() {
        for (int i = 1; i < _class.Count; i++) _class[i].SetActive(false);
        crochet.SetActive(false);
    }

    void OnMove(InputValue Moving) {
        xAxis = Moving.Get<float>();
        if (validate) return;
        if (xAxis <= -1) {
            _class[uiIndex].SetActive(false);
            _classObject[uiIndex].SetActive(false);
            if (uiIndex <= 0) uiIndex = 3;
            else uiIndex--;
            _class[uiIndex].SetActive(true);
            _classObject[uiIndex].SetActive(true);
        }
        if (xAxis >= 1) {
            _class[uiIndex].SetActive(false);
            _classObject[uiIndex].SetActive(false);
            if (uiIndex >= _class.Count-1) uiIndex = 0;
            else uiIndex++;
            _class[uiIndex].SetActive(true);
            _classObject[uiIndex].SetActive(true);
        }
    }

    void OnJump() {
        if (validate) return;
        playerData._aliveIndex = playerInput.playerIndex;
        playerData._classIndex = uiIndex;
        playerData.PlayerCount();
        crochet.SetActive(true);
        validate = true;
        ui.playerReadyCount++;
    }

    void OnBack() {
        if (ui.ui[2].activeSelf) return;
        if (!validate) return;
        playerData._aliveIndex = playerInput.playerIndex;
        playerData.PlayerLeft();
        crochet.SetActive(false);
        readyButton.interactable = false;
        ui.playerReadyCount--;
        validate = false;
    }
}
