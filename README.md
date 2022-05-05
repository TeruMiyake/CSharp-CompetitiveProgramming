# CSharp-CompetitiveProgramming
競技プログラミング（主に AtCoder ）用テンプレート

## 構成
### アルゴ用テンプレート
AtCoderTemplate プロジェクト
### ヒューリスティック用テンプレート
未作成
### 自作ライブラリ
LibraryForDotnetCore プロジェクト

## 特徴
- 通常の Program.cs ではなく LocalEntryPoint.cs をエントリポイントとすることで、ローカル環境と提出時の設定を分けています。
### 入出力
- input/ ディレクトリに *.in を入れておけば、それらが入力ファイルとみなされます。
- デバッグ(F5)などでプログラムを実行すると output/ ディレクトリに *.out が出力されます。
### 提出用ファイル生成
- デバッグ(F5)などでプログラムを実行すると [SourceExpander](https://github.com/kzrnm/SourceExpander) によって Combined.csx が生成されます。
- そのファイルを開き、中身のテキストをそのまま AtCoder などの提出欄に貼り付ければ OK です。
### ライブラリの利用
- 名前空間 LibraryForDotnetCore.* に記述されています。
- 適宜 using LibraryForDotnetCore.MySTL; などとして利用してください。
- （[SourceExpander](https://github.com/kzrnm/SourceExpander) によって Combined.csx に自動的に含められるので、コピー＆ペーストなどは不要です。）

## Requirements
- [SourceExpander（必須）](https://github.com/kzrnm/SourceExpander)
- [ac-library-csharp（推奨）](https://github.com/kzrnm/ac-library-csharp)

## How to Use
- 適当にダウンロードして Visual Studio で開く
- （必要に応じて、プロジェクト(P)->テンプレートのエクスポート(E) から AtCoderTemplate をテンプレート化し、そのテンプレートを使ってプロジェクトを新規作成して問題毎に分けると便利です。テンプレートは AtCoderTemplates 内にも用意してあります。）
- input/ ディレクトリに *.in（1.in, 2.in など）といった入力ファイルを追加
- Program.cs 内 Solver.Solve() に解答コードを記述
- デバッグ(F5) などで実行
- output/*.out の内容が問題ページのサンプルと一致しているか確認
- Combined.csx の内容をコピーしてコンテストページで提出

## License
- 自作部分の著作権は完全フリーとします。
- ただし他の方の記事・文献などを参考に作成した部分（コード内に注釈してあります）があり、それらの著作権の一部は当該記事・文献の著者に帰属する場合があります。