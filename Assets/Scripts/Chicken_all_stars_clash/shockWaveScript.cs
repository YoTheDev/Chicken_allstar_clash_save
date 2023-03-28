using System;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UnityEngine;

public class shockWaveScript : MonoBehaviour
{
    private Enemy enemy;

    [SerializeField] private float damage;

    private void Start() {
        enemy = FindObjectOfType<Enemy>();
    }

    private void OnTriggerEnter(Collider other) {
        enemy.attackDamageCoast = damage;
        if (other.gameObject.CompareTag("Wall")) {
            Destroy(gameObject);
        }
    }
}
