using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEngine;
using System;

namespace IFrameWork
{
    /// <summary>
    /// 生成UI代码
    /// </summary>
    public class GenerateUICode
    {
        /// <summary>
        /// 按钮绑定标识 eventTrigger
        /// </summary>
        static string eventTriggerName = "ATE";
        static string oldClassName = "#SCRIPTNAME#";
        static string statement = "//Statement";
        static string find = "//Find";
        static string method = "//Method";

        /// <summary>
        /// 是否为所有UI对象生成变量
        /// </summary>
        public static bool IsAll => GenerateUIEditor.IsAll;

        /// <summary>
        /// 是否生成按钮绑定方法
        /// </summary>
        public static bool IsGenerateButtonMethod => GenerateUIEditor.IsGenerateButtonMethod;

        /// <summary>
        /// 自动生成标签
        /// </summary>
        static string autoTag= "AT";

        /// <summary>
        /// 变量前缀
        /// </summary>
        static string prefix = "m_";

        /// <summary>
        /// UI代码路径
        /// </summary>
        static string uiCodePath = Application.dataPath + "/Scripts/UICode";

        /// <summary>
        /// UI逻辑层代码模板路径
        /// </summary>
        static string logicTemplatePath = Application.dataPath + "/Scripts/Editor/ScriptTemplates/C# Script-UILogicScript.txt";

        /// <summary>
        /// UI显示层代码模板路径
        /// </summary>
        static string viewTemplatePath = Application.dataPath + "/Scripts/Editor/ScriptTemplates/C# Script-UIViewScript.txt";

        /// <summary>
        /// 获取当前选择的游戏对象
        /// </summary>
        /// <returns></returns>
        static GameObject GetCurrentGameObject()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            
            return gameObjects[0];
        }

        /// <summary>
        /// 获取游戏对象的层级路径
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        static string GetGameObjectPath(Transform root, Transform trans)
        {
            string path = trans.name;
            while (trans.parent != null&& trans.parent != root)
            {
                trans = trans.parent;
                path = trans.name + "/" + path;
            }
            return path;

        }

        /// <summary>
        /// 获取指定游戏对象的所有子对象
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        static Transform[] GetGameObjectChilds(Transform root)
        {
            int count = root.transform.childCount;
            if (count>0)
            {
                return root.transform.GetComponentsInChildren<Transform>(true);
            }
            else
            {
                Debug.LogError("此游戏对象没有子物体 !");
                return null;
            }
        }

        static List<Transform>ArrayToList(Transform[] transforms)
        {
            List<Transform> transList = new List<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != null)
                    transList.Add(transforms[i]);
            }

            return transList;
        }

        static List<Transform> CheckUIChild(string condition,List<Transform> transforms)
        {
            for (int i = transforms.Count-1; i >=0; i--)
            {
                if(!transforms[i].name.Contains(condition))
                {
                    transforms.RemoveAt(i);
                }
            }
            return transforms;
        }

        /// <summary>
        /// 获取UI类型
        /// </summary>
        /// <returns></returns>
        static string GetUIType(string uiName)
        {
            foreach (var item in Enum.GetValues(typeof(UIType)))
            {
                string type = item.ToString();
                if (uiName.Contains(type))
                {
                    return type;
                }
            }
            //默认是GameObject类型
            return "Transform";
        }

        [MenuItem("GameObject/Generate/UIPanelCode", false, 2)]
        static void Generate()
        {
            GameObject curObj = GetCurrentGameObject();
            string uiName = curObj.name;
            if (!uiName.Contains("Panel"))
            {
                Debug.LogError("当前选中的游戏对象的名字中不包含“Panel” !");
                return;
            }

            if(curObj.transform.childCount==0)
            {
                Debug.Log(uiName+"必须要有一个子UI对象");
                return;
            }

            bool isPath = Directory.Exists(uiCodePath+ "/" + uiName);
            if (!isPath)
            {
                //创建文件夹
                Directory.CreateDirectory(uiCodePath + "/" + uiName);
            }


            List<string> buttonNames= CreateUIPanelCode(uiName, ".View.cs", viewTemplatePath, curObj);
            if(buttonNames!=null)
            CreateUILogicPanelCode(uiName,".cs", logicTemplatePath, buttonNames);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建UI界面代码
        /// </summary>
        /// <param name="className"></param>
        private static List<string> CreateUIPanelCode(string className, string suffix,string templatePath,GameObject uiPanel)
        {
            string codePath = uiCodePath + "/" + className + "/" + className + suffix;
            
            string[] dataArry= RedFile(templatePath);
            FileStream fileStream = File.Create(codePath);

            //ui变量名字
            List<string> uiNameList = new List<string>();
            //按钮名字
            List<string> buttonNameList = new List<string>();
            
            Transform[] uiChilds = GetGameObjectChilds(uiPanel.transform);
            List<Transform> childTrans = ArrayToList(uiChilds);
            childTrans.Remove(uiPanel.transform);
            if (IsAll==false)
            childTrans = CheckUIChild(autoTag, childTrans);

            if (childTrans == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < dataArry.Length; i++)
            {
                //类名替换
                if (dataArry[i].Contains(oldClassName))
                {
                    dataArry[i] =  dataArry[i].Replace(oldClassName, className);
                }

                //自动声明处
                
                if (dataArry[i].Contains(statement))
                {
                    string startSpace = dataArry[i].Replace(statement, "");
                    dataArry[i] = startSpace;
                    
                    for (int j = 0; j < childTrans.Count; j++)
                    {
                        //必须包含自动生成标志
                        if(!childTrans[j].name.Contains(autoTag)&& !IsAll)
                        {
                            continue;
                        }
                        Transform uiChild = childTrans[j];
                        string childName = uiChild.name;
                        childName = childName.Replace(" ", "");
                        childName = childName.Replace("(", "");
                        childName = childName.Replace(")", "");
                        uiChild.name = childName;
                        //  变量名   =  前缀 加  对象名字
                        string variableName = prefix + childName;
                        
                        //如果已经有了该变量名存在
                        if (uiNameList.Contains( variableName))
                        {
                            //去除自动生成标签再使用
                            uiChild.name = GetParentName(uiChild).Replace(autoTag,"") + variableName;
                            variableName = uiChild.name;
                            Debug.Log("解决重名冲突");
                        }
                        
                        string type = GetUIType(variableName);
                        stringBuilder.AppendLine($"{startSpace}private {type} {variableName};");
                        uiNameList.Add(variableName);
                        if(type==UIType.Button.ToString())
                        {
                            buttonNameList.Add(variableName);
                        }
                    }
                }

                //自动查找处
                if(dataArry[i].Contains(find))
                {
                    string startSpace = dataArry[i].Replace(find, "");
                    dataArry[i] = startSpace;

                    for (int k = 0; k < uiNameList.Count; k++)
                    {
                        string path = GetGameObjectPath(uiPanel.transform, childTrans[k].transform);
                        string type = GetUIType(uiNameList[k]);
                        stringBuilder.AppendLine($"{startSpace}{uiNameList[k]} = transform.Find(\"{path}\").GetComponent<{type}>();"); //Self.
                    }
                }

                if(IsGenerateButtonMethod)
                {
                    //自动绑定
                    string binding = "//Binding";
                    if (dataArry[i].Contains(binding) && buttonNameList.Count > 0)
                    {
                        string startSpace = dataArry[i].Replace(binding, "");
                        stringBuilder.AppendLine(startSpace + binding);
                        dataArry[i] = startSpace;

                        for (int b = 0; b < buttonNameList.Count; b++)
                        {
                            //方法名
                            string methodName = buttonNameList[b];
                            //将包含数字的按钮识别为共享一个方法
                            string result = System.Text.RegularExpressions.Regex.Replace(methodName, "[0-9]", "");
                           
                            //
                            if (methodName.Contains(eventTriggerName))
                            {
                                stringBuilder.AppendLine($"{startSpace}{buttonNameList[b]}.;");
                            }
                            else//普通绑定
                            {
                                

                                stringBuilder.AppendLine($"{startSpace}{buttonNameList[b]}.onClick.AddListener(On{result}Click);");
                            }
                           
                        }
                    }
                }
                

                    stringBuilder.AppendLine(dataArry[i]);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());


            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
            fileStream.Dispose();

            return buttonNameList;
        }


        /// <summary>
        /// 创建UI界面代码
        /// </summary>
        /// <param name="className"></param>
        private static void CreateUILogicPanelCode(string className, string suffix, string templatePath, List<string>buttonNames )
        {
            string codePath = uiCodePath + "/" + className + "/" + className + suffix;
            if (File.Exists(codePath) && suffix.Equals(".cs"))
            {
                ReplaceUILogicPanelCode(codePath, buttonNames, className);
                
            }
            else
            {
                string[] dataArry = RedFile(templatePath);
                FileStream fileStream = File.Create(codePath);
                WriteCode(dataArry, buttonNames, fileStream, className);
            }
            
        }

        /// <summary>
        /// 替换UI界面代码
        /// </summary>
        /// <param name="className"></param>
        private static void ReplaceUILogicPanelCode(string codePath, List<string> buttonNames,string className)
        {
            string[] dataArry = RedFile(codePath);
            string methodStart = "On";
            string methodEnd = "Click()";
            for (int i = 0; i < dataArry.Length; i++)
            {
                for (int j = buttonNames.Count-1; j >=0; j--)
                {
                    string methodName = System.Text.RegularExpressions.Regex.Replace(buttonNames[j], "[0-9]", "");
                    //如果有存在该按钮方法
                    if (dataArry[i].Contains(methodStart + methodName + methodEnd))
                    {
                        Debug.Log(dataArry[i]);
                        buttonNames.Remove(buttonNames[j]);
                    }

                }
            }

            FileStream fileStream = File.OpenWrite(codePath);
            WriteCode(dataArry, buttonNames, fileStream, className);
        }

        /// <summary>
        /// 写入方法代码
        /// </summary>
        /// <param name="dataArry"></param>
        /// <param name="buttonNames"></param>
        /// <param name="fileStream"></param>
        /// <param name="className"></param>
        static void WriteCode(string[] dataArry, List<string> buttonNames, FileStream fileStream,string className)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < dataArry.Length; i++)
            {
                string dataRow = dataArry[i];
                //类名替换
                if (dataRow.Contains(oldClassName))
                {
                    dataRow = dataRow.Replace(oldClassName, className);
                }

                if (dataRow.Contains(method) && IsGenerateButtonMethod)
                {
                    string startSpace = dataRow.Replace(method, "");

                    string lastMethodName = "";
                    for (int j = 0; j < buttonNames.Count; j++)
                    {
                        string methodName = $"On{buttonNames[j]}Click";
                        methodName = System.Text.RegularExpressions.Regex.Replace(methodName, "[0-9]", "");
                        if(lastMethodName.Equals(methodName))
                        {
                            continue;
                        }

                        stringBuilder.AppendLine($"{startSpace}private void {methodName}()");
                        stringBuilder.AppendLine(startSpace + "{");
                        stringBuilder.AppendLine(startSpace + $"    Debug.Log(\"{methodName}\");");
                        stringBuilder.AppendLine(startSpace + "}");
                        lastMethodName = methodName;
                    }
                }

                stringBuilder.AppendLine(dataRow);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());


            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
            fileStream.Dispose();
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static string[] RedFile(string path)
        {
            return File.ReadAllLines(path);
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="assembly"></param>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static UnityEngine.Component AddComponent(GameObject go, string assembly, string classname)
        {
            var asmb = System.Reflection.Assembly.Load(assembly);
            Debug.Log("Assembly:" + asmb);
            var t = asmb.GetType("IFrameWork." + classname);
            Debug.Log(t);
            if (null != t)
                return go.AddComponent(t);
            else
                return null;
        }

        /// <summary>
        /// 获取对象的父级名称  用于简单处理重名问题
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        private static string GetParentName(Transform trans)
        {
            if (trans.parent != null)
                return trans.parent.name;
            else
                return "Parent_Null";
        }
    }
}

