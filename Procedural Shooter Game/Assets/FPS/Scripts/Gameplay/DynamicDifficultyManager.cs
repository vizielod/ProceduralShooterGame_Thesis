using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.FPS.AI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Unity.FPS.Gameplay
{
    public class DynamicDifficultyManager : MonoBehaviour
    {
        public ObjectiveExtractData objectiveExtractData;
        public DynamicDifficultySettingsSO DynamicDifficultySettings;
        private DynamicDifficultyType currentDynamicDifficulty = DynamicDifficultyType.Easy;

        [Header("Debug Difficulty Settings")]
        public GameObject DebugUI;
        public TextMeshProUGUI DDAText;
        public TextMeshProUGUI MinDiffBoundaryText;
        public TextMeshProUGUI MaxDiffBoundaryText;
        public TextMeshProUGUI DifficultyGaugeText;
        public TextMeshProUGUI DynamicDifficultyText;
        public TextMeshProUGUI StaticDifficultyText;
        private bool isDebugUIActive = false;
        
        [Header("Difficulty Settings")]
        public bool useDDA = true;
        public StaticDifficultyType difficulty;
        //public DynamicDifficultyType currentDynamicDifficulty = DynamicDifficultyType.Easy;

        [Header("Player performance tracking")]
        public float HardDifficultyGaugeTimer = 0f;
        public float MediumToHardDifficultyGaugeTimer = 0f;
        public float EasyToMediumDifficultyGaugeTimer = 0f;
        public float EasyDifficultyGaugeTimer = 0f;
        public float timeSpentInDownload = 0f;
        
        public float NormalizedHardDifficultyGaugeTimer = 0f;
        public float NormalizedMediumToHardDifficultyGaugeTimer = 0f;
        public float NormalizedEasyToMediumDifficultyGaugeTimer = 0f;
        public float NormalizedEasyDifficultyGaugeTimer = 0f;

        public Dictionary<DynamicDifficultyType, float> NormalizedPlayerPerformanceDictionary = new Dictionary<DynamicDifficultyType, float>();
        


        [Header("Enemy types")]
        /*[SerializeField] private GameObject TankPrefab;
        [SerializeField] private GameObject SoldierPrefab;
        [SerializeField] private GameObject AssassinPrefab;*/
        [SerializeField]
        private GameObject EnemyPrefab;

        [SerializeField] private Transform MinEnemySpawnPostion;
        [SerializeField] private Transform MaxEnemySpawnPostion;

        [SerializeField] public GameObject Player;
        [SerializeField] private PlayerWeaponsManager _playerWeaponsManager;

        [Tooltip("Total enemy hit / Total shots")] [Range(0, 1)] [SerializeField]
        private float PlayerAccuracy = .0f;

        [SerializeField] private float playerTotalShotCount = .0f; //all the shots the player performed

        [SerializeField] private float playerTotalHitCount = .0f; //all the actual enemy hits

        [SerializeField] private float EnemyInRange = 10.0f;
        [SerializeField] private float HealthPickupRange = 10.0f;

        [SerializeField] private float enemyCountInRange = 0;
        private int AssassinInRange = 0;
        [SerializeField] private float AssassinMultiplier = 1.5f;
        private int SoldierInRange = 0;
        [SerializeField] private float SoldierMultiplier = 1.25f;
        private int TankInRange = 0;
        [SerializeField] private float TankMultiplier = 1f;

        [SerializeField] private int healthPickupsCountInRange = 0;

        private WeaponController _weaponController;

        public List<EnemyController> EnemyControllers = new List<EnemyController>();

        public List<GameObject> HealthPickups = new List<GameObject>();
        
        [Range(0, 5)] public float AdjustmentTimerFactor = 1f;

        [Tooltip("0 means Easiest, 1 means Hardest, 0.5 means Medium difficulty")] [Range(0, 1)]
        public float EstimatedDifficulty = 0.5f;

        [Tooltip("starts from (1 - EstimatedDifficulty) Then it always catches up to the temp difficulty gauge")]
        [Range(0, 1)]
        /*
         * Small gauge = Difficulty Experienced is High (Hard) -> decrease the tension
         * High gauge = Difficulty Experienced is Low (Easy) -> increase the tension
         */
        public float DifficultyGauge = 0.5f;

        [Range(0, 1)] private float _tempDifficultyGauge;
        [Range(0, 1)] private float MinDifficultyBoundary = 0f;
        [Range(0, 1)] private float MaxDifficultyBoundary = 1f;
        [Range(0, 1)] public float _tempEstimatedReverseDiff, previousEstimatedReverseDiff;
        [Range(0, 1)] public float _tempMinDifficultyBoundary = 0f;
        [Range(0, 1)] public float _tempMaxDifficultyBoundary = 1f;

        public float DiffGaugeRecalcPeriod = 3f; //Calc new Difficulty Gauge after every 3 sec

        public float BoundaryStepSize = 0.05f;
        public float timer = 0f;
        //Difficulty weights of different core variables
        [Header("Difficulty weights of core variables")]
        public float playerHealthDifficultyFactor;

        public float enemyCountDifficultyFactor;
        public float healthPickupsCountDifficultyFactor;
        public float playerAccuracyDifficultyFactor;

        public float difficultyVariableCount = 4.0f;

        public float playerHealthScaledForEstimatedDifficulty;
        public float playerAccuracyScaledForEstimatedDifficulty;
        public float enemyCountScaledForEstimatedDifficulty;
        public float healthPickupsCountScaledForEstimatedDifficulty;


        public float updateBoundariesPeriod = 2f, updateBoundariesTimer;

        public float originalBoundaryDistance;

        private bool minWasAdjusted = false, maxWasAdjusted = false;
        private float minAdjustedCount = 0, maxAdjustedCount = 0;

        private float previousTempMinValue, previousTempMaxValue;
        private float newTempMinValue, newTempMaxValue;
        private float newMinValue, newMaxValue;
        private float newTempDifficultyGauge;
        private float newTempEstimatedReverseDifficulty;

        private enum EnemyType
        {
            Tank = 0,
            Soldier = 1,
            Assasin = 2
        }

        private void Awake()
        {
            /*int difficultyIdx = PlayerPrefs.GetInt("difficulty");
            difficulty = (StaticDifficultyType) difficultyIdx;
            useDDA = PlayerPrefs.GetInt("useDDA") == 0 ? false : true;*/
        }

        // Start is called before the first frame update
        void Start()
        {

            timer = 0f;

            _playerWeaponsManager.OnAddedWeapon += OnAddedWeapon;

            foreach (var enemyController in EnemyControllers)
            {
                enemyController.PlayerRange = EnemyInRange;

                enemyController.onDamaged += OnEnemyHit;
                enemyController.onDie += OnEnemyDeath;
                enemyController.onDetectedTarget += OnEnemyDetectedPlayer;
                enemyController.onLostTarget += OnEnemyLostPlayer;
                enemyController.onHealthSpawned += OnHealthSpawned;
            }

            //Debug.Log("Error Function Test: " + ErrorFunction.TestErf());

            if (useDDA)
            {
                //EXPERIMENTING with DDA

                float difficultyVariableWeight = 1 / difficultyVariableCount;
                playerHealthDifficultyFactor = difficultyVariableWeight + 0.2f;
                enemyCountDifficultyFactor = difficultyVariableWeight + 0.1f;
                healthPickupsCountDifficultyFactor = difficultyVariableWeight - 0.15f;
                playerAccuracyDifficultyFactor = difficultyVariableWeight - 0.15f;

                EstimatedDifficulty = 0.5f;

                DifficultyGauge = 1 - EstimatedDifficulty;
                _tempDifficultyGauge = 1 - EstimatedDifficulty;
                _tempEstimatedReverseDiff = 1 - EstimatedDifficulty;
                previousEstimatedReverseDiff = 1 - EstimatedDifficulty;

                _tempMinDifficultyBoundary = _tempEstimatedReverseDiff - BoundaryStepSize <= 0
                    ? 0
                    : _tempEstimatedReverseDiff - BoundaryStepSize;
                _tempMaxDifficultyBoundary = _tempEstimatedReverseDiff + BoundaryStepSize >= 1
                    ? 1
                    : _tempEstimatedReverseDiff + BoundaryStepSize;
                newTempMinValue = _tempMinDifficultyBoundary;
                newTempMaxValue = _tempMaxDifficultyBoundary;

                MinDifficultyBoundary = 0.5f;
                MaxDifficultyBoundary = 0.5f;

                originalBoundaryDistance = MaxDifficultyBoundary - MinDifficultyBoundary;
            }
            else //No DDA, use Static difficulty
            {
                switch (difficulty)
                {
                    case StaticDifficultyType.Hard:
                    {
                        //Debug.Log(DynamicDifficultyType.Hard);
                        DifficultyGauge = 0.85f;
                        break;
                    }
                    case StaticDifficultyType.Medium:
                    {
                        //Debug.Log(DynamicDifficultyType.EasyToMedium);
                        DifficultyGauge = 0.5f;
                        break;
                    }
                    case StaticDifficultyType.Easy:
                    {
                        //Debug.Log(DynamicDifficultyType.EasyToMedium);
                        DifficultyGauge = 0.15f;
                        break;
                    }
                    default:
                        break;
                }
            }

        }

        private float updateTempBoundariesTimer = 0f, catchupTempBoundariesTimer = 0f;
        private bool adjustBoundaries = false;
        private float maxTargetValue, minTargetValue;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                isDebugUIActive = !isDebugUIActive;
                DebugUI.SetActive(isDebugUIActive);
            }

            UpdateDebugUI();
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SpawnEnemyAtRandomLocation(MinEnemySpawnPostion.position, MaxEnemySpawnPostion.position);
            }

            if (useDDA)
            {
                CalculateDifficulty();
                StartCoroutine(TempEstimatedReverseDifficultyCoroutine(_tempEstimatedReverseDiff,
                    newTempEstimatedReverseDifficulty, 1.5f / AdjustmentTimerFactor));

                updateTempBoundariesTimer += Time.deltaTime;
                if (updateTempBoundariesTimer > 2f / AdjustmentTimerFactor)
                {
                    CalculateTempBoundaries();
                    //AdjustTempBoundaries();
                    if (maxWasAdjusted)
                    {
                        StartCoroutine(TempMaxBoundaryCoroutine(_tempMaxDifficultyBoundary, newTempMaxValue,
                            1.5f / AdjustmentTimerFactor));
                    }

                    if (minWasAdjusted)
                    {
                        StartCoroutine(TempMinBoundaryCoroutine(_tempMinDifficultyBoundary, newTempMinValue,
                            1.5f / AdjustmentTimerFactor));
                    }

                    StartCoroutine(PreviousEstimatedReverseDiffCourutine(previousEstimatedReverseDiff,
                        _tempEstimatedReverseDiff, 1.5f / AdjustmentTimerFactor));
                    updateTempBoundariesTimer = 0;
                }

                catchupTempBoundariesTimer += Time.deltaTime;
                if (catchupTempBoundariesTimer > 2f / AdjustmentTimerFactor)
                {
                    AdjustTempBoundaries();
                    //or maybe do something like if !maxWasAdjusted && maxNeedsToBeAdjusted
                    if (maxWasAdjusted)
                    {
                        StartCoroutine(TempMaxBoundaryCoroutine(_tempMaxDifficultyBoundary, newTempMaxValue,
                            1.5f / AdjustmentTimerFactor));
                    }

                    if (minWasAdjusted)
                    {
                        StartCoroutine(TempMinBoundaryCoroutine(_tempMinDifficultyBoundary, newTempMinValue,
                            1.5f / AdjustmentTimerFactor));
                    }

                    catchupTempBoundariesTimer = 0;
                }

                /*updateBoundariesTimer += Time.deltaTime;
                if (updateBoundariesTimer > updateBoundariesPeriod / AdjustmentTimerFactor)
                {
                    AdjustBoundaries();
                    StartCoroutine(AdjustMaxBoundaryCoroutine(MaxDifficultyBoundary, newMaxValue, 1.5f / AdjustmentTimerFactor));
                    StartCoroutine(AdjustMinBoundaryCoroutine(MinDifficultyBoundary, newMinValue, 1.5f / AdjustmentTimerFactor));
                    updateBoundariesTimer = 0f;
                }

                timer += Time.deltaTime;
                if (timer > DiffGaugeRecalcPeriod / AdjustmentTimerFactor)
                {
                    CalculateGauge();
                    StartCoroutine(CalculateGaugeCoroutine(_tempDifficultyGauge, newTempDifficultyGauge, 1f / AdjustmentTimerFactor));
                    timer = 0f;
                }*/

                AdjustDifficultyGauge();
            }
        }

        private void UpdateDebugUI()
        {
            if (isDebugUIActive)
            {
                DDAText.text = useDDA ? "ON" : "OFF";
                MinDiffBoundaryText.text = _tempMinDifficultyBoundary.ToString("F2");
                MaxDiffBoundaryText.text = _tempMaxDifficultyBoundary.ToString("F2");
                DifficultyGaugeText.text = DifficultyGauge.ToString("F2");
                DynamicDifficultyText.text = GetCurrentDifficulty().ToString();
                StaticDifficultyText.text = difficulty.ToString();

            }
            /*public GameObject DDAText;
            public Text MinDiffBoundaryText;
            public Text MaxDiffBoundaryText;
            public Text DifficultyGaugeText;
            public Text DynamicDifficultyText;
            public Text StaticDifficultyText;*/
        }
        
        public DynamicDifficultyType GetCurrentDifficulty()
        {
            float currentDifficultyGauge = DifficultyGauge;

            for (int j = 0; j < DynamicDifficultySettings.DifficultyTypeByGaugeValueList.Count; j++)
            {
                if (j != DynamicDifficultySettings.DifficultyTypeByGaugeValueList.Count - 1)
                {
                    if (currentDifficultyGauge >=
                        DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j].MinDifficultyGauge &&
                        currentDifficultyGauge < DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j]
                            .MaxDifficultyGauge)
                    {
                        currentDynamicDifficulty = DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j]
                            .dynamicDifficulty;
                    }
                }
                else //This is needed so if the Gauge is at 1 it also takes it into account
                {
                    if (currentDifficultyGauge >=
                        DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j].MinDifficultyGauge &&
                        currentDifficultyGauge <= DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j]
                            .MaxDifficultyGauge)
                    {
                        currentDynamicDifficulty = DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j]
                            .dynamicDifficulty;
                    }
                }
            }

            return currentDynamicDifficulty;
        }

        public void CalculateNormalizedPlayerPerformance()
        {
            NormalizedHardDifficultyGaugeTimer =
                HardDifficultyGaugeTimer / timeSpentInDownload;
            NormalizedPlayerPerformanceDictionary.Add(DynamicDifficultyType.Hard, NormalizedHardDifficultyGaugeTimer);
            NormalizedMediumToHardDifficultyGaugeTimer =
                MediumToHardDifficultyGaugeTimer / timeSpentInDownload;
            NormalizedPlayerPerformanceDictionary.Add(DynamicDifficultyType.MediumToHard, NormalizedMediumToHardDifficultyGaugeTimer);
            NormalizedEasyToMediumDifficultyGaugeTimer =
                EasyToMediumDifficultyGaugeTimer / timeSpentInDownload;
            NormalizedPlayerPerformanceDictionary.Add(DynamicDifficultyType.EasyToMedium, NormalizedEasyToMediumDifficultyGaugeTimer);
            NormalizedEasyDifficultyGaugeTimer =
                EasyDifficultyGaugeTimer / timeSpentInDownload;
            NormalizedPlayerPerformanceDictionary.Add(DynamicDifficultyType.Easy, NormalizedEasyDifficultyGaugeTimer);
            
        }

        /*IEnumerator TestCourutine(float from, float to, float duration)
        {
            float timeStep = 0;
            while (timeStep <= duration)
            {
                timeStep = timeStep + Time.deltaTime;
                float step = Mathf.Clamp01(timeStep / duration);
                testValue = Mathf.Lerp(from, to, step);
                yield return null;
            }
        }*/

        private void CalculateDifficulty()
        {
            ScalePlayerHealth(0.4f, 0.8f);
            ScaleEnemyCount(5);
            ScaleHealthPickupsCount(10);
            ScalePlayerAccuracy(0.3f, 0.7f);

            EstimatedDifficulty = playerHealthDifficultyFactor * playerHealthScaledForEstimatedDifficulty +
                                  enemyCountDifficultyFactor * enemyCountScaledForEstimatedDifficulty +
                                  healthPickupsCountDifficultyFactor * healthPickupsCountScaledForEstimatedDifficulty +
                                  playerAccuracyDifficultyFactor * playerAccuracyScaledForEstimatedDifficulty;

            //_tempEstimatedReverseDiff = 1 - EstimatedDifficulty;
            newTempEstimatedReverseDifficulty = 1 - EstimatedDifficulty;
        }

        private IEnumerator TempEstimatedReverseDifficultyCoroutine(float from, float to, float duration)
        {
            float timeStep = 0f;

            float _from = from;
            while (timeStep <= duration)
            {
                timeStep = timeStep + Time.deltaTime;
                float step = Mathf.Clamp01(timeStep / duration);
                _tempEstimatedReverseDiff = Mathf.Lerp(_from, to, step);
                yield return null;
            }
        }

        private IEnumerator PreviousEstimatedReverseDiffCourutine(float from, float to, float duration)
        {
            float timeStep = 0f;

            float _from = from;
            while (timeStep <= duration)
            {
                timeStep = timeStep + Time.deltaTime;
                float step = Mathf.Clamp01(timeStep / duration);
                previousEstimatedReverseDiff = Mathf.Lerp(_from, to, step);
                yield return null;
            }
        }

        private IEnumerator TempMaxBoundaryCoroutine(float from, float to, float duration)
        {
            float timeStep = 0f;

            float _from = from;
            while (timeStep <= duration)
            {
                timeStep = timeStep + Time.deltaTime;
                float step = Mathf.Clamp01(timeStep / duration);
                _tempMaxDifficultyBoundary = Mathf.Lerp(_from, to, step);
                yield return null;
            }
        }

        private IEnumerator TempMinBoundaryCoroutine(float from, float to, float duration)
        {
            float timeStep = 0f;

            float _from = from;
            while (timeStep <= duration)
            {
                timeStep = timeStep + Time.deltaTime;
                float step = Mathf.Clamp01(timeStep / duration);
                _tempMinDifficultyBoundary = Mathf.Lerp(_from, to, step);
                yield return null;
            }
        }

        private void CalculateTempBoundaries()
        {
            if (_tempEstimatedReverseDiff > previousEstimatedReverseDiff &&
                _tempEstimatedReverseDiff > _tempMaxDifficultyBoundary - BoundaryStepSize)
            {
                //if greater, then we adjust the MAX boundary
                //MaxDifficultyBoundary = MaxDifficultyBoundary + BoundaryStepSize >= 1 ? 1 : MaxDifficultyBoundary + BoundaryStepSize;

                //_tempMaxDifficultyBoundary = _tempEstimatedReverseDiff + BoundaryStepSize >= 1 ? 1 : _tempEstimatedReverseDiff + BoundaryStepSize;

                /*newTempMaxValue = _tempEstimatedReverseDiff + BoundaryStepSize >= 1f
                    ? 1f
                    : _tempEstimatedReverseDiff + BoundaryStepSize;*/
                
                if (_tempEstimatedReverseDiff + BoundaryStepSize <= 0f)
                {
                    newTempMaxValue = 0f;
                }
                else if (_tempEstimatedReverseDiff + BoundaryStepSize >= 1f)
                {
                    newTempMaxValue = 1f;
                }
                else
                {
                    newTempMaxValue = _tempEstimatedReverseDiff + BoundaryStepSize;
                }
                
                maxWasAdjusted = true;
                minWasAdjusted = false;
                maxAdjustedCount++;
                minAdjustedCount = 0;

                if (maxAdjustedCount >= 2)
                {
                    //TODO: here I should also check if greater or lover than 0, otherwise newTempMin can be set higher or lower than 0
                    if (_tempMinDifficultyBoundary + 2 * BoundaryStepSize <= 0)
                    {
                        newTempMinValue = 0;
                    }
                    else if (_tempMinDifficultyBoundary + 2 * BoundaryStepSize >= 1)
                    {
                        newTempMinValue = 1;
                    }
                    else
                    {
                        newTempMinValue = _tempMinDifficultyBoundary + 2 * BoundaryStepSize;
                    }
                    //newTempMinValue = _tempMinDifficultyBoundary + 2 * BoundaryStepSize;
                    maxAdjustedCount = 0;
                    minWasAdjusted = true;
                }

            }
            else if (_tempEstimatedReverseDiff < previousEstimatedReverseDiff &&
                     _tempEstimatedReverseDiff < _tempMinDifficultyBoundary + BoundaryStepSize)
            {
                //MinDifficultyBoundary = MinDifficultyBoundary - BoundaryStepSize <= 0 ? 0 : MinDifficultyBoundary - BoundaryStepSize;
                //_tempMinDifficultyBoundary = _tempEstimatedReverseDiff - BoundaryStepSize <= 0 ? 0 : _tempEstimatedReverseDiff - BoundaryStepSize;

                
                /*newTempMinValue = _tempEstimatedReverseDiff - BoundaryStepSize <= 0f
                    ? 0f
                    : _tempEstimatedReverseDiff - BoundaryStepSize;*/

                if (_tempEstimatedReverseDiff - BoundaryStepSize <= 0f)
                {
                    newTempMinValue = 0f;
                }
                else if (_tempEstimatedReverseDiff - BoundaryStepSize >= 1f)
                {
                    newTempMinValue = 1f;
                }
                else
                {
                    newTempMinValue = _tempEstimatedReverseDiff - BoundaryStepSize;
                }
                minWasAdjusted = true;
                maxWasAdjusted = false;
                minAdjustedCount++;
                maxAdjustedCount = 0;

                if (minAdjustedCount >= 2)
                {
                    //make a step with max to left
                    if (_tempMaxDifficultyBoundary - 2 * BoundaryStepSize <= 0)
                    {
                        newTempMaxValue = 0;
                    }
                    else if (_tempMaxDifficultyBoundary - 2 * BoundaryStepSize >= 1)
                    {
                        newTempMaxValue = 1;
                    }
                    else
                    {
                        newTempMaxValue = _tempMinDifficultyBoundary + 2 * BoundaryStepSize;
                    }
                    //newTempMaxValue = _tempMaxDifficultyBoundary - 2 * BoundaryStepSize;
                    minAdjustedCount = 0;
                    maxWasAdjusted = true;
                }
            }
            else
            {
                //if equals do nothing
            }
        }

        private void AdjustTempBoundaries()
        {
            if ( /*newTempMaxValue - _tempEstimatedReverseDiff > 0.1f*/
                _tempMaxDifficultyBoundary - _tempEstimatedReverseDiff > 0.1f)
            {
                newTempMaxValue -= (newTempMaxValue - _tempEstimatedReverseDiff) / 2f;
                maxWasAdjusted = true;
                //maxNeedsToBeAdjusted = true;
            }

            if ( /*_tempEstimatedReverseDiff - newTempMinValue > 0.1f*/
                _tempEstimatedReverseDiff - _tempMinDifficultyBoundary > 0.1f)
            {
                newTempMinValue += (_tempEstimatedReverseDiff - newTempMinValue) / 2f;
                minWasAdjusted = true;
            }

        }

        /*private void AdjustTempBoundaries()
        {
            if (maxAdjustedCount >= 2)
            {
                //make a step with min to right
                _tempMinDifficultyBoundary = _tempMinDifficultyBoundary + 2 * BoundaryStepSize;
                maxAdjustedCount = 0;
            }
    
            if (minAdjustedCount >= 2)
            {
                //make a step with max to left
                _tempMaxDifficultyBoundary = _tempMaxDifficultyBoundary - 2 * BoundaryStepSize;
                minAdjustedCount = 0;
            }
    
            if (_tempEstimatedReverseDiff - _tempMinDifficultyBoundary > 0.2f)
            {
                _tempMinDifficultyBoundary = _tempMinDifficultyBoundary +
                                             (_tempEstimatedReverseDiff - _tempMinDifficultyBoundary) / 3f;
            }
    
            if (_tempMaxDifficultyBoundary - _tempEstimatedReverseDiff > 0.2f)
            {
                _tempMaxDifficultyBoundary = _tempMaxDifficultyBoundary -
                                             (_tempMaxDifficultyBoundary - _tempEstimatedReverseDiff) / 3f;
            }
            
        }*/

        private IEnumerator CalculateGaugeCoroutine(float from, float to, float duration)
        {
            float timeStep = 0f;

            float _from = from;
            while (timeStep <= duration)
            {
                timeStep = timeStep + Time.deltaTime;
                float step = Mathf.Clamp01(timeStep / duration);
                _tempDifficultyGauge = Mathf.Lerp(_from, to, step);
                yield return null;
            }
        }

        private void CalculateGauge()
        {
            newTempDifficultyGauge = MinDifficultyBoundary + (MaxDifficultyBoundary - MinDifficultyBoundary) / 2f;

            float scaledTempDiffGauge = (MinDifficultyBoundary - _tempDifficultyGauge) /
                                        (MinDifficultyBoundary - MaxDifficultyBoundary);
            //Debug.Log("_tempDifficultyGauge: " + _tempDifficultyGauge);
            //Debug.Log("ScaledTempDiffGauge: " + scaledTempDiffGauge);
        }

        private IEnumerator AdjustMaxBoundaryCoroutine(float from, float to, float duration)
        {
            float timeStep = 0f;

            float _from = from;
            while (timeStep <= duration)
            {
                timeStep = timeStep + Time.deltaTime;
                float step = Mathf.Clamp01(timeStep / duration);
                MaxDifficultyBoundary = Mathf.Lerp(_from, to, step);
                yield return null;
            }
        }

        private IEnumerator AdjustMinBoundaryCoroutine(float from, float to, float duration)
        {
            float timeStep = 0f;

            float _from = from;
            while (timeStep <= duration)
            {
                timeStep = timeStep + Time.deltaTime;
                float step = Mathf.Clamp01(timeStep / duration);
                MinDifficultyBoundary = Mathf.Lerp(_from, to, step);
                yield return null;
            }
        }

        private void AdjustBoundaries()
        {

            newMaxValue = MaxDifficultyBoundary + (_tempMaxDifficultyBoundary - MaxDifficultyBoundary) / 2f;
            newMinValue = MinDifficultyBoundary + (_tempMinDifficultyBoundary - MinDifficultyBoundary) / 2f;

            /*if (maxWasAdjusted)
            {
                float a = Mathf.Abs(originalBoundaryDistance - (MaxDifficultyBoundary - MinDifficultyBoundary));
                MinDifficultyBoundary = MinDifficultyBoundary + a / 2;
            }
    
            if (minWasAdjusted)
            {
                float a = Mathf.Abs(originalBoundaryDistance - (MaxDifficultyBoundary - MinDifficultyBoundary));
                MaxDifficultyBoundary = MaxDifficultyBoundary - a / 2;
            }*/
        }

        private void AdjustDifficultyGauge()
        {
            //float tempGaugeDiff = _tempDifficultyGauge - DifficultyGauge;
            float tempGaugeDiff = previousEstimatedReverseDiff - DifficultyGauge;
            DifficultyGauge = DifficultyGauge + tempGaugeDiff / 2f;
        }

        private void ScalePlayerHealth(float minLimit, float maxLimit)
        {
            float playerCurrentHealth = Player.GetComponent<Health>().CurrentHealth;
            float playerMaxHealth = Player.GetComponent<Health>().MaxHealth;
            /*
             * The estimated Difficulty based on player health is scaled to [0,1]
             * Where 0 means that the estimated difficulty is very low (EASY)
             * And 1 means that the estimated difficulty is very high (HARD)
             * 
             * If player health is above 80%, the estimated difficulty is set to 0
             * If player health is below 20%, the estimated difficulty is set to 1
             * If player health falls between 20%-80% it is scaled proportionately to [1,0] scale
             */
            if (playerCurrentHealth <= playerMaxHealth * minLimit)
            {
                playerHealthScaledForEstimatedDifficulty = 1;
            }
            else if (playerCurrentHealth >= playerMaxHealth * maxLimit)
            {
                playerHealthScaledForEstimatedDifficulty = 0;
            }
            else
            {
                float _maxLimit = playerMaxHealth * maxLimit;
                float _minLimit = playerMaxHealth * minLimit;
                playerHealthScaledForEstimatedDifficulty = (playerCurrentHealth - _maxLimit) / (_minLimit - _maxLimit);
            }
            //Debug.Log("playerHealthScaledForEstimatedDifficulty: " + playerHealthScaledForEstimatedDifficulty);
        }

        private void ScaleEnemyCount(float maxLimit)
        {
            /*
             * maxLimit means that even if there are more enemies than the limit,
             * the estimated difficulty still stays at HARD (value of 1)
             * The estimated Difficulty based on enemy count in range is scaled to [0,1]
             * Where 0 means that the estimated difficulty is very low (EASY)
             * And 1 means that the estimated difficulty is very high (HARD)
             *
             * if enemyCountInrange is above 10, the estimated difficulty is set to 1
             * if it is lower than 10, it is scaled proportionately to a [0,1] scale
             */

            enemyCountInRange = AssassinMultiplier * AssassinInRange +
                                SoldierMultiplier * SoldierInRange +
                                TankMultiplier * TankInRange;
            if (enemyCountInRange >= maxLimit)
            {
                enemyCountScaledForEstimatedDifficulty = 1;
            }
            else
            {
                enemyCountScaledForEstimatedDifficulty = enemyCountInRange / maxLimit;
            }
        }

        private void ScaleHealthPickupsCount(float maxLimit)
        {
            /*
             * maxLimit means that even if there are more pickups than the limit,
             * the estimated difficulty still stays at EASY (value of 0)
             * The estimated Difficulty based on health pickups in range is scaled to [0,1]
             * Where 0 means that the estimated difficulty is very low (EASY)
             * And 1 means that the estimated difficulty is very high (HARD)
             *
             * if healthPickupsCountInRange is above 10, the estimated difficulty is set to 0
             * if it is lower than 10, it is scaled proportionately to a [0,1] scale
             */
            if (healthPickupsCountInRange >= maxLimit)
            {
                healthPickupsCountScaledForEstimatedDifficulty = 0;
            }
            else
            {
                healthPickupsCountScaledForEstimatedDifficulty = (healthPickupsCountInRange - maxLimit) / (-maxLimit);
            }
        }

        private void ScalePlayerAccuracy(float minLimit, float maxLimit)
        {
            /*
             * The estimated Difficulty based on player accuracy is scaled to [0,1]
             * Where 0 means that the estimated difficulty is very low (EASY),
             * because player accuracy is high, which assumes the player is fairly good at the game
             * And 1 means that the estimated difficulty is very high (HARD)
             * because player accuracy is low, which assumes the player is fairly bad at the game
             * 
             * If player accuracy is above 70% (maxLimit), the estimated difficulty is set to 0
             * If player accuracy is below 30% (minLimit), the estimated difficulty is set to 1
             * If player accuracy falls between 30%-70% it is scaled proportionately to [1,0] scale
             */
            if (PlayerAccuracy <= minLimit)
            {
                playerAccuracyScaledForEstimatedDifficulty = 1;
            }
            else if (PlayerAccuracy >= maxLimit)
            {
                playerAccuracyScaledForEstimatedDifficulty = 0;
            }
            else
            {
                playerAccuracyScaledForEstimatedDifficulty = (PlayerAccuracy - maxLimit) / (minLimit - maxLimit);
            }
        }

        private void SpawnEnemyAtRandomLocation(Vector3 minPos, Vector3 maxPos)
        {
            Debug.Log("Spawn Enemy");
            //Need to get a random point on X axis in [-5, 7]
            //-5 is the x of minPos
            //7 is the x of maxPos
            float minX = minPos.x;
            float maxX = maxPos.x;
            float randX = Random.Range(minX, maxX);

            //Need to get a random point on Z axis in [40, 60]
            //40 is the z of maxPos
            //60 is the z of minPos
            float minZ = maxPos.z;
            float maxZ = minPos.z;
            float randZ = Random.Range(minZ, maxZ);

            Vector3 position = new Vector3(randX, 0.5f, randZ);
            GameObject newEnemy = Instantiate(EnemyPrefab, position, Quaternion.identity);

            EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
            EnemyControllers.Add(enemyController);
            RegisterEnemyController(enemyController);
        }

        private int i = 0;
        /*public void SpawnEnemy(Vector3 center, float radius)
        {
            Debug.Log("Spawn Enemy");
            //int i = Random.Range(0, 100);
            float angle = Mathf.PI * 2f / 10 * i;
            i++;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle)*radius, 0, Mathf.Sin(angle)*radius);
            Debug.Log("Enemy spawn position: " + newPos);
            //Vector3 position = new Vector3(-45f, 0, 70f);
    
            float random = Random.Range(0, 100);
            Debug.Log("Random enemy: " + random);
    
            //Easy
            if (DifficultyGauge < 0.3f)
            {
                //80% to spawn a tank
                //20% to spawn a soldier
                if (random >= 0 && random < 80)
                {
                    //Spawm a Tank
                    InstantiateEnemy(TankPrefab, newPos);
                }
                else
                {
                    //if random >= 80
                    //Spawn a Soldier
                    InstantiateEnemy(SoldierPrefab, newPos);
                }
            }
            //Medium-to-Easy
            if (DifficultyGauge >= 0.3f && DifficultyGauge < 0.5f)
            {
                //60% to spawn a tank
                //40% to spawn a soldier
                if (random >= 0 && random < 60)
                {
                    //Spawm a Tank
                    InstantiateEnemy(TankPrefab, newPos);
                }
                else
                {
                    //if random >= 60
                    //Spawn a Soldier
                    InstantiateEnemy(SoldierPrefab, newPos);
                }
            }
            //Medium-to-Hard
            if (DifficultyGauge >= 0.5f && DifficultyGauge < 0.7f)
            {
                //20% to spawn a tank
                //60% to spawn a soldier
                //20% to spawn a assassin
                if (random >= 0 && random < 20)
                {
                    //Spawm a Tank
                    InstantiateEnemy(TankPrefab, newPos);
                }
                else if (random >= 20 && random < 80)
                {
                    //Spawn a Soldier
                    InstantiateEnemy(SoldierPrefab, newPos);
                }
                else
                {
                    //if random >= 80
                    //Spawn an Assassin
                    InstantiateEnemy(AssassinPrefab, newPos);
                }
            }
            //Hard
            if (DifficultyGauge >= 0.7f)
            {
                //20% to spawn a tank
                //40% to spawn a soldier
                //40% to spawn a assassin
                if (random >= 0 && random < 20)
                {
                    //Spawm a Tank
                    InstantiateEnemy(TankPrefab, newPos);
                }
                else if (random >= 20 && random < 60)
                {
                    //Spawn a Soldier
                    InstantiateEnemy(SoldierPrefab, newPos);
                }
                else
                {
                    //if random >= 60
                    //Spawn an Assassin
                    InstantiateEnemy(AssassinPrefab, newPos);
                }
            }
            
        }*/

        private void InstantiateEnemy(GameObject enemyPrefab, Vector3 position)
        {
            Debug.Log("Enemy type spawned: " + enemyPrefab);
            GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);

            EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
            EnemyControllers.Add(enemyController);
            RegisterEnemyController(enemyController);
        }

        public void RegisterEnemyController(EnemyController enemyController)
        {
            enemyController.PlayerRange = EnemyInRange;

            enemyController.onDamaged += OnEnemyHit;
            enemyController.onDie += OnEnemyDeath;
            enemyController.onDetectedTarget += OnEnemyDetectedPlayer;
            enemyController.onLostTarget += OnEnemyLostPlayer;
            enemyController.onHealthSpawned += OnHealthSpawned;

            switch (enemyController.EnemyType)
            {
                case AI.EnemyType.Assasin:
                {
                    enemyController.onDetectedTarget += OnAssassinEnemyDetectedPlayer;
                    enemyController.onLostTarget += OnAssassinEnemyLostPlayer;
                    break;
                }
                case AI.EnemyType.Soldier:
                {
                    enemyController.onDetectedTarget += OnSoldierEnemyDetectedPlayer;
                    enemyController.onLostTarget += OnSoldierEnemyLostPlayer;
                    break;
                }
                case AI.EnemyType.Tank:
                {
                    enemyController.onDetectedTarget += OnTankEnemyDetectedPlayer;
                    enemyController.onLostTarget += OnTankEnemyLostPlayer;
                    break;
                }
                default:
                    break;
            }
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
                
                switch (enemyController.EnemyType)
                {
                    case AI.EnemyType.Assasin:
                    {
                        enemyController.onDetectedTarget -= OnAssassinEnemyDetectedPlayer;
                        enemyController.onLostTarget -= OnAssassinEnemyLostPlayer;
                        break;
                    }
                    case AI.EnemyType.Soldier:
                    {
                        enemyController.onDetectedTarget -= OnSoldierEnemyDetectedPlayer;
                        enemyController.onLostTarget -= OnSoldierEnemyLostPlayer;
                        break;
                    }
                    case AI.EnemyType.Tank:
                    {
                        enemyController.onDetectedTarget -= OnTankEnemyDetectedPlayer;
                        enemyController.onLostTarget -= OnTankEnemyLostPlayer;
                        break;
                    }
                    default:
                        break;
                }
                
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
            healthPickupComponent.Player = Player;
            healthPickupComponent.PlayerRange = HealthPickupRange;
            healthPickupComponent.onHealthPicked += OnHealthPicked;
            healthPickupComponent.onOutOfPlayerRange += OnHealthOutOfPlayerRange;
            healthPickupComponent.onInPlayerRange += OnHealthInPlayerRange;

            HealthPickups.Add(LootPrefab);

            /*if (Vector3.Distance(LootPrefab.transform.position, Player.transform.position) < range)
            {
                healthPickupsCountInRange++;
            }*/
        }

        private void OnHealthInPlayerRange()
        {
            healthPickupsCountInRange++;
        }

        private void OnHealthOutOfPlayerRange()
        {
            healthPickupsCountInRange--;
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
        
        private void OnAssassinEnemyDetectedPlayer()
        {
            AssassinInRange++;
        }
        
        private void OnSoldierEnemyDetectedPlayer()
        {
            SoldierInRange++;
        }
        
        private void OnTankEnemyDetectedPlayer()
        {
            TankInRange++;
        }

        private void OnEnemyLostPlayer()
        {
            enemyCountInRange--;
        }
        
        private void OnAssassinEnemyLostPlayer()
        {
            AssassinInRange--;
        }
        private void OnSoldierEnemyLostPlayer()
        {
            SoldierInRange--;
        }
        private void OnTankEnemyLostPlayer()
        {
            TankInRange--;
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
                
                switch (enemyController.EnemyType)
                {
                    case AI.EnemyType.Assasin:
                    {
                        AssassinInRange--;
                        break;
                    }
                    case AI.EnemyType.Soldier:
                    {
                        SoldierInRange--;
                        break;
                    }
                    case AI.EnemyType.Tank:
                    {
                        TankInRange--;
                        break;
                    }
                    default:
                        break;
                }
            }

            if (enemyController != null)
                EnemyControllers.Remove(enemyController);
        }

        private void OnAddedWeapon(WeaponController weaponInstance, int i)
        {
            _weaponController = weaponInstance;
            _weaponController.OnShoot += OnShoot;
        }

        private void OnShoot()
        {
            //Debug.Log("Shooooooting");
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
            Handles.DrawWireDisc(_centre, Vector3.up, EnemyInRange);
            Handles.DrawWireDisc(_centre, Vector3.up, HealthPickupRange);
        }

        public void SetPlayerAccuracy(float playerAccuracy)
        {
            PlayerAccuracy = playerAccuracy;
        }
    }
}
