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
    public Transform winDoorParent;
    public bool isSaved = false;
    public bool isCamera = true;
    public string currSave;
    public int saveIndex;

    public UIEventHandler uiEHScript;

    public Dictionary<string, GameData> allSaves = new Dictionary<string, GameData>();

    public void OnButtonPress()
    {
        GameObject RoomTop = GameObject.Find("Room Walls");
        GameObject ObjTop = GameObject.Find("Objects");
        GameObject WinDoorTop = GameObject.Find("WallObjects");

        saveFile.setRoomSize(5);
        saveFile.setObjSize(ObjTop.transform.childCount - 1);
        saveFile.setWinDoorSize(WinDoorTop.transform.childCount);

        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Equals("SaveBtn") && isSaved)
        {
            string save = getSaveData(RoomTop, ObjTop, WinDoorTop, currSave);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveLoad.json", save);
        }
        else if (buttonName.Equals("SaveAsCfrm"))
        {
            if (!saveAsTxt.text.Equals(""))
            {
                string save = getSaveData(RoomTop, ObjTop, WinDoorTop, saveAsTxt.text);
                System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveLoad.json", save);
                isSaved = true;
                currSave = saveAsTxt.text;
            }
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
            int winDoorSize = load.getWinDoorSize();

            for (int i = 1; i < RoomTop.transform.childCount; i++)
            {
                Vector3 pos = new Vector3(load.room[i-1].posX, load.room[i-1].posY, load.room[i - 1].posZ);
                Vector3 scale = new Vector3(load.room[i - 1].scaleX, load.room[i - 1].scaleY, load.room[i - 1].scaleZ);
                
                RoomTop.transform.GetChild(i).gameObject.transform.position = pos;
                RoomTop.transform.GetChild(i).gameObject.transform.localScale = scale;
            }

            for (int i = 0; i < WinDoorTop.transform.childCount; i++)
            {
                Destroy(WinDoorTop.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < winDoorSize; i++)
            {
                Vector3 posWD = new Vector3(load.winDoors[i].posX, load.winDoors[i].posY, load.winDoors[i].posZ);
                Vector3 scaleWD = new Vector3(load.winDoors[i].scaleX, load.winDoors[i].scaleY, load.winDoors[i].scaleZ);

                GameObject winDoorPrefab = null;

                if (load.winDoors[i].name.Equals("Window(Clone)"))
                {
                    winDoorPrefab = winPrefab;
                }
                else if (load.winDoors[i].name.Equals("Door(Clone)"))
                {
                    winDoorPrefab = doorPrefab;
                }

                GameObject obj = Instantiate(winDoorPrefab, posWD, Quaternion.identity);
                obj.transform.localScale = scaleWD;
                obj.transform.parent = winDoorParent;
                obj.name = load.winDoors[i].name;

                string wall = load.winDoors[i].ParentWall;

                if (wall.Equals("BottomWall"))
                {
                    obj.GetComponent<SelectableObject>().parentWall = RoomTop.transform.GetChild(1).gameObject;
                } else if (wall.Equals("TopWall"))
                {
                    obj.GetComponent<SelectableObject>().parentWall = RoomTop.transform.GetChild(2).gameObject;
                } else if (wall.Equals("LeftWall"))
                {
                    obj.GetComponent<SelectableObject>().parentWall = RoomTop.transform.GetChild(3).gameObject;
                } else if (wall.Equals("RightWall"))
                {
                    obj.GetComponent<SelectableObject>().parentWall = RoomTop.transform.GetChild(4).gameObject;
                }
            }

            for (int i = 1; i < ObjTop.transform.childCount; i++)
            {
                Destroy(ObjTop.transform.GetChild(i).gameObject);
            }

            uiEHScript.ResetFurniture();

            for (int i = 0; i < objSize; i++)
            {
                Vector3 pos = new Vector3(load.objects[i].posX, load.objects[i].posY, load.objects[i].posZ);
                Vector3 scale = new Vector3(load.objects[i].scaleX, load.objects[i].scaleY, load.objects[i].scaleZ);

                GameObject obj = Instantiate(objPrefab, pos, Quaternion.identity);
                obj.transform.parent = objParent;
                obj.transform.localScale = scale;
                obj.name = load.objects[i].name;
                uiEHScript.AddFurniture(obj);
            }

            currSave = saveName;
            isSaved = true;
            isCamera = false;
        }
    }

    private string getSaveData (GameObject RoomTop, GameObject ObjTop, GameObject WinDoorTop, string saveName)
    {
        string save;
        if (System.IO.File.Exists(Application.persistentDataPath + "/SaveLoad.json"))
        {
            save = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveLoad.json");
            allSaves = JsonConvert.DeserializeObject<Dictionary<string, GameData>>(save);
        }

        for (int i = 1; i < RoomTop.transform.childCount; i++)
        {
            GameObject wall = RoomTop.transform.GetChild(i).gameObject;
            saveFile.room[i-1].name = wall.name;

            saveFile.room[i-1].posX = wall.transform.position.x;
            saveFile.room[i-1].posY = wall.transform.position.y;
            saveFile.room[i - 1].posZ = wall.transform.position.z;

            saveFile.room[i - 1].scaleX = wall.transform.localScale.x;
            saveFile.room[i - 1].scaleY = wall.transform.localScale.y;
            saveFile.room[i - 1].scaleZ = wall.transform.localScale.z;
        }
        for (int i = 0; i < WinDoorTop.transform.childCount; i++)
        {
            GameObject objs = WinDoorTop.transform.GetChild(i).gameObject;

            saveFile.winDoors[i].name = objs.name;
            saveFile.winDoors[i].ParentWall = objs.GetComponent<SelectableObject>().parentWall.name;

            saveFile.winDoors[i].posX = objs.transform.position.x;
            saveFile.winDoors[i].posY = objs.transform.position.y;
            saveFile.winDoors[i].posZ = objs.transform.position.z;

            saveFile.winDoors[i].scaleX = objs.transform.localScale.x;
            saveFile.winDoors[i].scaleY = objs.transform.localScale.y;
            saveFile.winDoors[i].scaleZ = objs.transform.localScale.z;

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