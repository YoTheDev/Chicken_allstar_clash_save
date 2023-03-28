using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData : ScriptableObject {
    
    public abstract float DamageData { get; }
    public abstract float ScoreData { get; }
    public abstract float currentAirProjectile { get; set; }
    public abstract void DoSimple(Player_class player);
    public abstract void DoAirSimple(Player_class player);
    public abstract void DoBlock(Player_class player);
    public abstract void DoUnBlock(Player_class player);
    public abstract bool SimpleMultipleDamage { get; }
    public abstract void Interrupt(Player_class player);
}
