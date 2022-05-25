using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _01_Scripts.General.Editor
{
    /// <summary>
    /// 인스펙터 창에서 AudioClip을 그리는 프로퍼티 드로워
    /// 인스펙터 창에서 바로 재생시킬 수 있고 간단한 샘플링 프리뷰를 볼 수 있음
    /// 오디오 관련 클래스가 업데이트되면서 코드를 약간 수정함
    ///
    /// Editor 폴더에 넣고 사용
    /// 원본 : https://www.reddit.com/r/Unity3D/comments/5n6ddx/audioclip_propertydrawer/
    /// </summary>
    [CustomPropertyDrawer(typeof(AudioClip))]
    public class AudioClipPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(prop);
        }

        private readonly Dictionary<ButtonState, Action<SerializedProperty, AudioClip>> audioButtonStates = new Dictionary<ButtonState, Action<SerializedProperty, AudioClip>>
        {
            { ButtonState.Play, Play },
            { ButtonState.Stop, Stop },
        };

        private enum ButtonState
        {
            Play,
            Stop
        }

        private static string currentClip;


        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            if (prop.objectReferenceValue != null)
            {
                float totalWidth = position.width;
                position.width = totalWidth - (totalWidth / 4);
                EditorGUI.PropertyField(position, prop, true);

                position.x += position.width;
                position.width = totalWidth / 4;
                DrawButton(position, prop);
            }
            else
            {
                EditorGUI.PropertyField(position, prop, true);
            }

            EditorGUI.EndProperty();
        }

        private void DrawButton(Rect position, SerializedProperty prop)
        {
            if (prop.objectReferenceValue == null) return;
            position.x += 4;
            position.width -= 5;

            var clip = prop.objectReferenceValue as AudioClip;

            var buttonRect = new Rect(position)
            {
                width = 20
            };

            var waveformRect = new Rect(position);
            waveformRect.x += 22;
            waveformRect.width -= 22;
            var waveformTexture = AssetPreview.GetAssetPreview(prop.objectReferenceValue);
            if (waveformTexture != null)
                GUI.DrawTexture(waveformRect, waveformTexture);

            var isPlaying = AudioUtility.IsClipPlaying() && (currentClip == prop.propertyPath);
            string buttonText;
            Action<SerializedProperty, AudioClip> buttonAction;
            if (isPlaying)
            {
                EditorUtility.SetDirty(prop.serializedObject.targetObject);
                buttonAction = GetStateInfo(ButtonState.Stop, out buttonText);

                var progressRect = new Rect(waveformRect);
                var percentage = (float)AudioUtility.GetClipSamplePosition() / AudioUtility.GetSampleCount(clip);
                var width = progressRect.width * percentage;
                progressRect.width = Mathf.Clamp(width, 6, width);
                GUI.Box(progressRect, "", "SelectionRect");
            }
            else
            {
                buttonAction = GetStateInfo(ButtonState.Play, out buttonText);
            }

            if (!GUI.Button(buttonRect, buttonText)) return;
            AudioUtility.StopAllClips();
            buttonAction(prop, clip);
        }

        private static void Play(SerializedProperty prop, AudioClip clip)
        {
            currentClip = prop.propertyPath;
            AudioUtility.PlayClip(clip);
        }

        private static void Stop(SerializedProperty prop, AudioClip clip)
        {
            currentClip = string.Empty;
            AudioUtility.StopAllClips();
        }

        private Action<SerializedProperty, AudioClip> GetStateInfo(ButtonState state, out string buttonText)
        {
            buttonText = state == ButtonState.Play ? "▶" : "■";
            return audioButtonStates[state];
        }
    }
}