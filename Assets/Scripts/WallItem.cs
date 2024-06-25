using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public GameObject objEdit, genInteract;
    public ObjEditUIEventHandler objEditScript;
    public GeneralInteractionEventHandler genIntScript;
    private Color originalColor;
    private bool isSelected = false;
    private Renderer objectRenderer;
    private float Increment;

    void Start()
    {
        objEdit = GameObject.Find("ObjEditUIEventHandler");
        objEditScript = objEdit.GetComponent<ObjEditUIEventHandler>();
        genInteract = GameObject.Find("GeneralInteractionEH");
        genIntScript = genInteract.GetComponent<GeneralInteractionEventHandler>();
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    void Update(){

        if (isSelected && !Camera.main.orthographic && !Input.GetKey(KeyCode.LeftShift)){
            Increment = objEditScript.Increment;
            objEditScript.interact2D(Increment, this.gameObject, true);
        }
        checkSelection();
    }

    void OnMouseDown()
    {
        objEditScript.objectSelected = this.gameObject;
        isSelected = !isSelected;
        UpdateColor();
    }


    private void UpdateColor()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = isSelected ? Color.green : originalColor;
        }
    }

    void checkSelection(){
        if (Input.GetMouseButtonDown(0))
        {
            bool isPointerOverSelectableObject = genIntScript.IsPointerOverGameObject("Window") || genIntScript.IsPointerOverGameObject("Door");

            if (!isPointerOverSelectableObject || (!objEditScript.objectSelected.Equals(this.gameObject) && this.gameObject.GetComponent<SelectableObject>().isSelected))
            {
                isSelected = false;
                UpdateColor();
            }
        }
    }

    
}
