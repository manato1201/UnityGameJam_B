using System;
using System.Collections.Generic;
using UnityEngine;


namespace DB {

    [CreateAssetMenu(fileName = "WordImage", menuName = "Scriptable Objects/WordImage")]
    public class WordImage : ScriptableObject
    {

        #region QOL向上処理
        // CakeParamsSOが保存してある場所のパス
        public const string PATH = "WordImage";

        // CakeParamsDBの実体
        private static WordImage _entity = null;
        public static WordImage Entity
        {
            get
            {
                // 初アクセス時にロードする
                if (_entity == null)
                {
                    _entity = Resources.Load<WordImage>(PATH);

                    //ロード出来なかった場合はエラーログを表示
                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [Header("ワードスプライト")] public List<NameSprite> ItemSprites;
    }
    [Serializable]
    public class NameSprite
    {
        public string Name;
        public Sprite[] Sprite;
    }
}