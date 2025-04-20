using UnityEngine;

public class RandomMover : MonoBehaviour
{
    // 基本パラメータ（Inspectorで設定）
    public float moveSpeed = 2f;
    public float maxRotationSpeed = 90f;
    public float directionChangeInterval = 3f;

    // Endlessモード時のパラメータ
    public float endlessMoveSpeed = 2f;
    public float endlessMaxRotationSpeed = 90f;
    public float endlessDirectionChangeInterval = 3f;

    // 移動制限の境界
    public float xMin = -5f;
    public float xMax = 5f;
    public float yMin = -5f;
    public float yMax = 5f;

    private Vector3 moveDirection;
    private float rotationSpeed;
    private float timer;

    void Start()
    {
        // 難易度に応じてパラメータを調整
        if (Select.isEasy)
        {
            moveSpeed = moveSpeed * 0.5f;
            maxRotationSpeed = maxRotationSpeed * 0.5f;
            directionChangeInterval = directionChangeInterval * 2f;
        }
        else if (Select.isNormal)
        {
            // デフォルト設定のまま
        }
        else if (Select.isHard)
        {
            moveSpeed = moveSpeed * 1.5f;
            maxRotationSpeed = maxRotationSpeed * 1.5f;
            directionChangeInterval = directionChangeInterval * 0.75f;
        }
        else if (Select.isEndless)
        {
            // Endlessモード用
            moveSpeed = endlessMoveSpeed;
            maxRotationSpeed = endlessMaxRotationSpeed;
            directionChangeInterval = endlessDirectionChangeInterval;
        }

        // 初期のランダムな方向と回転速度
        moveDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0f
        ).normalized;
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        timer = directionChangeInterval;
    }

    void Update()
    {
        // 移動と回転
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // 境界判定＆反転
        Vector3 pos = transform.position;
        float clampedX = Mathf.Clamp(pos.x, xMin, xMax);
        if (pos.x != clampedX)
        {
            moveDirection.x = -moveDirection.x;
            pos.x = clampedX;
        }
        float clampedY = Mathf.Clamp(pos.y, yMin, yMax);
        if (pos.y != clampedY)
        {
            moveDirection.y = -moveDirection.y;
            pos.y = clampedY;
        }
        transform.position = pos;

        // 一定間隔で方向と速度を再設定
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            moveDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ).normalized;
            rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
            timer = directionChangeInterval;
        }
    }
}
