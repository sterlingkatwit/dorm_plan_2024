using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneralInteractionEventHandler : MonoBehaviour
{

    [HideInInspector] public GameObject obj;
    public Transform objParent, clipboard;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V) && clipboard.childCount != 0){
            pasteObject();
        }
    }

    void pasteObject(){
        GameObject pasting = clipboard.GetChild(0).gameObject;
        Transform state = pasting.gameObject.GetComponent<Transform>();
        obj = Instantiate(pasting, new Vector3(state.position.x, state.position.y, state.position.z), Quaternion.identity);
        obj.transform.parent = objParent;
        obj.SetActive(true);
    }
}
