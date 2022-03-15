using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoKeyPickup : MonoBehaviour
{

    public int keyID = 0;

    public float currentRotation = 0f;
    public float rotationSpeed = 10f;

    public GameObject keyIconImage;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentRotation += rotationSpeed * Time.deltaTime;
        this.transform.rotation = Quaternion.AngleAxis(currentRotation, Vector3.up);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Added key: " + keyID);
        if(other.gameObject.tag == "Player") {
            if(!DemoPlayer.HasKey(keyID)) {
                DemoPlayer.AddKey(keyID);

                GameObject keyIcon = Instantiate(keyIconImage, transform.position, Quaternion.identity);
                
                GameObject parentGO = GameObject.Find("HUD");
                keyIcon.transform.SetParent(parentGO.transform);

                RectTransform keyIconRectTransform = keyIcon.GetComponent<RectTransform>();

                keyIconRectTransform.position = new Vector3(30 + 30*(DemoPlayer.keysFound.Count-1), 60, 0);
                
                RawImage keyImage = keyIcon.GetComponent<RawImage>();
                Color newColor = new Color(transform.GetComponentInChildren<Renderer>().material.color.r, 
                    transform.GetComponentInChildren<Renderer>().material.color.g, 
                    transform.GetComponentInChildren<Renderer>().material.color.b);
                newColor.a = 1;
                keyImage.color = newColor;

                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
