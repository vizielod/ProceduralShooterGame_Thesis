using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectivePickupHealth : Objective
    {
        public bool IsActive = false;
        [Tooltip("Item to pickup to complete the objective")]
        public GameObject ItemToPickup;
        public GameObject LockedDoor_2;
        public ObjectiveKillTutorialEnemy ObjectiveKillTutorialEnemy;

        public void SetPickupHealthObjective()
        {
            base.Start();

            EventManager.AddListener<PickupEvent>(OnPickupEvent);

            IsActive = true;
        }

        void OnPickupEvent(PickupEvent evt)
        {
            if (IsCompleted || ItemToPickup != evt.Pickup)
                return;
            
            Destroy(LockedDoor_2);
            ObjectiveKillTutorialEnemy.SetKillEnemyObjective();

            // this will trigger the objective completion
            // it works even if the player can't pickup the item (i.e. objective pickup healthpack while at full heath)
            CompleteObjective(string.Empty, string.Empty, "Objective complete : " + Title);

            /*if (gameObject)
            {
                Destroy(gameObject);
            }*/
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<PickupEvent>(OnPickupEvent);
        }
    }
}
