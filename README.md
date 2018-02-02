<<<<<<< HEAD
ファイル  
MainWindow.xaml - OverParseのUIになるxaml  
MainWindow.xaml.cs - 起動時に呼ばれ、設定の読み込みやループ処理の開始を担当  
Log.cs - インストール関連、ログ書き込み関連、.csv読み込み関連  
Click.cs - MainWindow.xaml.csからClick関連の処理を分割したもの partial class  
Hotkey.cs - ホットキー関連  
WindowsServices.cs - HideIfInactiveから呼ばれているアクティブなアプリのウィンドゥタイトル取得(?)  
Detalis - ListViewItemをダブルクリックした時のウィンドゥ  
FontDialogEx - フォントの選択ウィンドゥ  
inputbox - 自動エンカウント終了・csv更新間隔の秒数入力欄(WPF)  
=======
### OverParse Zer0 Edition
Credit to Variant for keeping the damage logs up to date
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa

Credit to TyroneSama for making the original version of OverParse

<<<<<<< HEAD
処理の流れ  
MainWindow.xaml.cs / MainWindow.MainWindow() - 起動時読み込み  
  ↓  
 Log.cs / Log.Log() - MainWindow()から呼ばれる、インストール関連  
  ↓  
 UpdateForm - 設定したms毎に情報・画面更新のループ処理
 Log.cs / Log.UpdateLog() - UpdateForm()から呼ばれるログ更新
 HideIfInactive - 1s毎にアクティブウィンドゥのタイトルを取得するループ処理
 CheckForNewLog - 1s毎に新しい.csvファイルが無いかどうかを確認するループ処理  
   
イベントハンドラまみれだったMainWindow.xaml.csをClick.csに分けたので大分見やすくなったと思いますがまだまだ見通しが悪いと言えば悪いです  
 
 翻訳に利用させて貰ったデータ
 
https://github.com/SkrubZer0/OverParse/

https://github.com/mysterious64/OverParse/
=======
Credit to Remon-7L for his original JA + Crit additions

### PLEASE NOTE
This tool and any other damage parser currently violates PSO2's Terms of Service ([In fact it is a bannable offense according to SEGA](http://pso2.jp/players/news/9224/)). Please do not misuse information given to you by this tool (or any other 3rd party parsing tool for that matter)

---

### For people that want to work on this for whatever reason...
-Need .NET Framework 4.7.1 in order to build.

-There's a lot of spaghetti that I may or may not be bothered to fix.

### Dependencies
-NHotkey and NHotkey.Wpf
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
