using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    [Serializable]
    public struct ObjStruct
    {
        public string name;

        public float posX;
        public float posY;
        public float posZ;

        public float scaleX;
        public float scaleY;
        public float scaleZ;
    }

    public ObjStruct[] room; 
    public ObjStruct[] objects;
    public Dictionary<string, ObjStruct> allSaves = new Dictionary<string, ObjStruct>();

    public int getRoomSize()
    {
        return room.Length; 
    }

    public int getObjSize()
    {
        return objects.Length;
    }

    public void setRoomSize(int size)
    {
        room = new ObjStruct[size];
    }

    public void setObjSize(int size)
    {
        objects = new ObjStruct[size];
    }
}