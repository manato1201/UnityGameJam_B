using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace Transition
{


    public class ShaderTransitionController : MonoBehaviour
    {
        [Header("Settings")]
        public Material transitionMaterial; // トランジション用のマテリアル
        public float transitionSpeed = 1.0f; // トランジションの速度

        public static float Value = 0.0f; // トランジションの進行状況
        private bool isTransitioning = false; // トランジション中かどうか
        private System.Action onTransitionComplete; // トランジション完了時のコールバック

        /// <summary>
        /// トランジションを開始する
        /// </summary>
        /// <param name="callback">トランジション完了時に呼び出すコールバック（任意）</param>
        public async UniTask StartTransition()
        {
            if (isTransitioning) return; // すでにトランジション中なら無視

            isTransitioning = true;
            Value = 0.0f;

            while (Value < 1.0f)
            {
                Value += Time.deltaTime * transitionSpeed;
                //Debug.Log(Value);
                transitionMaterial.SetFloat("_Value", Value);

                await UniTask.Yield(PlayerLoopTiming.Update); // フレーム待ち
            }

            // トランジション完了状態を設定
            Value = 1.0f;
            transitionMaterial.SetFloat("_Value", Value);
            isTransitioning = false;
        }


        public async UniTask EndTransition()
        {
            if (isTransitioning) return; // すでにトランジション中なら無視

            isTransitioning = true;
            Value = 1.0f;

            while (Value > 0f)
            {
                Value -= Time.deltaTime * transitionSpeed;
                //Debug.Log(Value);
                transitionMaterial.SetFloat("_Value", Value);

                await UniTask.Yield(PlayerLoopTiming.Update); // フレーム待ち
            }

            // トランジション完了状態を設定
            Value = 0f;
            transitionMaterial.SetFloat("_Value", Value);
            isTransitioning = false;
        }




        /// <summary>
        /// トランジションをリセット（値を初期化）
        /// </summary>
        public void ResetTransition()
        {
            isTransitioning = false;
            Value = 0.0f;
            transitionMaterial.SetFloat("_Value", Value);
        }
    }
}