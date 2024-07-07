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
    public TMP_Text selectedObjDisplay, selectedObjTagDisplay, addTagText, freeSpaceText;
    public TMP_InputField newTagInput;
    public Image objCreateImg, roomCreateImg, toolBarImg, dropdownImg;
    public Canvas canvMain;
    public TMP_Dropdown typeDropdown, typeDropdownEdit2D, typeDropdownEdit3D;
    public GameObject wallPrefab;
    public GameObject objPrefab;
    public Transform wallParent;
    public Transform objParent;
    public bool RoomCreated = false;

    public Material outlineMaterial;
    [HideInInspector] public List<string> objectTypes = new List<string> { "", "Bed", "Chair", "Desk", "Drawer" };
    [HideInInspector] public List<GameObject> furnitureList = new List<GameObject>();


    [HideInInspector] public GameObject selectedWall;
    [HideInInspector] public Vector3 pointOnWall;


    [HideInInspector] public GameObject wallBottom, wallTop, wallLeft, wallRight, floor, obj;
    private float roomWidth, roomLength;

    void Start(){
        roomCreateImg.gameObject.SetActive(true);
        newTagInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        TypeDropdownStart();
    }

    public void OnButtonPress()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        TMP_Text buttonText = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>();

        if (buttonName.Equals("RoomCreateBtn"))
        {
            if (wallBottom == null)
            {
                GenerateRoom();
                roomCreateImg.gameObject.SetActive(false);
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
    }

    void GenerateObj()
    {
        if (ofX.text != null && ofY.text != null && ofZ.text != null)
        {
            float x = castFloat(ofX.text);
            float y = castFloat(ofY.text);
            float z = castFloat(ofZ.text);

            obj = Instantiate(objPrefab, new Vector3(0, y/2, 0), Quaternion.identity);
            obj.transform.SetParent(objParent);
            obj.transform.localScale = new Vector3(x, y, z);
            obj.name = ofName.text;
            obj.GetComponent<FurnitureInteraction>().currentPos = new Vector3(0, y/2, 0);
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

    public void AddFurniture(GameObject furniture){
        furnitureList.Add(furniture);
        UpdateFloorSpaceText();
    }

    private float CalculateTotalFloorSpace(){
        float totalFloorSpace = 0f;

        foreach (GameObject furniture in furnitureList){
            Vector3 furnitureSize = GetFurnitureSize(furniture);
            totalFloorSpace += furnitureSize.x * furnitureSize.z;
        }

        return totalFloorSpace;
    }

    private Vector3 GetFurnitureSize(GameObject furniture){
        MeshFilter meshFilter = furniture.GetComponent<MeshFilter>();
        Collider collider = furniture.GetComponent<Collider>();

        Vector3 size = Vector3.zero;

        if (meshFilter != null){
            Vector3 meshSize = meshFilter.mesh.bounds.size;
            Vector3 objectScale = furniture.transform.localScale;
            size = Vector3.Scale(meshSize, objectScale);
        } else if (collider != null){
            Vector3 colliderSize = collider.bounds.size;
            Vector3 objectScale = furniture.transform.localScale;
            size = new Vector3(
                colliderSize.x / furniture.transform.lossyScale.x,
                colliderSize.z / furniture.transform.lossyScale.z,
                colliderSize.y / furniture.transform.lossyScale.y
            );
        }

        return size;
    }

    // Method to update the text element with the total floor space
    private void UpdateFloorSpaceText(){
        float totalFloorSpace = CalculateTotalFloorSpace();
        float roomTotalSpace = roomWidth * roomLength;
        freeSpaceText.text = totalFloorSpace.ToString("F2") + " / " + roomTotalSpace.ToString("F2") + " ft²";
    }
}