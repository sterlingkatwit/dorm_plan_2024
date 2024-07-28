using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class UIEventHandler : MonoBehaviour
{
    public TMP_InputField ifX, ifY, ifZ;

    public TMP_InputField ofName;
    public TMP_InputField ofX;
    public TMP_InputField ofY;
    public TMP_InputField ofZ;
    public TMP_Text selectedObjDisplay, selectedObjTagDisplay, addTagText, freeSpaceText, totalObjsText;
    public TMP_InputField newTagInput;
    public Image objCreateImg, mainMenuImg,  toolBarImg, dropdownImg, menuConfirmImg;
    public Canvas canvMain;
    public TMP_Dropdown typeDropdown, typeDropdownEdit2D, typeDropdownEdit3D;
    public GameObject wallPrefab;
    public Transform wallParent;
    public Transform objParent;
    public bool RoomCreated = false;

    // Prefabs of furniture
    public GameObject objPrefab;
    public GameObject bedPrefab;
    public GameObject chairPrefab;
    public GameObject deskPrefab;
    public GameObject drawerPrefab;

    public Material outlineMaterial;
    [HideInInspector] public List<string> objectTypes = new List<string> { "", "Bed", "Chair", "Desk", "Drawer" };
    [HideInInspector] public List<GameObject> furnitureList = new List<GameObject>();
    [HideInInspector] public Dictionary<GameObject, float> furnitureFloorSpaceDict; 
    [HideInInspector] public float totalFloorSpace = 0f;


    [HideInInspector] public GameObject selectedWall;
    [HideInInspector] public Vector3 pointOnWall;


    [HideInInspector] public GameObject wallBottom, wallTop, wallLeft, wallRight, floor, obj;
    private float roomWidth, roomLength;

    void Start(){
        mainMenuImg.gameObject.SetActive(true);
        newTagInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        TypeDropdownStart();
        furnitureFloorSpaceDict = new Dictionary<GameObject, float>();
        CalculateFloorSpaceForFurniture();
        UpdateObjectCountText();
        UpdateFloorSpaceText();
    }

    void Update(){
        UpdateObjectCountText();
    }

    public void OnButtonPress()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        TMP_Text buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>();

        if (buttonName.Equals("RoomCreateBtn"))
        {
            if(ifX.text != null && ifY != null && ifZ != null){
                if (wallBottom == null)
                {
                    GenerateRoom();
                    mainMenuImg.gameObject.SetActive(false);
                    toolBarImg.gameObject.SetActive(true);
                }
                else
                {
                    Destroy(wallBottom.gameObject);
                    Destroy(wallTop.gameObject);
                    Destroy(wallLeft.gameObject);
                    Destroy(wallRight.gameObject);
                    Destroy(floor.gameObject);
                    GenerateRoom();
                }
            }
        }
        else if (buttonName.Equals("ObjCreateBtn"))
        {
            GenerateObj();
        }
        else if (buttonName.Equals("ObjTagAddBtn")){
            if(newTagInput.IsActive()){
                AddCustomType();
            }
            else{
                newTagInput.gameObject.SetActive(true);
                addTagText.gameObject.SetActive(true);
            }
        }
        else if (buttonName.Equals("DropdownBtn")){
            if(!dropdownImg.IsActive()){
                buttonText.text = "v";
                dropdownImg.gameObject.SetActive(true);
            } else {
                buttonText.text = "<";
                dropdownImg.gameObject.SetActive(false);
            }
        }
        else if (buttonName.Equals("MainMenuButton")){
            if(menuConfirmImg.IsActive()){
                menuConfirmImg.gameObject.SetActive(false);
            } else {
                menuConfirmImg.gameObject.SetActive(true);
            }
        }
        else if (buttonName.Equals("MenuConfirmBtn")){
            DestroyRoom();
            menuConfirmImg.gameObject.SetActive(false);
            toolBarImg.gameObject.SetActive(false);
            mainMenuImg.gameObject.SetActive(true);
        }
        else if (buttonName.Equals("MenuCancelBtn")){
            menuConfirmImg.gameObject.SetActive(false);
        }
    }

    void GenerateObj()
    {
        if (ofX.text != null && ofY.text != null && ofZ.text != null)
        {
            float x = castFloat(ofX.text);
            float y = castFloat(ofY.text);
            float z = castFloat(ofZ.text);

            obj = Instantiate(objPrefab, new Vector3(0, y/2, 0), Quaternion.identity);

            /*
            // change prefab based on tag
            GameObject newItemPrefab;
            string type = typeDropdown.options[typeDropdown.value].text;

            if (type.Equals("Bed"))
            {
                newItemPrefab = bedPrefab;
            } 
            else if (type.Equals("Chair"))
            {
                newItemPrefab = chairPrefab;
            } 
            else if (type.Equals("Desk"))
            {
                newItemPrefab = deskPrefab;
            } 
            else if (type.Equals("Drawer"))
            {
                newItemPrefab = drawerPrefab;
            }
            else
            {
                newItemPrefab = objPrefab;
            }

            obj = Instantiate(newItemPrefab, new Vector3(0, y / 2, 0), Quaternion.identity);*/
            obj.transform.SetParent(objParent);
            obj.transform.localScale = new Vector3(x, y, z);
            obj.name = ofName.text;
            obj.GetComponent<FurnitureInteraction>().currentPos = new Vector3(0, y/2, 0);
            //obj.GetComponent<FurnitureInteraction>().type = type;
            obj.GetComponent<FurnitureInteraction>().type = typeDropdown.options[typeDropdown.value].text;

            AddFurniture(obj);

            // Reset the window on creation
            ofX.text = ofY.text = ofZ.text = ofName.text = "";
            objCreateImg.gameObject.SetActive(false);
        }
    }

     void GenerateRoom()
    {
        if(ifX.text != null && ifY.text != null){
            float x = castFloat(ifX.text);
            float y = castFloat(ifY.text);
            float z = castFloat(ifZ.text);

            roomWidth = x;
            roomLength = y;

            // Half values to help build around (0,0). 0.1f accounts for the scaling of the walls and
            // ensures the area of the inside is x*y.
            float halfX = (x / 2) + 0.1f;
            float halfZ = (z / 2) + 0.1f;

            // Create and position the four walls around (0,0). In order: Bottom/Top/Left/Right
            wallBottom = BuildWall(new Vector3(0, y / 2, -halfZ), new Vector3(x, y, 0.2f), "BottomWall");
            wallTop = BuildWall(new Vector3(0, y / 2, halfZ), new Vector3(x, y, 0.2f), "TopWall");
            wallLeft = BuildWall(new Vector3(-halfX, y / 2, 0), new Vector3(0.2f, y, z), "LeftWall");
            wallRight = BuildWall(new Vector3(halfX, y / 2, 0), new Vector3(0.2f, y, z), "RightWall");
            floor = BuildWall(new Vector3(0, -0.1f, 0), new Vector3(x+0.4f, 0.2f, z+0.4f), "Floor");

            Renderer floorRenderer = floor.GetComponent<Renderer>();
            Material floorMaterial = new Material(Shader.Find("Standard"));
            floorMaterial.color = Color.gray;
            floorRenderer.material = floorMaterial;
            floor.tag = "Floor";
            wallBottom.tag = wallTop.tag = "WallZ";
            wallLeft.tag = wallRight.tag = "WallX";

            //Now allow camera controls
            RoomCreated = true;
        }
    }

    public void GenerateRoomForLoad(){
    
        wallBottom = BuildWall(new Vector3(1, 1, 1), new Vector3(1, 1, 1), "BottomWall");
        wallTop = BuildWall(new Vector3(1, 1, 1), new Vector3(1, 1, 1), "TopWall");
        wallLeft = BuildWall(new Vector3(1, 1, 1), new Vector3(1, 1, 1), "LeftWall");
        wallRight = BuildWall(new Vector3(1, 1, 1), new Vector3(1, 1, 1), "RightWall");
        floor = BuildWall(new Vector3(1, 1, 1), new Vector3(1, 1, 1), "Floor");

        Renderer floorRenderer = floor.GetComponent<Renderer>();
        Material floorMaterial = new Material(Shader.Find("Standard"));
        floorMaterial.color = Color.gray;
        floorRenderer.material = floorMaterial;
        floor.tag = "Floor";
        wallBottom.tag = wallTop.tag = "WallZ";
        wallLeft.tag = wallRight.tag = "WallX";

        RoomCreated = true;
    }

    public void DestroyRoom(){
        if (RoomCreated){
            Destroy(wallBottom.gameObject);
            Destroy(wallTop.gameObject);
            Destroy(wallLeft.gameObject);
            Destroy(wallRight.gameObject);
            Destroy(floor.gameObject);

            if(wallParent.childCount > 0){
                foreach(Transform child in wallParent){
                    Destroy(child.gameObject);
                }
            }
            RoomCreated = false;
        }
    }

    public float castFloat(String inp){
        float x = 0f;
        float.TryParse(inp, out x);
        return x;
    }


    GameObject BuildWall(Vector3 position, Vector3 scale, String name)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
        wall.transform.localScale = scale;
        wall.transform.parent = wallParent;
        wall.name = name;

        Renderer wallRenderer = wall.GetComponent<Renderer>();
        Material wallMaterial = new Material(Shader.Find("Standard"));
        wallMaterial.color = new Color(0.8f, 0.8f, 0.8f);
        wallRenderer.material = wallMaterial;

        return wall;
    } 


    void TypeDropdownStart(){
        tagDropdownUpdate();

        typeDropdown.value = 0;
        typeDropdown.RefreshShownValue();
    }

    void AddCustomType(){
        if(newTagInput.IsActive() && !newTagInput.text.Equals("")){
            objectTypes.Add(newTagInput.text);
            tagDropdownUpdate();
            newTagInput.gameObject.SetActive(false);
            addTagText.gameObject.SetActive(false);
        }
    }

    void OnInputFieldEndEdit(string text){
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            AddCustomType();
        }
    }

    public void tagDropdownUpdate(){
        typeDropdown.ClearOptions();
        typeDropdown.AddOptions(objectTypes);
        typeDropdownEdit2D.ClearOptions();
        typeDropdownEdit2D.AddOptions(objectTypes);
        typeDropdownEdit3D.ClearOptions();
        typeDropdownEdit3D.AddOptions(objectTypes);
    }


    // Method to add a new furniture object and update related information
    public void AddFurniture(GameObject furniture)
    {
        AddFurnitureToList(furniture);
        UpdateObjectCountText();
        UpdateFloorSpaceText();
    }

    // Method to update the text element with the total floor space

    public void UpdateFloorSpaceText(){
        float totalFloorSpace = CalculateTotalFloorSpace();
        float roomTotalSpace = roomWidth * roomLength;
        freeSpaceText.text = totalFloorSpace.ToString("F2") + " / " + roomTotalSpace.ToString("F2") + " ft²";
    }

    public void UpdateObjectCountText()
    {
        // Count number of child objects minus one for Clipboard
        int objectCount = objParent.childCount - 1;
        totalObjsText.text = objectCount.ToString();
    }

    // Method to calculate and store the floor space for each furniture object
    void CalculateFloorSpaceForFurniture()
    {
        furnitureFloorSpaceDict.Clear(); // Clear the dictionary to recalculate all values
        foreach (GameObject furniture in furnitureList)
        {
            Renderer furnitureRenderer = furniture.GetComponent<Renderer>();
            if (furnitureRenderer != null)
            {
                float floorSpace = furnitureRenderer.bounds.size.x * furnitureRenderer.bounds.size.z;
                furnitureFloorSpaceDict[furniture] = floorSpace;
            }
        }
    }

    // Method to add a new furniture object to the list and calculate its floor space
    public void AddFurnitureToList(GameObject newFurniture)
    {
        if (!furnitureList.Contains(newFurniture))
        {
            furnitureList.Add(newFurniture);
            Renderer furnitureRenderer = newFurniture.GetComponent<Renderer>();
            if (furnitureRenderer != null)
            {
                float floorSpace = furnitureRenderer.bounds.size.x * furnitureRenderer.bounds.size.z;
                furnitureFloorSpaceDict[newFurniture] = floorSpace;
            }
        }
    }

    // Method to get the floor space of a specific furniture object
    public float GetFurnitureFloorSpace(GameObject furniture)
    {
        if (furnitureFloorSpaceDict.ContainsKey(furniture))
        {
            return furnitureFloorSpaceDict[furniture];
        }
        return 0f;
    }

    // Method to calculate the total floor space occupied by all furniture objects
    public float CalculateTotalFloorSpace()
    {
        float newFloorSpace = 0f;
        foreach (var entry in furnitureFloorSpaceDict)
        {
            newFloorSpace += entry.Value;
        }
        totalFloorSpace = newFloorSpace;
        return totalFloorSpace;
    }

    // Method to remove a furniture object from the list and update related information
    public void RemoveFurniture(GameObject furnitureToRemove)
    {
        if (furnitureList.Contains(furnitureToRemove))
        {
            furnitureList.Remove(furnitureToRemove);
            if (furnitureFloorSpaceDict.ContainsKey(furnitureToRemove))
            {
                furnitureFloorSpaceDict.Remove(furnitureToRemove);
            }
            UpdateFloorSpaceText();
            UpdateObjectCountText();
        }
    }

    public void ResetFurniture()
    {
        furnitureList.Clear();
        furnitureFloorSpaceDict.Clear();
    }
}
