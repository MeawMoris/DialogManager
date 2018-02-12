using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HelloWorldObject))]
public class HelloPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.width = 200;
        position.height = 100;
        EditorGUI.LabelField(position, "Hello World!!!!!");
    }
}