using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using Button = UnityEngine.UI.Button;
using UnityEditor.TerrainTools;
using System.Drawing;
using UnityEditor;

public class DataHandler : MonoBehaviour 
{
    GameData saveFile = new GameData();

    [SerializeField] private UIEventHandler uiEH;

    public GameObject wallPrefab;
    public Transform wallParent;
    public GameObject objPrefab;
    public Transform objParent;
    public bool isSaved = false;
    public int saveIndex;

    public Dictionary<int, GameData> allSaves = new Dictionary<int, GameData>();

    public void OnButtonPress()
    {
        GameObject RoomTop = GameObject.Find("Room Walls");
        GameObject ObjTop = GameObject.Find("Objects");
        saveFile.setRoomSize(5);
        saveFile.setObjSize(ObjTop.transform.childCount - 1);
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Equals("TestSaveBtn"))
        {
            for (int i = 0; i < RoomTop.transform.childCount; i++)
            {
                saveFile.room[i].name = RoomTop.transform.GetChild(i).gameObject.name;

                saveFile.room[i].posX = RoomTop.transform.GetChild(i).gameObject.transform.position.x;
                saveFile.room[i].posY = RoomTop.transform.GetChild(i).gameObject.transform.position.y;
                saveFile.room[i].posZ = RoomTop.transform.GetChild(i).gameObject.transform.position.z;

                saveFile.room[i].scaleX = RoomTop.transform.GetChild(i).gameObject.transform.localScale.x;
                saveFile.room[i].scaleY = RoomTop.transform.GetChild(i).gameObject.transform.localScale.y;
                saveFile.room[i].scaleZ = RoomTop.transform.GetChild(i).gameObject.transform.localScale.z;
            }
            for (int i = 1; i < ObjTop.transform.childCount; i++)
            {
                saveFile.objects[i-1].name = ObjTop.transform.GetChild(i).gameObject.name;

                saveFile.objects[i-1].posX = ObjTop.transform.GetChild(i).gameObject.transform.position.x;
                saveFile.objects[i - 1].posY = ObjTop.transform.GetChild(i).gameObject.transform.position.y;
                saveFile.objects[i - 1].posZ = ObjTop.transform.GetChild(i).gameObject.transform.position.z;

                saveFile.objects[i - 1].scaleX = ObjTop.transform.GetChild(i).gameObject.transform.localScale.x;
                saveFile.objects[i - 1].scaleY = ObjTop.transform.GetChild(i).gameObject.transform.localScale.y;
                saveFile.objects[i - 1].scaleZ = ObjTop.transform.GetChild(i).gameObject.transform.localScale.z;
            }
            int currInt = PlayerPrefs.GetInt("currIndex", 0);
            allSaves.Add(currInt, saveFile);
            saveIndex = currInt;
            currInt++;
            PlayerPrefs.SetInt("currIndex", currInt);
            string save = JsonConvert.SerializeObject(allSaves, Formatting.Indented);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveLoad.json", save);
            isSaved = true;

        } else if (buttonName.Equals("TestLoadBtn"))
        {
            string save = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveLoad.json");
            allSaves = JsonConvert.DeserializeObject<Dictionary<int, GameData>>(save);
            GameData load = allSaves[0];
            int roomSize = load.getRoomSize();
            int objSize = load.getObjSize();
            
            for (int i = 0; i < roomSize; i++) 
            {
                Vector3 pos = new Vector3(load.room[i].posX, load.room[i].posY, load.room[i].posZ);
                Vector3 scale = new Vector3(load.room[i].scaleX, load.room[i].scaleY, load.room[i].scaleZ);

                GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                wall.transform.localScale = scale;
                wall.transform.parent = wallParent;
                wall.name = load.room[i].name;

               /*if (load.room[i].name.Equals("Floor")) {

                } 
                else if (load.room[i].name.Equals("Floor"))
                {

                } 
                else if (load.room[i].name.Equals("Floor"))
                {

                } 
                else if (load.room[i].name.Equals("Floor"))
                {

                }*/

                if (load.room[i].name.Equals("Floor"))
                {
                    Renderer floorRenderer = wall.GetComponent<Renderer>();
                    Material floorMaterial = new Material(Shader.Find("Standard"));
                    floorMaterial.color = UnityEngine.Color.gray;
                    floorRenderer.material = floorMaterial;
                }
            }

            for (int i = 0; i < objSize; i++)
            {
                Vector3 pos = new Vector3(load.objects[i].posX, load.objects[i].posY, load.objects[i].posZ);
                Vector3 scale = new Vector3(load.objects[i].scaleX, load.objects[i].scaleY, load.objects[i].scaleZ);

                GameObject obj = Instantiate(objPrefab, pos, Quaternion.identity);
                obj.transform.localScale = scale;
                obj.transform.parent = objParent;
                obj.name = load.objects[i].name;
            }

            isSaved = true;
            uiEH.RoomCreated = true;
        }
    }
}