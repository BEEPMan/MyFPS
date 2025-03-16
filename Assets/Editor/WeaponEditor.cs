using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Weapon weapon = target as Weapon;

        weapon.zoomable = EditorGUILayout.Toggle("Zoomable", weapon.zoomable);

        if (weapon.zoomable)
        {
            weapon.aimRecoilRate = EditorGUILayout.Vector2Field("Aim Recoil Rate", weapon.aimRecoilRate);
            weapon.zoomRate = EditorGUILayout.FloatField("Zoom Rate", weapon.zoomRate);
        }

        weapon.spreadable = EditorGUILayout.Toggle("Spreadable", weapon.spreadable);

        if (weapon.spreadable)
        {
            weapon.numOfShell = EditorGUILayout.IntField("Number of Shell", weapon.numOfShell);
            weapon.scatterRate = EditorGUILayout.FloatField("Scatter Rate", weapon.scatterRate);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(weapon);
        }
    }
}
