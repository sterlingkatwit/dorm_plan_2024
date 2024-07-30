using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public GameObject objEdit, genInteract, roomEdit;
    public ObjEditUIEventHandler objEditScript;
    public GeneralInteractionEventHandler genIntScript;
    public RoomEditUIEventHandler roomEditScript;
    private Color originalColor;
    public GameObject parentWall;
    private bool isSelected = false;
    private Renderer objectRenderer;
    private float Increment;
    public Vector3 originalScale;

    void Start()
    {
        objEdit = GameObject.Find("ObjEditUIEventHandler");
        objEditScript = objEdit.GetComponent<ObjEditUIEventHandler>();
        genInteract = GameObject.Find("GeneralInteractionEH");
        genIntScript = genInteract.GetComponent<GeneralInteractionEventHandler>();
        roomEdit = GameObject.Find("RoomEditUIEventHandler");
        roomEditScript = roomEdit.GetComponent<RoomEditUIEventHandler>();
        objectRenderer = GetComponent<Renderer>();
        originalColor = Color.grey;
        originalScale = this.transform.localScale;
    }

    void Update(){

        if (isSelected && !Camera.main.orthographic && !Input.GetKey(KeyCode.LeftShift)){
            Increment = objEditScript.Increment;
            objEditScript.interact2D(Increment, this.gameObject, true);
        }
        
        if(!parentWall.gameObject.activeSelf){
            this.transform.localScale = Vector3.zero;
        } else {
            this.transform.localScale = originalScale;
        }

        checkSelection();
        delWindow();
    }

    void OnMouseDown()
    {
        isSelected = !isSelected;
        UpdateColor();
    }

    void OnMouseOver(){
        if (Input.GetMouseButtonDown(0)){
            roomEditScript.objectSelected = this.gameObject;
        }
    }

    void delWindow(){
        if(isSelected && (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))){
            Destroy(this.gameObject);
        }
    }


    private void UpdateColor(){
        if (objectRenderer != null){
            objectRenderer.material.color = isSelected ? Color.green : originalColor;
        }
    }

    void checkSelection(){
        bool isPointerOverSelectableObject = genIntScript.IsPointerOverGameObject("Window") || genIntScript.IsPointerOverGameObject("Door");

        if ((Input.GetMouseButtonDown(0) && !isPointerOverSelectableObject) || (!roomEditScript.objectSelected.Equals(this.gameObject) && this.gameObject.GetComponent<SelectableObject>().isSelected)){
            isSelected = false;
            UpdateColor();
        }
    }

    
}
