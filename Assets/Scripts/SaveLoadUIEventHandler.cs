using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class SaveLoadUI : MonoBehaviour
{
    public Button save, load, saveas, mainLoad, exit;
    public Image saveasScreen, loadScreen;
    public TMP_Dropdown dropdown, mainDropdown;

    public UIEventHandler uiEH;

    private bool isMainPressed = false;
    private bool isSaveAsPressed = false;
    private bool isLoadPressed = false;

    void Start(){
        if (File.Exists(Application.persistentDataPath + "/SaveLoad.json"))
        {
            mainDropdown.ClearOptions();
        
            string save = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveLoad.json");
            Dictionary<string, GameData> allSaves = JsonConvert.DeserializeObject<Dictionary<string, GameData>>(save);
            List<string> saveNames = new List<string>();

                for (int x = 0; x < allSaves.Count; x++)
                {
                    saveNames.Add(allSaves.Keys.ElementAt(x));
                }

            mainDropdown.AddOptions(saveNames);
        }
    }

    public void OnButtonPress()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Equals("SaveLoadBtn") && !isMainPressed)
        {
            save.gameObject.SetActive(true);
            saveas.gameObject.SetActive(true);
            load.gameObject.SetActive(true);
            exit.gameObject.SetActive(true);
            isMainPressed = true;
        }
        else if (buttonName.Equals("SaveLoadBtn") && isMainPressed)
        {
            save.gameObject.SetActive(false);
            saveas.gameObject.SetActive(false);
            load.gameObject.SetActive(false);
            exit.gameObject.SetActive(false);
            isMainPressed = false;
        }
        else if (buttonName.Equals("SaveAsBtn") && !isSaveAsPressed)
        {
            isLoadPressed = false;
            loadScreen.gameObject.SetActive(false);
            saveasScreen.gameObject.SetActive(true);
            isSaveAsPressed = true;
        }
        else if (buttonName.Equals("SaveAsBtn") && isSaveAsPressed)
        {
            saveasScreen.gameObject.SetActive(false);
            isSaveAsPressed = false;
        }
        else if (buttonName.Equals("LoadBtn") && !isLoadPressed)
        {
            if (File.Exists(Application.persistentDataPath + "/SaveLoad.json"))
            {
                dropdown.ClearOptions();
                isSaveAsPressed = false;
                saveasScreen.gameObject.SetActive(false);
                loadScreen.gameObject.SetActive(true);

                string save = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveLoad.json");
                Dictionary<string, GameData> allSaves = JsonConvert.DeserializeObject<Dictionary<string, GameData>>(save);
                List<string> saveNames = new List<string>();

                for (int x = 0; x < allSaves.Count; x++)
                {
                    saveNames.Add(allSaves.Keys.ElementAt(x));
                }

                dropdown.AddOptions(saveNames);
                isLoadPressed = true;
            }
        }
        else if (buttonName.Equals("LoadBtn") && isLoadPressed)
        {
            loadScreen.gameObject.SetActive(false);
            isLoadPressed = false;
        }
        else if (buttonName.Equals("SaveAsCfrm"))
        {
            saveasScreen.gameObject.SetActive(false);
            isSaveAsPressed = false;
        }
        else if (buttonName.Equals("LoadCnfrm"))
        {
            if(uiEH.mainMenuImg.IsActive()){
                uiEH.mainMenuImg.gameObject.SetActive(false);
            } else {
                loadScreen.gameObject.SetActive(false);
                isLoadPressed = false;
            }
        } 
    }
}
