using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using AES;

public class AESDataExploration : MonoBehaviour
{
    // Enum: キーの名前と型を指定するための列挙型
    public enum DataType
    {
        Int,
        Float,
        String,
        Bool,
        List,
        Vector2,
        Vector3,
        Dictionary
    }

    // 保存形式の選択（JSONまたはTXT）
    public enum FileExtension
    {
        Txt,
        Json
    }

    // キーと型のペアをリストに格納
    [System.Serializable]
    public class KeyValuePair
    {
        public string key;  // キー名
        public DataType dataType;  // データの型
    }

    [SerializeField]
    private List<KeyValuePair> keyValueList = new List<KeyValuePair>();

    [SerializeField, Header("保存形式")]
    private FileExtension fileExtension = FileExtension.Json;

    [SerializeField, Header("保存先")]
    private string saveDirectory = "Assets/SaveData";

    [SerializeField, Header("ファイル名")]
    private string fileName = "EncryptedData";

    // 暗号化処理を実行するメソッド
    private string GetEncryptedValueBasedOnType(string key, DataType dataType)
    {
        switch (dataType)
        {
            case DataType.Int:
                return AESCore.GetEncryptedInt(key).ToString();

            case DataType.Float:
                return AESCore.GetEncryptedFloat(key).ToString();

            case DataType.String:
                return AESCore.GetEncryptedString(key);

            case DataType.Bool:
                return AESCore.GetEncryptedBool(key).ToString();

            case DataType.List:
                return GetEncryptedListValue(key);

            case DataType.Vector2:
                return AESCore.GetEncryptedVector2(key).ToString();

            case DataType.Vector3:
                return AESCore.GetEncryptedVector3(key).ToString();

            case DataType.Dictionary:
                return AESCore.GetEncryptedDictionary(key).ToString();

            default:
                Debug.LogError("対応していないデータ型です");
                return null;
        }
    }

    // リスト型の暗号化処理を行うメソッド
    private string GetEncryptedListValue(string key)
    {
        List<string> encryptedList = AESCore.GetEncryptedList<string>(key);
        string listJson = JsonUtility.ToJson(new AESCore.Serialization<string>(encryptedList));
        return listJson;
    }

    // データを保存するメインメソッド
    public void SaveEncryptedData()
    {
        // 保存先ディレクトリが存在しない場合、作成
        EnsureSaveDirectoryExists();

        // 暗号化したデータをリストに格納
        List<KeyValuePair<string, string>> encryptedKeyValuePairs = GetEncryptedKeyValuePairs();

        // 保存先のファイルパスを決定
        string filePath = GetFilePath();

        // 保存形式に応じてデータをファイルに保存
        if (fileExtension == FileExtension.Json)
        {
            SaveToJson(filePath, encryptedKeyValuePairs);
        }
        else
        {
            SaveToTxt(filePath, encryptedKeyValuePairs);
        }
    }

    // 保存先ディレクトリが存在しない場合、作成するメソッド
    private void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
            Debug.Log("保存先のディレクトリを作成しました: " + saveDirectory);
        }
    }

    // キーと暗号化された値をリストに格納するメソッド
    private List<KeyValuePair<string, string>> GetEncryptedKeyValuePairs()
    {
        List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

        foreach (var kv in keyValueList)
        {
            string encryptedValue = GetEncryptedValueBasedOnType(kv.key, kv.dataType);
            keyValuePairs.Add(new KeyValuePair<string, string>(kv.key, encryptedValue));
        }

        return keyValuePairs;
    }

    // 保存するファイルのパスを決定するメソッド
    private string GetFilePath()
    {
        string fileExtensionString = (fileExtension == FileExtension.Json) ? ".json" : ".txt";
        return Path.Combine(saveDirectory, fileName + fileExtensionString);
    }

    // JSON形式で保存するメソッド
    private void SaveToJson(string filePath, List<KeyValuePair<string, string>> keyValuePairs)
    {
        try
        {
            string json = JsonUtility.ToJson(new AESCore.Serialization<KeyValuePair<string, string>>(keyValuePairs));
            File.WriteAllText(filePath, json);
            #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
            #endif
            Debug.Log($"データをJSON形式で保存しました: {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"データ保存に失敗しました(JSON): {ex.Message}");
        }
    }

    // TXT形式で保存するメソッド
    private void SaveToTxt(string filePath, List<KeyValuePair<string, string>> keyValuePairs)
    {
        try
        {
            List<string> lines = new List<string>();
            foreach (var pair in keyValuePairs)
            {
                lines.Add($"キー: {pair.Key}, 保存された値: {pair.Value}");
            }

            File.WriteAllLines(filePath, lines);
            #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
            #endif
            Debug.Log($"データをTXT形式で保存しました: {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"データ保存に失敗しました(TXT): {ex.Message}");
        }
    }

    // インスペクターから呼ばれる保存ボタンのメソッド
    public void OnGenerateDataButtonPressed()
    {
        SaveEncryptedData();
    }
}
