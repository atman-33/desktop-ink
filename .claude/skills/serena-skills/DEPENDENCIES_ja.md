# Serena Skills - 必要な依存パッケージ

## ディレクトリ構造

```
serena-skills/
├── lib/               # 共有実装ライブラリ
│   ├── common/       # 共通ユーティリティ
│   ├── serena_deps/  # Serena依存コード
│   └── solidlsp/     # LSP実装
└── skills/           # サブスキル群
    ├── code-editor/
    ├── symbol-search/
    └── ...
```

## 初期セットアップ（環境ごとに1回だけ実行）

スクリプトは自動的に仮想環境を検出して使用します。

### Linux/WSL/macOS

```bash
# uv がインストールされていない場合
curl -LsSf https://astral.sh/uv/install.sh | sh

# serena-skills ディレクトリに移動
cd ~/.codex/skills/serena-skills

# 仮想環境を作成
uv venv

# 必要なパッケージをインストール
uv pip install pathspec requests pyright joblib pyyaml psutil overrides python-dotenv
```

### Windows (PowerShell)

```powershell
# uv がインストールされていない場合
irm https://astral.sh/uv/install.ps1 | iex

# serena-skills ディレクトリに移動
cd ~\.codex\skills\serena-skills

# 仮想環境を作成
uv venv

# 必要なパッケージをインストール
uv pip install pathspec requests pyright joblib pyyaml psutil overrides python-dotenv
```

### 仕組み

- スクリプトは自動的に `.venv` ディレクトリを検出して使用
- 仮想環境の手動アクティベーションは不要
- クロスプラットフォーム: Linux、WSL、macOS、Windows で動作
- ポータブル: serena-skills フォルダを任意のプロジェクトにコピー可能（異なるOS では `uv venv` を再実行）

### 注意事項

- 仮想環境（`.venv`）は OS 固有です
- 異なる OS に移動した場合は、上記セットアップを再実行してください
- Linux環境で「externally-managed-environment」エラーが出る場合は、この方法を使用してください

## パッケージの説明

- **pathspec**: ファイルパターンマッチング用
- **requests**: HTTP クライアント (charset-normalizer を含む)
- **pyright**: Python言語サーバー
- **joblib**: 並列処理ユーティリティ
- **pyyaml**: YAML設定ファイルのパース用

## 言語サーバーのインストール (オプション)

Python以外の言語をサポートする場合は、対応する言語サーバーをインストールしてください:

- **TypeScript/JavaScript**: `npm install -g typescript typescript-language-server`
- **Go**: `gopls` をインストール
- **Rust**: `rust-analyzer` をインストール

詳細はSerenaのドキュメントを参照してください。
