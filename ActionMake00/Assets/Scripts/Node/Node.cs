using System.Collections.Generic;


/// <summary>
/// base nodeState
/// </summary>
public enum NodeState
{
    Running,
    Failure,
    Success
}

public abstract class Node
{
    protected NodeState state;                                                  //현재 노드상태확인
    public Node parentNode;                                                     //부모 노드
    protected List<Node> childrenNode = new List<Node>();                       //자식 노드 리스트

    public Node()
    {
        //최초 생성시 데이터 미할당
        parentNode = null;
    }

    /// <summary>
    /// 자식 노드를 부여할 경우의 생성자
    /// </summary>
    /// <param name="children">부모 노드에 넣을 자식 노드 리스트</param>
    public Node(List<Node> children)
    {
        foreach(Node child in children)
        {
            AttatchChild(child);
        }
    }

    /// <summary>
    /// 자식 노드를 추가하며 부모 노드의 값에 설정한다
    /// </summary>
    /// <param name="child"></param>
    public void AttatchChild(Node child)
    {
        childrenNode.Add(child);
        child.parentNode = this;
    }

    public abstract NodeState Evaluate();
}
