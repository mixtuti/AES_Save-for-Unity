# AES_Save
![代替テキスト](https://img.shields.io/badge/Unity-2022.3+-orange) <img src="http://img.shields.io/badge/License-Unlicense license-blue.svg?style=flat"> <img src="http://img.shields.io/badge/Language-C%23-green.svg?style=flat"><br>
PlayerPrefsをAES暗号を用いて保存するライブラリ<br>
int float string以外の型も保存可能

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
### 1. Unity Package Managerを使う方法
Window > Package Managerを開き、Add Package from git URLを選択する。
その後、以下のURLを入力する。
```
https://github.com/mixtuti/AES_Save-for-Unity.git?path=AESSave
```
### 2. Import Packageを使う方法
リリースから最新のUnity Packageをダウンロードする。

## 関数
## リファレンス
