using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Button = UnityEngine.UI.Button;


public class FurnitureInteraction : MonoBehaviour
{
    // Should be seperated into two scripts? -Lucas

    private GameObject genIntEV, uiEH, camEH, tempObj;
    private GeneralInteractionEventHandler genIntScript;
    private UIEventHandler uiEHScript;
    private CameraEventHandler camEHScript;

    private bool isLeftClicked = false;
    private bool isRightClicked = false;
    private bool isRotatable = false;
    private Rigidbody rb;
    private int contacts;
    private Vector3 currentPos;
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

        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isLeftClicked)
        {
            objectSelected();
        }

        else if(contacts == 0){
            transform.position = currentPos;    
        }

        if (Input.GetKeyDown(KeyCode.Delete) && isRightClicked)
        {
            Destroy(gameObject);
        }

        if (isRightClicked)
        {
            float rotationSpeed = 50f;
            float horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);
        }

    }

 

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !isRightClicked)
        {
            isLeftClicked = switchSelect(isLeftClicked);
        }
        else if (Input.GetMouseButtonDown(1) && !isLeftClicked)
        {
            isRightClicked = switchSelect(isRightClicked);
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
            GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }

        return swtch;
    }

    void moveObject(){

        // Get the current mouse position in screen coordinates
        Vector3 mPosScreen = Input.mousePosition;

        // Convert the screen mouse position to world point
        Vector3 mPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mPosScreen.x, mPosScreen.y, transform.position.y - Camera.main.transform.position.y));

        // Update the position of the object to the mouse position on only X and Z axes.
        currentPos = transform.position = new Vector3(mPosWorld.x, transform.position.y, mPosWorld.z);
    }

    void interact3D(float increment){
        int dir = 0;
        float curX = transform.position.x;
        float curY = transform.position.y;
        float curZ = transform.position.z;

        //arrows keys?
        switch(dir){
            case 0:
                if(Input.GetKeyDown(KeyCode.UpArrow)){
                    currentPos = transform.position = new Vector3(curX+increment, curY, curZ);
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){
                    currentPos = transform.position = new Vector3(curX-increment, curY, curZ);
                }
            break;
            case 1:
                if(Input.GetKeyDown(KeyCode.UpArrow)){
                    currentPos = transform.position = new Vector3(curX, curY+increment, curZ);
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){
                    currentPos = transform.position = new Vector3(curX, curY-increment, curZ);
                }
            break;
            case 2:
                if(Input.GetKeyDown(KeyCode.UpArrow)){
                    currentPos = transform.position = new Vector3(curX, curY, curZ+increment);
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){
                    currentPos = transform.position = new Vector3(curX, curY, curZ-increment);
                }
            break;
        }

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

        // This locks the object in place when switching from 3D to 2D so obj doesn't teleport.
        if(camEHScript.camSwitched){
            isLeftClicked = false;
            camEHScript.camSwitched = false;
        }
        else if(camEHScript.mainCam.orthographic){
            moveObject();
        }
        else if(!camEHScript.mainCam.orthographic){
            interact3D(1);
        }




        if(Input.GetKey(KeyCode.LeftControl)){

            if(Input.GetKeyDown(KeyCode.C)){
                copyObj();
            }
            else if(Input.GetKeyDown(KeyCode.X)){
                copyObj();
                Destroy(this.gameObject);
            }
        }

        uiEHScript.ofName.text = this.gameObject.name;
        uiEHScript.ofX.text = this.gameObject.GetComponent<Renderer>().bounds.size.x.ToString();
        uiEHScript.ofY.text = this.gameObject.GetComponent<Renderer>().bounds.size.z.ToString();

    }






    // void OnCollisionEnter(Collision col){ 
    //     if(col.gameObject.CompareTag("Wall")){
    //         contacts++;
    //     }
    //     Debug.Log(contacts);
    // }
    // void OnCollisionExit(Collision col){ 
    //     if(col.gameObject.CompareTag("Wall")){
    //         contacts--;;
    //     }
    //     Debug.Log(contacts);
    // }
}
