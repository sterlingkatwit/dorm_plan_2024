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

public class UIEventHandler : MonoBehaviour
{
    public TMP_InputField ifX;
    public TMP_InputField ifY;
    public TMP_InputField ofName;
    public TMP_InputField ofX;
    public TMP_InputField ofY;
    public float ofZ;
    public float ifZ;
    public GameObject wallPrefab;
    public GameObject objPrefab;
    public Transform wallParent;
    public Transform objParent;
    public bool RoomCreated = false;

    [HideInInspector] public GameObject wallBottom, wallTop, wallLeft, wallRight, floor, obj;
    
    public void OnButtonPress()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Equals("RoomCreateBtn"))
        {
            if (wallBottom == null)
            {
                GenerateRoom();
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
    }

    void GenerateObj()
    {
        if (ofX != null && ofY != null && ofZ != null)
        {
            float x = castFloat(ofX.text);
            float y = castFloat(ofY.text);
            ofZ = 3f;

            float halfX = (x / 2) + 0.1f;
            float halfY = (y / 2) + 0.1f;

            obj = Instantiate(objPrefab, new Vector3(0, ofZ / 2, halfY), Quaternion.identity);
            obj.transform.localScale = new Vector3(x, 0.2f, y);
            obj.transform.parent = objParent;
            obj.name = ofName.text;
        }
    }

     void GenerateRoom()
    {
        if(ifX != null && ifY != null){
            float x = castFloat(ifX.text);
            float y = castFloat(ifY.text);
            ifZ = 3f;

            // Half values to help build around (0,0). 0.1f accounts for the scaling of the walls and
            // ensures the area of the inside is x*y.
            float halfX = (x / 2) + 0.1f;
            float halfY = (y / 2) + 0.1f;

            // Create and position the four walls around (0,0). In order: Bottom/Top/Left/Right
            wallBottom = BuildWall(new Vector3(0, ifZ / 2, -halfY), new Vector3(x, ifZ, 0.2f), "BottomWall");
            wallTop = BuildWall(new Vector3(0, ifZ / 2, halfY), new Vector3(x, ifZ, 0.2f), "TopWall");
            wallLeft = BuildWall(new Vector3(-halfX, ifZ / 2, 0), new Vector3(0.2f, ifZ, y), "LeftWall");
            wallRight = BuildWall(new Vector3(halfX, ifZ / 2, 0), new Vector3(0.2f, ifZ, y), "RightWall");
            floor = BuildWall(new Vector3(0, -0.1f, 0), new Vector3(x+0.4f, 0.2f, y+0.4f), "Floor");

            Renderer floorRenderer = floor.GetComponent<Renderer>();
            Material floorMaterial = new Material(Shader.Find("Standard"));
            floorMaterial.color = Color.gray;
            floorRenderer.material = floorMaterial;

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
        return wall;
    } 
}