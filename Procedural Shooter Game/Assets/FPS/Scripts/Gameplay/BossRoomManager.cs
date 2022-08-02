using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class BossRoomManager : MonoBehaviour
    {
        public GameObject ExternalWalls;
        public GameObject InternalWalls_Easy;
        public GameObject InternalWalls_Medium;
        public GameObject InternalWalls_Hard;
        public GameObject Floor;
        public Transform Center;
        

        public void ColorBossRoom(Color c) {

            List<Renderer> childMats = ExternalWalls.GetComponentsInChildren<Renderer>().ToList();
            for(int i = 0; i < childMats.Count; i++) {
                childMats[i].material.color = c;
            }
        }
    }
}
