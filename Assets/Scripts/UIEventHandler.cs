using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEventHandler : MonoBehaviour
{
    public TMP_InputField ifX;
    public TMP_InputField ifY;
    public float ifZ;
    public GameObject wallPrefab;
    public Transform wallParent;


    private GameObject wallBottom, wallTop, wallLeft, wallRight;
    

    public void OnButtonPress()
    {
        if(wallBottom == null){
            GenerateRoom();
        }
        else{
            Destroy(wallBottom.gameObject);
            Destroy(wallTop.gameObject);
            Destroy(wallLeft.gameObject);
            Destroy(wallRight.gameObject);
            GenerateRoom();
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

