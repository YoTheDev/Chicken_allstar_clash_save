using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Metal fist",menuName = "ChickenAllStarsClash/InGame/Weapon/Metal fist")]
public class MetalFist : WeaponData {
    private float damageGiven;
    private float scoreGiven;

    public float simpleDamage;
    public float airSimpleDamage;
    public float shieldDamage;
    public float simpleScore;
    public float airSimpleScore;
    public float shieldScore;
    public float saveDamage;
    public float reloadTime;
    public float airReloadTime;
    public bool doMultipleDamage;

    public override float DamageData => damageGiven;
    public override float ScoreData => scoreGiven;
    public override float currentAirProjectile { get; set; }

    public override void DoSimple(Player_class player) {
        player.reloadTimer = reloadTime;
        scoreGiven = simpleScore;
        player._attack = true;
        saveDamage = simpleDamage;
        player.attackBox.SetActive(true);
        simpleDamage = Random.Range(simpleDamage, simpleDamage + 3);
        damageGiven = simpleDamage;
        simpleDamage = saveDamage;
    }

    public override void DoAirSimple(Player_class player) {
        player.reloadTimer = airReloadTime;
        player._canAirAttack = false;
        saveDamage = airSimpleDamage;
        player.attack2Box.SetActive(true);
        player._doubleJump = false;
        player._rigidbody.velocity = Vector3.zero;
        if (player._axisX != 0) {
            if (!player._saveAxisXpositive) {
                player._rigidbody.AddForce(Vector3.right * 30,ForceMode.Impulse);
            }
            else {
                player._rigidbody.AddForce(Vector3.left * 30,ForceMode.Impulse);
            }
        }
        player._airAttack = true;
        player._rigidbody.AddForce(Vector3.up * player.airattackjumpHeight,ForceMode.Impulse);
        airSimpleDamage = Random.Range(airSimpleDamage, airSimpleDamage + 3);
        scoreGiven = airSimpleScore;
        damageGiven = airSimpleDamage;
        airSimpleDamage = saveDamage;
    }

    public override void DoBlock(Player_class player) {
        player.gameObject.layer = LayerMask.NameToLayer("IgnoreCollision");
        scoreGiven = shieldScore;
        saveDamage = shieldDamage;
        player.Shield.SetActive(true);
        shieldDamage = Random.Range(shieldDamage, shieldDamage + 3);
        scoreGiven = shieldScore;
        damageGiven = shieldDamage;
        shieldDamage = saveDamage;
    }

    public override void DoUnBlock(Player_class player) {
        player.gameObject.layer = LayerMask.NameToLayer("Player_one");
        player.Shield.SetActive(false);
    }

    public override bool SimpleMultipleDamage => doMultipleDamage;

    public override void Interrupt(Player_class player) {
        player.attackBox.SetActive(false); player.attack2Box.SetActive(false);
    }
}
