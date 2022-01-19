#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BCurve))]
[CanEditMultipleObjects]
public class BCurveEditor : Editor
{

	SerializedProperty Points, kEpsilon, tangentkEpsilon, drawTangents, drawGizmos, drawKEpsilon, drawCurve;

	private void OnEnable()
	{
		Points = serializedObject.FindProperty("Points");
		kEpsilon = serializedObject.FindProperty("kEpsilon");
		tangentkEpsilon = serializedObject.FindProperty("tangentkEpsilon");
		drawTangents = serializedObject.FindProperty("drawTangents");
		drawGizmos = serializedObject.FindProperty("drawGizmos");
		drawKEpsilon = serializedObject.FindProperty("drawKEpsilon");
		drawCurve = serializedObject.FindProperty("drawCurve");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(Points);
		EditorGUILayout.PropertyField(kEpsilon);
		EditorGUILayout.PropertyField(tangentkEpsilon);
		EditorGUILayout.PropertyField(drawTangents);
		EditorGUILayout.PropertyField(drawGizmos);
		EditorGUILayout.PropertyField(drawKEpsilon);
		EditorGUILayout.PropertyField(drawCurve);

		if (kEpsilon.floatValue <= 0)
			kEpsilon.floatValue = 10e-6f;

		if (tangentkEpsilon.floatValue <= 0)
			tangentkEpsilon.floatValue = 10e-6f;

		serializedObject.ApplyModifiedProperties();
	}

	public void OnSceneGUI()
	{
		BCurve curve = target as BCurve;
		EditorGUI.BeginChangeCheck();
		if (curve.Points != null)
		{
			for (int i = 0; i < curve.Points.Length; i++)
			{
				curve.Points[i] = Handles.PositionHandle(curve.Points[i], Quaternion.identity);
			}
		}
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(curve, "Curve changes");
			curve.Update();
		}
	}

}

#endif