using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Grid
{
    [CustomEditor(typeof(Gridify))]
    public class GridifyEditor : Editor
    {
        private SerializedProperty gridMaterial;
        private SerializedProperty treePrefabs;
        private SerializedProperty width;
        private SerializedProperty height;
        private SerializedProperty size;
        private SerializedProperty planeScale;
        private SerializedProperty customTile;

        private void OnEnable()
        {
            treePrefabs = serializedObject.FindProperty("treePrefabs");
            width = serializedObject.FindProperty("width");
            height = serializedObject.FindProperty("height");
            size = serializedObject.FindProperty("size");
            planeScale = serializedObject.FindProperty("planeScale");

            customTile = serializedObject.FindProperty("customTile");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(treePrefabs, new GUIContent("Tree prefabs"));
            EditorGUILayout.PropertyField(width, new GUIContent("width"));
            EditorGUILayout.PropertyField(height, new GUIContent("height"));
            EditorGUILayout.PropertyField(size, new GUIContent("size"));
            EditorGUILayout.PropertyField(planeScale, new GUIContent("planeScale"));
            EditorGUILayout.PropertyField(customTile, new GUIContent("customTile"));

            Gridify gridify = (Gridify)target;
            if (GUILayout.Button("Generate"))
            {
                gridify.GenerateTiles();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}