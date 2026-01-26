public abstract class TreeCtrl : Enemy
{
    protected override void Start()
    {
        base.Start();
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