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

        MaterialEditor materialEditor;
        bool isFirstTimeApplying;

        public void FindProperties(MaterialProperty[] props)
        {
            maintex = FindProperty("_MainTex", props);
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

                if (check.changed)
                {
                    MaterialsChanged(materials);
                }
            }
        }

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
