using System;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private int collideCount;
    private bool ForcedTurn;

    private void OnTriggerEnter(Collider other) {
        if (enemy.target.Count <= 0) return;
        collideCount++;
        if (other.gameObject == enemy.target[enemy._rngPlayer]) ForcedTurn = false;
    }

    private void OnTriggerExit(Collider other) {
        if (enemy.target.Count <= 0) return;
        collideCount--;
        if (collideCount <= 0 || other.gameObject == enemy.target[enemy._rngPlayer]) ForcedTurn = true;
    }

    private void Update() {
        if(!ForcedTurn && enemy._afterAction != 2) return;
        enemy._afterAction = 3;
        ForcedTurn = false;
    }
}
