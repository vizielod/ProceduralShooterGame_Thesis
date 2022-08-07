using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.Servers;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class TutorialObjectiveManager : MonoBehaviour
    {
        public PlayerCharacterController m_PlayerCharacterController;
        public GameObject LockedDoor_1;
        public GameObject LockedDoor_2;
        public GameObject ObjectiveReachPoint;
        public GameObject CentralComputer;
        public GameObject HealthPickup;
        
        public ObjectiveTryMoving ObjectiveTryMoving;
        public ObjectiveTrySprinting ObjectiveTrySprinting;
        public ObjectiveTryJumping ObjectiveTryJumping;
        public ObjectiveTryOpeningMap ObjectiveTryOpeningMap;
        public ObjectiveTryShooting ObjectiveTryShooting;
        public ObjectivePickupHealth ObjectivePickupHealth;
        public ObjectiveKillTutorialEnemy ObjectiveKillTutorialEnemy;
        public ObjectiveExtractDataTutorial ObjectiveExtractDataTutorial;

        public bool SpawnReachPointObjective = false;
        public bool GotHealed = false;
        public bool OnHealedSetup = false;
        

        //public GameObject ObjectiveKillEnemy;
        // Start is called before the first frame update
        void Start()
        {
            //ObjectiveTryMoving.gameObject.SetActive(true);
            //m_PlayerCharacterController.m_Health.OnHealed += OnHealed;
            ObjectiveTryMoving.SetMoveAroundObjective();
        }

        private void OnHealed(float amount)
        {
            GotHealed = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_PlayerCharacterController.m_Health != null && !OnHealedSetup)
            {
                m_PlayerCharacterController.m_Health.OnHealed += OnHealed;
                OnHealedSetup = true;
            }
            if (!ObjectiveTryMoving.IsCompleted && ObjectiveTryMoving.IsActive)
            {
                if (m_PlayerCharacterController.m_InputHandler.m_MoveInputWasHeld)
                {
                    MoveAroundEvent evt = Events.MoveAroundEvent;
                    EventManager.Broadcast(evt);
                    //ObjectiveTrySprinting.gameObject.SetActive(true);
                    ObjectiveTrySprinting.SetTrySpriningObjective();
                    //m_PlayerCharacterController.m_InputHandler.m_MoveInputWasHeld = false;
                }
            }

            if (!ObjectiveTrySprinting.IsCompleted && ObjectiveTrySprinting.IsActive)
            {
                if (m_PlayerCharacterController.m_InputHandler.m_MoveInputWasHeld && m_PlayerCharacterController.isSprinting)
                {
                    SprintEvent evt = Events.SprintEvent;
                    EventManager.Broadcast(evt);
                    ObjectiveTryJumping.SetTryJumpingObjective();
                }
            }

            if (!ObjectiveTryJumping.IsCompleted && ObjectiveTryJumping.IsActive)
            {
                if (m_PlayerCharacterController.HasJumpedThisFrame)
                {
                    JumpEvent evt = Events.JumpEvent;
                    EventManager.Broadcast(evt);
                    ObjectiveTryOpeningMap.SetTryOpeningMapObjective();
                }
            }
            
            if (!ObjectiveTryOpeningMap.IsCompleted && ObjectiveTryOpeningMap.IsActive)
            {
                if (m_PlayerCharacterController.MapImage.activeInHierarchy)
                {
                    OpenMapEvent evt = Events.OpenMapEvent;
                    EventManager.Broadcast(evt);
                    ObjectiveTryShooting.SetTryShootingObjective();
                }
            }
            
            if (!ObjectiveTryShooting.IsCompleted && ObjectiveTryJumping.IsActive)
            {
                if (m_PlayerCharacterController.m_InputHandler.m_FireInputWasHeld)
                {
                    ShootEvent evt = Events.ShootEvent;
                    EventManager.Broadcast(evt);

                    Destroy(LockedDoor_1);
                    //StartCoroutine(DamagePlayerCoroutine());
                    //m_PlayerCharacterController.m_Health.TakeDamage(50f, null);
                    
                    //HealthPickup.SetActive(true);
                    ObjectivePickupHealth.SetPickupHealthObjective();

                    //ObjectiveKillTutorialEnemy.SetKillEnemyObjective();
                    
                }
            }

            if (!ObjectivePickupHealth.IsCompleted && ObjectivePickupHealth.IsActive)
            {
                if (GotHealed)
                {
                    PickupEvent evt = Events.PickupEvent;
                    EventManager.Broadcast(evt);

                    GotHealed = false;
                }
            }
        }
        
        IEnumerator DamagePlayerCoroutine()
        {
            //Print the time of when the function is first called.
            Debug.Log("Started Coroutine at timestamp : " + Time.time);

            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(3);
            
            m_PlayerCharacterController.m_Health.TakeDamage(50f, null);
            //After we have waited 5 seconds print the time again.
            Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        }

        void InstantiateReachPointObjectives()
        {
            Vector3 position = CentralComputer.GetComponent<CentralComputer>().ParentRoom.transform.Find("Center").transform.position;
            GameObject newReachPointObjective = Instantiate(ObjectiveReachPoint, position, Quaternion.identity);
            newReachPointObjective.GetComponent<ObjectiveReachPoint>().Title = $"Reach Extraction Area";
        }
        
    }
}