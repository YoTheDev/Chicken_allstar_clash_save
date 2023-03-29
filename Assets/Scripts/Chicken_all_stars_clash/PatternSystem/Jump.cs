using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[CreateAssetMenu(fileName = "New Jump", menuName = "ChickenAllStarsClash/InGame/PatternAction/Jump")]
public class Jump : PatternAction
{
    public float Duration;
    public float Damage;
    public Vector3 JumpPower;

    public override void Do(Enemy enemy) {
        enemy.Rigidbody.velocity = Vector3.zero;
        enemy.Rigidbody.AddForce(enemy.transform.TransformDirection(JumpPower), ForceMode.Impulse);
        enemy.camera_script.ShakeDistance = 2;
        enemy.camera_script.ShakeDuration = 5;
        enemy.Jump = true;
        enemy.animator.SetBool("grounded",false);
    }
    
    public override bool IsFinished(Enemy enemy) {
        return enemy.Rigidbody.velocity.y <= 0;
    }

    public override void isCollided(Enemy enemy)
    {
        enemy.Rigidbody.velocity = Vector3.zero;
    }

    public override void isCollidedWall(Enemy enemy)
    {
        return;
    }

    public override void isCollidedGround(Enemy enemy)
    {
        return;
    }

    public override float PatternDuration => Duration;
    public override float PatternDamage => Damage;
}
