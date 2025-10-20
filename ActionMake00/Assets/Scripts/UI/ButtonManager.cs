using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void CloseUI()
    {
        GameObject UI = this.gameObject.transform.parent.gameObject;
        UI.SetActive(false);
    }

    
}
