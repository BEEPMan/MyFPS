using EnumTypes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TriggeredEffect))]
public class TriggeredEffectDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var name = property.FindPropertyRelative("name");
        var trigger = property.FindPropertyRelative("trigger");
        var actionType = property.FindPropertyRelative("actionType");
        var elementType = property.FindPropertyRelative("elementType");
        var value = property.FindPropertyRelative("value");
        var duration = property.FindPropertyRelative("duration");
        var cooldown = property.FindPropertyRelative("cooldown");
        var maxStacks = property.FindPropertyRelative("maxStacks");
        var condition = property.FindPropertyRelative("condition");

        Rect line = position;
        line.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(line, name);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(line, trigger);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(line, actionType);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        switch ((EffectActionType)actionType.enumValueIndex)
        {
            case EffectActionType.ModifyElementalDamage:
                EditorGUI.PropertyField(line, elementType);
                line.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(line, value);
                line.y += EditorGUIUtility.singleLineHeight + 2;
                break;
            default:
                EditorGUI.PropertyField(line, value);
                line.y += EditorGUIUtility.singleLineHeight + 2;
                break;

        }

        EditorGUI.PropertyField(line, duration);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(line, cooldown);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(line, maxStacks);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(line, condition);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.EndProperty();
    }
}
