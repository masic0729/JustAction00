public abstract class Tree : Enemy
{
    Node rootNode;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Init();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (rootNode is null)
            return;
        rootNode.Evaluate();
    }

    protected override void Init()
    {
        base.Init();
        rootNode = SetupBehaviorTree();

    }

    protected abstract Node SetupBehaviorTree();
}