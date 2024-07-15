using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjEditUIEventHandler : MonoBehaviour
{
    public Toggle togX, togY;
    public TMP_InputField incrementValue, editX, editY, editZ, editName, editX3D, editY3D, editZ3D, editName3D, objTagEditCreate2D, objTagEditCreate3D;
    public TMP_Text rotateIncrement, currentRotation, rotateIncrement3D, currentRotation3D, objTagEditText2D, objTagEditText3D;
    public TMP_Dropdown colorDropdown, objTagEditSelect2D, objTagEditSelect3D;
    private Dictionary<string, Color> colorDictionary;

    public Camera mainCam;
    public Image objEditWindow2D, objEditWindow3D, roomEdit;
    public Canvas canvMain;
    [HideInInspector] public GameObject objectSelected;
    [HideInInspector] public GameObject objCurrentEdit;
    public GeneralInteractionEventHandler genIntEH;
    public UIEventHandler uiEH;
    [HideInInspector] public int interact3Dvalue, incTextValue, IncrementRotate, incrementArray;
    [HideInInspector] public float Increment;

    // Start is called before the first frame update
    void Start()
    {
        incTextValue = 0;
        interact3Dvalue = 0;
        Increment = 1;
        incrementArray = 2;
        IncrementRotate = 15;
        togX.onValueChanged.AddListener(delegate { OnToggleChanged(togX); });
        togY.onValueChanged.AddListener(delegate { OnToggleChanged(togY); });
        incrementValue.onEndEdit.AddListener(OnInputFieldEndEditRotate);
        objTagEditCreate2D.onEndEdit.AddListener(OnInputFieldEndEditTag2D);
        objTagEditCreate3D.onEndEdit.AddListener(OnInputFieldEndEditTag3D);

        colorDropdownStart();
        TypeDropdownStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(mainCam.orthographic){
            ObjectEditWindow(objEditWindow2D);
        }
        else{
            ObjectEditWindow(objEditWindow3D);
        }

        changeIncrement();
        ObjectRotate();
    }


    public void OnButtonPress(){
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        if (buttonName.Equals("ObjEditBtn")){
            editFunctionality();
            uiEH.RemoveFurniture(objCurrentEdit);
            uiEH.AddFurniture(objCurrentEdit);
        }
        else if(buttonName.Equals("ObjTagAddBtn2D")){
            if(objTagEditCreate2D.IsActive()){
                AddCustomType(false);
            }
            else{
                objTagEditCreate2D.gameObject.SetActive(true);
                objTagEditText2D.gameObject.SetActive(true);
            }
        }
        else if(buttonName.Equals("ObjTagAddBtn3D")){
            if(objTagEditCreate3D.IsActive()){
                AddCustomType(true);
            }
            else{
                objTagEditCreate3D.gameObject.SetActive(true);
                objTagEditText3D.gameObject.SetActive(true);
            }
        }

    }


    void ObjectEditWindow(Image window){

        // Opens the object creation window. Extra logic in here to make it appear on mouse cursor
        // and make sure it doesn't go off-screen at all.

        if (Input.GetMouseButtonDown(1)){

            if (genIntEH.IsPointerOverGameObject("Object")){
                
                if (window.gameObject != null && objectSelected != null){

                    if(uiEH.objCreateImg.IsActive()){
                        uiEH.objCreateImg.gameObject.SetActive(false);
                    }
                    if(roomEdit.IsActive()){
                        roomEdit.gameObject.SetActive(false);
                    }

                    window.gameObject.SetActive(true);


                    MeshFilter meshFilter = objectSelected.GetComponent<MeshFilter>();

                    // Makes sure when objecct dimensions are loaded up that they represent the actual dimensions.
                    if (meshFilter != null){
                        Vector3 meshSize = meshFilter.mesh.bounds.size;
                        Vector3 objectScale = objectSelected.transform.localScale;
                        Vector3 trueSize = Vector3.Scale(meshSize, objectScale);

                        editX.text = editX3D.text = trueSize.x.ToString();
                        editY.text = editY3D.text = trueSize.y.ToString();
                        editZ.text = editZ3D.text = trueSize.z.ToString();
                    }

                    editName.text = editName3D.text = objectSelected.name;
                    objTagEditSelect2D.value = objTagEditSelect3D.value = uiEH.objectTypes.IndexOf(objectSelected.GetComponent<FurnitureInteraction>().type);
                    objCurrentEdit = objectSelected;



                    Vector3 mousePosition = Input.mousePosition;

                    // Convert the screen position to a local position within the canvas
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvMain.transform as RectTransform, mousePosition, canvMain.worldCamera, out Vector2 localPoint);

                    RectTransform rectTransform = window.gameObject.GetComponent<RectTransform>();
                    Vector2 adjustedPosition = localPoint - new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);

                    // Checks that the window stays within the screen
                    float halfWidth = rectTransform.rect.width / 2;
                    float halfHeight = rectTransform.rect.height / 2;
                    float canvasWidth = canvMain.GetComponent<RectTransform>().rect.width;
                    float canvasHeight = canvMain.GetComponent<RectTransform>().rect.height;


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
            if(objEditWindow2D.IsActive()){
                objEditWindow2D.gameObject.SetActive(false);
            }
            if(objEditWindow3D.IsActive()){
                objEditWindow3D.gameObject.SetActive(false);
            }
        }

    }

    // Finish this function for 3D
    void editFunctionality(){
        if(mainCam.orthographic){
            if (editX.text != null && editY.text != null && editZ.text != null){
                float x = uiEH.castFloat(editX.text);
                float y = uiEH.castFloat(editY.text);
                float z = uiEH.castFloat(editZ.text);

                objCurrentEdit.name = editName.text;
                objCurrentEdit.gameObject.transform.localScale = new Vector3 (x,y,z);
                objCurrentEdit.GetComponent<FurnitureInteraction>().type = objTagEditSelect2D.options[objTagEditSelect2D.value].text;
            }
        } else {
            if (editX3D.text != null && editY3D.text != null && editZ3D.text != null){
                float x = uiEH.castFloat(editX3D.text);
                float y = uiEH.castFloat(editY3D.text);
                float z = uiEH.castFloat(editZ3D.text);

                objCurrentEdit.name = editName3D.text;
                objCurrentEdit.gameObject.transform.localScale = new Vector3 (x,y,z);
                objCurrentEdit.GetComponent<FurnitureInteraction>().type = objTagEditSelect3D.options[objTagEditSelect3D.value].text;
            }
        }
    }



    void OnInputFieldEndEditRotate(string text)
    {
        // Check if the Enter key was pressed
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            Increment = incrementToFloat();
            incTextValue = -1;
        }
    }

    void OnInputFieldEndEditTag2D(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            AddCustomType(false);
        }
    }

    void OnInputFieldEndEditTag3D(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            AddCustomType(true);
        }
    }


    void OnToggleChanged(Toggle changedToggle)
    {
        if (changedToggle.isOn)
        {
            if (changedToggle != togX)
            {
                togX.isOn = false;
            }
            if (changedToggle != togY)
            {
                togY.isOn = false;
            }
        }

        interact3D();
    }


    void interact3D(){
        if(togX.isOn){
            interact3Dvalue = 0;
        }
        else if(togY.isOn){
            interact3Dvalue = 1;
        }
        else{
            interact3Dvalue = -1;
        }
    }

    
    public Vector3 interact2D(float increment, GameObject furniture, bool isWindow){
        Vector3 currentPos = furniture.transform.position;
        float curX = furniture.transform.position.x;
        float curY = furniture.transform.position.y;
        float curZ = furniture.transform.position.z;

        // Get the parent wall to determine orientation
        Transform parentWall = furniture.transform.parent;
        bool isWallX = parentWall.CompareTag("WallX");
        bool isWallZ = parentWall.CompareTag("WallZ");
        bool isWallRight = parentWall.name == "RightWall";
        bool isWallBottom = parentWall.name == "BottomWall";

        if (isWindow)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && !furniture.CompareTag("Door")){
                currentPos = furniture.transform.position = new Vector3(curX, curY + increment, curZ);
            } else if (Input.GetKeyDown(KeyCode.DownArrow) && !furniture.CompareTag("Door")){
                currentPos = furniture.transform.position = new Vector3(curX, curY - increment, curZ);
            }
            else if (isWallX){
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    currentPos = furniture.transform.position = isWallRight ? 
                        new Vector3(curX, curY, curZ + increment) : 
                        new Vector3(curX, curY, curZ - increment);
                } else if (Input.GetKeyDown(KeyCode.RightArrow)){
                    currentPos = furniture.transform.position = isWallRight ? 
                        new Vector3(curX, curY, curZ - increment) : 
                        new Vector3(curX, curY, curZ + increment);
                }
            } else if (isWallZ){
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    currentPos = furniture.transform.position = isWallBottom ? 
                        new Vector3(curX + increment, curY, curZ) : 
                        new Vector3(curX - increment, curY, curZ);
                } else if (Input.GetKeyDown(KeyCode.RightArrow)){
                    currentPos = furniture.transform.position = isWallBottom ? 
                        new Vector3(curX - increment, curY, curZ) : 
                        new Vector3(curX + increment, curY, curZ);
                }
            }
        }
        else{
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)){
                if (Input.GetKeyDown(KeyCode.UpArrow) && objMoveCheck(furniture, Vector3.forward, increment, false)){
                    currentPos = furniture.transform.position = new Vector3(curX, curY, curZ + increment);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && objMoveCheck(furniture, Vector3.back, increment, false)){
                    currentPos = furniture.transform.position = new Vector3(curX, curY, curZ - increment);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) && objMoveCheck(furniture, Vector3.left, increment, false)){
                    currentPos = furniture.transform.position = new Vector3(curX - increment, curY, curZ);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) && objMoveCheck(furniture, Vector3.right, increment, false)){
                    currentPos = furniture.transform.position = new Vector3(curX + increment, curY, curZ);
                }
            }
        }

        return currentPos;
    }


    private bool objMoveCheck(GameObject furniture, Vector3 direction, float inc, bool third)
    {
        Collider furnitureCollider = furniture.GetComponent<Collider>();
        Vector3 currentPos = furniture.transform.position;
        Vector3 raycastStartPos = currentPos;

        if (furnitureCollider == null){
            Debug.LogError("Object to move must have a collider.");
            return false;
        }

        // Determine the raycast starting position based on the direction
        if (direction == Vector3.forward){
            raycastStartPos = new Vector3(furniture.transform.position.x, furniture.transform.position.y, furnitureCollider.bounds.max.z);
        }else if (direction == Vector3.back){
            raycastStartPos = new Vector3(furniture.transform.position.x, furniture.transform.position.y, furnitureCollider.bounds.min.z);
        }else if (direction == Vector3.left){
            raycastStartPos = new Vector3(furnitureCollider.bounds.min.x, furniture.transform.position.y, furniture.transform.position.z);
        }else if (direction == Vector3.right){
            raycastStartPos = new Vector3(furnitureCollider.bounds.max.x, furniture.transform.position.y, furniture.transform.position.z);
        }else if (third){
            if (direction == Vector3.up){
                raycastStartPos = new Vector3(furniture.transform.position.x, furnitureCollider.bounds.max.y, furniture.transform.position.z);
            }else if (direction == Vector3.down){
                raycastStartPos = new Vector3(furniture.transform.position.x, furnitureCollider.bounds.min.y, furniture.transform.position.z);
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(raycastStartPos, direction, out hit, inc)){
            // If a collision is detected, move the object to the point just before the collision
            Vector3 moveDistance = direction * (hit.distance - 0.01f);
            furniture.transform.position += moveDistance;
            return false;
        }
        else {
            // If no collision is detected, move the object by the full increment
            Vector3 moveDistance = direction * inc;
            furniture.transform.position += moveDistance;
            return true;
        }
    }



    public Vector3 interact3D(float increment, int dir, GameObject furniture){
        Vector3 currentPos = furniture.transform.position;
        float curX = furniture.transform.position.x;
        float curY = furniture.transform.position.y;
        float curZ = furniture.transform.position.z;

        if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)){
            switch(dir){
                case 0:
                    if(Input.GetKeyDown(KeyCode.RightArrow) && objMoveCheck(furniture, Vector3.right, increment, true)){
                        currentPos = furniture.transform.position = new Vector3(curX+increment, curY, curZ);
                    }
                    else if(Input.GetKeyDown(KeyCode.LeftArrow) && objMoveCheck(furniture, Vector3.left, increment, true)){
                        currentPos = furniture.transform.position = new Vector3(curX-increment, curY, curZ);
                    }
                    else if(Input.GetKeyDown(KeyCode.UpArrow) && objMoveCheck(furniture, Vector3.forward, increment, true)){
                        currentPos = furniture.transform.position = new Vector3(curX, curY, curZ+increment);
                    }
                    else if(Input.GetKeyDown(KeyCode.DownArrow) && objMoveCheck(furniture, Vector3.back, increment, false)){
                        currentPos = furniture.transform.position = new Vector3(curX, curY, curZ-increment);
                    }
                break;
                case 1:
                    if(Input.GetKeyDown(KeyCode.UpArrow) && objMoveCheck(furniture, Vector3.up, increment, true)){
                        currentPos = furniture.transform.position = new Vector3(curX, curY+increment, curZ);
                    }
                    else if(Input.GetKeyDown(KeyCode.DownArrow) && objMoveCheck(furniture, Vector3.down, increment, true)){
                        currentPos = furniture.transform.position = new Vector3(curX, curY-increment, curZ);
                    }
                break;
            }
        }
        return currentPos;
    }

    void changeIncrement(){
        if(Input.GetKey(KeyCode.LeftShift)){
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                incTextValue++;
                if(incTextValue > 5){
                    incTextValue = 0;
                }
                changeIncrementText(incTextValue);
                Increment = incrementToFloat();
            }
            else if(Input.GetKeyDown(KeyCode.UpArrow)){
                incTextValue--;
                if(incTextValue < 0){
                    incTextValue = 5;
                }
                changeIncrementText(incTextValue);
                Increment = incrementToFloat();
            }
        }
    }

    void changeIncrementText(int incState){
        switch(incState){
            case -1:
                break;
            case 0:
                incrementValue.text = "1";
            break;
            case 1:
                incrementValue.text = "1/2";
            break;
            case 2:
                incrementValue.text = "1/4";
            break;
            case 3:
                incrementValue.text = "1/8";
            break;
            case 4:
                incrementValue.text = "1/12";
            break;
            case 5:
                incrementValue.text = "1/24";
            break;
        }
    }

    public float incrementToFloat(){

        //Takes the value in the input field and converts it to a real float.

        String input = incrementValue.text;
        if (input.Contains("/")){
            string[] parts = input.Split('/');
            matchState();
            if (parts.Length == 2){
                if (float.TryParse(parts[0], out float numerator) && float.TryParse(parts[1], out float denominator)){
                    if (denominator != 0){
                        return numerator / denominator;
                    }
                    else{
                        throw new DivideByZeroException("Denominator cannot be zero.");
                    }
                }
                else{
                    throw new FormatException("Invalid format for fraction.");
                }
            }
            else{
                throw new FormatException("Invalid format for fraction.");
            }
        }
        else{
            if (float.TryParse(input, out float result)){
                return result;
            }
            else{
                throw new FormatException("Invalid format for float.");
            }
        }
    }

    
    void matchState(){

        // Just sets state value to match the text if a default increment is put in.
        String input = incrementValue.text;
        switch(input){
            case "1":
                incTextValue = 0;
            break;
            case "1/2":
                incTextValue = 1;
            break;
            case "1/4":
                incTextValue = 2;
            break;
            case "1/8":
                incTextValue = 3;
            break;
            case "1/12":
                incTextValue = 4;
            break;
            case "1/24":
                incTextValue = 5;
            break;
        }
    }


    void ObjectRotate(){
        String[] incValues = {"1", "5", "15", "30", "60", "90"};
        if(objEditWindow2D.IsActive() || objEditWindow3D.IsActive()){
            currentRotation.text = Math.Round(objCurrentEdit.transform.eulerAngles.y).ToString();
            currentRotation3D.text = Math.Round(objCurrentEdit.transform.eulerAngles.y).ToString();
            if(Input.GetKey(KeyCode.LeftControl)){


                if(Input.GetKeyDown(KeyCode.UpArrow)){
                    incrementArray++;
                    if(incrementArray > 5){
                        incrementArray = 0;
                    }
                    rotateIncrement.text = incValues[incrementArray]; 
                    rotateIncrement3D.text = incValues[incrementArray];
                    int.TryParse(rotateIncrement.text, out IncrementRotate);
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){
                    incrementArray--;
                    if(incrementArray < 0){
                        incrementArray = 5;
                    }
                    rotateIncrement.text = incValues[incrementArray];
                    rotateIncrement3D.text = incValues[incrementArray];
                    int.TryParse(rotateIncrement.text, out IncrementRotate);
                }
            }
        }
        else{
            if(Input.GetKey(KeyCode.LeftControl)){
                if(Input.GetKeyDown(KeyCode.RightArrow)){
                    objectSelected.transform.Rotate(0, IncrementRotate, 0);
                    currentRotation.text = Math.Round(objectSelected.transform.eulerAngles.y).ToString();
                    currentRotation3D.text = Math.Round(objectSelected.transform.eulerAngles.y).ToString();
                }
                else if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    objectSelected.transform.Rotate(0, -IncrementRotate, 0);
                    currentRotation.text = Math.Round(objectSelected.transform.eulerAngles.y).ToString();   
                    currentRotation3D.text = Math.Round(objectSelected.transform.eulerAngles.y).ToString();
                }
            }
        }
    }

    void colorDropdownStart()
    {
        // Initialize the color dictionary with color names and their corresponding Color values
        colorDictionary = new Dictionary<string, Color>
        {
            { "Red", Color.red },
            { "Green", Color.green },
            { "Blue", Color.blue },
            { "Yellow", Color.yellow },
            { "Cyan", Color.cyan },
            { "Magenta", Color.magenta },
            { "Black", Color.black },
            { "White", Color.white }
        };

        // Populate the dropdown options
        List<string> options = new List<string>(colorDictionary.Keys);
        colorDropdown.AddOptions(options);

        // Add listener to handle dropdown value change
        colorDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
    }


    void OnDropdownValueChanged()
    {
        // Get the selected option text
        string selectedColorName = colorDropdown.options[colorDropdown.value].text;

        // Apply the selected color to the target object
        if (colorDictionary.TryGetValue(selectedColorName, out Color selectedColor))
        {
            Renderer renderer = objCurrentEdit.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = selectedColor;
                objCurrentEdit.gameObject.GetComponent<FurnitureInteraction>().prevColor = selectedColor;
            }
        }
        else
        {
            Debug.LogWarning("Selected color not found in dictionary.");
        }
    }

    void TypeDropdownStart(){
        uiEH.tagDropdownUpdate();

        objTagEditSelect2D.value = 0;
        objTagEditSelect2D.RefreshShownValue();
        objTagEditSelect3D.value = 0;
        objTagEditSelect3D.RefreshShownValue();
    }

    void AddCustomType(bool dim){
        if(!dim){
            if(objTagEditCreate2D.IsActive() && !objTagEditCreate2D.text.Equals("")){
                uiEH.objectTypes.Add(objTagEditCreate2D.text);
                uiEH.tagDropdownUpdate();
                objTagEditCreate2D.gameObject.SetActive(false);
                objTagEditText2D.gameObject.SetActive(false);
            }
        } else {
            if(objTagEditCreate3D.IsActive() && !objTagEditCreate3D.text.Equals("")){
                uiEH.objectTypes.Add(objTagEditCreate3D.text);
                uiEH.tagDropdownUpdate();
                objTagEditCreate3D.gameObject.SetActive(false);
                objTagEditText3D.gameObject.SetActive(false);
            }
        }

    }


}



