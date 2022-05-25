using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _01_Scripts.General.Editor 
{
	/// <summary>
	/// UnityEditor.AudioUtil의 일부 method를 코드에서 간단히 사용할 수 있게 리플렉션으로 binding 시켜놓은 유틸리티 클래스
	/// AudioUtil가 업데이트 되면서 일부 method들의 이름이나 parameter가 달라져
	/// 원본 코드가 정상적으로 동작하지 않아 사용하는 일부 기능을 제외하고 모두 삭제했음.
	/// 남아있는 기능도 업데이트된 Util class에 맞게 일부 수정함
	///
	/// Editor 폴더에 넣고 사용
	/// 원본 : https://forum.unity.com/threads/reflected-audioutil-class-for-making-audio-based-editor-extensions.308133/
	/// </summary>
	public static class AudioUtility
	{
		public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
		{
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			var method = audioUtilClass.GetMethod(
				"PlayPreviewClip",
				BindingFlags.Static | BindingFlags.Public,
				null,
				new[]
				{
					typeof(AudioClip),
					typeof(int),
					typeof(bool)
				},
				null
			);
			if (method != null)
				method.Invoke(
					null,
					new object[]
					{
						clip,
						startSample,
						loop
					}
				);

			SetClipSamplePosition(clip, startSample);
		}

		public static bool IsClipPlaying()
		{
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			var method = audioUtilClass.GetMethod(
				"IsPreviewClipPlaying",
				BindingFlags.Static | BindingFlags.Public
			);

			var playing = (bool) method?.Invoke(
				null,
				new object[] { }
			)!;

			return playing;
		}

		public static void StopAllClips()
		{
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			var method = audioUtilClass.GetMethod(
				"StopAllPreviewClips",
				BindingFlags.Static | BindingFlags.Public
			);

			if (method != null)
				method.Invoke(
					null,
					null
				);
		}

		public static int GetClipSamplePosition()
		{
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			var method = audioUtilClass.GetMethod(
				"GetPreviewClipSamplePosition",
				BindingFlags.Static | BindingFlags.Public
			);

			var position = (int) method?.Invoke(
				null,
				new object[] { }
			)!;

			return position;
		}

		public static void SetClipSamplePosition(AudioClip clip, int iSamplePosition)
		{
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			var method = audioUtilClass.GetMethod(
				"SetPreviewClipSamplePosition",
				BindingFlags.Static | BindingFlags.Public
			);

			if (method != null)
				method.Invoke(
					null,
					new object[]
					{
						clip,
						iSamplePosition
					}
				);
		}

		public static int GetSampleCount(AudioClip clip)
		{
			var unityEditorAssembly = typeof(AudioImporter).Assembly;
			var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			var method = audioUtilClass.GetMethod(
				"GetSampleCount",
				BindingFlags.Static | BindingFlags.Public
			);

			if (method != null)
			{
				var samples = (int) method.Invoke(
					null,
					new object[]
					{
						clip
					}
				);

				return samples;
			}

			return 0;
		}
	}
}