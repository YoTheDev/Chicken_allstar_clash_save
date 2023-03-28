using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public List<GameObject> UiObject = new List<GameObject>();
    public List<Button> UiButton = new List<Button>();
    public List<GameObject> PlayerSpawner = new List<GameObject>();
    public List<GameObject> PlayerUI = new List<GameObject>();
    public List<UI_manager> UiAction = new List<UI_manager>();
    public bool _attackPressed;
    public bool _jumpPressed;

    private int ButtonAction;

    void OnAttackHold() {
        _attackPressed = !_attackPressed;
    }
    void OnJumpHold() {
        _jumpPressed = !_jumpPressed;
    }
    void OnBack() {
        UiAction[ButtonAction].Back(this);
    }
    private void Start() {
        for (int i = 0; i < UiAction.Count; i++) {
            UiButton[i].onClick.AddListener(() => ButtonDown());
        }
    }
    private void Update() {
        if (_jumpPressed && _attackPressed && UiObject[0].activeSelf) {
            UiObject[0].SetActive(false);
                UiObject[1].SetActive(true);
        }
    }
    void ButtonDown() {
        Debug.Log("hello");
    }
}
