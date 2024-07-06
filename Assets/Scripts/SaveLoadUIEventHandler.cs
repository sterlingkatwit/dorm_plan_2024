using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class SaveLoadUI : MonoBehaviour
{
    public Button save, load, saveas;
    public Image saveasScreen, loadScreen;
    private bool isMainPressed = false;
    private bool isSaveAsPressed = false;
    private bool isSavePressed = false;
    private bool isLoadPressed = false;
    private bool isSidePressed = false;

    public void OnButtonPress()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Equals("SaveLoadBtn") && !isMainPressed)
        {
            save.gameObject.SetActive(true);
            saveas.gameObject.SetActive(true);
            load.gameObject.SetActive(true);
            isMainPressed = true;
        }
        else if (buttonName.Equals("SaveLoadBtn") && isMainPressed)
        {
            save.gameObject.SetActive(false);
            saveas.gameObject.SetActive(false);
            load.gameObject.SetActive(false);
            isMainPressed = false;
        }
        else if (buttonName.Equals("SaveAsBtn") && !isSaveAsPressed)
        {
            loadScreen.gameObject.SetActive(false);
            saveasScreen.gameObject.SetActive(true);
            isSaveAsPressed = true;
            isSidePressed = true;
        }
        else if (buttonName.Equals("SaveAsBtn") && isSaveAsPressed)
        {
            saveasScreen.gameObject.SetActive(false);
            isSaveAsPressed = false;
            isSidePressed = false;
        }
        else if (buttonName.Equals("LoadBtn") && !isLoadPressed)
        {
            saveasScreen.gameObject.SetActive(false);
            loadScreen.gameObject.SetActive(true);
            isLoadPressed = true;
            isSidePressed = true;
        }
        else if (buttonName.Equals("LoadBtn") && isLoadPressed)
        {
            loadScreen.gameObject.SetActive(false);
            isLoadPressed = false;
            isSidePressed = false;
        }
    }
}
