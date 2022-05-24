using System;
using UnityEditor;
using UnityEngine;

namespace _01_Scripts.General.Utils
{
#if UNITY_EDITOR
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class MethodButtonAttribute : PropertyAttribute
    { 
        public string MethodName { get; private set; }
        public MethodButtonAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }

    [CustomPropertyDrawer(typeof(MethodButtonAttribute), true)]
    public class MethodButtonAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var methodName = (attribute as MethodButtonAttribute)?.MethodName;
            var target = property.serializedObject.targetObject;
            var type = target.GetType();
            if (methodName == null) return;
            var method = type.GetMethod(methodName);
            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.red
                }
            };

            if (method == null)
            {
                GUI.Label(position, "Method could not be found.", labelStyle); 
                return;
            }
            if (method.GetParameters().Length > 0)
            {
                GUI.Label(position, "Method cannot have params.", labelStyle);
            }
            if (GUI.Button(position, methodName))
            {
                method.Invoke(target, null);
            }
        }
    }
#endif
}