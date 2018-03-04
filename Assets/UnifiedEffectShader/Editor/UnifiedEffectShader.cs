using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace UnifiedEffect
{
    public class EffectShader : ShaderGUI
    {
        MaterialProperty maintex;
        MaterialProperty color;
        MaterialProperty isVertexColorEnabled;

        MaterialEditor materialEditor;
        bool isFirstTimeApplying;

        public void FindProperties(MaterialProperty[] props)
        {
            maintex = FindProperty("_MainTex", props);
            color = FindProperty("_Color", props);
            isVertexColorEnabled = FindProperty("_IsVertexColorEnabled", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            var materials = materialEditor.targets.Cast<Material>().ToArray();
            if (isFirstTimeApplying)
            {
                MaterialsChanged(materials);
                isFirstTimeApplying = false;
            }

            this.materialEditor = materialEditor;

            FindProperties(props);
            ShaderPropertiesGUI(materials);
        }

        public void ShaderPropertiesGUI(Material[] materials)
        {
            if (materials.Length == 0)
            {
                return;
            }

            materialEditor.SetDefaultGUIWidths();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                materialEditor.DefaultShaderProperty(maintex, "Texture");
                materialEditor.DefaultShaderProperty(color, "Color");
                Toggle(isVertexColorEnabled, "Vertex Color");

                if (check.changed)
                {
                    MaterialsChanged(materials);
                }
            }
        }

        #region GUI Utilities

        class MixedValueGroup : GUI.Scope
        {
            bool parentValue;

            public MixedValueGroup(bool showMixedValue)
            {
                parentValue = EditorGUI.showMixedValue;
                EditorGUI.showMixedValue = parentValue || showMixedValue;
            }

            protected override void CloseScope()
            {
                EditorGUI.showMixedValue = parentValue;
            }
        }

        static bool Toggle(MaterialProperty property, string label, params GUILayoutOption[] options)
        {
            using (new MixedValueGroup(property.hasMixedValue))
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                bool enabled = Mathf.Approximately(1.0f, property.floatValue);
                enabled = EditorGUILayout.Toggle(label, enabled, options);

                if (check.changed)
                {
                    property.floatValue = enabled ? 1.0f : 0.0f;
                }
            }

            return IsToggleEnabled(property);
        }

        static bool IsToggleEnabled(MaterialProperty property)
        {
            return property.hasMixedValue || Mathf.Approximately(1.0f, property.floatValue);
        }

        #endregion

        static void MaterialsChanged(Material[] materials)
        {
            foreach (var material in materials)
            {
                MaterialChanged(material);
            }
        }

        static void MaterialChanged(Material material)
        {
            SetupMaterialKeywords(material);
        }

        static void SetupMaterialKeywords(Material material)
        {
            // Nothing to do.
        }
    }
}
