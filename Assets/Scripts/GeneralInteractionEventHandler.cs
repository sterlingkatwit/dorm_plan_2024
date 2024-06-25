using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GeneralInteractionEventHandler : MonoBehaviour
{

    [HideInInspector] public GameObject obj;
    public ObjEditUIEventHandler objEditEH;
    public UIEventHandler uiEH;
    public CameraEventHandler camEH;
    public Transform objParent, clipboard;
    public Image roomEdit;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        openObjectCreate();
        pasteObject();
    }

    

    void openObjectCreate(){

        // Opens the object creation window. Extra logic in here to make it appear on mouse cursor
        // and make sure it doesn't go off-screen at all.

        if (Input.GetMouseButtonDown(1)){
            if (!EventSystem.current.IsPointerOverGameObject() && !IsPointerOverGameObject("Object") && !IsPointerOverGameObject("Wall") &&
             !IsPointerOverGameObject("WallZ") && !IsPointerOverGameObject("WallX") && !IsPointerOverGameObject("Window") && !IsPointerOverGameObject("Door")){
                if (uiEH.objCreateImg.gameObject != null){

                    if(objEditEH.objEditWindow2D.IsActive() || objEditEH.objEditWindow3D.IsActive() || roomEdit.IsActive()){
                        objEditEH.objEditWindow2D.gameObject.SetActive(false);
                        objEditEH.objEditWindow3D.gameObject.SetActive(false);
                        roomEdit.gameObject.SetActive(false);
                    }

                    uiEH.objCreateImg.gameObject.SetActive(true);
                    Vector3 mousePosition = Input.mousePosition;

                    // Convert the screen position to a local position within the canvas
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(uiEH.canvMain.transform as RectTransform, mousePosition, uiEH.canvMain.worldCamera, out Vector2 localPoint);

                    RectTransform rectTransform = uiEH.objCreateImg.gameObject.GetComponent<RectTransform>();
                    Vector2 adjustedPosition = localPoint - new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);

                    // Checks that the window stays within the screen
                    float halfWidth = rectTransform.rect.width / 2;
                    float halfHeight = rectTransform.rect.height / 2;
                    float canvasWidth = uiEH.canvMain.GetComponent<RectTransform>().rect.width;
                    float canvasHeight = uiEH.canvMain.GetComponent<RectTransform>().rect.height;


                    if (adjustedPosition.x - halfWidth < -canvasWidth / 2){
                        adjustedPosition.x = -canvasWidth / 2 + halfWidth;
                    }
                    else if (adjustedPosition.x + halfWidth > canvasWidth / 2){
                        adjustedPosition.x = canvasWidth / 2 - halfWidth;
                    }


                    if (adjustedPosition.y - halfHeight < -canvasHeight / 2){
                        adjustedPosition.y = -canvasHeight / 2 + halfHeight;
                    }
                    else if (adjustedPosition.y + halfHeight > canvasHeight / 2){
                        adjustedPosition.y = canvasHeight / 2 - halfHeight;
                    }


                    rectTransform.localPosition = adjustedPosition;
                    }
                else{
                    Debug.LogWarning("UI Window reference is not set.");
                }
            }
        }
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()){
            uiEH.objCreateImg.gameObject.SetActive(false);
        }
    }

    public bool IsPointerOverGameObject(string tag){

        // Helper function
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit)){
            return hit.collider.CompareTag(tag);
        }
        return false;
    }


    void pasteObject(){
        if(Input.GetKeyDown(KeyCode.V) && clipboard.childCount != 0){
            GameObject pasting = clipboard.GetChild(0).gameObject;
            Transform state = pasting.gameObject.GetComponent<Transform>();
            obj = Instantiate(pasting, new Vector3(state.position.x, state.position.y, state.position.z), Quaternion.identity);
            obj.transform.parent = objParent;
            obj.SetActive(true);
        }
    }
}
