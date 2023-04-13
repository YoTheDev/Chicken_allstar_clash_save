using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Death balloon attack", menuName = "ChickenAllStarsClash/InGame/PatternAction/Death ballon attack")]
public class DeathBallonAttack : WeaponData
{
    private float damageGiven;
    private float scoreGiven;
    
    public float airSimpleScore;    
    public float airSimpleDamage;
    public float deathBalloonAttackDamage;

    public override float DamageData => damageGiven;
    public override float ScoreData => scoreGiven;
    public override float currentAirProjectile { get; set; }
    public override void DoSimple(Player_class player) { throw new System.NotImplementedException(); }

    public override void DoAirSimple(Player_class player) {
        player.balloonCharge.SetActive(true);
        player._rigidbody.velocity = new Vector3(player._rigidbody.velocity.x, 0);
        player._rigidbody.AddForce(Vector3.down * 1.5f,ForceMode.Impulse);
        damageGiven = deathBalloonAttackDamage;
        scoreGiven = airSimpleScore;
    }

    public override void DoBlock(Player_class player) { }
    public override void DoUnBlock(Player_class player) { }

    public override bool SimpleMultipleDamage { get; }

    public override void Interrupt(Player_class player) {
        damageGiven = airSimpleDamage;
    }
}
