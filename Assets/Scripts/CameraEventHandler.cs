using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraEventHandler : MonoBehaviour
{
    [SerializeField] private UIEventHandler uiEH;
    public Camera mainCam;
    public Button camCW, camCCW;
    private int camState = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonPress(){
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        TMP_Text buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>();

        //Switches camera between orthographic and perspective
        if(buttonName.Equals("CameraOrthoToggle")){
            if(buttonText.text.Equals("2D")){

                //Make 3D controls inaccessible.
                camCW.gameObject.SetActive(false);
                camCCW.gameObject.SetActive(false);

                //Reset 2D camera position.
                mainCam.orthographic = true;
                mainCam.transform.position = new Vector3(0, 10, 0);
                mainCam.transform.rotation = Quaternion.Euler(90, 0, 0);
                mainCam.rect = new Rect(0,0,0.85f,1);

                //All walls active.
                roomState(-1);

                buttonText.SetText("3D");
            }
            else{

                //Make 3D controls accessible.
                camCW.gameObject.SetActive(true);
                camCCW.gameObject.SetActive(true);

                //Set 3D camera position. Last 3D position should be saved by camState.
                mainCam.orthographic = false;
                mainCam.rect = new Rect(0,0,0.85f,1);
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
                camState = 0;
            }
            cameraOrientation();
        }
    }


    private void cameraOrientation(){
        if(uiEH.RoomCreated){
            if(camState.Equals(0)){
                mainCam.transform.position = new Vector3(0, 3, -10);
                mainCam.transform.rotation = Quaternion.Euler(10, 0, 0);
                uiEH.wallBottom.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(1)){
                mainCam.transform.position = new Vector3(-10, 3, 0);
                mainCam.transform.rotation = Quaternion.Euler(10, 90, 0);

                uiEH.wallLeft.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(2)){
                mainCam.transform.position = new Vector3(0, 3, 10);
                mainCam.transform.rotation = Quaternion.Euler(10, 180, 0);
                uiEH.wallTop.SetActive(false);
                roomState(camState);
            }
            else if(camState.Equals(3)){
                mainCam.transform.position = new Vector3(10, 3, 0);
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
