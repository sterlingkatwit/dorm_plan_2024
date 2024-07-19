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

    private float zoomSpeed = 2f;
    private float minZoom = 5f;
    private float maxZoom = 50f;
    private GameObject previousWall = null;



    // Start is called before the first frame update
    void Start()
    {
        // So wall will get reset after camera is rotated away.
        previousWall = uiEH.wallBottom;
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
        MoveCamera();
        CamZoom();
        RotateCamera();
    }

    private void CamZoom(){
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (mainCam.orthographic && freeEnabled){
            mainCam.orthographicSize -= scroll * zoomSpeed;
            mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minZoom, maxZoom);
        }
    }

    private void RotateCamera(){
        if(!mainCam.orthographic){
            if (Input.GetMouseButtonDown(0)){
                dragOrigin = Input.mousePosition;
            }
            if (Input.GetMouseButton(0)){
                Vector3 difference = Input.mousePosition - dragOrigin;
                //float rotationX = difference.y * dragSpeed * Time.deltaTime;
                float rotationY = difference.x * dragSpeed * Time.deltaTime;

                mainCam.transform.RotateAround(Vector3.zero, Vector3.up, rotationY);
                //mainCam.transform.RotateAround(Vector3.zero, mainCam.transform.right, rotationX);

                dragOrigin = Input.mousePosition;
                DetectWallIntersection();
            }
        }
    }

    private void MoveCamera(){
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

            camDist = Math.Max(wallsX,wallsZ) + 4f;

            if(camState.Equals(0)){
                mainCam.transform.position = new Vector3(0, 5, -camDist);
                mainCam.transform.rotation = Quaternion.Euler(12, 0, 0);
                uiEH.wallBottom.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(1)){
                mainCam.transform.position = new Vector3(-camDist, 5, 0);
                mainCam.transform.rotation = Quaternion.Euler(12, 90, 0);

                uiEH.wallLeft.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(2)){
                mainCam.transform.position = new Vector3(0, 5, camDist);
                mainCam.transform.rotation = Quaternion.Euler(12, 180, 0);
                uiEH.wallTop.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(3)){
                mainCam.transform.position = new Vector3(camDist, 5, 0);
                mainCam.transform.rotation = Quaternion.Euler(12, 270, 0);
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
    private void DetectWallIntersection()
    {
        Vector3 direction = Vector3.zero - mainCam.transform.position;
        float rayLength = direction.magnitude;
        Ray ray = new Ray(mainCam.transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            if(hit.collider.CompareTag("WallX") || hit.collider.CompareTag("WallZ")){
                GameObject hitWall = hit.collider.gameObject;

                if (previousWall != null && previousWall != hitWall)
                {
                    previousWall.SetActive(true);
                }

                hitWall.SetActive(false);
                previousWall = hitWall;

                if(hitWall.name.Equals("BottomWall")){
                    camState = 0;
                    roomState(camState);
                }
                else if(hitWall.name.Equals("LeftWall")){
                    camState = 1;
                    roomState(camState);
                }  
                else if(hitWall.name.Equals("TopWall")){
                    camState = 2;
                    roomState(camState);
                }                
                else if(hitWall.name.Equals("RightWall")){
                    camState = 3;
                    roomState(camState);
                }
            }
        }
        else
        {
            if (previousWall != null)
            {
                previousWall.SetActive(true);
                previousWall = null;
            }
        }
    }
}
