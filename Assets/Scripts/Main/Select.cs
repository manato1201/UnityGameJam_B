using UnityEngine;

public class Select : MonoBehaviour
{
    [Header("難易度設定 (いずれかtrueにする)")]
    public static bool isEasy;
    public static bool isNormal;
    public static bool isHard;
    public static bool isEndless;
    /// <summary>
    /// SelectScene のボタンから難易度設定
    /// </summary>
    public void SetEasy() { isEasy = true; isNormal = isHard = isEndless = false; }
    public void SetNormal() { isNormal = true; isEasy = isHard = isEndless = false; }
    public void SetHard() { isHard = true; isEasy = isNormal = isEndless = false; }
    public void SetEndless() { isEndless = true; isEasy = isNormal = isHard = false; }
}
