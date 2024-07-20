using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : ObjData
{
    public ObjData[] winDoorArray;

    public void setWinDoorSize(int size)
    {
        winDoorArray = arrayOf<ObjData>(size);
    }

    public T[] arrayOf<T>(int count) where T : new()
    {
        T[] arr = new T[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = new T();
        }

        return arr;
    }

    public int getWinDoorSizeSize()
    {
        return winDoorArray.Length;
    }
}
