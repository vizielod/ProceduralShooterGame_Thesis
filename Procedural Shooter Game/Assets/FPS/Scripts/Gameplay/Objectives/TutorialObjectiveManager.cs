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
        public GameObject LockedDoor;
        
        public ObjectiveTryMoving ObjectiveTryMoving;
        public ObjectiveTrySprinting ObjectiveTrySprinting;
        public ObjectiveTryJumping ObjectiveTryJumping;
        public ObjectiveTryShooting ObjectiveTryShooting;
        public ObjectiveKillTutorialEnemy ObjectiveKillTutorialEnemy;

        //public GameObject ObjectiveKillEnemy;
        // Start is called before the first frame update
        void Start()
        {
            //ObjectiveTryMoving.gameObject.SetActive(true);
            ObjectiveTryMoving.SetMoveAroundObjective();
        }

        // Update is called once per frame
        void Update()
        {
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
                    ObjectiveTryShooting.SetTryShootingObjective();
                }
            }
            
            if (!ObjectiveTryShooting.IsCompleted && ObjectiveTryJumping.IsActive)
            {
                if (m_PlayerCharacterController.m_InputHandler.m_FireInputWasHeld)
                {
                    ShootEvent evt = Events.ShootEvent;
                    EventManager.Broadcast(evt);
                    ObjectiveKillTutorialEnemy.SetKillEnemyObjective();
                    
                    Destroy(LockedDoor);
                }
            }
        }
    }
}