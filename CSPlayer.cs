using UnityEngine;

namespace DarkNaku.CoroutineStream
{
    public class CSPlayer : MonoBehaviour
    {
        public static CSPlayer Instance
        {
            get
            {
                if (_isQuitting) return null;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var instances = FindObjectsByType<CSPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                        if (instances.Length > 0)
                        {
                            _instance = instances[0];

                            for (int i = 1; i < instances.Length; i++)
                            {
                                Debug.LogWarningFormat("[CSPlayer] Instance Duplicated - {0}", instances[i].name);
                                Destroy(instances[i]);
                            }
                        }
                        else
                        {
                            _instance = new GameObject($"[CoroutineStream] CSPlayer").AddComponent<CSPlayer>();
                        }
                    }

                    return _instance;
                }
            }
        }
    
        private static readonly object _lock = new();
        private static CSPlayer _instance;
        private static bool _isQuitting;

        public static CoroutineStream CoroutineStream() => new CoroutineStream(Instance);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubSystemRegistration()
        {
            _instance = null;
        }
    
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.LogWarningFormat("[CSPlayer] Duplicated - {0}", name);
                Destroy(gameObject);
                return;
            }
        
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            _isQuitting = true;
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}