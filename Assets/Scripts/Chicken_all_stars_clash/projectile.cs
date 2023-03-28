using System;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class projectile : MonoBehaviour
{
    [SerializeField] private bool isExplode;
    [SerializeField] private bool isExplosion;
    [SerializeField] private bool savantPotion;
    [SerializeField] private float potionDamage;
    [SerializeField] private bool mageStick;
    [SerializeField] private float stickDamage;
    [SerializeField] private int explosionTime;
    [SerializeField] private GameObject explosion;

    public Rigidbody rb;
    public Player_class playerScript;
    [HideInInspector] public float damage;

    private float saveDamage;

    private void Start() {
        if (isExplode) explosion.SetActive(false);
        playerScript = GetComponentInParent<Player_class>();
        GameObject player = playerScript.gameObject;
        var transform1 = transform;
        transform1.position = player.transform.position;
        transform1.parent = null;
        if (savantPotion) {
            saveDamage = potionDamage;
            rb.AddForce(Vector3.up * 60,ForceMode.Impulse);
            if(playerScript._saveAxisXpositive) rb.AddForce(Vector3.left * 20,ForceMode.Impulse);
            else rb.AddForce(Vector3.right * 20,ForceMode.Impulse);
            potionDamage = Random.Range(potionDamage, potionDamage + 3);
            damage = potionDamage;
            potionDamage = saveDamage;
        }
        if (mageStick) {
            saveDamage = stickDamage;
            rb.AddForce(Vector3.down * 100,ForceMode.Impulse);
            stickDamage = Random.Range(stickDamage, stickDamage + 3);
            damage = stickDamage;
            stickDamage = saveDamage;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("DeathZone")) Destroy(gameObject);
        if (other.gameObject.CompareTag("Ground") && isExplode) {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            explosion.SetActive(true);
            Invoke(nameof(ExplosionEnd),explosionTime);
        }
    }
    void ExplosionEnd() { 
        explosion.SetActive(false);
        Destroy(gameObject); 
    }
}
