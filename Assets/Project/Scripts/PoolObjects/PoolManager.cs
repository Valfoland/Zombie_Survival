using System.Collections.Generic;
using UnityEngine;

namespace UnityNightPool
{
    public static class PoolManager
    {
        private const string PoolName = "Pools";
        static List<Pool> pools = new List<Pool>();
        static bool init = false;

        public static Transform parent
        {
            get;
            private set;
        }

        public static PoolObject Get(int key)
        {
            Init();
            var p = pools.Find(x => x.id == key);
            if (p != null)
                return p.Get();
            return null;
        }


        static void Init()
        {
            if (!init)
            {
                parent = (new GameObject(PoolName)).transform;
                Object.DontDestroyOnLoad(parent.gameObject);
                for (int i = 0; i < PoolConfig.pools.Count; i++)
                {
                    if (PoolConfig.pools[i].prefab != null)
                    {
                        Pool p = new Pool(PoolConfig.pools[i]);
                        pools.Add(p);
                    }
                }
                init = true;
            }
        }
    }
}