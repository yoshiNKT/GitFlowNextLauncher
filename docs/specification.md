## 1. 概要

**GitFlowNextLauncher** は、`git-flow-next` を利用してGit FlowのFeatureブランチ作成をGUIから行うための小規模なWPFアプリケーションである。

SourceTreeのGit Flow機能が使用できなくなったことをきっかけとして、自分が実際に使用する操作に限定したGit Flowクライアントとして作成した。

本アプリではGit Flow全体をGUI化するのではなく、現在の運用で必要となるFeatureブランチの開始に機能を限定する。

___

## 2. 作成経緯
### 2.1 Git 2.55へのアップデート

開発環境でGitをバージョン2.55.0へアップデートしたところ、SourceTreeからGit FlowのFeatureブランチを作成しようとした際にエラーが発生するようになった。

SourceTree側を3.4.31.0へアップデートしても問題は解消しなかった。

一方、SourceTreeでEmbedded Gitを使用するとGit 2.51系ではFeature作成が正常に動作したため、Git本体のバージョンとSourceTreeのGit Flow機能との互換性に問題があると判断した。

### 2.2 Git FlowはGit本体の必須機能ではない

Git FlowはGitそのものの基本機能ではなく、Gitを利用したブランチ運用方法およびその操作を補助するツールである。

そのため、Git本体のバージョンアップによってGit Flow関連機能が分離されたとしても、Gitによるブランチ管理そのものには影響しない。

実際、Featureブランチについては通常のGitブランチとして、

```
develop
   └── feature/xxx
```

を作成し、GitHub上でdevelopへマージするだけでも現在の運用を成立させられる。

### 2.3 git-flow-nextの利用

SourceTreeに依存せずGit Flowを利用するため、git-flow-next を導入した。

インストール後、

```
git flow version
```

で、

```
git-flow-next version 1.2.0
```

が確認できた。

これにより、GUIから直接 `git-flow-next` を呼び出せば、SourceTreeのGit Flow機能に依存せずFeatureブランチを作成できることが確認できた。

### 2.4 必要な機能を整理

改めて普段のGit Flowの利用方法を整理した結果、実際に使用しているのはFeatureブランチの開始だけだった。

Featureの終了については、

1. FeatureブランチをGitHubへPush
2. GitHub上でdevelopへマージ
3. マージ後にFeatureブランチを自動削除

という運用を行っている。

そのため、Git Flowの `finish` 操作は現在の運用では不要と判断した。

この結果、

> SourceTreeのGit Flow機能を完全に再現する必要はなく、Featureブランチを開始できれば目的を達成できる

という結論になった。

___

## 3. アプリの目的

GitFlowNextLauncherの目的は、

> 現在開いているGitリポジトリのdevelopブランチから、git-flow-nextを利用してFeatureブランチを簡単に開始すること

である。

SourceTreeそのものを置き換えることは目的とせず、Feature開始という必要な操作だけを小さな専用GUIとして提供する。

___

## 4. 機能仕様
### 4.1 リポジトリを開く

任意のGitリポジトリをアプリに読み込む。

#### フォルダ選択

「リポジトリを開く」ボタンからフォルダ選択ダイアログを表示する。

選択されたフォルダについてGitリポジトリであることを確認し、正常な場合は以下を更新する。

* Repositoryパス
* 現在のブランチ
* Feature開始ボタンの有効状態

Gitリポジトリでない場合はエラーとして扱い、ステータス表示と警告音で通知する。

#### ドラッグ＆ドロップ

Explorer等からGitリポジトリのフォルダをドラッグ＆ドロップして開くことができる。

複数のパスがドロップされた場合やフォルダ以外がドロップされた場合はエラーとして扱う。

フォルダ選択とドラッグ＆ドロップのどちらから開いた場合も、共通の `OpenRepositoryAsync()` を使用する。

___

## 5. 前回のリポジトリを復元

最後に開いていたリポジトリを設定として保存し、次回起動時に自動的に開く。

保存対象は以下。

```
LastRepositoryPath
InitialDirPath
```

#### LastRepositoryPath

最後に開いていたGitリポジトリのパス。

アプリ起動時に存在する場合、自動的にリポジトリを開く。

#### InitialDirPath

次回のフォルダ選択ダイアログで使用する初期ディレクトリ。

リポジトリを正常に開いた際、そのリポジトリの1階層上のディレクトリを設定する。

例えば、

```
C:\Projects\MyProject
```

を開いた場合、

```
C:\Projects
```

を次回の初期ディレクトリとする。

設定はJSON形式で `LocalApplicationData` 配下に保存する。

___

## 6. 現在のブランチ表示

開いているリポジトリについて、Gitから現在のブランチを取得して画面に表示する。

また、アプリがアクティブになった際にも現在のブランチを再取得する。

これにより、例えば、

```
GitFlowNextLauncher
        ↓
Visual Studio / SourceTree
        ↓
developへcheckout
        ↓
GitFlowNextLauncherへ戻る
```

という操作を行った場合でも、現在のブランチとFeature開始ボタンの状態を更新できる。

___

## 7. Featureブランチ開始
### 7.1 入力

Feature名のみを入力する。

画面上では、

```
feature/ [ FeatureName ] [フィーチャー開始]
```

という形式で表示する。

入力値にはfeature/を含めず、Feature名だけを入力する。

例えば、

```
ImageCacheManager
```

と入力すると、

```
feature/ImageCacheManager
```

が作成される。

### 7.2 開始条件

Featureブランチを作成できるのは、現在のブランチが対象ブランチである場合のみとする。

現在の対象ブランチは、

```
develop
```

である。

比較時は大文字・小文字を区別しない。

```
develop
Develop
DEVELOP
```

はいずれも対象ブランチとして扱う。

### 7.3 Feature開始処理

Feature開始時は、

```
現在のブランチ取得
        ↓
対象ブランチ(develop)か確認
        ↓
git flow feature start <FeatureName>
        ↓
作成後の現在ブランチ取得
        ↓
画面更新
```

という流れで処理する。

実際のGit Flow操作は `git-flow-next` を利用する。

___

## 8. エラー処理

以下のケースについてユーザーへ通知する。

* リポジトリが開かれていない
* Feature名が入力されていない
* 現在ブランチの取得に失敗
* develop以外のブランチからFeatureを開始しようとした
* git-flow-nextによるFeature作成に失敗
* Feature作成後の現在ブランチ取得に失敗
* 予期しない例外が発生した

エラー発生時にはステータスメッセージを表示し、警告音を鳴らす。

ステータスメッセージには時刻も付与する。

```
[2026/08/22 22:xx:xx] メッセージ
```

___

## 9. Feature開始ボタンの状態管理

Feature開始ボタンは現在ブランチに応じて有効・無効を切り替える。

```
develop
    ↓
有効

それ以外
    ↓
無効
```

対象ブランチ判定は `IsTargetBranch()` に集約し、ボタン状態の更新は `UpdateFeatureAvailability()` で行う。

これにより、リポジトリを開いたときだけでなく、Windowが再びアクティブになったときにも状態を更新できる。

___

## 10. システム構成

アプリはWPFで実装し、NuGetパッケージを追加せず、標準.NET APIを中心に構成する。

```
GitFlowNextLauncher
│
├─ MainWindow.xaml
│
├─ MainWindow.xaml.cs
│   ├─ Window Loaded
│   ├─ Window Closing
│   ├─ Window Activated
│   ├─ Drag & Drop
│   └─ リポジトリ共通処理
│
├─ MainWindow.Repository.cs
│   ├─ フォルダ選択
│   └─ リポジトリDrop
│
├─ MainWindow.Feature.cs
│   └─ Feature開始
│
├─ GitManagement.cs
│   └─ git.exe操作
│
├─ GitFlowManagement.cs
│   └─ git-flow-next操作
│
└─ AppSettings.cs
    └─ 設定保存・読み込み
```

#### GitManagement

Git本体に対する操作を担当する。

現在は、

* Gitリポジトリ判定
* 現在ブランチ取得
* Git CLI実行

を担当する。

Git CLIの標準出力・標準エラーにはUTF-8を指定し、日本語を含むブランチ名にも対応する。

#### GitFlowManagement

git-flow-nextを利用したGit Flow操作を担当する。

現在はFeature開始のみを提供する。

```
git flow feature start "<FeatureName>"
```

を実行する。

___

## 11. 非対応機能

意図的に以下は実装対象外とする。

* Feature Finish
* Release Flow
* Hotfix Flow
* GitHub上のPull Request操作
* Merge操作
* Push操作
* Branch削除
* SourceTreeの代替となるGit操作全般
* Gitコミット操作

Featureの終了やマージについては既存のGitHub運用を継続する。

___

## 12. 現時点での完成状態

初期目的として定義した機能は実装済み。

```
任意のGitリポジトリを開く
        │
        ├─ フォルダ選択
        └─ ドラッグ＆ドロップ
                ↓
        現在ブランチ表示
                ↓
       developならFeature開始可能
                ↓
       Feature名を入力
                ↓
       git-flow-next実行
                ↓
       feature/xxx 作成
                ↓
       現在ブランチ表示更新
```

また、アプリを終了しても最後に開いていたリポジトリを記憶し、次回起動時に復元できる。

___

## 13. 作成結果

今回の開発では、SourceTreeのGit Flow機能そのものを再現することを目的とせず、実際の利用状況から必要な操作だけを抽出した。

その結果、必要だった機能は、

> 「Gitリポジトリを開いて、developからFeatureブランチを開始する」

という非常に小さなものだった。

`git-flow-next` をバックエンドとして利用することで、Git本体やSourceTreeのGit Flow実装に依存せず、自分のGit運用に必要なFeature開始機能を独立したGUIとして実現した。

初期バージョンとしてはここを完成点とし、今後必要になった機能のみ追加する方針とする。
