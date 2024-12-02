using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AESDataExploration))]
public class AESDataExplorationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // AESDataExplorationクラスのインスタンスにアクセス
        AESDataExploration aesDataExploration = (AESDataExploration)target;

        // データを確認するボタン
        if (GUILayout.Button("データを確認して保存"))
        {
            aesDataExploration.OnGenerateDataButtonPressed();
        }
    }
}
