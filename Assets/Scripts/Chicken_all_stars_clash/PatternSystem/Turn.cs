using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turn", menuName = "ChickenAllStarsClash/InGame/PatternAction/Turn")]
public class Turn : PatternAction
{
    public Vector3 JumpPower;
    public float Duration;
    public float damage;

    private Vector3 knockbackDirection;

    public override void isCollidedWall(Enemy enemy) {
        return;
    }

    public override void isCollidedGround(Enemy enemy) {
        return;
    }

    public override float PatternDuration => Duration;
    public override float PatternDamage => damage;

    public override void Do(Enemy enemy) {
        enemy.Rigidbody.velocity = Vector3.zero;
        enemy.Rigidbody.AddForce(enemy.transform.TransformDirection(JumpPower), ForceMode.Impulse);
        enemy.Turn = true;
        enemy.animator.SetBool("grounded",false);
        enemy.camera_script.ShakeDistance = 0.2f;
        enemy.camera_script.ShakeDuration = 1;
        enemy.Turn_wave.SetActive(true);
    }

    public override bool IsFinished(Enemy enemy) {
        return enemy.Rigidbody.velocity.y <= 0;
    }

    public override void isCollided(Enemy enemy) {
        enemy.Rigidbody.velocity = Vector3.zero;
    }
}
