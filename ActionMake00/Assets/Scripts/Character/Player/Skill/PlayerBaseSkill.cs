using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseSkill : MonoBehaviour
{
    [SerializeField]protected GameObject skillPrefab;
    protected Player player;

    protected virtual void Start()
    {
        player = transform.parent.GetComponent<Player>();
    }

    public virtual void SkillUse()
    {

    }
}
