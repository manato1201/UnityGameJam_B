using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    // 生成するプレハブ（Inspectorから設定）
    public GameObject objectPrefab;
    // 生成するオブジェクト数
    public int spawnCount = 10;
    // 生成位置の範囲（x,y軸の正負の最大値）
    public float spawnRange = 5f;

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // ランダムな位置を決定
            Vector3 randomPos = new Vector3(Random.Range(-spawnRange, spawnRange),
                                            Random.Range(-spawnRange, spawnRange), 0f);
            // ランダムな初期回転（Z軸を中心にランダムな角度）
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            GameObject instance = Instantiate(objectPrefab, randomPos, randomRotation);
            // 生成したオブジェクトに「RandomMover」スクリプトを追加（まだ付いていない場合）
            if (instance.GetComponent<RandomMover>() == null)
            {
                instance.AddComponent<RandomMover>();
            }
        }
    }
}
