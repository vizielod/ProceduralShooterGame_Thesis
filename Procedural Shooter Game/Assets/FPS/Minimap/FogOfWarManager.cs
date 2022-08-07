using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class FogOfWarManager : MonoBehaviour
{
    
    [Header("Player and Map")] 
    [SerializeField] private GameObject player;
    [SerializeField] private Material fogPlaneMaterial;
    [SerializeField] private Camera mapCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateFogOfWarPlane();
        SetMapCamera();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private GameObject fogPlane;
    
    private void GenerateFogOfWarPlane()
    {
        //create a square plane where the length of the side is the bigger value from width_x and width_z
        float width = 200f;
        float height = 200f;
            
        ProBuilderMesh fogPlaneMesh = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, height, 30, 30, Axis.Up);
            
        fogPlane = fogPlaneMesh.gameObject;

        fogPlane.transform.position = new Vector3(this.transform.position.x, 90, this.transform.position.z);

        //Set the material of the FogOfWar Plane
        fogPlane.GetComponent<MeshRenderer>().material = fogPlaneMaterial;

        fogPlane.AddComponent<MeshCollider>();
            
        //IEnumerable<Face> faces = fogPlaneMesh.faces;
        //fogPlaneMesh.SetMaterial(faces, fogPlaneMaterial);

        fogPlane.layer = LayerMask.NameToLayer("FogOfWar");

        fogPlane.gameObject.name = "FogPlane";
    }

    private void SetMapCamera()
    {
        mapCamera.orthographic = true;

        mapCamera.orthographicSize = 200f / 2f;
            
        //mapCamera.rect = new Rect(0, 0, 5, 5);
            
        mapCamera.transform.position = new Vector3(this.transform.position.x, 110, this.transform.position.z);
            
        mapCamera.gameObject.SetActive(true);

        mapCamera.GetComponent<FogOfWar>().m_fogOfWarPlane = fogPlane;
            
        mapCamera.GetComponent<FogOfWar>().Initialize();

        mapCamera.GetComponent<FogOfWar>().startUpdate = true;

        //mapCamera.orthographicSize = width_x;
    }
}
