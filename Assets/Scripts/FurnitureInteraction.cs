using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;


public class FurnitureInteraction : MonoBehaviour
{
    // Should be seperated into two scripts? -Lucas

    private GameObject genIntEV, uiEH, camEH, objEditEH, tempObj;
    private GeneralInteractionEventHandler genIntScript;
    private UIEventHandler uiEHScript;
    private CameraEventHandler camEHScript;
    private ObjEditUIEventHandler objEditScript;
    private Material defaultMaterial;
    private Renderer objectRenderer;

    public Color prevColor;

    public string type;
    private bool isLeftClicked = false;
    private bool isRightClicked = false;
    private bool isRotatable = false;
    private Rigidbody rb;
    private int contacts;
    [HideInInspector] public Vector3 currentPos;
    public Button objDel;
    public Button objRotate;

    // Start is called before the first frame update
    void Start()
    {
        // Try to avoid this but do what you gotta do :)
        genIntEV = GameObject.Find("GeneralInteractionEH");
        genIntScript = genIntEV.GetComponent<GeneralInteractionEventHandler>();
        uiEH = GameObject.Find("UIEventHandler");
        uiEHScript = uiEH.GetComponent<UIEventHandler>();
        camEH = GameObject.Find("CameraEventHandler");
        camEHScript = camEH.GetComponent<CameraEventHandler>();
        objEditEH = GameObject.Find("ObjEditUIEventHandler");
        objEditScript = objEditEH.GetComponent<ObjEditUIEventHandler>();

        // shaderStart();
        rb = gameObject.GetComponent<Rigidbody>();
        prevColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if(isLeftClicked){
            //Move objects when left clicked. Check if cam switched recently.
            if(camEHScript.camSwitched){
                isLeftClicked = false;
                camEHScript.camSwitched = false;
            }
            else{
                moveObject();
            }

        } else if (isRightClicked){
            objectSelected();
        }
        else{
            uiEHScript.selectedObjDisplay.text = "";
            uiEHScript.selectedObjTagDisplay.text = "";
        }

        if (Input.GetKeyDown(KeyCode.Delete) && isRightClicked)
        {
            Destroy(gameObject);
        }



    }

 

    void OnMouseOver(){
        if (Input.GetMouseButtonDown(0) && !isRightClicked)
        {
            isLeftClicked = switchSelect(isLeftClicked);
            objEditScript.objectSelected = this.gameObject;
        }
        else if (Input.GetMouseButtonDown(1) && !isLeftClicked)
        {
            isRightClicked = switchSelect(isRightClicked);
            objEditScript.objectSelected = this.gameObject;
        }
    }

    // Switches mouse bools and selection color of objects
    private bool switchSelect(bool swtch)
    {
        // Inverts current bool value
        swtch = !swtch;

        // Switches color of objects
        if (swtch)
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
        else
        {
            GetComponent<Renderer>().material.SetColor("_Color", prevColor);
        }
        // if (swtch)
        // {
        //     EnableShaderMaterial();
        // }
        // else
        // {
        //     DisableShaderMaterial();
        // }

        return swtch;
    }

    void moveObject(){
        isRightClicked = false;
        uiEHScript.selectedObjDisplay.text = this.name;
        uiEHScript.selectedObjTagDisplay.text = this.type;
        
        // Object can be moved with mouse while holding shift.
        if(Input.GetKey(KeyCode.LeftShift) && Camera.main.orthographic){
            // Get the current mouse position in screen coordinates
            Vector3 mPosScreen = Input.mousePosition;

            // Convert the screen mouse position to world point
            Vector3 mPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mPosScreen.x, mPosScreen.y, transform.position.y - Camera.main.transform.position.y));

            // Update the position of the object to the mouse position on only X and Z axes.
            currentPos = transform.position = new Vector3(mPosWorld.x, transform.position.y, mPosWorld.z);
        }


        if(Input.GetKey(KeyCode.LeftControl)){

            if(Input.GetKeyDown(KeyCode.C)){
                copyObj();
            }
            else if(Input.GetKeyDown(KeyCode.X)){
                copyObj();
                Destroy(this.gameObject);
            }
        } else if(Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace)){
            Destroy(this.gameObject);
        }


        if(!camEHScript.mainCam.orthographic){
            currentPos = objEditScript.interact3D(objEditScript.Increment, objEditScript.interact3Dvalue, this.gameObject);
        }
        else{
            currentPos = objEditScript.interact2D(objEditScript.Increment, this.gameObject, false);
        }

        checkSelection();
    }



    void copyObj(){
        
        if(genIntScript.clipboard.childCount > 0){
            Destroy(genIntScript.clipboard.GetChild(0).gameObject);
        }

        Transform state = this.gameObject.GetComponent<Transform>();
        tempObj = Instantiate(this.gameObject, new Vector3(state.position.x, state.position.y, state.position.z), Quaternion.identity);
        tempObj.transform.parent = genIntScript.clipboard;
        tempObj.name = this.gameObject.name + "(Copy)";
        tempObj.SetActive(false);
    }

    void objectSelected(){
        isLeftClicked = false;
        uiEHScript.selectedObjDisplay.text = this.name;
        uiEHScript.selectedObjTagDisplay.text = this.type;

        if(Input.GetKey(KeyCode.LeftControl)){

            if(Input.GetKeyDown(KeyCode.C)){
                copyObj();
            }
            else if(Input.GetKeyDown(KeyCode.X)){
                copyObj();
                Destroy(this.gameObject);
            }
        }

        checkSelection();
    }


    void checkSelection(){

        // First line gets rid of green/click when clicking off the obj.
        // Second line gets rid of green/click on a previous object when left clicking a new object
        // Third gets rid of green/click when right clicking away from obj.
        // Fourth line gets rid of green/click on a previous object when right clicking a new object
        if((Input.GetMouseButtonDown(0) && !genIntScript.IsPointerOverGameObject("Object")) ||
        (!objEditScript.objectSelected.Equals(this.gameObject) && objEditScript.objectSelected.GetComponent<FurnitureInteraction>().isLeftClicked) ||
        (Input.GetMouseButtonDown(1) && !genIntScript.IsPointerOverGameObject("Object")) ||
        (!objEditScript.objectSelected.Equals(this.gameObject) && objEditScript.objectSelected.GetComponent<FurnitureInteraction>().isRightClicked)){
            isRightClicked = false;
            isLeftClicked = false;
            this.GetComponent<Renderer>().material.SetColor("_Color", prevColor);
            // DisableShaderMaterial();
        }

    }


    // Object collisions
    void OnCollisionEnter(Collision col){ 
        if(col.gameObject.CompareTag("WallX") || col.gameObject.CompareTag("WallZ")){
            // change material or smth
        }
    }
    void OnCollisionExit(Collision col){ 
        if(col.gameObject.CompareTag("Object")){
            // change material or smth
        }
    }

    void shaderStart(){
        objectRenderer = GetComponent<Renderer>();
        
        if (objectRenderer != null && objectRenderer.materials.Length > 0)
        {
            defaultMaterial = objectRenderer.materials[0];
        }
    }

    public void EnableShaderMaterial()
    {
        if (objectRenderer != null && uiEHScript.outlineMaterial != null)
        {
            // Create a new array with an additional slot for the shader material
            Material[] materials = new Material[2];
            materials[0] = defaultMaterial; 
            materials[1] = uiEHScript.outlineMaterial;

            objectRenderer.materials = materials;
        }
    }

    public void DisableShaderMaterial()
    {
        if (objectRenderer != null)
        {
            // Revert to the default material only
            Material[] materials = new Material[1];
            materials[0] = defaultMaterial;

            objectRenderer.materials = materials;
        }
    }
    
}
