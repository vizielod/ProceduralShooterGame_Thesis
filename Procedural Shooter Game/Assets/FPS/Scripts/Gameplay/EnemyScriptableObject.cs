using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;

/// <summary>
/// Scriptableobject that holds the Base Stats for an enemy. These can be modified at object creation time to buff up enemies
/// and to reset their stats if they died or were modified at runtime
/// </summary>
///

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    [System.Serializable] public struct WeaponModule
    {
        public string WeaponName;
        public float Damage;
        public float DelayBetweenShots;
        public float BulletSpreadAngle;
    }
    //Enemy base stats
    public float Health;
    public float MovementSpeed;
    public float Acceleration;
    public float AngularSpeed;
    public float DetectionRange;
    public float AttackRange;

    public List<WeaponModule> Weapons;
    //Enemy projectile stats
    /*public float Damage;
    public float DelayBetweenShots;
    public float BulletSpreadAngle;*/

}
