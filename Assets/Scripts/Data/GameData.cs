using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public RoomData[] room; 
    public ObjData[] objects;

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
        room = arrayOf<RoomData>(size);
    }

    public void setObjSize(int size)
    {
        objects = arrayOf<ObjData>(size);
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