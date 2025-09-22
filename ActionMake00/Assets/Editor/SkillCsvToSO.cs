#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SkillCsvToSO
{
    [MenuItem("Tools/Skills/Import CSV → Selected PlayerSkillData (direct)")]
    public static void ImportCsvDirect()
    {
        // 0) 선택된 SO 확인
        var so = Selection.activeObject as PlayerSkillData;
        if (so == null)
        {
            EditorUtility.DisplayDialog("Import CSV",
                "Project 창에서 PlayerSkillData.asset을 선택하세요.", "OK");
            return;
        }

        // 1) CSV 선택
        var csvPath = EditorUtility.OpenFilePanel("Select CSV (UTF-8)", Application.dataPath, "csv");
        if (string.IsNullOrEmpty(csvPath)) return;

        string[] lines;
        try { lines = File.ReadAllLines(csvPath); }           // UTF-8 가정
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Import CSV", $"파일 읽기 실패:\n{e.Message}", "OK");
            return;
        }
        if (lines.Length < 2)
        {
            EditorUtility.DisplayDialog("Import CSV", "데이터가 비어있습니다.", "OK");
            return;
        }

        // 2) 파싱 (헤더 스킵)
        // 헤더: WeaponType,SkillName,Description,CoolTime,TriggerName,IconPath
        var list = new List<PlayerSkillBase>();
        string weaponTypeFromCsv = null;

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = SplitCsvLine(line);
            if (cols.Count < 6) { Debug.LogWarning($"[CSV] 열 개수 부족 (line {i + 1})"); continue; }

            // 무기타입은 동일하다고 가정, 첫 줄 값 사용
            if (weaponTypeFromCsv == null) weaponTypeFromCsv = cols[0].Trim();

            int cool = 0; int.TryParse(cols[3], out cool);

            var entry = new PlayerSkillBase
            {
                skillName = cols[1].Trim(),
                description = cols[2],
                coolTime = cool,
                triggerName = cols[4].Trim(),
                icon = Resources.Load<Sprite>(cols[5].Trim()) // 예: Icons/09_Melee_slash
            };
            list.Add(entry);
        }

        // 3) SO에 기록
        Undo.RecordObject(so, "Import CSV → PlayerSkillData");
        if (!string.IsNullOrEmpty(weaponTypeFromCsv)) so.weaponType = weaponTypeFromCsv;
        so.weaponSkillBase = list.ToArray();
        EditorUtility.SetDirty(so);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Import CSV",
            $"완료!\nWeaponType: {so.weaponType}\nImported Skills: {list.Count}", "OK");
    }

    // 따옴표 지원 간단 CSV 파서
    static List<string> SplitCsvLine(string line)
    {
        var res = new List<string>();
        bool inQ = false;
        var cur = new System.Text.StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '\"')
            {
                if (inQ && i + 1 < line.Length && line[i + 1] == '\"') { cur.Append('\"'); i++; }
                else inQ = !inQ;
                continue;
            }
            if (c == ',' && !inQ) { res.Add(cur.ToString()); cur.Clear(); }
            else cur.Append(c);
        }
        res.Add(cur.ToString());
        return res;
    }
}
#endif
