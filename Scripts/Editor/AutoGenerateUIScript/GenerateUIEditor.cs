using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IFrameWork
{
    public class GenerateUIEditor : EditorWindow
    {
        /// <summary>
        /// 是否为所有UI对象生成变量
        /// </summary>
        public static bool IsAll
        {
            get
            {
                return EditorPrefs.GetBool("IsAll", false);
            }
            set
            {
                EditorPrefs.SetBool("IsAll", value);
            }
        }

        /// <summary>
        /// 是否生成按钮绑定方法
        /// </summary>
        public static bool IsGenerateButtonMethod
        {
            get
            {
                return EditorPrefs.GetBool("IsGenerateButtonMethod", false);
            }
            set
            {
                EditorPrefs.SetBool("IsGenerateButtonMethod", value);

            }
        }

        [MenuItem("Generate/GenerateUIEditor")]
        static void OpenGenerateUIEditor()
        {
            GetWindow<GenerateUIEditor>().Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("UI代码生成工具编辑器");
            IsAll = GUILayout.Toggle(IsAll, "是否生成所有UI子对象变量");
            IsGenerateButtonMethod = GUILayout.Toggle(IsGenerateButtonMethod, "是否生成按钮绑定方法");
        }
    }
}
