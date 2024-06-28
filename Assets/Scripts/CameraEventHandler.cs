using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CameraEventHandler : MonoBehaviour
{
    [SerializeField] public UIEventHandler uiEH;
    [HideInInspector] public bool camSwitched = false;
    public Camera mainCam;
    public Button camCW, camCCW, camFree, camOrtho;
    private int camState = 0;
    private float camDist = 10f;

    private bool roomSizeFlag = true;

    private bool freeEnabled = false;
    private float dragSpeed = 12;
    private Vector3 dragOrigin;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(uiEH.RoomCreated && roomSizeFlag){
            camFree.gameObject.SetActive(true);
            camOrtho.gameObject.SetActive(true);
            orthoAdjust();
            roomSizeFlag = false;
        }

        //If free-cam is enabled, can mouse down to move camera while in 2D mode.
        if(freeEnabled && mainCam.orthographic){

            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 difference = mainCam.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
                Vector3 move = new Vector3(difference.x * dragSpeed, 0, difference.y * dragSpeed);

                Vector3 newPosition = mainCam.transform.position + move;
                mainCam.transform.position = newPosition;
                dragOrigin = Input.mousePosition;
            }
        }
    }

    public void OnButtonPress(){
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        TMP_Text buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>();

        //Switches camera between orthographic and perspective
        if(buttonName.Equals("CameraOrthoToggle")){
            if(buttonText.text.Equals("2D")){
                camSwitched = true;

                //Make 3D controls inaccessible.
                camCW.gameObject.SetActive(false);
                camCCW.gameObject.SetActive(false);
                camFree.gameObject.SetActive(true);

                //Reset 2D camera position.
                mainCam.orthographic = true;
                mainCam.transform.position = new Vector3(0, camDist, 0);
                mainCam.transform.rotation = Quaternion.Euler(90, 0, 0);
                orthoAdjust();

                //All walls active.
                roomState(-1);

                buttonText.SetText("3D");
            }
            else{

                //Make 3D controls accessible.
                camCW.gameObject.SetActive(true);
                camCCW.gameObject.SetActive(true);
                camFree.gameObject.SetActive(false);


                //Set 3D camera position. Last 3D position should be saved by camState.
                mainCam.orthographic = false;
                cameraOrientation();

                buttonText.SetText("2D");
            }
        }
        else if(buttonName.Equals("CameraMoveCW")){
            camState++;
            if(camState > 3){
                camState = 0;
            }
            cameraOrientation();
        }
        else if(buttonName.Equals("CameraMoveCounterCW")){
            camState--;
            if(camState < 0){
                camState = 3;
            }
            cameraOrientation();
        }
        else if(buttonName.Equals("CameraFreeCam")){
            if(buttonText.text.Equals("Free")){

                freeEnabled = true;
                buttonText.SetText("Lock");
            }
            else if(buttonText.text.Equals("Lock")){

                freeEnabled = false;
                buttonText.SetText("Free");     
            }
        }
    }

    private void orthoAdjust(){
        float wallsX = uiEH.wallBottom.gameObject.transform.localScale.x;
        float wallsZ = uiEH.wallLeft.gameObject.transform.localScale.z;

        camDist = Math.Max(wallsX,wallsZ) + 2f;
        mainCam.orthographicSize = camDist-3f;
    }


    private void cameraOrientation(){
        if(uiEH.RoomCreated){

            float wallsX = uiEH.wallBottom.gameObject.transform.localScale.x;
            float wallsZ = uiEH.wallLeft.gameObject.transform.localScale.z;

            camDist = Math.Max(wallsX,wallsZ) + 2f;

            if(camState.Equals(0)){
                mainCam.transform.position = new Vector3(0, 3, -camDist);
                mainCam.transform.rotation = Quaternion.Euler(10, 0, 0);
                uiEH.wallBottom.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(1)){
                mainCam.transform.position = new Vector3(-camDist, 3, 0);
                mainCam.transform.rotation = Quaternion.Euler(10, 90, 0);

                uiEH.wallLeft.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(2)){
                mainCam.transform.position = new Vector3(0, 3, camDist);
                mainCam.transform.rotation = Quaternion.Euler(10, 180, 0);
                uiEH.wallTop.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(3)){
                mainCam.transform.position = new Vector3(camDist, 3, 0);
                mainCam.transform.rotation = Quaternion.Euler(10, 270, 0);
                uiEH.wallRight.SetActive(false);
                roomState(camState);
            }
        }
        
    }

    private void roomState(int cur){
        if(uiEH.RoomCreated){
            if(!cur.Equals(0)){
                uiEH.wallBottom.SetActive(true);
            }
            if(!cur.Equals(1)){
                uiEH.wallLeft.SetActive(true);
            }
            if(!cur.Equals(2)){
                uiEH.wallTop.SetActive(true);
            }
            if(!cur.Equals(3)){
                uiEH.wallRight.SetActive(true);
            }
        }
    }
}
