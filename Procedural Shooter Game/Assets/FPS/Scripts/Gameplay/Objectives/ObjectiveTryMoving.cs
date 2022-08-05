using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveTryMoving : Objective
    {
        public bool IsActive = false;
        public void SetMoveAroundObjective()
        {
            base.Start();

            EventManager.AddListener<MoveAroundEvent>(OnMovingEvent);

            IsActive = true;
        }
        
        void OnMovingEvent(MoveAroundEvent evt)
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
            EventManager.RemoveListener<MoveAroundEvent>(OnMovingEvent);
        }
        
    }
}
