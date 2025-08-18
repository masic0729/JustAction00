// Assets/Editor/AnimatorControllerCleaner.cs
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;

public static class AnimatorControllerCleaner
{
    [MenuItem("Tools/Animator/Clean Selected Controller")]
    public static void CleanSelected()
    {
        var ctrl = Selection.activeObject as AnimatorController;
        if (!ctrl)
        {
            EditorUtility.DisplayDialog("Animator Cleaner", "Project 창에서 AnimatorController를 선택하세요.", "OK");
            return;
        }

        Undo.RegisterCompleteObjectUndo(ctrl, "Clean Animator Controller");
        int removed = 0;

        foreach (var layer in ctrl.layers)
            removed += CleanStateMachineRecursive(layer.stateMachine);

        EditorUtility.SetDirty(ctrl);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Animator Cleaner", $"고아 전이 {removed}개 제거 완료.", "OK");
    }

    static int CleanStateMachineRecursive(AnimatorStateMachine sm)
    {
        int removed = 0;

        // 1) 상태 전이 점검
        foreach (var cs in sm.states)
        {
            var state = cs.state;
            // 상태 전이 중 목적지가 null(Exit 제외)인 것 제거
            removed += RemoveInvalidStateTransitions(state, sm);
        }

        // 2) Any State 전이 점검
        removed += RemoveInvalidAnyStateTransitions(sm);

        // 3) Entry/Exit 등 StateMachine 전이 점검
        removed += RemoveInvalidStateMachineTransitions(sm);

        // 4) 하위 스테이트머신 재귀
        foreach (var child in sm.stateMachines)
            removed += CleanStateMachineRecursive(child.stateMachine);

        return removed;
    }

    static int RemoveInvalidStateTransitions(AnimatorState state, AnimatorStateMachine sm)
    {
        int removed = 0;
        var toRemove = new List<AnimatorStateTransition>();

        foreach (var t in state.transitions)
        {
            bool destMissing = (t.destinationState == null && t.destinationStateMachine == null && !t.isExit);
            if (destMissing) toRemove.Add(t);
        }

        if (toRemove.Count > 0)
            Undo.RecordObject(sm, "Remove invalid state transitions");

        foreach (var t in toRemove)
        {
            state.RemoveTransition(t); 
            removed++;
        }
        return removed;
    }

    static int RemoveInvalidAnyStateTransitions(AnimatorStateMachine sm)
    {
        int removed = 0;
        var toRemove = new List<AnimatorStateTransition>();

        foreach (var t in sm.anyStateTransitions)
        {
            bool destMissing = (t.destinationState == null && t.destinationStateMachine == null);
            if (destMissing) toRemove.Add(t);
        }

        if (toRemove.Count > 0)
            Undo.RecordObject(sm, "Remove invalid AnyState transitions");

        foreach (var t in toRemove)
        {
            sm.RemoveAnyStateTransition(t); 
            removed++;
        }
        return removed;
    }

    static int RemoveInvalidStateMachineTransitions(AnimatorStateMachine sm)
    {
        int removed = 0;

        // Entry 등 StateMachine → State/SM 전이
        var sms = sm.GetStateMachineTransitions(sm);
        var toRemove = new List<AnimatorTransition>();
        foreach (var t in sms)
        {
            bool destMissing = (t.destinationState == null && t.destinationStateMachine == null);
            if (destMissing) toRemove.Add(t);
        }

        if (toRemove.Count > 0)
            Undo.RecordObject(sm, "Remove invalid state machine transitions");

        foreach (var t in toRemove)
        {
            sm.RemoveStateMachineTransition(sm, t); 
            removed++;
        }

        return removed;
    }
}
#endif
