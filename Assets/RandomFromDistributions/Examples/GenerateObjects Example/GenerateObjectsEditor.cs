using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GenerateObjects))]
public class GenerateObjectsEditor : Editor {

	GameObject generated_objects = null;


	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		if (generated_objects) {
			
			if(GUILayout.Button("Re-Generate Objects")) {

				Undo.DestroyObjectImmediate(generated_objects);
				generate_objects();
			}
			if(GUILayout.Button("Generate New Objects")) {
				
				Undo.RecordObject (this, "Generate Objects");

				generate_objects();
			}
			if(GUILayout.Button("Destroy Last Generated Objects")) {
				
				Undo.RecordObject (this, "Generate Objects");

				Undo.DestroyObjectImmediate(generated_objects);
			}

		}
		else {
			if(GUILayout.Button("Generate Objects")) {
				
				Undo.RecordObject (this, "Generate Objects");
				
				generate_objects();
			}
		}

	}
	private void generate_objects() {
		GameObject objects = ((GenerateObjects)target).Generate();
		Undo.RegisterCreatedObjectUndo (objects, "Generate objects");
		generated_objects = objects;
		EditorUtility.SetDirty( this );
	}
}


