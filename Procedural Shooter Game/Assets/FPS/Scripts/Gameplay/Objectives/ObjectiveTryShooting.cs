using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveTryShooting : Objective
    {
        public bool IsActive = false;
        public void SetTryShootingObjective()
        {
            base.Start();

            EventManager.AddListener<ShootEvent>(OnShootingEvent);

            IsActive = true;
        }
        
        void OnShootingEvent(ShootEvent evt)
        {
            if (IsCompleted)
                return;

            // this will trigger the objective completion
            // it works even if the player can't pickup the item (i.e. objective pickup healthpack while at full heath)
            CompleteObjective(string.Empty, string.Empty, "Objective complete : " + Title);

            if (gameObject)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<ShootEvent>(OnShootingEvent);
        }
    }
}
