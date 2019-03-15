using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

[CustomEditor(typeof(BufferCreator))]
public class BufferCreatorEditor : Editor 
{
	private ReorderableList list;
	
	private void OnEnable() 
	{
		list = new ReorderableList(serializedObject, serializedObject.FindProperty("configs"), true, true, true, true);
		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("shader"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, rect.width - 150, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.width - 50, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("format"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + rect.width - 20, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("render"), GUIContent.none);
		};
	}
	
	public override void OnInspectorGUI() 
	{
		serializedObject.Update();
		list.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}
}
