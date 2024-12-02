using UnityEngine;

namespace AES
{
    public class AESKeyManager : MonoBehaviour
    {
        [SerializeField, Header("暗号化を使用するかどうか")] bool useEncryption = true;  // 暗号化を使用するかどうか
        [SerializeField, Header("暗号化キー"), Tooltip("暗号化キーを設定します。32バイトの長さが必要です。")] string encryptionKey = "your-encryption-key";  // 暗号化キー
        [SerializeField, Header("DontDestroyOnLoadにするかどうか")] bool dontDestroyOnLoad = false;

        // 暗号化キーを返すプロパティ
        public string EncryptionKey
        {
            get
            {
                // 32バイトに調整（足りない場合はパディング）
                return encryptionKey.PadRight(32).Substring(0, 32);
            }
        }

        // useEncryptionプロパティを追加（インスペクターから設定可能）
        public bool UseEncryption
        {
            get { return useEncryption; }
            set { useEncryption = value; }  // 必要に応じて設定変更も可能
        }

        // ランダムキー生成
        public void GenerateRandomEncryptionKey()
        {
            encryptionKey = GenerateRandomString(32);  // 32バイトのランダムな文字列を生成
            Debug.Log("New random encryption key generated: " + encryptionKey);
        }

        // ランダムな文字列を生成するヘルパーメソッド
        private string GenerateRandomString(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(length);
            System.Random random = new System.Random();
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(validChars[random.Next(validChars.Length)]);
            }
            return stringBuilder.ToString();
        }

        // シングルトンインスタンス
        private static AESKeyManager _instance;
        public static AESKeyManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AESKeyManager>();
                    if (_instance != null && _instance.dontDestroyOnLoad)
                    {
                        // DontDestroyOnLoadを適用
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            // インスタンスがすでに存在しているか確認
            if (_instance == null)
            {
                // インスタンスがない場合は、このオブジェクトをシングルトンとして設定
                _instance = this;
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(this.gameObject);  // シーン遷移時に破棄しない
                }
            }
            else
            {
                // すでにインスタンスがある場合は、このオブジェクトを破棄
                if (_instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
