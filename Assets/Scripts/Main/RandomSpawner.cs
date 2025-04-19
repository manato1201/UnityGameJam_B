using UnityEngine;
using System.Collections.Generic;
using DB;  // WordImage を使う名前空間

public class RandomSpawner : MonoBehaviour
{
    [Header("生成するオブジェクトプレハブのリスト")]
    public List<GameObject> objectPrefabs;

    [Header("各プレハブごとの生成間隔（秒）")]
    public List<float> spawnIntervals;

    [Header("生成位置の範囲(±X, ±Y)")]
    public float spawnRange = 5f;

    [Header("同時に生成可能な最大オブジェクト数")]
    public int maxActiveObjects = 50;

    // 内部用タイマー
    private List<float> timers;
    // 生成したオブジェクトの管理リスト
    private List<GameObject> spawnedObjects;
    // 生成停止フラグ
    private bool isPaused = false;

    void Awake()
    {
        // Prefab と Interval の数が一致しないと動かないのでチェック
        if (objectPrefabs.Count != spawnIntervals.Count)
        {
            Debug.LogError("RandomSpawner: objectPrefabs と spawnIntervals の要素数を揃えてください");
        }

        // タイマーをすべて 0 で初期化
        timers = new List<float>(new float[objectPrefabs.Count]);
        // 生成オブジェクトリストの初期化
        spawnedObjects = new List<GameObject>();
    }

    void Update()
    {
        // リストから破棄済みオブジェクトを削除
        spawnedObjects.RemoveAll(item => item == null);

        // 生成停止中かどうかを制御
        if (isPaused)
        {
            // 全てのオブジェクトが消えたら再開
            if (spawnedObjects.Count == 0)
            {
                isPaused = false;
            }
            else
            {
                return; // 生成をスキップ
            }
        }
        else
        {
            // 一度でも上限を超えたら停止
            if (spawnedObjects.Count > maxActiveObjects)
            {
                isPaused = true;
                return;
            }
        }

        // 通常のタイマー処理
        for (int i = 0; i < objectPrefabs.Count; i++)
        {
            timers[i] += Time.deltaTime;
            if (timers[i] >= spawnIntervals[i])
            {
                SpawnObject(i);
                timers[i] = 0f;
            }
        }
    }

    private void SpawnObject(int index)
    {
        // ランダム位置・回転を決定
        Vector3 pos = new Vector3(
            Random.Range(-spawnRange, spawnRange),
            Random.Range(-spawnRange, spawnRange),
            0f
        );
        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        // インスタンス化
        GameObject instance = Instantiate(objectPrefabs[index], pos, rot);
        // 生成リストに追加
        spawnedObjects.Add(instance);

        // —————— スプライトをランダム割り当て（一つ目と二つ目だけ） ——————
        var db = WordImage.Entity;
        if (db != null)
        {
            Sprite chosen = null;
            if (index == 0 && db.ItemSprites.Count > 0)
            {
                var sprites0 = db.ItemSprites[0].Sprite;
                if (sprites0 != null && sprites0.Length > 0)
                {
                    chosen = sprites0[Random.Range(0, sprites0.Length)];
                }
            }
            else if (index == 1 && db.ItemSprites.Count > 1)
            {
                var sprites1 = db.ItemSprites[1].Sprite;
                if (sprites1 != null && sprites1.Length > 0)
                {
                    chosen = sprites1[Random.Range(0, sprites1.Length)];
                }
            }

            if (chosen != null)
            {
                var sr = instance.GetComponent<SpriteRenderer>()
                         ?? instance.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = chosen;
                }
            }
        }

        // —————— 必要なら RandomMover を追加 ——————
        if (instance.GetComponent<RandomMover>() == null)
        {
            instance.AddComponent<RandomMover>();
        }
    }
}