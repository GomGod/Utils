using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.General
{
    public static class GeneralCoroutine
    {
        private class FloatComparer : IEqualityComparer<float>
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            bool IEqualityComparer<float>.Equals(float x, float y) => x==y;
            int IEqualityComparer<float>.GetHashCode(float x) => x.GetHashCode();
        }

        private static readonly Dictionary<float, WaitForSeconds> DictionaryWaitTimer = new Dictionary<float, WaitForSeconds>(new FloatComparer());

        private static readonly Dictionary<float, WaitForSecondsRealtime> DictionaryWaitTimerRealTimes =
            new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());


        /// <summary>
        /// duration 값으로 지정한 WaitForSeconds를 리턴 <br/>
        /// 한 번 사용한 WaitForSeconds(duration)은 딕셔너리에 캐싱됨
        /// </summary>
        /// <param name="duration">대기 시간</param>
        /// <returns>WaitForSeconds(duration)</returns>
        public static WaitForSeconds WaitTimer(float duration)
        {
            if (DictionaryWaitTimer.TryGetValue(duration, out var timer)) 
                return timer;
            
            timer = new WaitForSeconds(duration);
            DictionaryWaitTimer.Add(duration, timer);
            return timer;
        }
        
        /// <summary>
        /// duration 값으로 지정한 WaitForSecondsRealTime를 리턴 <br/>
        /// 한 번 사용한 WaitForSecondsRealTime(duration)은 딕셔너리에 캐싱됨
        /// </summary>
        /// <param name="duration">대기 시간</param>
        /// <returns>WaitForSecondsRealTime(duration)</returns>
        public static WaitForSecondsRealtime WaitTimerInRealTime(float duration)
        {
            if (DictionaryWaitTimerRealTimes.TryGetValue(duration, out var timer))
                return timer;

            timer = new WaitForSecondsRealtime(duration);
            DictionaryWaitTimerRealTimes.Add(duration, timer);
            return timer;
        }

        /// <summary>
        /// 타이머가 종료되면 callback으로 넘겨준 method를 실행시킴
        /// 타이머는 Time.timeScale 기준
        /// (invoke에 타이머 걸고 사용하는 것과 동일한데, 리플렉션으로 인한 오버헤드를 줄이기 위해 별도 구현)
        /// </summary>
        /// <param name="delay">타이머 </param>
        /// <param name="callback">타이머가 끝나면 실행할 콜백</param>
        /// <returns> IEnumerator, StartCoroutine과 함께 사용 </returns>
        public static IEnumerator DelayedCallback(float delay, Action callback)
        {
            var timer = 0.0f; 
            while (timer < delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            callback?.Invoke();
        }

        /// <summary>
        /// 타이머가 종료되면 callback으로 넘겨준 method를 실행시킴
        /// 타이머는 unscaled timeScale 기준
        /// (invoke에 타이머 걸고 사용하는 것과 동일한데, 리플렉션으로 인한 오버헤드를 줄이기 위해 별도 구현)
        /// </summary>
        /// <param name="delay">타이머 </param>
        /// <param name="callback">타이머가 끝나면 실행할 콜백</param>
        /// <returns> IEnumerator, StartCoroutine과 함께 사용 </returns>
        public static IEnumerator DelayedCallbackInRealTime(float delay, Action callback)
        {
            var timer = 0.0f; 
            while (timer < delay)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            callback?.Invoke();
        }
    }
}
