using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatData))]
public class StatDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StatData statData = target as StatData;

        statData.HPType = (EnumTypes.HPType)EditorGUILayout.EnumPopup("HP Type", statData.HPType);
        statData.HP = EditorGUILayout.IntField("HP", statData.HP);

        switch (statData.HPType)
        {
            case EnumTypes.HPType.Shield:
                statData.Shield = EditorGUILayout.IntField("Shield", statData.Shield);
                break;
            case EnumTypes.HPType.Armor:
                statData.Armor = EditorGUILayout.IntField("Armor", statData.Armor);
                break;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(statData);
        }
    }
}
