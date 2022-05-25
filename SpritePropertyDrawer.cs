using UnityEngine;
using UnityEditor;

namespace _01_Scripts.General.Editor
{
    /// <summary>
    /// 적용된 스프라이트의 이미지 Preview를 인스펙터 창에서 바로 볼 수 있도록 하는 스프라이트 프로퍼티 드로워
    /// 원본 코드는 스프라이트가 비어있을 때 인스펙터 창에 변수 이름이 나오지 않아 불편한 점이 있어 약간 수정함 
    /// 
    /// 원본 : https://www.reddit.com/r/Unity3D/comments/5cxzcj/sprite_property_drawer/
    /// </summary>
    [CustomPropertyDrawer(typeof(Sprite))]
    public class SpritePropertyDrawer : PropertyDrawer
    {
        private const float TextureSize = 64;
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) => 64;
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            position.width = EditorGUIUtility.labelWidth;
            GUI.Label(position, prop.displayName);
            position.x += position.width;
            position.width = TextureSize;
            position.height = TextureSize;

            prop.objectReferenceValue =
                EditorGUI.ObjectField(position, prop.objectReferenceValue, typeof(Sprite), false);

            EditorGUI.EndProperty();
        }
    }
}