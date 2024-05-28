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

    public void OnButtonPress()
    {
        GenerateRoom();
    }

     void GenerateRoom()
    {
        if(ifX != null && ifY != null){
            float x = castFloat(ifX.text);
            float y = castFloat(ifY.text);
            ifZ = 3f;
            float halfX = x / 2;
            float halfY = y / 2;

            // Create and position the four walls
            BuildWall(new Vector3(0, ifZ / 2, -halfY), new Vector3(x, ifZ, 0.2f)); // Front wall
            BuildWall(new Vector3(0, ifZ / 2, halfY), new Vector3(x, ifZ, 0.2f)); // Back wall
            BuildWall(new Vector3(-halfX, ifZ / 2, 0), new Vector3(0.2f, ifZ, y)); // Left wall
            BuildWall(new Vector3(halfX, ifZ / 2, 0), new Vector3(0.2f, ifZ, y)); // Right wall
        }

    }

    public float castFloat(String inp){
        float x = 0f;
        float.TryParse(inp, out x);
        return x;
    }

    void BuildWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
        wall.transform.localScale = scale;
    }
}

