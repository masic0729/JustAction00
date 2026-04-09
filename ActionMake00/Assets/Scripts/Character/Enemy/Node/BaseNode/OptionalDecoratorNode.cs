// 조건 불충족 시 Success로 통과하는 전용 데코레이터
// 시퀀스 중간에 선택적으로 실행되는 노드에 사용한다
using System;

public class OptionalDecoratorNode : Node
{
    protected Node child;
    private Func<bool> condition;

    public OptionalDecoratorNode(Node child, Func<bool> condition)
    {
        this.child = child;
        this.condition = condition;
    }

    public override NodeState Evaluate()
    {
        // 조건 불충족 시 자식 실행 없이 Success 반환
        // 시퀀스 흐름을 끊지 않고 해당 노드만 건너뛴다
        if (!condition()) return NodeState.Success;
        return child.Evaluate();
    }
}