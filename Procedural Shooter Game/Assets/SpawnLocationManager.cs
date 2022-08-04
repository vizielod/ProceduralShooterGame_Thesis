using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnLocationManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDrawGizmos()
    {
        // Draw DDA area around player. This range is used to calculate enemy count for adjusting difficulty
        Gizmos.color = Color.blue;
        Vector3 _centre = new Vector3(this.transform.position.x, this.transform.position.y + 1f,
            this.transform.position.z);
        //Gizmos.DrawWireSphere(_centre, 1f);
        Gizmos.DrawSphere(_centre, 1f);
    }
}
