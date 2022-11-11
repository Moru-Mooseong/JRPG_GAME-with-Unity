#if UNITY_EDITOR

using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using UnityEditor.PackageManager.UI;


public class MapEditor : EditorWindow
{
    [SerializeField]
    static Material source;
    Material target;
    static EditorWindow window;
    bool _RemovedUnusedParameter = false;
    [MenuItem("CONTEXT/MapEditor")]
    static void Init(MenuCommand command)
    {
        source = (Material)command.context;
        window = EditorWindow.GetWindow<MapEditor>(true, "Copy Material Parameter : Select Materials", true);
        window.Show();
    }
    [ContextMenu("MapEditor")]
    private void OnGUI()
    {
        source = (Material)EditorGUILayout.ObjectField("Source material", source, typeof(Material), true);
        target = (Material)EditorGUILayout.ObjectField("Target material", target, typeof(Material), true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Copy Sorce ¢¡ Target"))
        {
            //CopyMaterial(source, target);
            window.Close();
        }

        if (GUILayout.Button("Switch Sorce/Target"))
        {
            _RemovedUnusedParameter = false;
            var tmp = target;
            target = source;
            source = tmp;
        }

        if (GUILayout.Button("Remove Unused Properties from Sorce"))
        {
            //RemoveUnusedMaterialProperties(source);
            //_RemovedUnusedParameter = true;
        }

        if (_RemovedUnusedParameter)
        {
            EditorGUILayout.HelpBox("Unused Material Properties are removed.", MessageType.Info);
        }
    }
}
#endif