using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastToNPC : MonoBehaviour
{
    RaycastHit[] hits;              //NPC를 찾기 위한 레이케스트
    RaycastHit hit;              //NPC를 찾기 위한 레이케스트
    BaseNPC npc;                //캐스팅된 레이캐스트데이터를 저장하는 곳

    const float rayRadius = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindNPC();
    }

    /// <summary>
    /// RayCast 기반으로 NPC를 찾아낸다
    /// </summary>
    void FindNPC()
    {
        //hits = Physics.RaycastAll(transform.position, Vector3.forward, 3f);

        npc = null;
        GetComponent<PlayerController>().SetCanInteraction(false);

        

        Ray ray = new Ray(transform.position, transform.forward);
        hits = Physics.SphereCastAll(ray, rayRadius);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "NPC")
            {
                npc = hit.collider.gameObject.GetComponent<BaseNPC>();
                //Debug.Log(npc + "우와! NPC다");
                GetComponent<PlayerController>().SetCanInteraction(true);
                return;
            }
        }
    }

    public BaseNPC GetNpc() => npc;
}