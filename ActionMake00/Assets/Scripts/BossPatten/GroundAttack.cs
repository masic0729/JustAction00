using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundAttack : MonoBehaviour
{
    Character owner;
    [SerializeField] GameObject[] teles;
    [SerializeField] string bossGroundAttackName;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnableTeles());
        Destroy(this.gameObject, 3f);
    }


    IEnumerator EnableTeles()
    {
        for(int i = teles.Length - 1; i >= 0; i-= 2)
        {
            teles[i].SetActive(true);
            teles[i - 1].SetActive(true);
            yield return new WaitForSeconds(0.3f);

        }

    }

    public void AttackTelePatten()
    {
        StartCoroutine(AttackByTeles());
    }

    

    /// <summary>
    /// 텔레그래피가 사라질 때마다 해당 위치에 폭발이 일어나고, 해당 텔레가 사라진다.
    /// 코루틴이 끝날 때, 
    /// </summary>
    /// <returns></returns>
    
    IEnumerator AttackByTeles()
    {
        for (int i = teles.Length - 1; i >= 0; i -= 2)
        {
            teles[i].SetActive(false);
            teles[i - 1].SetActive(false);

            PoolManager.instance.Spawn(bossGroundAttackName, teles[i].transform.position, owner);
            PoolManager.instance.Spawn(bossGroundAttackName, teles[i - 1].transform.position, owner);
            yield return new WaitForSeconds(0.3f);

        }
        
    }

    public void SetOwner(Character owner) => this.owner = owner;
}
