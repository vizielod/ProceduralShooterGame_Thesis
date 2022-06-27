using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyScriptableObject EnemyScriptableObject;
    [SerializeField] private Health Health;
    [SerializeField] private NavMeshAgent NavMeshAgent;

    [SerializeField] private DetectionModule DetectionModule;

    [SerializeField] private WeaponController WeaponController;

    private void Awake()
    {
        SetupEnemyFromConfiguration();
    }
    
    public virtual void SetupEnemyFromConfiguration()
    {
        Health.MaxHealth = EnemyScriptableObject.Health;
        NavMeshAgent.speed = EnemyScriptableObject.MovementSpeed;
        NavMeshAgent.angularSpeed = EnemyScriptableObject.AngularSpeed;
        NavMeshAgent.acceleration = EnemyScriptableObject.Acceleration;
        DetectionModule.DetectionRange = EnemyScriptableObject.DetectionRange;
        DetectionModule.AttackRange = EnemyScriptableObject.AttackRange;

        WeaponController.ProjectilePrefab.GetComponent<ProjectileStandard>().Damage = EnemyScriptableObject.Damage;

        WeaponController.DelayBetweenShots = EnemyScriptableObject.DelayBetweenShots;
        WeaponController.BulletSpreadAngle = EnemyScriptableObject.BulletSpreadAngle;
    }
}
