using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _01_Scripts.General.Utils
{
    /// <summary>
    /// 간단한 범용 오브젝트 풀
    /// </summary>
    /// <typeparam name="T">풀에서 바로 꺼내서 사용할 컴포넌트 타입.</typeparam>
    public class SimpleObjectPool<T> where T : Behaviour
    {
        private readonly Transform objHolder;
        private readonly T targetPrefab;
        private readonly List<T> usingPool = new List<T>();
        private readonly List<T> sleepingPool = new List<T>();
        
        public SimpleObjectPool(T objToUse, Transform objHolder, int preWarmCount = 5)
        {
            targetPrefab = objToUse;
            this.objHolder = objHolder;
            PreWarm(preWarmCount);
        }

        /// <summary>
        /// 풀의 상태가 정상적인지 확인하는 method.
        /// 만약 비정상적이라면 풀을 정리하는 기능도 겸함
        /// </summary>
        /// <returns>true : 정상</returns>
        public bool CheckPoolStatus()
        {
            var ret =objHolder && targetPrefab;
            if (ret) return true; //정상적이지 않은 경우 리스트를 정리함
            
            CleanUpPool();

            return false;
        }
        
        /// <summary>
        /// 오브젝트 풀을 정리하고 폐기함
        /// </summary>
        /// <returns></returns>
        public void CleanUpPool()
        {
            var targetToClean = new List<T>();
            targetToClean.AddRange(usingPool);
            targetToClean.AddRange(sleepingPool);

            foreach (var obj in targetToClean)
            {
                Object.Destroy(obj);
            }
            
            usingPool.Clear();
            sleepingPool.Clear();
        }

        /// <summary>
        /// 오브젝트를 미리 추가로 생성할 수 있는 기능을 제공하는 method
        /// </summary>
        /// <param name="countToGenerate">미리 추가로 생성할 오브젝트 수</param>
        public void PreWarm(int countToGenerate)
        {
            GenerateObjects(countToGenerate);
        }

        /// <summary>
        /// 명시적으로 오브젝트 풀에 반환하여 순간적인 부하를 미리 부담함 
        /// </summary>
        /// <param name="obj">sleeping pool에 반환할 오브젝트</param>
        public void ReturnObject(T obj)
        {
            obj.gameObject.SetActive(false);
            if(!usingPool.Remove(obj)) //풀에 의해 제어되지 않고 있는 오브젝트를 반환 시도하는 경우 예외 발생함 
                throw new InvalidDataException();
            sleepingPool.Add(obj);
        }

        /// <summary>
        /// 오브젝트를 미리 생성함
        /// </summary>
        /// <param name="count"> 생성할 오브젝트 수 </param>
        private void GenerateObjects(int count = 5)
        {
            for (var i = 0; i < count; i++)
            {
                var newObj = Object.Instantiate(targetPrefab.gameObject, objHolder).GetComponent<T>();
                newObj.gameObject.SetActive(false);
                sleepingPool.Add(newObj);
            }
        }

        /// <summary>
        /// 미사용 오브젝트를 가져와 활성화하고 반환함
        /// </summary>
        /// <param name="getNonActivated"> 오브젝트를 활성화되지 않은 체로 반환 </param>
        /// <returns> 활성화된 오브젝트 </returns>
        public T GetObj(bool getNonActivated = false)
        {
            if (sleepingPool.Count == 0)
            {
                if (!ClearUsingPool())
                    GenerateObjects();
            }

            var retObj = sleepingPool[0];
            sleepingPool.Remove(retObj);
            usingPool.Add(retObj);
            if(!getNonActivated)
                retObj.gameObject.SetActive(true);
            
            return retObj;
        }

        private readonly List<T> tempList = new List<T>();

        /// <summary>
        /// 풀의 무결성을 유지하기 위해 미사용 중인 오브젝트가 사용 중 풀에 있는 경우 청소함 
        /// </summary>
        /// <returns> 청소된 오브젝트가 있는지 (없으면 false, 있으면 true) </returns>
        private bool ClearUsingPool()
        {
            tempList.Clear();
            tempList.AddRange(usingPool.Where(obj => !obj.gameObject.activeSelf));
            sleepingPool.AddRange(tempList);
            usingPool.RemoveAll(tempList.Contains);
            return tempList.Count != 0;
        }
    }
    
    /// <summary>
    /// GameObject용 오브젝트 풀
    /// 제네릭 오브젝트 풀은 범용성이 뛰어나지만, 자료구조로 관리하기 어렵기 때문에 게임오브젝트 전용 풀이 필요함
    /// </summary>
    public class SimpleObjectPool
    {
        private readonly Transform objHolder;
        private readonly GameObject targetPrefab;
        private readonly List<GameObject> usingPool = new List<GameObject>();
        private readonly List<GameObject> sleepingPool = new List<GameObject>();
        
        public SimpleObjectPool(GameObject objToUse, Transform objHolder, int preWarmCount = 5)
        {
            targetPrefab = objToUse;
            this.objHolder = objHolder;
            PreWarm(preWarmCount);
        }

        /// <summary>
        /// 풀의 상태가 정상적인지 확인하는 method.
        /// 만약 비정상적이라면 풀을 정리하는 기능도 겸함
        /// </summary>
        /// <returns>true : 정상</returns>
        public bool CheckPoolStatus()
        {
            var ret =objHolder && targetPrefab;
            if (ret) return true; //정상적이지 않은 경우 리스트를 정리함

            CleanUpPool();

            return false;
        }

        /// <summary>
        /// 오브젝트 풀을 정리하고 폐기함
        /// </summary>
        /// <returns></returns>
        public void CleanUpPool()
        {
            var targetToClean = new List<GameObject>();
            targetToClean.AddRange(usingPool);
            targetToClean.AddRange(sleepingPool);

            foreach (var obj in targetToClean)
            {
                Object.Destroy(obj);
            }
            
            usingPool.Clear();
            sleepingPool.Clear();
        }

        /// <summary>
        /// 오브젝트를 미리 추가로 생성할 수 있는 기능을 제공하는 method
        /// </summary>
        /// <param name="countToGenerate">미리 추가로 생성할 오브젝트 수</param>
        public void PreWarm(int countToGenerate)
        {
            GenerateObjects(countToGenerate);
        }

        /// <summary>
        /// 명시적으로 오브젝트 풀에 반환하여 순간적인 부하를 미리 부담함 
        /// </summary>
        /// <param name="obj">sleeping pool에 반환할 오브젝트</param>
        public void ReturnObject(GameObject obj)
        {
            obj.gameObject.SetActive(false);
            if(!usingPool.Remove(obj)) //풀에 의해 제어되지 않고 있는 오브젝트를 반환 시도하는 경우 예외 발생함 
                throw new InvalidDataException();
            sleepingPool.Add(obj);
        }

        /// <summary>
        /// 오브젝트를 미리 생성함
        /// </summary>
        /// <param name="count"> 생성할 오브젝트 수 </param>
        private void GenerateObjects(int count = 5)
        {
            for (var i = 0; i < count; i++)
            {
                var newObj = Object.Instantiate(targetPrefab.gameObject, objHolder);
                newObj.gameObject.SetActive(false);
                sleepingPool.Add(newObj);
            }
        }

        /// <summary>
        /// 미사용 오브젝트를 가져와 활성화하고 반환함
        /// </summary>
        /// <param name="getNonActivated"> 오브젝트를 활성화되지 않은 체로 반환 </param>
        /// <returns> 활성화된 오브젝트 </returns>
        public GameObject GetObj(bool getNonActivated = false)
        {
            if (sleepingPool.Count == 0)
            {
                if (!ClearUsingPool())
                    GenerateObjects();
            }

            var retObj = sleepingPool[0];
            sleepingPool.Remove(retObj);
            usingPool.Add(retObj);
            if(!getNonActivated)
                retObj.gameObject.SetActive(true);
            
            return retObj;
        }

        private readonly List<GameObject> tempList = new List<GameObject>();

        /// <summary>
        /// 풀의 무결성을 유지하기 위해 미사용 중인 오브젝트가 사용 중 풀에 있는 경우 청소함 
        /// </summary>
        /// <returns> 청소된 오브젝트가 있는지 (없으면 false, 있으면 true) </returns>
        private bool ClearUsingPool()
        {
            tempList.Clear();
            tempList.AddRange(usingPool.Where(obj => !obj.gameObject.activeSelf));
            sleepingPool.AddRange(tempList);
            usingPool.RemoveAll(tempList.Contains);
            return tempList.Count != 0;
        }
    }
}
