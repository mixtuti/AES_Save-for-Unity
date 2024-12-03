# AES_Save

![代替テキスト](https://img.shields.io/badge/Unity-2022.3+-orange) <img src="http://img.shields.io/badge/License-Unlicense license-blue.svg?style=flat"> <img src="http://img.shields.io/badge/Language-C%23-green.svg?style=flat"><br>
PlayerPrefsをAES暗号を用いて保存するライブラリです。<br>
int float string以外の型も保存可能です。

## システム要件

Unity 2022.3.28 での動作は確認済みです。

## 概要

Unity標準のPlayerPrefsの拡張機能です。<br>
AES暗号を用いてセーブデータを暗号化します。<br>
int型やfloat型、string型などの基本の型に加え、Bool型、List型、Vector2型、Vector3型、Dictionary型に対応しています。<br>
int型、Float型限定で2つの値を比較する機能も付いています。
> [!IMPORTANT]
> あくまでPlayerPrefsの拡張であるため特別な理由がない限り利用は推奨しません。

## 依存関係

なし

## 導入方法

### 1. プロジェクトへの導入
導入方法は大きく分けて2つあります。お好きな方法で導入してください。

#### 1. Unity Package Managerを使う方法
「Window > Package Manager」を開き、「Add Package from git URL」を選択します。<br>
その後、以下のURLを入力してください。
```
https://github.com/mixtuti/AES_Save-for-Unity.git?path=AESSave
```
#### 2. Import Packageを使う方法
リリースから最新のUnity Packageをダウンロードし、インポートします。
> [!TIP]
> 更新が遅くなることが多いので1の方法を使うことをお勧めします。

### 2. 利用方法
適当なゲームオブジェクトに、`AESKeyManager.cs`、`AESDataExploration.cs`をアタッチする。
> [!NOTE]
> `AESDataExploration.cs`は、必須ではなく任意でアタッチすることができます。

## スクリプトの解説

### 1. AESCore.cs
int,float,string型のデータを保存し暗号化するためのスクリプトです。<br>
暗号化以外にも２つのデータの差を比較する比較する機能もあります。<br>
AESKeyManagerで生成された暗号キーを用いて暗号化と復号を行います。<br>

### 2. AESKeyManager.cs
適当なオブジェクトにアタッチして用いるスクリプト。<br>
キーの生成には32バイトの長さが必要です。
> [!WARNING]
> 途中で暗号化キーを変更してしまうと正常に復号ができなくなります。

### 3. AESDataExploration.cs
保存された内容をtxtやjsonファイルに書き出し、閲覧するためのデバッグ用ツールです。

### 4. AESKeyManagerEditor.cs
ランダムキー生成用のボタンを追加するためのエディタ拡張機能です。

### 5. AESDataExplorationEditor.cs
保存用のボタンを追加するためのエディタ拡張機能です。

## 関数

1. SetEncryptedInt(string key, int value)
2. GetEncryptedInt(string key, int defaultValue = 0)
3. SetEncryptedFloat(string key, float value)
4. GetEncryptedFloat(string key, float defaultValue = 0.0f)
5. SetEncryptedString(string key, string value)
6. GetEncryptedString(string key, string defaultValue = "")
7. SetEncryptedBool(string key, bool value)
8. GetEncryptedBool(string key, bool defaultValue = false)
9. SetEncryptedList<T>(string key, List<T> list)
10. GetEncryptedList<T>(string key)
11. SetEncryptedVector2(string key, Vector2 vector)
12. GetEncryptedVector2(string key)
13. SetEncryptedVector3(string key, Vector3 vector)
14. GetEncryptedVector3(string key)
15. DeleteEncryptedKey(string key)
16. SetEncryptedDictionary(string key, Dictionary<string, string> dictionary)
17. GetEncryptedDictionary(string key)
18. DeleteAllEncryptedData
19. CompareIntDifference(string key1, string key2)
20. CompareFloatDifference(string key1, string key2)

#### 1. SetEncryptedInt(string key, int value)
int型の値を保存するための関数。<br>
第１引数はキーの名前(string型)、第２引数は保存する値(int型)<br>
戻り値は、なし

#### 2. GetEncryptedInt(string key, int defaultValue = 0)
保存した値をロードするための関数。<br>
第１引数はキーの名前(string型)<br>
戻り値は、defaultValue(int型)<br>
<br>
3~16は基本的に同じなので省略。<br>
#### 17. DeleteEncryptedKey(string key)
指定したキーのデータを削除する関数。<br>
第１引数はキーの名前(string型)

#### 18. DeleteAllEncryptedData
すべてのセーブデータを削除する関数。

#### 19. CompareIntDifference(string key1, string key2)
二つのセーブデータの差を比較して数値を返す関数。<br>
引数はどちらもキーの名前(string型)<br>
key1からkey2を引く形で差の計算をします。<br>
<br>
20は19と基本的に同じなので省略。<br>

## リファレンス

```cs
using UnityEngine;
using AES; // AESCoreを使用するには必須の名前空間

public class Exsample : MonoBehaviour
{
    void Start()
    {
        // 数値を保存
        AESCore.SetEncryptedInt("score1", 120);  // score1 に 120 を保存
        Debug.Log("スコア1に保存しました：" + AESCore.GetEncryptedInt("score1"));
        AESCore.SetEncryptedInt("score2", 150);  // score2 に 150 を保存
        Debug.Log("スコア2に保存しました：" + AESCore.GetEncryptedInt("score2"));

        // int 型の比較と差を計算
        int intDifference = AESCore.CompareIntDifference("score1", "score2");
        Debug.Log($"score1 と score2 の差: {intDifference}");

        // データを削除
        AESCore.DeleteEncryptedKey("score1");

        // データをすべて削除
        AESCore.DeleteAllEncryptedData();
    }
}
```
