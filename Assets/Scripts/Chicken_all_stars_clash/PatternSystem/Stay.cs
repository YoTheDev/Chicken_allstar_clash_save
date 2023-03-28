using PatternSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stay", menuName = "ChickenAllStarsClash/InGame/PatternAction/Stay")]
public class Stay : PatternAction {
    
    public float Duration;

    public override void isCollided(Enemy enemy) {
        enemy.Rigidbody.velocity = Vector3.zero;
    }

    public override void isCollidedWall(Enemy enemy) {
        enemy.Rigidbody.velocity = Vector3.zero;
    }

    public override void isCollidedGround(Enemy enemy) {
        return;
    }

    public override float PatternDuration => Duration;
    public override float PatternDamage => 3;

    public override void Do(Enemy enemy) {
        enemy.Rigidbody.velocity = Vector3.zero;
        enemy.camera_script.ShakeDistance = 0.2f;
        enemy.camera_script.ShakeDuration = 1;
    }

    public override bool IsFinished(Enemy enemy) {
        return true;
    }
}