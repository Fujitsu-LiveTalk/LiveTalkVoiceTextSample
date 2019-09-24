# LiveTalkVoiceTextSample
LiveTalk常時ファイル出力で出力したテキストを、HOYA VoiceText を使って音声合成出力するサンプルです。  
本サンプルコードは、.NET Core 3.0で作成しています。コードレベルでは.NET Framework 4.6と互換性があります。

![Process](https://github.com/FujitsuSSL-LiveTalk/LiveTalkVoiceTextSample/blob/images/README.png)

# サンプルコードの動き
サンプルコード動作を簡単に説明すると次のような動作をします。  
1. LiveTalkで音声認識した結果がファイルに出力されるので、それを自動的に読込み、VoiceText Web APIを呼び出します。
2. VoiceText Web APIから戻ってきた音声ファイルをスピーカーの既定のデバイスで再生します。
3. もし、翻訳が表示されていたら、発話側ではなく翻訳側を音声合成します。ただし、話者はhikari固定です。
4. LiveTalkを英語音声認識で日本語翻訳にして英語で発話すると、英語で発話したものをhikariが日本語で発話してくれます。


# 事前準備
1. VoiceText Web API [https://cloud.voicetext.jp/webapi](https://cloud.voicetext.jp/webapi) の無料利用登録を行います。
2. 登録したメールアドレスにAPI キーが送られてきます。
3. 送られてきたAPIキーをユーザIDとしてサンプルコードに指定します。パスワードは指定しません。
4. インターネットとの接続がPROXY経由の場合、PROXYサーバーや認証情報を設定してください。

# 連絡事項
本ソースコードは、LiveTalkの保守サポート範囲に含まれません。  
頂いたissueについては、必ずしも返信できない場合があります。  
LiveTalkそのものに関するご質問は、公式WEBサイトのお問い合わせ窓口からご連絡ください。
