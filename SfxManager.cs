using System.Collections.Generic;
using _01_Scripts.General;
using _01_Scripts.General.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace _01_Scripts.InGame.InGameCore
{
    /// <summary>
    /// Object Pooling 기반으로 동작하는 사운드 매니저
    /// 
    /// </summary>
    public class SfxManager : Singleton<SfxManager>
    {
        [Header("Mixer Group")]
        [SerializeField] private AudioMixerGroup backgroundMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        
        [Header("Controller Option")]
        [SerializeField] private AnimationCurve fadeCurve;
        
        [Header("Necessary Objects")]
        [SerializeField] private AudioSource backgroundAudioSource;
        [SerializeField] private AudioSource backgroundAudioSourceSupport;
        [SerializeField] private GameObject prefabAudioSource;
        
        [SerializeField]
        private SerializableDictionary<string, AudioClip> audioClipDictionary =
            new SerializableDictionary<string, AudioClip>();

        private SimpleObjectPool<AudioSource> objPoolAudioSources;

        private void Awake()
        {
            var objToUse = prefabAudioSource.GetComponent<AudioSource>();
            if(!objToUse)
                Debug.LogError("프리팹에 AudioSource가 없습니다.");
            objPoolAudioSources = new SimpleObjectPool<AudioSource>(objToUse, transform);
        }

        public void LoadBGM(string key)
        {
            if (!audioClipDictionary.TryGetValue(key, out var foundClip))
            {
                Debug.Log("해당 키 값을 가진 클립이 없습니다.");
                return;
            }
            LoadBGM(foundClip);
        }

        private bool isUsingSupport;
        public void LoadBGM(AudioClip clip)
        {
            var targetSource = isUsingSupport ? backgroundAudioSourceSupport : backgroundAudioSource;
            var nextSource = isUsingSupport ? backgroundAudioSource : backgroundAudioSourceSupport;
            nextSource.volume = 0.0f;
            
            if (targetSource.isPlaying)
            {//자연스럽게 BGM 교체
                targetSource.DOFade(0.0f, 3.0f).SetEase(fadeCurve);
            }

            nextSource.outputAudioMixerGroup = backgroundMixerGroup;
            nextSource.clip = clip;
            nextSource.loop = true;
            nextSource.Play();
            nextSource.DOFade(1.0f, 3.0f).SetEase(fadeCurve);
            isUsingSupport = !isUsingSupport;
        }
        
        /// <summary>
        /// sfx를 로드하고 실행
        /// key로 clip을 dictionary에서 찾아 실행함
        /// </summary>
        /// <param name="key">Audio clip key</param>
        /// <param name="sourceOwner">사운드 발생원의 transform.(for 3d sound)</param>
        public void LoadSfx(string key, Transform sourceOwner = null)
        {
            if (!audioClipDictionary.TryGetValue(key, out var foundClip))
            {
                Debug.LogError("해당 키 값을 가진 클립이 없습니다.");
                return;
            }
            LoadSfx(foundClip, sourceOwner);
        }

        /// <summary>
        /// sfx를 로드하고 실행
        /// clip을 곧바로 받아 실행함
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="sourceOwner"></param>
        public void LoadSfx(AudioClip clip, Transform sourceOwner = null)
        {
            var source = objPoolAudioSources.GetObj();
            source.transform.SetParent(sourceOwner);
            source.outputAudioMixerGroup = sfxMixerGroup;
            source.clip = clip;
            source.Play();
            DOVirtual.DelayedCall(clip.length, () => { objPoolAudioSources.ReturnObject(source); });
        }
    }
}
