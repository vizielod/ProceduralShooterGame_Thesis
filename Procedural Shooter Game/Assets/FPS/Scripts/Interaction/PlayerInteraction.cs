using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction: MonoBehaviour {

    public float interactionDistance;

    public TMPro.TextMeshProUGUI interactionText;
    //public GameObject interactionHoldGO; // the ui parent to disable when not interacting
    //public UnityEngine.UI.Image interactionHoldProgress; // the progress bar for hold interaction type

    [Header("References")] [Tooltip("Secondary camera used to avoid seeing weapon go throw geometries")]
    public Camera WeaponCamera;
    [Tooltip("Position for weapons when aiming")]
    public Transform AimingWeaponPosition;

    void Start() {
        //cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        bool successfulHit = false;
        
        if (Physics.Raycast(WeaponCamera.transform.position, WeaponCamera.transform.forward, out RaycastHit hit,
                interactionDistance, -1, QueryTriggerInteraction.Ignore))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null) {
                HandleInteraction(interactable);
                interactionText.text = interactable.GetDescription();
                successfulHit = true;
            }
        }

        // if we miss, hide the UI
        if (!successfulHit) {
            interactionText.text = "";
        }
    }

    void HandleInteraction(Interactable interactable) {
        KeyCode key = KeyCode.E;
        if (Input.GetKeyDown(key)) {
            interactable.Interact();
        }
    }
}
