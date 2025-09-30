using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BuffType
{
    Buff,
    Debuff
}

public class CharacterBuff : MonoBehaviour
{
    List<BuffBase> buffs = new List<BuffBase>();
    List<BuffBase> debuffs = new List<BuffBase>();                      //추후 확장성에 사용할 수 있으나, 스케쥴 상 될 지는 의문

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBuff(BuffBase buff)
    {

    }


}
