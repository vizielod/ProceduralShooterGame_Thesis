using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameplayRoom : MonoBehaviour {

    public bool initialized = false;
    public int roomIndex = -1;

    public List<Transform> spawnKeyLocations = new List<Transform>();
    public List<Transform> spawnLocations = new List<Transform>();
    public List<GameObject> spawnables = new List<GameObject>();
    public List<GameObject> spawnedRandomProps = new List<GameObject>();
    
    public List<GameObject> possibleEnemiesToSpawn = new List<GameObject>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    
    public int enemiesToSpawnMin = 5;
    public int enemiesToSpawnMax = 10;

    public NavMeshSurface navMeshSurface;

    

    public void Init(int roomIndex, System.Random rand, bool spawnEnemies) {

        this.initialized = true;
        this.roomIndex = roomIndex;
        

        //spawn props randomly in the room based off locators set when buildin the room prefab
        int numSpawn = rand.Next(spawnLocations.Count);
        List<Transform> possibleSpawnLocations = new List<Transform>(spawnLocations);
        List<Transform> possiblePropLocations = new List<Transform>(spawnLocations);

        possiblePropLocations.Shuffle(rand);
        for(int i =0; i < numSpawn; i++) {
            Vector3 spawnLocation = new Vector3(possiblePropLocations[i].position.x, possiblePropLocations[i].position.y - 0.1f, possiblePropLocations[i].position.z);
            possibleSpawnLocations.Remove(possiblePropLocations[i]);
            float randomAngle = (float)rand.Next(360);
            Quaternion spawnRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);

            //choose a random spawnable
            int spawnableToUse = rand.Next(spawnables.Count);
            GameObject spawned = GameObject.Instantiate(spawnables[spawnableToUse], spawnLocation, spawnRotation, this.transform);
            spawnedRandomProps.Add(spawned);
        }
        
        /*if (navMeshSurface != null)
        {
            BuildNavMesh();
        }*/

        if (possibleEnemiesToSpawn.Count > 0 && spawnEnemies)
        {
            List<Transform> possibleEnemyLocations = new List<Transform>();
            for (int i = 0; i < possibleSpawnLocations.Count; i++)
            {
                possibleEnemyLocations.Add(possibleSpawnLocations[i]);
            }
            //possibleEnemyLocations = possiblePropLocations;

            Debug.Log("Spawn Enemies");
            //spawn enemies randomly in the room based off locators set when buildin the room prefab
            int numEnemiesToSpawn = enemiesToSpawnMin + rand.Next((enemiesToSpawnMax - enemiesToSpawnMin)); //get a random amount of enemies between [enemiesToSpawnMin, enemiesToSpawnMax]
            Debug.Log("CallbackExample::Computing Locks and Keys - Total keys: " + numEnemiesToSpawn);

            possibleEnemyLocations.Shuffle(rand);
            for (int i = 0; i < numEnemiesToSpawn; i++)
            {
                Vector3 spawnLocationOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                Vector3 spawnLocation = possibleEnemyLocations[rand.Next(possibleEnemyLocations.Count)].position + spawnLocationOffset;

                float randomAngle = (float) rand.Next(360);
                Quaternion spawnRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);

                //choose a random enemy
                int enemyToUse = rand.Next(possibleEnemiesToSpawn.Count);
                GameObject spawnedEnemy = GameObject.Instantiate(possibleEnemiesToSpawn[enemyToUse],
                    new Vector3(spawnLocation.x, 0, spawnLocation.z), spawnRotation);
                spawnedEnemies.Add(spawnedEnemy);
            }
        }
    }

    public void BuildNavMesh()
    {
        //this.gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
        navMeshSurface.BuildNavMesh();
    }

    public void ColorRoom(Color c) {
        List<Renderer> childMats = this.GetComponentsInChildren<Renderer>().ToList();
        for(int i = 0; i < childMats.Count; i++) {
            childMats[i].material.color = c;
        }
    }
    
    public Vector3 GetRandomKeySpawnPosition()
    {
        Vector3 keyOffset = new Vector3(Random.Range(-0.1f, 0.1f), 0f, Random.Range(-0.1f, 0.1f));
        if (spawnKeyLocations.Count <= 0)
        {
            return this.transform.position + keyOffset;
        }
        else
        {
            return spawnKeyLocations[0].position + keyOffset;
        }
    }
    /*public Vector3 GetRandomKeySpawnLocation()
    {
        if (spawnKeyLocations.Count <= 0) return;
        return spawnKeyLocations[0].position;
    }*/
}
