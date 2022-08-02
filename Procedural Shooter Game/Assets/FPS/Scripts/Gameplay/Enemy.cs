using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.FPS.Gameplay
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyScriptableObject EnemyScriptableObject;
        [SerializeField] private Health Health;
        [SerializeField] private NavMeshAgent NavMeshAgent;

        [SerializeField] public DetectionModule DetectionModule;

        [SerializeField] private List<WeaponController> WeaponControllers;

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

            foreach (var weaponController in WeaponControllers)
            {
                for (int i = 0; i < EnemyScriptableObject.Weapons.Count; i++)
                {
                    if (weaponController.WeaponName.Equals(EnemyScriptableObject.Weapons[i].WeaponName))
                    {
                        weaponController.ProjectilePrefab.GetComponent<ProjectileStandard>().Damage = EnemyScriptableObject.Weapons[i].Damage;

                        weaponController.DelayBetweenShots = EnemyScriptableObject.Weapons[i].DelayBetweenShots;
                        weaponController.BulletSpreadAngle = EnemyScriptableObject.Weapons[i].BulletSpreadAngle;

                        if (weaponController.WeaponName.Equals("Module_Railgun_R") ||
                            weaponController.WeaponName.Equals("Module_Railgun_L"))
                        {
                            weaponController.ProjectilePrefab.GetComponent<ProjectileChargeParameters>().Damage.Min = EnemyScriptableObject.Weapons[i].Damage;
                            weaponController.ProjectilePrefab.GetComponent<ProjectileChargeParameters>().Damage.Max = EnemyScriptableObject.Weapons[i].Damage;
                        }
                    }
                }
                /*weaponController.ProjectilePrefab.GetComponent<ProjectileStandard>().Damage = EnemyScriptableObject.Damage;

                weaponController.DelayBetweenShots = EnemyScriptableObject.DelayBetweenShots;
                weaponController.BulletSpreadAngle = EnemyScriptableObject.BulletSpreadAngle;*/
            }
        }
    }
}
