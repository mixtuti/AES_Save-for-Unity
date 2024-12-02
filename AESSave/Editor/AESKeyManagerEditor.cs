using UnityEditor;
using UnityEngine;
using AES;

[CustomEditor(typeof(AESKeyManager))]
public class AESKeyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // デフォルトのインスペクターを描画
        DrawDefaultInspector();

        AESKeyManager manager = (AESKeyManager)target;

        // ランダムキー生成ボタンを追加
        if (GUILayout.Button("ランダムキーを生成"))
        {
            manager.GenerateRandomEncryptionKey();
            EditorUtility.SetDirty(manager);  // 変更を保存するために必要
        }
    }
}
