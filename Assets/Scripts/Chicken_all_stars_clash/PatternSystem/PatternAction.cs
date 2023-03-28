using PatternSystem;
using UnityEngine;

public abstract class PatternAction : ScriptableObject {

    
    public abstract void Do(Enemy enemy);
    public abstract bool IsFinished(Enemy enemy);
    public abstract void isCollided(Enemy enemy);
    public abstract void isCollidedWall(Enemy enemy);
    public abstract void isCollidedGround(Enemy enemy);
    public abstract float PatternDuration { get; }
    public abstract float PatternDamage { get; }

}
