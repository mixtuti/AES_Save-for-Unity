using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AES
{
    public static class AESCore
    {
        // AESKeyManagerのインスタンスから取得
        private static bool UseEncryption
        {
            get
            {
                // AESKeyManagerインスタンスからuseEncryptionフラグを取得
                if (AESKeyManager.Instance != null)
                {
                    return AESKeyManager.Instance.UseEncryption;
                }
                else
                {
                    throw new InvalidOperationException("AESKeyManagerが見つかりません。シーンに配置してください。");
                }
            }
        }

        // 暗号化キーを取得
        private static string EncryptionKey
        {
            get
            {
                // AESKeyManagerインスタンスから取得
                if (AESKeyManager.Instance != null)
                {
                    return AESKeyManager.Instance.EncryptionKey;
                }
                else
                {
                    throw new InvalidOperationException("AESKeyManagerが見つかりません。シーンに配置してください。");
                }
            }
        }

        // データを暗号化するメソッド
        private static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);  // 32バイトのキー
                aesAlg.IV = new byte[16];  // 初期化ベクター（IV）はゼロに設定

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // 暗号化されたデータを復号化するメソッド
        private static string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);  // 32バイトのキー
                aesAlg.IV = new byte[16];  // 初期化ベクター（IV）はゼロに設定

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        #region PlayerPrefs用の暗号化保存メソッド

        // int型のデータを暗号化して保存
        public static void SetEncryptedInt(string key, int value)
        {
            string valueToSave = value.ToString();

            // useEncryptionがtrueの場合のみ暗号化
            if (UseEncryption)
            {
                valueToSave = Encrypt(valueToSave);
            }

            PlayerPrefs.SetString(key, valueToSave);
        }

        // 暗号化されたint型のデータを復号化して取得
        public static int GetEncryptedInt(string key, int defaultValue = 0)
        {
            string valueToLoad = PlayerPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(valueToLoad))
            {
                if (UseEncryption)
                {
                    valueToLoad = Decrypt(valueToLoad);
                }

                if (int.TryParse(valueToLoad, out int result))
                {
                    return result;
                }
            }
            return defaultValue;
        }

        // float型のデータを暗号化して保存
        public static void SetEncryptedFloat(string key, float value)
        {
            string valueToSave = value.ToString();

            // useEncryptionがtrueの場合のみ暗号化
            if (UseEncryption)
            {
                valueToSave = Encrypt(valueToSave);
            }

            PlayerPrefs.SetString(key, valueToSave);
        }

        // 暗号化されたfloat型のデータを復号化して取得
        public static float GetEncryptedFloat(string key, float defaultValue = 0.0f)
        {
            string valueToLoad = PlayerPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(valueToLoad))
            {
                if (UseEncryption)
                {
                    valueToLoad = Decrypt(valueToLoad);
                }

                if (float.TryParse(valueToLoad, out float result))
                {
                    return result;
                }
            }
            return defaultValue;
        }

        // string型のデータを暗号化して保存
        public static void SetEncryptedString(string key, string value)
        {
            string valueToSave = value;

            // useEncryptionがtrueの場合のみ暗号化
            if (UseEncryption)
            {
                valueToSave = Encrypt(valueToSave);
            }

            PlayerPrefs.SetString(key, valueToSave);
        }

        // 暗号化されたstring型のデータを復号化して取得
        public static string GetEncryptedString(string key, string defaultValue = "")
        {
            string valueToLoad = PlayerPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(valueToLoad))
            {
                if (UseEncryption)
                {
                    valueToLoad = Decrypt(valueToLoad);
                }

                return valueToLoad;
            }
            return defaultValue;
        }

        // bool型のデータを保存
        public static void SetEncryptedBool(string key, bool value)
        {
            // boolをintとして保存（true -> 1, false -> 0）
            int intValue = value ? 1 : 0;

            // useEncryptionがtrueの場合のみ暗号化
            if (UseEncryption)
            {
                string encryptedValue = Encrypt(intValue.ToString());
                PlayerPrefs.SetString(key, encryptedValue);
            }
            else
            {
                PlayerPrefs.SetInt(key, intValue);
            }
        }

        // 暗号化されたbool型のデータを取得
        public static bool GetEncryptedBool(string key, bool defaultValue = false)
        {
            if (UseEncryption)
            {
                string encryptedValue = PlayerPrefs.GetString(key, string.Empty);
                if (!string.IsNullOrEmpty(encryptedValue))
                {
                    string decryptedValue = Decrypt(encryptedValue);
                    if (int.TryParse(decryptedValue, out int result))
                    {
                        return result == 1;
                    }
                }
            }
            else
            {
                int value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
                return value == 1;
            }

            return defaultValue;
        }

        // リストをシリアライズして保存
        public static void SetEncryptedList<T>(string key, List<T> list)
        {
            // List<T>をJSON形式でシリアライズ
            string json = JsonUtility.ToJson(new Serialization<T>(list));

            // useEncryptionがtrueの場合のみ暗号化
            if (UseEncryption)
            {
                json = Encrypt(json);
            }

            PlayerPrefs.SetString(key, json);
        }

        // シリアライズされたリストを復元
        public static List<T> GetEncryptedList<T>(string key)
        {
            string json = PlayerPrefs.GetString(key, string.Empty);

            if (!string.IsNullOrEmpty(json))
            {
                // useEncryptionがtrueの場合のみ復号化
                if (UseEncryption)
                {
                    json = Decrypt(json);
                }

                // JSONからリストを復元
                Serialization<T> wrapper = JsonUtility.FromJson<Serialization<T>>(json);
                return wrapper.list;
            }

            return new List<T>();
        }

        // Vector2の保存メソッド
        public static void SetEncryptedVector2(string key, Vector2 vector)
        {
            // Vector2をシリアライズして保存
            string json = JsonUtility.ToJson(new SerializableVector2(vector));

            // useEncryptionがtrueの場合のみ暗号化
            if (UseEncryption)
            {
                json = Encrypt(json);
            }

            PlayerPrefs.SetString(key, json);
        }

        // Vector2の取得メソッド
        public static Vector2 GetEncryptedVector2(string key)
        {
            string json = PlayerPrefs.GetString(key, string.Empty);

            if (!string.IsNullOrEmpty(json))
            {
                // useEncryptionがtrueの場合のみ復号化
                if (UseEncryption)
                {
                    json = Decrypt(json);
                }

                // JSONからVector2を復元
                SerializableVector2 wrapper = JsonUtility.FromJson<SerializableVector2>(json);
                return new Vector2(wrapper.x, wrapper.y);
            }

            return Vector2.zero;
        }

        // Vector3の保存メソッド
        public static void SetEncryptedVector3(string key, Vector3 vector)
        {
            // Vector3をシリアライズして保存
            string json = JsonUtility.ToJson(new SerializableVector3(vector));

            // useEncryptionがtrueの場合のみ暗号化
            if (UseEncryption)
            {
                json = Encrypt(json);
            }

            PlayerPrefs.SetString(key, json);
        }

        // Vector3の取得メソッド
        public static Vector3 GetEncryptedVector3(string key)
        {
            string json = PlayerPrefs.GetString(key, string.Empty);

            if (!string.IsNullOrEmpty(json))
            {
                // useEncryptionがtrueの場合のみ復号化
                if (UseEncryption)
                {
                    json = Decrypt(json);
                }

                // JSONからVector3を復元
                SerializableVector3 wrapper = JsonUtility.FromJson<SerializableVector3>(json);
                return new Vector3(wrapper.x, wrapper.y, wrapper.z);
            }

            return Vector3.zero;
        }

        // 辞書型のデータを暗号化して保存
        public static void SetEncryptedDictionary(string key, Dictionary<string, string> dictionary)
        {
            // 辞書型をJSON形式に変換
            string jsonData = JsonUtility.ToJson(new Serialization<string, string>(dictionary));

            // useEncryptionがtrueの場合のみ暗号化
            if (UseEncryption)
            {
                jsonData = Encrypt(jsonData); // 暗号化
            }

            // PlayerPrefsに保存
            PlayerPrefs.SetString(key, jsonData);
        }

        // 暗号化された辞書型のデータを復号化して取得
        public static Dictionary<string, string> GetEncryptedDictionary(string key)
        {
            string jsonData = PlayerPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(jsonData))
            {
                // useEncryptionがtrueの場合のみ復号化
                if (UseEncryption)
                {
                    jsonData = Decrypt(jsonData); // 復号化
                }

                // JSON文字列を辞書型に変換
                return JsonUtility.FromJson<Serialization<string, string>>(jsonData).ToDictionary();
            }

            return new Dictionary<string, string>();
        }

        // JSONを使用してリストをラップするためのラッパークラス
        [System.Serializable]
        public class Serialization<T>
        {
            public List<T> list;

            public Serialization(List<T> list)
            {
                this.list = list;
            }
        }

        // 辞書型をJSON形式で保存するためのラッパークラス
        [System.Serializable]
        public class Serialization<TKey, TValue>
        {
            public List<TKey> keys;
            public List<TValue> values;

            public Serialization(Dictionary<TKey, TValue> dictionary)
            {
                keys = new List<TKey>(dictionary.Keys);
                values = new List<TValue>(dictionary.Values);
            }

            public Dictionary<TKey, TValue> ToDictionary()
            {
                Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

                for (int i = 0; i < keys.Count; i++)
                {
                    dictionary.Add(keys[i], values[i]);
                }

                return dictionary;
            }
        }

        #endregion

        #region シリアライズ用ラッパークラス

        [System.Serializable]
        public class SerializableVector2
        {
            public float x;
            public float y;

            // コンストラクタ
            public SerializableVector2(Vector2 vector)
            {
                x = vector.x;
                y = vector.y;
            }
        }

        [System.Serializable]
        public class SerializableVector3
        {
            public float x;
            public float y;
            public float z;

            // コンストラクタ
            public SerializableVector3(Vector3 vector)
            {
                x = vector.x;
                y = vector.y;
                z = vector.z;
            }
        }

        #endregion

        #region データ削除

        // 指定したキーの暗号化データを削除
        public static void DeleteEncryptedKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            Debug.Log($"{key}のキーを削除しました");
        }

        // 全てのPlayerPrefsのデータを削除
        public static void DeleteAllEncryptedData()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("すべてのデータを削除しました。");
        }

        #endregion

        #region 値の差を計算するメソッド

        // int型の値の差を計算
        public static int CompareIntDifference(string key1, string key2)
        {
            int value1 = GetEncryptedInt(key1);
            int value2 = GetEncryptedInt(key2);

            // 差を計算
            return value1 - value2;
        }

        // float型の値の差を計算
        public static float CompareFloatDifference(string key1, string key2)
        {
            float value1 = GetEncryptedFloat(key1);
            float value2 = GetEncryptedFloat(key2);

            // 差を計算
            return value1 - value2;
        }
        #endregion
    }
}
