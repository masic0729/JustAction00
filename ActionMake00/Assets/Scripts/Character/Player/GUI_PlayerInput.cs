using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_PlayerInput : MonoBehaviour
{
    public GameObject inventory;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
    }

    void InputKey()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {

        }
    }
}
