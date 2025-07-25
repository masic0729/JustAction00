using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tree : MonoBehaviour
{
    Node rootNode;
    // Start is called before the first frame update
    virtual protected void Start()
    {

        rootNode = SetupBehaviorTree();
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if (rootNode is null)
            return;
        rootNode.Evaluate();
    }

    protected abstract Node SetupBehaviorTree();
}