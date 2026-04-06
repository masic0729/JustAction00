using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    [SerializeField]Player player;
    [SerializeField] TextMeshProUGUI logText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        logText.text = player.GetIsIgnoreDamage().ToString();
    }
}
