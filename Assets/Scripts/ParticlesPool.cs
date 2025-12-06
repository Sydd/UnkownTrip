using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPool : MonoBehaviour
{
    [Serializable]
    public class ParticleEntry
    {
        public string name;
        public GameObject prefab;
        public int prewarmCount = 5;
    }

    [SerializeField] private List<ParticleEntry> particles;
    private readonly Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
    public static ParticlesPool Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        BuildPool();
    }

    private void BuildPool()
    {
        pool.Clear();
        if (particles == null) return;

        foreach (var entry in particles)
        {
            if (entry == null || string.IsNullOrEmpty(entry.name) || entry.prefab == null) continue;
            if (!pool.ContainsKey(entry.name))
            {
                pool[entry.name] = new Queue<GameObject>();
            }
            var q = pool[entry.name];
            for (int i = 0; i < Mathf.Max(0, entry.prewarmCount); i++)
            {
                var go = Instantiate(entry.prefab, transform);
                go.SetActive(false);
                q.Enqueue(go);
            }
        }
    }

    public GameObject Spawn(string name, Vector3 position, Quaternion rotation, float lifetimeSeconds = 0f)
    {
        if (string.IsNullOrEmpty(name) || !pool.ContainsKey(name))
        {
            Debug.LogWarning($"ParticlesPool: No pool found for '{name}'");
            return null;
        }

        var q = pool[name];
        GameObject go = null;

        // Reuse if available, else create new from matching entry prefab
        if (q.Count > 0)
        {
            go = q.Dequeue();
        }
        else
        {
            var entry = particles.Find(p => p.name == name);
            if (entry == null || entry.prefab == null)
            {
                Debug.LogWarning($"ParticlesPool: Missing prefab for '{name}'");
                return null;
            }
            go = Instantiate(entry.prefab, transform);
        }

        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);

        // Return to pool logic: prefer explicit lifetimeSeconds if provided
        if (lifetimeSeconds > 0f)
        {
            StartCoroutine(ReturnAfterDelay(name, go, lifetimeSeconds));
        }
        else
        {
            // If it has ParticleSystem, schedule return to pool when finished
            var ps = go.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                StartCoroutine(ReturnWhenDone(name, go, ps));
            }
            else
            {
                // Fallback: return after short delay
                StartCoroutine(ReturnAfterDelay(name, go, 1f));
            }
        }

        return go;
    }

    private IEnumerator ReturnWhenDone(string name, GameObject go, ParticleSystem ps)
    {
        // Wait until particle stops (including sub-emitters)
        yield return new WaitWhile(() => ps.IsAlive(true));
        Return(name, go);
    }

    private IEnumerator ReturnAfterDelay(string name, GameObject go, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Return(name, go);
    }

    private void Return(string name, GameObject go)
    {
        if (!pool.ContainsKey(name))
        {
            go.SetActive(false);
            go.transform.SetParent(transform);
            return;
        }
        go.SetActive(false);
        go.transform.SetParent(transform);
        pool[name].Enqueue(go);
    }
}
