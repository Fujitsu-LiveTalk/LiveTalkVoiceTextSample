/*
 * Copyright 2019 FUJITSU SOCIAL SCIENCE LABORATORY LIMITED
 * システム名：LiveTalkVoiceTextSample
 * 概要      ：LiveTalk-VoiceText連携サンプルアプリ
*/
using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LiveTalkVoiceTextSample
{
    class Program
    {
        static FileCollaboration FileInterface;
        static BlockingCollection<byte[]> AudioQueue = new BlockingCollection<byte[]>();
        static CancellationTokenSource TokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            var model = new Models.VoiceTextModel();
            var param = new string[]
            {
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "LiveTalkOutput.csv"),
            };
            if (args.Length >= 1)
            {
                param[0] = args[0];
            }
            Console.WriteLine("InputCSVFileName  :" + param[0]);
            FileInterface = new FileCollaboration(param[0], "");

            // ファイル入力(LiveTalk常時ファイル出力からの入力)
            FileInterface.RemoteMessageReceived += async (s) =>
            {
                var reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var items = reg.Split(s);

                Console.WriteLine(">>>>>>>");
                Console.WriteLine("DateTime:" + items[0]);
                Console.WriteLine("Speaker:" + items[1]);
                Console.WriteLine("Speech contents:" + items[2]);
                Console.WriteLine("Translate content:" + items[3]);

                (byte[] waveData, string errorMessage) = await model.TextToSpeechAsync(items[3] == "\"\"" ? items[2] : items[3]);
                if (waveData != null)
                {
                    // 音声合成キューにエントリ
                    AudioQueue.Add(waveData);
                }
                else
                {
                    // エラーメッセージ表示
                    Console.WriteLine(errorMessage);
                }
            };

            // 音声合成キュー処理
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // 音声合成の再生
                    if (AudioQueue.TryTake(out byte[] data, -1, TokenSource.Token))
                    {
                        using (var ms = new MemoryStream(data))
                        {
                            using (var audio = new WaveFileReader(ms))
                            {
                                using (var outputDevice = new WaveOutEvent())
                                {
                                    outputDevice.Init(audio);
                                    outputDevice.Play();
                                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                                    {
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                        }
                    }
                }
            });

            // ファイル監視開始
            if (System.IO.File.Exists(param[0]))
            {
                System.IO.File.Delete(param[0]);
            }
            FileInterface.WatchFileSart();

            // 処理終了待ち
            var message = Console.ReadLine();

            // ファイル監視終了
            TokenSource.Cancel(true);
            FileInterface.WatchFileStop();
        }
    }
}
