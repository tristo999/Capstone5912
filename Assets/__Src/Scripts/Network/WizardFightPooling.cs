/*using UnityEngine;

using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class WizardFightPooling : IPrefabPool
{
    private List<PooledObject> objectsToPool = new List<PooledObject>();
    private Dictionary<PrefabId, Queue<GameObject>> pool = new Dictionary<PrefabId, Queue<GameObject>>();

    public WizardFightPooling() {
        objectsToPool = Object.FindObjectsOfType<PooledObject>().ToList();
    }

    public void LoadSceneObjects(string scene) {
        pool.Clear();
        IEnumerable<PooledObject> newPoolPrefabs = objectsToPool.Where(po => po.scene.ToLower() == scene.ToLower());
        foreach (PooledObject po in newPoolPrefabs) {
            PrefabId key = po.GetComponent<BoltEntity>().prefabId;

            if (!pool.ContainsKey(key)) {
                pool.Add(key, new Queue<GameObject>());
            }
            for (int i = 0; i < po.poolAmt; i++) 
                pool[key].Enqueue(InstantiatePooled(po.gameObject));
        }
    }

    public GameObject Instantiate(PrefabId prefabId, Vector3 position, Quaternion rotation) {
        GameObject prefab = LoadPrefab(prefabId);
        PooledObject po = prefab.GetComponent<PooledObject>();
        if (po && po.scene == SceneManager.GetActiveScene().name) {
            PrefabId key = po.GetComponent<BoltEntity>().prefabId;
            GameObject go = pool[key].Dequeue();
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);
            return go;
        } else {
            return this.Instantiate(prefab, position, rotation);
        }
    }

    private GameObject InstantiatePooled(GameObject prefab) {
        GameObject go = GameObject.Instantiate(prefab, new Vector3(0f, -500f, 0f), Quaternion.identity);
        BoltEntity entity = go.GetComponent<BoltEntity>();
        entity.enabled = true;
        go.SetActive(false);
        return go;
    }

    private GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation) {
        GameObject go = GameObject.Instantiate(prefab, position, rotation);
        go.GetComponent<BoltEntity>().enabled = true;

        return go;
    }

    public void Destroy(GameObject gameObject) {
        PooledObject po = gameObject.GetComponent<PooledObject>();
        if (po && SceneManager.GetActiveScene().name == po.scene) {
            PrefabId key = po.GetComponent<BoltEntity>().prefabId;

            gameObject.transform.position = new Vector3(0f, -500f, 0f);
            gameObject.SetActive(false);
            pool[key].Enqueue(gameObject);
        } else {
            GameObject.Destroy(gameObject);
        }
    }

    public GameObject LoadPrefab(PrefabId prefabId) {
        return PrefabDatabase.Find(prefabId);
    }
}*/