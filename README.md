# ARKitten2019
技術評論社の月刊誌Software Designで連載中の「スマホARアプリ開発入門」で開発するサンプルのリポジトリです。

[2019年10月号](http://gihyo.jp/magazine/SD/archive/2019/201910)の記事での完成状態がコミットされています。

<img src="ARKitten2019.jpg" width="420px">

## 2019年10月号

### 誌面掲載ソースコードの全文
- [子猫出現時に向きを変える機能を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/1098dbfb1e70a0830e73e71d0fbd2dabd31488c2#diff-edc75b09adb2b481f7fd694404d9495d)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/201910_part1/ARKitten/Assets/Scripts/PlaceObject.cs)

- [カメラの動きに応じて子猫の向きを変える機能を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/5012f8a4f9c3216f31a6c3ccf5dd045a4ba6599a#diff-edc75b09adb2b481f7fd694404d9495d)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/201910_part2/ARKitten/Assets/Scripts/PlaceObject.cs)

- [カメラの動きに応じて子猫の向きをアニメーションする機能を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/f22b1f30a967d7a58334c538d027eeb1f8fc980a#diff-edc75b09adb2b481f7fd694404d9495d)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/201910/ARKitten/Assets/Scripts/PlaceObject.cs)

### 追加の解説
- [プレイモードで子猫が向きを変える動作を確認する手順](https://youtu.be/C1y0m2BwYNQ)
- [スマートフォンでの実行時の動画](https://youtu.be/t644iFvGyHI)

---

## 2019年9月号

### 誌面掲載ソースコードの全文
- [LightEstimationのスクリプト](https://github.com/ktaka/ARKitten2019/blob/201909/ARKitten/Assets/Scripts/LightEstimation.cs)
- [ARShadow.shader](https://github.com/ktaka/ARKitten2019/blob/201909/ARKitten/Assets/Materials/ARShadow.shader)

### 追加の解説
- [光源推定の組み込み手順の動画](https://youtu.be/ooC_l_VHQPc)
- [影（キャストシャドウ）組み込み手順の動画](https://youtu.be/KO2YyDGR5Lk)
- [スマートフォンでの実行時の動画](https://youtu.be/_qIKQdJlpak)

---

## 2019年8月号

### 誌面掲載ソースコードの全文
- [PlaceObjectのスクリプト](https://github.com/ktaka/ARKitten2019/blob/201908/ARKitten/Assets/Scripts/PlaceObject.cs)

### 追加の解説
- [Unity HubからのUnityのインストール解説動画](https://youtu.be/BWoLqxWhHUw)
- [スマートフォンでの実行時の動画](https://youtu.be/n6xoIz4smTk)

### ビルドについて
この完成状態のリポジトリをダウンロードしてビルドする際は、Unityのパッケージマネージャーから下記のパッケージのインストールが必要です。
- AR Foundation (version 2.2.0)
- ARCore XR Plugin (version 2.1.0)
- ARKit XR Plugin (version 2.1.0)

---

## Cute Kittenのアセット
このプロジェクトで使用している猫のモデルのアセット（Cute Kitten）は元はUnityのAsset Storeからダウンロード、インポートしていたものです。
アセットの作者の事情によりAsset Storeからはこのアセットはダウンロードできなくなってしまいましたが、このサンプルプロジェクトに含める許可を頂きました。

作者の[Alexey Kuznetsov](http://leshiy3d.com/)氏に感謝します。

Special thanks to [Alexey Kuznetsov](http://leshiy3d.com/), the creator of Cute Kitten.
