using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class SaveLoadUI : MonoBehaviour
{
    public Button save, load, saveas;
    public bool isPressed = false;

    public void OnButtonPress()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Equals("SaveLoadBtn") && !isPressed)
        {
            save.gameObject.SetActive(true);
            saveas.gameObject.SetActive(true);
            load.gameObject.SetActive(true);
            isPressed = true;
        }
        else if (buttonName.Equals("SaveLoadBtn") && isPressed)
        {
            save.gameObject.SetActive(false);
            saveas.gameObject.SetActive(false);
            load.gameObject.SetActive(false);
            isPressed = false;
        }
    }
}
