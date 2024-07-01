using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;

public class SaveLoadUI : MonoBehaviour
{
    public void OnButtonPress()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Equals("SaveLoadBtn"))
        {

        }
    }
}
