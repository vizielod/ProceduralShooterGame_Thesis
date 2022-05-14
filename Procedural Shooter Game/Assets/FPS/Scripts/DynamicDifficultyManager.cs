using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;

public class DynamicDifficultyManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private PlayerWeaponsManager _playerWeaponsManager;
    
    private WeaponController _weaponController;

    [SerializeField] private float PlayerAccuracy = .0f;

    [SerializeField] private float playerTotalShotCount = .0f; //all the shots the player performed

    [SerializeField] private float playerTotalHitCount = .0f; //all the actual enemy hits

    [SerializeField] private float range = 10.0f;
    
    [SerializeField] private int enemyCountInRange = 0;
    
    [SerializeField] private int healthPickupsCountInRange = 0;

    public List<EnemyController> EnemyControllers = new List<EnemyController>();
    
    public List<GameObject> HealthPickups = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {

        _playerWeaponsManager.OnAddedWeapon += OnAddedWeapon;

        foreach (var enemyController in EnemyControllers)
        {
            enemyController.PlayerRange = range;
            
            enemyController.onDamaged += OnEnemyHit;
            enemyController.onDie += OnEnemyDeath;
            enemyController.onDetectedTarget += OnEnemyDetectedPlayer;
            enemyController.onLostTarget += OnEnemyLostPlayer;
            enemyController.onHealthSpawned += OnHealthSpawned;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (_weaponController == null) return;

        _weaponController.OnShoot -= OnShoot;
        _weaponController = null;
        
        foreach (var enemyController in EnemyControllers)
        {
            enemyController.onDamaged -= OnEnemyHit;
            enemyController.onDie -= OnEnemyDeath;
            enemyController.onDetectedTarget -= OnEnemyDetectedPlayer;
            enemyController.onLostTarget -= OnEnemyLostPlayer;
            enemyController.onHealthSpawned -= OnHealthSpawned;
        }

        foreach (var healthPickup in HealthPickups)
        {
            healthPickup.GetComponent<HealthPickup>().onHealthPicked -= OnHealthPicked;
        }
        
        EnemyControllers.Clear();
        HealthPickups.Clear();
    }

    private void OnHealthSpawned(GameObject LootPrefab)
    {
        HealthPickup healthPickupComponent = LootPrefab.GetComponent<HealthPickup>();
        healthPickupComponent.onHealthPicked += OnHealthPicked;
        
        HealthPickups.Add(LootPrefab);
        if (Vector3.Distance(LootPrefab.transform.position, Player.transform.position) < range)
        {
            healthPickupsCountInRange++;
        }
    }

    private void OnHealthPicked(GameObject LootPrefab)
    {
        HealthPickup healthPickupComponent = LootPrefab.GetComponent<HealthPickup>();
        healthPickupComponent.onHealthPicked -= OnHealthPicked;
        HealthPickups.Remove(LootPrefab);
        healthPickupsCountInRange--;
        //Debug.Log("On health picked fired");
    }

    private void OnEnemyDetectedPlayer()
    {
        enemyCountInRange++;
    }

    private void OnEnemyLostPlayer()
    {
        enemyCountInRange--;
    }
    private void OnEnemyHit()
    {
        playerTotalHitCount++;
    }

    private void OnEnemyDeath(EnemyController enemyController)
    {
        if (enemyController.DetectionModule.HadKnownTarget)
        {
            enemyCountInRange--;
        }
        if(enemyController != null)
            EnemyControllers.Remove(enemyController);
    }
    private void OnAddedWeapon(WeaponController weaponInstance, int i)
    {
        _weaponController = weaponInstance;
        _weaponController.OnShoot += OnShoot;
    }
    private void OnShoot()
    {
        Debug.Log("Shooooooting");
        playerTotalShotCount++;
        CalculatePlayerAccuracy();
    }

    private void CalculatePlayerAccuracy()
    {
        PlayerAccuracy = playerTotalHitCount / playerTotalShotCount;
        //Debug.Log("Player Accuracy: " + PlayerAccuracy);
    }

    private void OnDrawGizmos()
    {
        // Draw DDA area around player. This range is used to calculate enemy count for adjusting difficulty
        Handles.color = Color.green;
        Vector3 _centre = new Vector3(Player.transform.position.x, Player.transform.position.y + 1f,
            Player.transform.position.z);
        Handles.DrawWireDisc(_centre, Vector3.up, range);
    }
}
