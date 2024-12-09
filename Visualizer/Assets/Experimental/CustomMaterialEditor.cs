using UnityEditor;
using UnityEngine;

public class CustomMaterialEditor : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        Material targetMaterial = materialEditor.target as Material;

        // Mostrar la opción de Render Queue en el inspector
        targetMaterial.renderQueue = EditorGUILayout.IntField("Render Queue", targetMaterial.renderQueue);
    }
}
