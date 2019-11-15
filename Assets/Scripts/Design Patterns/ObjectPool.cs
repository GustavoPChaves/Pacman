using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private bool disable = true;
        public GameObject pooledObject;
        public int pooledAmount = 3;
        public bool willGrow = true;
        public int growLimit = 0;

        List<GameObject> pooledObjects;
        //optmizations: queue
        // Use this for initialization
        void Awake()
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < pooledAmount; i++)
            {
                GameObject obj = (GameObject)Instantiate(pooledObject);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                pooledObjects.Add(obj);
            }
        }

        public GameObject GetPooledObject()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }
            if (willGrow)
            {
                if (pooledObjects.Count >= (growLimit == 0 ? int.MaxValue : growLimit))
                {
                    return null;
                }
                GameObject obj = (GameObject)Instantiate(pooledObject);
                pooledObjects.Add(obj);
                obj.transform.SetParent(transform);
                return obj;
            }
            return null;
        }

        public GameObject PooledInstantiate(Vector3 position, Quaternion rotation)
        {
            var obj = GetPooledObject();
            if (obj == null)
                return null;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }
        public GameObject PooledInstantiate(Vector3 position)
        {
            var obj = GetPooledObject();
            if (obj == null)
                return null;
            obj.transform.position = position;
            return obj;
        }
        public GameObject PooledInstantiate(Vector2 position)
        {
            var obj = GetPooledObject();
            if (obj == null)
                return null;
            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
        }

        void Update()
        {

        }

        void OnDisable()
        {
            DisableAllPolledObjects(disable);

        }

        void DisableAllPolledObjects(bool op)
        {
            if (!op)
                return;
            var count = pooledObjects.Count;
            for (int i = 0; i < count; i++)
            {
                pooledObjects[i].SetActive(false);
            }
        }


    }
}
