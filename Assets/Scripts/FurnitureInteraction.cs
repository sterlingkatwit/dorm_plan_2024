using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FurnitureInteraction : MonoBehaviour
{

    private bool clickHold = false;
    private Rigidbody rb;
    private int contacts;
    private Vector3 currentPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(clickHold){
            moveObject();
        }
        else if(contacts == 0){
            transform.position = currentPos;    
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


    void OnMouseDown()
    {
        clickHold = true;        
    }

    void OnMouseUp()
    {
        clickHold = false;
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
