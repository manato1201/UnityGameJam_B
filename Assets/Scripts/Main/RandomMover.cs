using UnityEngine;

public class RandomMover : MonoBehaviour
{
    // 移動速度
    public float moveSpeed = 2f;
    // 最大回転速度（正負の値でランダム設定）
    public float maxRotationSpeed = 90f;
    // 方向変更の間隔（秒）
    public float directionChangeInterval = 3f;

    // 移動制限の境界（Inspectorで調整可能）
    public float xMin = -5f;
    public float xMax = 5f;
    public float yMin = -5f;
    public float yMax = 5f;

    private Vector3 moveDirection;
    private float rotationSpeed;
    private float timer;

    void Start()
    {
        // 初期のランダムな移動方向と回転速度を設定
        moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        timer = directionChangeInterval;
    }

    void Update()
    {
        // 移動と回転の更新
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // 現在の位置を取得
        Vector3 pos = transform.position;

        // Clamp関数で位置を境界内に収め、境界に到達したら対応する軸の移動方向を反転
        float clampedX = Mathf.Clamp(pos.x, xMin, xMax);
        if (pos.x != clampedX)
        {
            // 横方向の境界に達した場合、X方向の速度を反転
            moveDirection.x = -moveDirection.x;
            pos.x = clampedX;
        }
        float clampedY = Mathf.Clamp(pos.y, yMin, yMax);
        if (pos.y != clampedY)
        {
            // 縦方向の境界に達した場合、Y方向の速度を反転
            moveDirection.y = -moveDirection.y;
            pos.y = clampedY;
        }
        transform.position = pos;

        // 定期的に新たなランダムな方向と回転速度に切り替える
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
            rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
            timer = directionChangeInterval;
        }
    }
}
