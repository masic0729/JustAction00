using UnityEngine;

public abstract class Tree : Character
{
    Node rootNode;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rootNode = SetupBehaviorTree();

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (rootNode is null)
            return;
        rootNode.Evaluate();
    }

    protected abstract Node SetupBehaviorTree();
}