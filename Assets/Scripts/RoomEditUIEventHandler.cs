using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class RoomEditUIEventHandler : MonoBehaviour
{
    public Camera mainCamera;
    public Image wallEditWin, wallObjEditWin, objEdit2Win, objEdit3Win, objCreateWin;
    public TMP_InputField windowX, windowY, winEditX, winEditY;
    public Button editWallButton;
    public Toggle windowOrDoor;
    public UIEventHandler uiEH;
    public GeneralInteractionEventHandler genIntEH;
    public ObjEditUIEventHandler objEditEH;
    public GameObject windowPrefab, doorPrefab;
    public Transform wallObjects;
    [HideInInspector] public GameObject objectSelected, objCurrentEdit;
    private Camera uiCamera;
    private float initialZScale;



    void Start(){
        uiCamera = uiEH.canvMain.worldCamera;
        initialZScale = 0.1f;
    }

    void Update(){
        DetectRightClickOnWall();
    }


    public void OnButtonPress(){
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        if (buttonName.Equals("RoomEditButton")){
            if(windowOrDoor.isOn){
                AddWindow();
            } else {
                AddDoor();
            }
            windowX.text = "";
            windowY.text = "";
        }
        // else if (buttonName.Equals("RoomObjEditBtn")){
        //     WindowEditFunc();
        // }

    }



    void DetectRightClickOnWall(){
        if(!mainCamera.orthographic){
            if (Input.GetMouseButtonDown(1)){
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)){
                    if (hit.collider.CompareTag("WallX") || hit.collider.CompareTag("WallZ")){
                        OpenUIWindow(hit.point, hit.collider.gameObject);
                    }
                    // else if (hit.collider.CompareTag("Window") || hit.collider.CompareTag("Door")){
                    //     OpenUIWindow(hit.point, hit.collider.gameObject);
                    // }
                }
            }
            if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()){
                wallEditWin.gameObject.SetActive(false);
                wallObjEditWin.gameObject.SetActive(false);
            }
        }
    }
    void OpenUIWindow(Vector3 position, GameObject hitObject)
    {
        if (hitObject.CompareTag("WallX") || hitObject.CompareTag("WallZ"))
        {
            if (wallEditWin != null)
            {
                if (wallObjEditWin.IsActive())
                {
                    wallObjEditWin.gameObject.SetActive(false);
                }
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(position);

                RectTransform rectTransform = wallEditWin.GetComponent<RectTransform>();
                rectTransform.position = screenPosition;

                wallEditWin.gameObject.SetActive(true);

                uiEH.selectedWall = hitObject;
                uiEH.pointOnWall = position;

            }
        }
        // else if (hitObject.CompareTag("Window") || hitObject.CompareTag("Door"))
        // {
        //     if (wallObjEditWin != null){
        //         if (wallEditWin.IsActive())
        //         {
        //             wallEditWin.gameObject.SetActive(false);
        //         }

        //         objCurrentEdit = objectSelected;


        //         Vector3 screenPosition = mainCamera.WorldToScreenPoint(position);

        //         RectTransform rectTransform = wallObjEditWin.GetComponent<RectTransform>();
        //         rectTransform.position = screenPosition;

        //         wallObjEditWin.gameObject.SetActive(true);

        //         if (wallObjEditWin != null && objectSelected != null)
        //         {
        //             // Determine if the object is on a WallX or WallZ
        //             bool isOnWallX = objectSelected.transform.parent.CompareTag("WallX");
        //             bool isOnWallZ = objectSelected.transform.parent.CompareTag("WallZ");

        //             // Get the size of the object
        //             Vector3 size = objectSelected.GetComponent<Renderer>().bounds.size;

        //             if (isOnWallZ)
        //             {
        //                 winEditX.text = size.x.ToString();
        //                 winEditY.text = size.y.ToString();
        //             }
        //             else if (isOnWallX)
        //             {
        //                 winEditX.text = size.z.ToString();
        //                 winEditY.text = size.y.ToString();
        //             }
        //             else
        //             {
        //                 Debug.LogError("Object is not on a WallX or WallZ.");
        //             }

        //         }

        //         else{
        //             Debug.LogWarning("UI Window reference is not set.");
        //         }
        //     }
        // }

        if (objEdit2Win.IsActive() || objEdit3Win.IsActive() || objCreateWin.IsActive())
        {
            objEdit2Win.gameObject.SetActive(false);
            objEdit3Win.gameObject.SetActive(false);
            objCreateWin.gameObject.SetActive(false);
        }
    }


    // I MIGHT DROP THIS SHIT. NO EDIT JUST DELETE AND MAKE A NEW ONE


    // void WindowEditFunc()
    // {
    //     if (genIntEH.IsPointerOverGameObject("Window") || genIntEH.IsPointerOverGameObject("Door"))
    //     {
    //         if (winEditX.text != null && winEditY.text != null)
    //         {
    //             // Get the new dimensions from the input fields
    //             float newWidth = uiEH.castFloat(winEditX.text);
    //             float newHeight = uiEH.castFloat(winEditY.text);

    //             // Determine if the object is on a WallX or WallZ
    //             bool isOnWallX = objCurrentEdit.transform.parent.CompareTag("WallX");
    //             bool isOnWallZ = objCurrentEdit.transform.parent.CompareTag("WallZ");

    //             // Get the parent's scale
    //             Vector3 parentScale = objCurrentEdit.transform.parent.lossyScale;

    //             // Calculate the desired local scale based on wall orientation
    //             if (isOnWallZ)
    //             {
    //                 objCurrentEdit.transform.localScale = new Vector3(newWidth / parentScale.x, newHeight / parentScale.y, initialZScale / parentScale.z);
    //             }
    //             else if (isOnWallX)
    //             {
    //                 objCurrentEdit.gameObject.GetComponent<Renderer>().bounds.size.Set(initialZScale / parentScale.x, newHeight / parentScale.y, newWidth / parentScale.z);
    //                 // objCurrentEdit.transform.localScale = new Vector3(initialZScale / parentScale.x, newHeight / parentScale.y, newWidth / parentScale.z);
    //             }
    //             else
    //             {
    //                 Debug.LogError("Object is not on a WallX or WallZ.");
    //             }
    //         }
    //     }
    // }
    


    public void AddWindow()
    {
        float windowWidth = float.Parse(windowX.text);
        float windowHeight = float.Parse(windowY.text);

        Vector3 windowPosition = uiEH.pointOnWall;
        Quaternion windowRotation = Quaternion.identity;

        if (uiEH.selectedWall.CompareTag("WallX"))
        {
            windowRotation = Quaternion.Euler(0, 90, 0);
            windowPosition += new Vector3(0, 0, -windowWidth / 2);
        }
        else if (uiEH.selectedWall.CompareTag("WallZ"))
        {
            windowPosition += new Vector3(-windowWidth / 2, 0, 0);
        }

        GameObject window = Instantiate(windowPrefab, windowPosition, windowRotation);
        window.transform.localScale = new Vector3(windowWidth, windowHeight, 0.1f);
        window.GetComponent<SelectableObject>().parentWall = uiEH.selectedWall;
        window.transform.SetParent(wallObjects, true);

        Renderer windowRenderer = window.GetComponent<Renderer>();
        if (windowRenderer != null)
        {
            Material windowMaterial = new Material(Shader.Find("Standard"));
            windowMaterial.color = Color.gray;
            windowRenderer.material = windowMaterial;
        }

        wallEditWin.gameObject.SetActive(false);
    }

    public void AddDoor()
    {
        float doorWidth = float.Parse(windowX.text);
        float doorHeight = float.Parse(windowY.text);

        Vector3 doorPosition = uiEH.pointOnWall;
        Quaternion doorRotation = Quaternion.identity;

        if (uiEH.selectedWall.CompareTag("WallX"))
        {
            doorRotation = Quaternion.Euler(0, 90, 0);
            doorPosition += new Vector3(0, 0, -doorWidth / 2);
        }
        else if (uiEH.selectedWall.CompareTag("WallZ"))
        {
            doorPosition += new Vector3(-doorWidth / 2, 0, 0);
        }

        Vector3 wallPosition = uiEH.selectedWall.transform.position;
        Vector3 wallScale = uiEH.selectedWall.transform.localScale;
        if (uiEH.selectedWall.CompareTag("WallX"))
        {
            float halfWallDepth = wallScale.z / 2;
            doorPosition.z = Mathf.Clamp(doorPosition.z, wallPosition.z - halfWallDepth + (doorWidth / 2), wallPosition.z + halfWallDepth - (doorWidth / 2));
        }
        else if (uiEH.selectedWall.CompareTag("WallZ"))
        {
            float halfWallWidth = wallScale.x / 2;
            doorPosition.x = Mathf.Clamp(doorPosition.x, wallPosition.x - halfWallWidth + (doorWidth / 2), wallPosition.x + halfWallWidth - (doorWidth / 2));
        }

        GameObject door = Instantiate(doorPrefab, doorPosition, doorRotation);
        door.transform.localScale = new Vector3(doorWidth, doorHeight, 0.1f);
        door.GetComponent<SelectableObject>().parentWall = uiEH.selectedWall;
        door.transform.SetParent(wallObjects, true);

        Renderer doorRenderer = door.GetComponent<Renderer>();
        if (doorRenderer != null)
        {
            Material doorMaterial = new Material(Shader.Find("Standard"));
            doorMaterial.color = Color.gray;
            doorRenderer.material = doorMaterial;
        }

        wallEditWin.gameObject.SetActive(false);
    }


}
