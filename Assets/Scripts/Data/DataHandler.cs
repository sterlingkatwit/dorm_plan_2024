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
using TMPro;

public class DataHandler : MonoBehaviour 
{
    GameData saveFile = new GameData();

    public TMP_InputField saveAsTxt;
    public TMP_Dropdown loadKey, mainLoadKey;

    public GameObject wallPrefab;
    public Transform wallParent;
    public GameObject objPrefab;
    public Transform objParent;
    public GameObject winPrefab;
    public GameObject doorPrefab;
    public bool isSaved = false;
    public string currSave;
    public int saveIndex;

    public UIEventHandler uiEHScript;

    public Dictionary<string, GameData> allSaves = new Dictionary<string, GameData>();

    public void OnButtonPress()
    {
        GameObject RoomTop = GameObject.Find("Room Walls");
        GameObject ObjTop = GameObject.Find("Objects");
        saveFile.setRoomSize(5);
        saveFile.setObjSize(ObjTop.transform.childCount - 1);

        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Equals("SaveBtn") && isSaved)
        {
            string save = getSaveData(RoomTop, ObjTop, currSave);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveLoad.json", save);
        }
        else if (buttonName.Equals("SaveAsCfrm"))
        {
            string save = getSaveData(RoomTop, ObjTop, saveAsTxt.text);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveLoad.json", save);
            isSaved = true;
            currSave = saveAsTxt.text;
        } 
        else if (buttonName.Equals("LoadCnfrm"))
        {
            string saveName = null;
            string save = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveLoad.json");
            allSaves = JsonConvert.DeserializeObject<Dictionary<string, GameData>>(save);

            if(uiEHScript.mainMenuImg.IsActive()){
                uiEHScript.GenerateRoomForLoad();
                uiEHScript.toolBarImg.gameObject.SetActive(true);
                int dropVal = mainLoadKey.value;
                saveName = mainLoadKey.options[dropVal].text;
            } else {
                int dropVal = loadKey.value;
                saveName = loadKey.options[dropVal].text;
            }

            GameData load = allSaves[saveName];
            int roomSize = load.getRoomSize();
            int objSize = load.getObjSize();

            for (int i = 0; i < RoomTop.transform.childCount; i++)
            {
                Vector3 pos = new Vector3(load.room[i].posX, load.room[i].posY, load.room[i].posZ);
                Vector3 scale = new Vector3(load.room[i].scaleX, load.room[i].scaleY, load.room[i].scaleZ);
                
                RoomTop.transform.GetChild(i).gameObject.transform.position = pos;
                RoomTop.transform.GetChild(i).gameObject.transform.localScale = scale;

                for (int j = 0; j < RoomTop.transform.GetChild(i).childCount; j++)
                {
                    Destroy(RoomTop.transform.GetChild(i).GetChild(j).gameObject);
                }
                
                for (int k = 0; k < load.room[i].getWinDoorSizeSize(); k++)
                {
                    Vector3 posWD = new Vector3(load.room[i].winDoorArray[k].posX, load.room[i].winDoorArray[k].posY, load.room[i].winDoorArray[k].posZ);
                    Vector3 scaleWD = new Vector3(load.room[i].winDoorArray[k].scaleX, load.room[i].winDoorArray[k].scaleY, load.room[i].winDoorArray[k].scaleZ);

                    GameObject winDoorPrefab = null;

                    if (load.room[i].winDoorArray[k].name.Equals("Window(Clone)"))
                    {
                        winDoorPrefab = winPrefab;
                    } 
                    else if (load.room[i].winDoorArray[k].name.Equals("Door(Clone)"))
                    {
                        winDoorPrefab = doorPrefab;
                    }

                    GameObject obj = Instantiate(winDoorPrefab, posWD, Quaternion.identity);
                    obj.transform.localScale = scaleWD;
                    obj.transform.parent = RoomTop.transform.GetChild(i).gameObject.transform;
                    obj.name = load.room[i].winDoorArray[k].name;
                }
            }

            for (int i = 1; i < ObjTop.transform.childCount; i++)
            {
                Destroy(ObjTop.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < objSize; i++)
            {
                Vector3 pos = new Vector3(load.objects[i].posX, load.objects[i].posY, load.objects[i].posZ);
                Vector3 scale = new Vector3(load.objects[i].scaleX, load.objects[i].scaleY, load.objects[i].scaleZ);

                GameObject obj = Instantiate(objPrefab, pos, Quaternion.identity);
                obj.transform.parent = objParent;
                obj.transform.localScale = scale;
                obj.name = load.objects[i].name;
            }
            currSave = saveName;
            isSaved = true;
        }
    }

    private string getSaveData (GameObject RoomTop, GameObject ObjTop, string saveName)
    {
        string save;
        if (System.IO.File.Exists(Application.persistentDataPath + "/SaveLoad.json"))
        {
            save = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveLoad.json");
            allSaves = JsonConvert.DeserializeObject<Dictionary<string, GameData>>(save);
        }

        for (int i = 0; i < RoomTop.transform.childCount; i++)
        {
            GameObject wall = RoomTop.transform.GetChild(i).gameObject;

            saveFile.room[i].name = wall.name;

            saveFile.room[i].posX = wall.transform.position.x;
            saveFile.room[i].posY = wall.transform.position.y;
            saveFile.room[i].posZ = wall.transform.position.z;

            saveFile.room[i].scaleX = wall.transform.localScale.x;
            saveFile.room[i].scaleY = wall.transform.localScale.y;
            saveFile.room[i].scaleZ = wall.transform.localScale.z;

            saveFile.room[i].setWinDoorSize(wall.transform.childCount);

            for (int x = 0; x < wall.transform.childCount; x++)
            {
                GameObject objs = wall.transform.GetChild(x).gameObject;

                saveFile.room[i].winDoorArray[x].name = objs.name;

                saveFile.room[i].winDoorArray[x].posX = objs.transform.position.x;
                saveFile.room[i].winDoorArray[x].posY = objs.transform.position.y;
                saveFile.room[i].winDoorArray[x].posZ = objs.transform.position.z;

                saveFile.room[i].winDoorArray[x].scaleX = objs.transform.localScale.x;
                saveFile.room[i].winDoorArray[x].scaleY = objs.transform.localScale.y;
                saveFile.room[i].winDoorArray[x].scaleZ = objs.transform.localScale.z;
            }
        }
        for (int i = 1; i < ObjTop.transform.childCount; i++)
        {
            saveFile.objects[i - 1].name = ObjTop.transform.GetChild(i).gameObject.name;

            saveFile.objects[i - 1].posX = ObjTop.transform.GetChild(i).gameObject.transform.position.x;
            saveFile.objects[i - 1].posY = ObjTop.transform.GetChild(i).gameObject.transform.position.y;
            saveFile.objects[i - 1].posZ = ObjTop.transform.GetChild(i).gameObject.transform.position.z;

            saveFile.objects[i - 1].scaleX = ObjTop.transform.GetChild(i).gameObject.transform.localScale.x;
            saveFile.objects[i - 1].scaleY = ObjTop.transform.GetChild(i).gameObject.transform.localScale.y;
            saveFile.objects[i - 1].scaleZ = ObjTop.transform.GetChild(i).gameObject.transform.localScale.z;
        }

        bool isAdded = allSaves.TryAdd(saveName, saveFile);

        if (!isAdded)
        {
            allSaves[saveName] = saveFile;
        }

        save = JsonConvert.SerializeObject(allSaves, Formatting.Indented);
        return save;
    }
}