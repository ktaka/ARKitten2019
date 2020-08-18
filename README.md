# ARKitten2019
技術評論社の月刊誌Software Designで連載中の「スマホARアプリ開発入門」で開発するサンプルのリポジトリです。

※ Android用にビルドする場合は[こちら](https://github.com/ktaka/ARKitten2019/wiki/Android%E7%94%A8%E3%81%AE%E3%83%93%E3%83%AB%E3%83%89%E8%A8%AD%E5%AE%9A%EF%BC%8864bit%E5%AF%BE%E5%BF%9C%EF%BC%89)をご覧ください。64bit対応のアプリをビルドする方法を追記してあります。

masterブランチには[2020年8月号](http://gihyo.jp/magazine/SD/archive/2020/202008)の記事での完成状態がコミットされています。
ビルドする際には次の3つのアセットとARCore Extensions for AR Foundationパッケージをインポートしてください。

- アセット
  - [Yughues Free Fabric Materials（ボールのマテリアルに使用）](https://assetstore.unity.com/packages/2d/textures-materials/fabric/yughues-free-fabric-materials-13002)
  - [FREE Casual Food Pack（子猫に上げるご飯に使用）](https://assetstore.unity.com/packages/3d/props/food/free-casual-food-pack-mobile-vr-85884)
  - [139 Vector Icons（カメラアイコンに使用）](https://assetstore.unity.com/packages/2d/gui/icons/139-vector-icons-69968)

- パッケージ
  - [ARCore Extensions for AR Foundation](https://developers.google.com/ar/develop/unity-arf/enable-arcore#get-package)

<img src="ARKitten2019.jpg" width="420px">

## 2020年9月号

[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/202009r)を参照ください。

※ Android用にビルドする場合は[こちら](https://github.com/ktaka/ARKitten2019/wiki/Android%E7%94%A8%E3%81%AE%E3%83%93%E3%83%AB%E3%83%89%E8%A8%AD%E5%AE%9A%EF%BC%8864bit%E5%AF%BE%E5%BF%9C%EF%BC%89)をご覧ください。64bit対応のアプリをビルドする方法を追記してあります。

## 2020年8月号

### 誌面掲載URL
-[EDM4Uのダウンロード](https://github.com/googlesamples/unity-jar-resolver/raw/master/external-dependency-manager-latest.unitypackage)

-[UnityプロジェクトにFirebaseを追加する（公式ページ）](https://firebase.google.com/docs/unity/setup?hl=ja)

-[Firebaseコンソール](https://console.firebase.google.com/)


### 誌面掲載ソースコードの全文

- [macOSで環境変数JAVA_HOMEを設定するファイル]((https://github.com/ktaka/ARKitten2019/blob/202008r/environment.plist)

- [PlaceObjectでFirebase Realtime Databaseにアクセスできるようにする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/f2dd504753851a49c7801ed696a00d4901cc4129#diff-edc75b09adb2b481f7fd694404d9495d)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202008r/ARKitten/Assets/Scripts/PlaceObject.cs)

---

## 2020年7月号
[2020年7月号](http://gihyo.jp/magazine/SD/archive/2020/202007)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/202007d)にあります。

### 誌面掲載URL
- [GitHubのarcore-unity-extensionsのリリースページ (ARCore Extensions for AR Foundation)](https://github.com/google-ar/arcore-unity-extensions/releases/)
- [ARCore Cloud Anchor APIのサイト](https://console.cloud.google.com/apis/library/arcorecloudanchor.googleapis.com)

### 誌面掲載ソースコードの全文

- [ShareCloudAnchorスクリプト](https://github.com/ktaka/ARKitten2019/blob/202007d/ARKitten/Assets/Scripts/ShareCloudAnchor.cs)

- [PlaceObjectでCloud Anchorを扱えるようにする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/558df18cb25523605f0cec5d391b0991e2a2d0b2#diff-edc75b09adb2b481f7fd694404d9495d)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202007d/ARKitten/Assets/Scripts/PlaceObject.cs)

- [UIManagerでCloud Anchorを扱えるようにする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/558df18cb25523605f0cec5d391b0991e2a2d0b2#diff-1bc3ba85714110219306dc15bb756ead)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202007d/ARKitten/Assets/Scripts/UIManager.cs)


---

## 2020年6月号
[2020年6月号](http://gihyo.jp/magazine/SD/archive/2020/202006)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/202006)にあります。

### 誌面掲載ソースコードの全文

- [PhotoCaptureスクリプト](https://github.com/ktaka/ARKitten2019/blob/202006/ARKitten/Assets/Scripts/PhotoCapture.cs)

- [AddToPhotoLibrary.m（iOS用プラグイン）](https://github.com/ktaka/ARKitten2019/blob/202006/ARKitten/Assets/Plugins/iOS/AddToPhotoLibrary.m)

---

## 2020年5月号
[2020年5月号](http://gihyo.jp/magazine/SD/archive/2020/202005)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/202005r)にあります。

### 誌面掲載ソースコードの全文

- [CatPreferencesスクリプト](https://github.com/ktaka/ARKitten2019/blob/202005r/ARKitten/Assets/Scripts/CatPreferences.cs)

- [BallOperationで経験値を読み書きできるようにする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/41497028a50906ba27ecb6f590d639346b1f312d#diff-b6b6d71d3b256e23f6770bb5234920c2)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202005r/ARKitten/Assets/Scripts/BallOperation.cs)

- [CatControlで経験値を保存できるようにする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/41497028a50906ba27ecb6f590d639346b1f312d#diff-39b62311bbe2ea5e0e37aa2edc44fc5c)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202005r/ARKitten/Assets/Scripts/CatControl.cs)

- [FoodOperationで経験値を保存できるようにする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/41497028a50906ba27ecb6f590d639346b1f312d#diff-f6aeaf34b92b4fb004d561dee6096083)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202005r/ARKitten/Assets/Scripts/FoodOperation.cs)

- [PlaceObjectで経験値を反映できるようにする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/41497028a50906ba27ecb6f590d639346b1f312d#diff-edc75b09adb2b481f7fd694404d9495d)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202005r/ARKitten/Assets/Scripts/PlaceObject.cs)

---

## 2020年4月号
[2020年4月号](http://gihyo.jp/magazine/SD/archive/2020/202004)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/202004r)にあります。

- [スマートフォンでの実行時の動画](https://youtu.be/3XxeNConOC0)

### 誌面掲載ソースコードの全文

#### なでる操作

- [CatControlスクリプト](https://github.com/ktaka/ARKitten2019/blob/202004r_part1/ARKitten/Assets/Scripts/CatControl.cs)

#### ごはんをあげる操作

- [FoodOperationスクリプト](https://github.com/ktaka/ARKitten2019/blob/202004r/ARKitten/Assets/Scripts/FoodOperation.cs)

- [FoodControlスクリプト](https://github.com/ktaka/ARKitten2019/blob/202004r/ARKitten/Assets/Scripts/FoodControl.cs)

- [UIManagerでごはんをあげる操作を選択できるようにする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/ba02a70cddfa616dc9f1ff3c4385dd0ccae0db6e#diff-1bc3ba85714110219306dc15bb756ead)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202004r/ARKitten/Assets/Scripts/UIManager.cs)

- [CatControlでごはんに接触したら動きを止める（差分表示）](https://github.com/ktaka/ARKitten2019/commit/ba02a70cddfa616dc9f1ff3c4385dd0ccae0db6e#diff-39b62311bbe2ea5e0e37aa2edc44fc5c)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202004r/ARKitten/Assets/Scripts/CatControl.cs)

---

## 2020年3月号
[2020年3月号](http://gihyo.jp/magazine/SD/archive/2020/202003)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/202003r)にあります。

### リソース

- [Dropdown用のアイコンをダウンロード](https://github.com/ktaka/ARKitten2019/raw/202003r/dropdown_icon.unitypackage)

### 誌面掲載ソースコードの全文

- [UIManagerスクリプトにDropdownのコールバックを追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/ebe39c6e757ff44f0be00781bf08884ed9a45b1a#diff-1bc3ba85714110219306dc15bb756ead)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202003_part1/ARKitten/Assets/Scripts/UIManager.cs)

- [UIManagerスクリプトのタップ処理を修正する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/dc6564e26466b5bf0f448bf72eba25460e4c46b7#diff-1bc3ba85714110219306dc15bb756ead)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202003r/ARKitten/Assets/Scripts/UIManager.cs)

- [BallControlスクリプトにレイヤ指定を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/dc6564e26466b5bf0f448bf72eba25460e4c46b7#diff-d5678ccd00ac1fff0ea8cbb19593cb35)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202003r/ARKitten/Assets/Scripts/BallControl.cs)

---

## 2020年2月号
[2020年2月号](http://gihyo.jp/magazine/SD/archive/2020/202002)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/202002r)にあります。

### リソース
- [AR Foundationのサンプル集（AR Foundation 2.1系）をダウンロード](https://github.com/Unity-Technologies/arfoundation-samples/archive/2.1.zip)

- [PlanePatternPadテクスチャ](https://github.com/ktaka/ARKitten2019/raw/202002r/ARKitten/Assets/Materials/PlanePatternPawPad.png)

### 誌面掲載ソースコードの全文
- [ARFeatheredPlaneMeshVisualizerスクリプト（日本語解説つき）](https://github.com/ktaka/ARKitten2019/blob/202002r/ARKitten/Assets/Scripts/ARFeatheredPlaneMeshVisualizer.cs)

- [FadePlaneOnBoundaryChangeスクリプト（日本語解説つき）](https://github.com/ktaka/ARKitten2019/blob/202002r/ARKitten/Assets/Scripts/FadePlaneOnBoundaryChange.cs)

- [PlaceObjectスクリプトにUI制御機能を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/b7273f8169b01609072e9b5d8b37e505cab6f9c4#diff-edc75b09adb2b481f7fd694404d9495d)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202002r/ARKitten/Assets/Scripts/PlaceObject.cs)

- [BallControlスクリプトのUI修正（差分表示）](https://github.com/ktaka/ARKitten2019/commit/b7273f8169b01609072e9b5d8b37e505cab6f9c4#diff-d5678ccd00ac1fff0ea8cbb19593cb35)
    - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202002r/ARKitten/Assets/Scripts/BallControl.cs)

- [UI Managerスクリプト](https://github.com/ktaka/ARKitten2019/blob/202002r/ARKitten/Assets/Scripts/UIManager.cs)

---

## 2020年1月号
[2020年1月号](http://gihyo.jp/magazine/SD/archive/2020/202001)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/202001r2)にあります。

### 誌面掲載ソースコードの全文
- [BallControlスクリプトのRaycast対応（差分表示）](https://github.com/ktaka/ARKitten2019/commit/173b30e3cac14699165058992c404b95727bdc6e#diff-d5678ccd00ac1fff0ea8cbb19593cb35)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202001r2/ARKitten/Assets/Scripts/BallControl.cs)

- [BallOperationスクリプトのイベント対応（差分表示）](https://github.com/ktaka/ARKitten2019/commit/173b30e3cac14699165058992c404b95727bdc6e#diff-b6b6d71d3b256e23f6770bb5234920c2)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202001r2/ARKitten/Assets/Scripts/BallOperation.cs)

- [CameraControlスクリプト](https://github.com/ktaka/ARKitten2019/blob/202001r2/ARKitten/Assets/Scripts/CameraControl.cs)

- [PlaceObjectスクリプトに子猫が走る機能を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/f8e4f33e33e0d4e82e449a795dad37f0fbb631e9#diff-edc75b09adb2b481f7fd694404d9495d)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/202001r2/ARKitten/Assets/Scripts/PlaceObject.cs)

### 動画解説
- [スマートフォンでの実行時の動画](https://youtu.be/reDrOik5acM)

---

## 2019年12月号
[2019年11月号](http://gihyo.jp/magazine/SD/archive/2019/201912)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/201912r)にあります。

### 誌面掲載ソースコードの全文
- [PlaceObjectのタップ処理を2本指にする（差分表示）](https://github.com/ktaka/ARKitten2019/commit/0b37ae7a57c8994fdf3f9a1fcf6d547c92e4f7bc#diff-edc75b09adb2b481f7fd694404d9495d)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/201912r_part_1/ARKitten/Assets/Scripts/PlaceObject.cs)

- [BallControlスクリプト](https://github.com/ktaka/ARKitten2019/blob/201912r_part_1/ARKitten/Assets/Scripts/BallControl.cs)

- [PlaceObjectにRigidbodyによる制御を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/f78edc42b6547c22676b657c7a1f43b60deaf83b#diff-edc75b09adb2b481f7fd694404d9495d)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/201912r/ARKitten/Assets/Scripts/PlaceObject.cs)

- [RootMotionにRigidbodyによる制御を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/f78edc42b6547c22676b657c7a1f43b60deaf83b#diff-6db954ca0fb619beaca540bcc60e2a2a)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/201912r/ARKitten/Assets/Scripts/RootMotion.cs)

- [BallOperationスクリプト](https://github.com/ktaka/ARKitten2019/blob/201912r/ARKitten/Assets/Scripts/BallOperation.cs)

- [BallControlに子猫が反応するための機能を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/f78edc42b6547c22676b657c7a1f43b60deaf83b#diff-d5678ccd00ac1fff0ea8cbb19593cb35)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/201912r/ARKitten/Assets/Scripts/BallControl.cs)

### 動画解説
- [スマートフォンでの実行時の動画](https://youtu.be/iByN_FFv370)

---

## 2019年11月号
[2019年11月号](http://gihyo.jp/magazine/SD/archive/2019/201911)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/201911r)にあります。

### 誌面掲載ソースコードの全文
- [PlaceObjectにアニメーション制御を追加する（差分表示）](https://github.com/ktaka/ARKitten2019/commit/20ea10f2e973e1eb67af021bad706b834350dd6b#diff-edc75b09adb2b481f7fd694404d9495d)
  - [全体表示](https://github.com/ktaka/ARKitten2019/blob/201911r/ARKitten/Assets/Scripts/PlaceObject.cs)

- [RootMotionスクリプト](https://github.com/ktaka/ARKitten2019/blob/201911r/ARKitten/Assets/Scripts/RootMotion.cs)

### 動画解説
- [Animator Controllerを作成する手順](https://youtu.be/jHsp4NLCsUs)
- [プレハブの作成](https://youtu.be/dxLWEPFlESo)
- [スマートフォンでの実行時の動画](https://youtu.be/qD2VeAyYbZI)

---

## 2019年10月号
[2019年10月号](http://gihyo.jp/magazine/SD/archive/2019/201910)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/201910)にあります。

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
[2019年09月号](http://gihyo.jp/magazine/SD/archive/2019/201909)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/201909)にあります。

### 誌面掲載ソースコードの全文
- [LightEstimationのスクリプト](https://github.com/ktaka/ARKitten2019/blob/201909/ARKitten/Assets/Scripts/LightEstimation.cs)
- [ARShadow.shader](https://github.com/ktaka/ARKitten2019/blob/201909/ARKitten/Assets/Materials/ARShadow.shader)

### 追加の解説
- [光源推定の組み込み手順の動画](https://youtu.be/ooC_l_VHQPc)
- [影（キャストシャドウ）組み込み手順の動画](https://youtu.be/KO2YyDGR5Lk)
- [スマートフォンでの実行時の動画](https://youtu.be/_qIKQdJlpak)

---

## 2019年8月号
[2019年08月号](http://gihyo.jp/magazine/SD/archive/2019/201908)の記事での完成状態は[こちらのブランチ](https://github.com/ktaka/ARKitten2019/tree/201908)にあります。

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
