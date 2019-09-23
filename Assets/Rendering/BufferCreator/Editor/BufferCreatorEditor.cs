using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

[CustomEditor(typeof(BufferCreatorManager))]
public class BufferCreatorEditor : Editor 
{
	private ReorderableList list;
	
	private void OnEnable() 
	{
		list = new ReorderableList(serializedObject, serializedObject.FindProperty("configs"), true, true, true, true);
		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			float h = EditorGUIUtility.singleLineHeight;
			list.elementHeight = h * 3.5f;
			rect.y += 5;

			EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 175, h),					element.FindPropertyRelative("shader"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.width - 135, rect.y, 100, h),				element.FindPropertyRelative("format"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + rect.width - 40, rect.y, 30, h),	element.FindPropertyRelative("render"), GUIContent.none);

			EditorGUI.TextField(new Rect(rect.x, rect.y + h + 7, rect.width - 175, h),	"Shader Global Name");
			EditorGUI.TextField(new Rect(rect.width - 135, rect.y + h + 7, 150, h),		"Shader Render Tag");
		};
	}
	
	public override void OnInspectorGUI() 
	{
		serializedObject.Update();
		list.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}
}