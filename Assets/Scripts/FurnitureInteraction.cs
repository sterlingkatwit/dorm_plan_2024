using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Button = UnityEngine.UI.Button;


public class FurnitureInteraction : MonoBehaviour
{
    // Should be seperated into two scripts? -Lucas

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
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isLeftClicked)
        {
           moveObject();
        }
        else if(contacts == 0){
            transform.position = currentPos;    
        }

        if (Input.GetKeyDown(KeyCode.R) && isRightClicked)
        {
            isRotatable = !isRotatable;
        }

        if (Input.GetKeyDown(KeyCode.Delete) && isRightClicked)
        {
            Destroy(gameObject);
        }

        if (isRotatable)
        {
            float rotationSpeed = 50f;
            float horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision col){ 
        if(col.gameObject.CompareTag("Wall")){
            contacts++;
        }
        Debug.Log(contacts);
    }
    void OnCollisionExit(Collision col){ 
        if(col.gameObject.CompareTag("Wall")){
            contacts--;;
        }
        Debug.Log(contacts);
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


}
