using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.General
{
    public static class MathLib
    {
        public static void CalculateOutQuintPos(ref Vector3[] pointsArr, Vector3 start, float distance)
        {
            var count = pointsArr.Length;
            for (var i = 0; i < count; i++)
            {
                var tValue = 1.0f - Pow(1.0f - (float) i / count, 5);
                var pos = new Vector3(start.x + distance * tValue, start.y, start.z);
                pointsArr[i] = pos;
            }
        }

        #region 심플 베지어
        /// <summary>
        /// 시작점과 목표를 잇는 커브의 t value지점의 벡터 포인트를 계산하는 method
        /// </summary>
        /// <param name="start">커브의 시작점</param>
        /// <param name="target">커브의 종료지점</param>
        /// <param name="curve">베지어 커브 컨트롤 포인트</param>
        /// <param name="tValue">tValue (0f~1f)</param>
        /// <returns>계산된 벡터 포인트</returns>
        public static Vector3 GetCurvePointTValue(Vector3 start, Vector3 target, Vector3 curve ,float tValue)
        {
            var u = 1.0f - tValue;
            var t2 = tValue * tValue;
            var u2 = u * u;

            return start * u2 + 
                   curve * (tValue * u * 2) 
                   +target * t2;
        }
        #endregion

        #region 단순화된 Mathf method
        /// <summary>
        /// 효율을 개선한 Pow method입니다.<br/>
        /// Mathf.Pow 비용이 사용 중인 용도에 비해 지나치게 무거워 별도 구현하여 사용합니다. <br/>
        /// 대신 지수는 unsigned integer만 지원합니다.
        /// </summary>
        /// <param name="a">밑</param>
        /// <param name="b">지수</param>
        /// <returns>밑을 지수곱만큼 제곱한 결괏값(double)</returns>
        public static double Pow(double a, uint b)
        {
            double y = 1;
            while (true)
            {
                if ((b & 1) != 0) y = a * y;
                b = b >> 1;
                if (b == 0) return y;
                a *= a;
            }
        }

        /// <summary>
        /// 효율을 개선한 Pow method입니다.<br/>
        /// Mathf.Pow 비용이 사용 중인 용도에 비해 지나치게 무거워 별도 구현하여 사용합니다. <br/>
        /// 대신 지수는 unsigned integer만 지원합니다.
        /// </summary>
        /// <param name="a">밑</param>
        /// <param name="b">지수</param>
        /// <returns>밑을 지수곱만큼 제곱한 결괏값(float)</returns>
        public static float Pow(float a, uint b)
        {
            float y = 1;
            while(true)
            {
                if ((b & 1) != 0) y = a * y;
                b = b >> 1;
                if (b == 0) return y;
                a *= a;
            }
        }

        /// <summary>
        /// 효율을 개선한 Pow method입니다.<br/>
        /// Mathf.Pow 비용이 사용 중인 용도에 비해 지나치게 무거워 별도 구현하여 사용합니다. <br/>
        /// 대신 지수는 unsigned integer만 지원합니다.
        /// </summary>
        /// <param name="a">밑</param>
        /// <param name="b">지수</param>
        /// <returns>밑을 지수곱만큼 제곱한 결괏값(int)</returns>
        public static int Pow(int a, uint b)
        {
            int y = 1;
            while (true)
            {
                if ((b & 1) != 0) y = a * y;
                b = b >> 1;
                if (b == 0) return y;
                a *= a;
            }
        }
        #endregion

        /// <summary>
        /// 정규 분포를 따르는 랜덤 method
        /// 성능이 그리 좋지는 못하므로 프레임 단위 난수에는 사용을 지양
        /// </summary>
        /// <param name="minValue">최소값</param>
        /// <param name="maxValue">최대값</param>
        /// <returns></returns>
        public static float RandomGaussian(float minValue, float maxValue)
        {
            float u;
            float s;
            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                var v = 2.0f * UnityEngine.Random.value - 1.0f;
                s = u * u + v * v;
            } while (s >= 1.0f);

            var std = u * Mathf.Sqrt(-2.0f * Mathf.Log(s) / s);
            var mean = (minValue + maxValue) * 0.5f;
            var sigma = (maxValue - mean) * 0.3333333f;
            return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
        }

        /// <summary>
        /// 플래그값의 인덱스를 구하는 method
        /// 주로 flag attribute를 갖는 enum을 대상으로 사용함
        /// </summary>
        /// <param name="val">플래그 값</param>
        /// <returns>인덱스</returns>
        public static int GetFlagIndex(int val)
        {
            var ret = 0;
            while (val > 0)
            {
                ret++;
                val /= 2;
            }

            return ret;
        }
    }
}
