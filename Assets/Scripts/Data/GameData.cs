using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Storage class used in JSON conversion

[Serializable]
public class GameData
{
    public ObjData[] room; 
    public ObjData[] objects;
    public WinDoorData[] winDoors;

    public int getRoomSize()
    {
        return room.Length; 
    }

    public int getObjSize()
    {
        return objects.Length;
    }

    public int getWinDoorSize()
    { 
        return winDoors.Length; 
    }

    public void setRoomSize(int size)
    {
        room = arrayOf<ObjData>(size);
    }

    public void setObjSize(int size)
    {
        objects = arrayOf<ObjData>(size);
    }

    public void setWinDoorSize(int size)
    {
        winDoors = arrayOf<WinDoorData>(size);
    }

    // array intializer from https://stackoverflow.com/questions/49114218
    public T[] arrayOf<T>(int count) where T : new()
    {
        T[] arr = new T[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = new T();
        }

        return arr;
    }
}