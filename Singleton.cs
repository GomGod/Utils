using UnityEngine;

namespace _01_Scripts.General
{
    /// 
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton {}
    /// 
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        private static object mutexLock = new object();
        private static bool shuttingDown = false;
        private static T mInstance;
 
        /// 
        /// Access singleton instance through this propriety.
        /// 
        public static T Instance
        {
            get
            {
                if (shuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                    return null;
                }
 
                lock (mutexLock)
                {
                    if (mInstance) return mInstance;
                    mInstance = (T)FindObjectOfType(typeof(T));
                    if (mInstance) return mInstance;
                
                    var singletonObject = new GameObject();
                    mInstance = singletonObject.AddComponent<T>();
                    singletonObject.name = $"{typeof(T)}(Singleton)";
 
                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);

                    return mInstance;
                }
            }
        }
        private void OnApplicationQuit()
        {
            //shuttingDown = true;
        }
        private void OnDestroy()
        {
            //shuttingDown = true;
        }
    }
}