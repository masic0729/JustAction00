using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_KeyManager : MonoBehaviour
{

    public void TransGUI_Enable(GameObject target)
    {
        if(target.activeSelf == false)
        {
            target.SetActive(true);
        }
        else
        {
            target.SetActive(false);
        }
    }
}
