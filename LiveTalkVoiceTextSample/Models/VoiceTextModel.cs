/*
 * Copyright 2019 FUJITSU SOCIAL SCIENCE LABORATORY LIMITED
 * クラス名　：VoiceTextModel
 * 概要      ：HOYA VoiceTextと連携
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LiveTalkVoiceTextSample.Models
{
    internal class VoiceTextModel
    {
        private string APIKey = "<<<<VoiceText APIKey>>>>";
        private string Url = "https://api.voicetext.jp/v1/tts";
        private string ProxyServer = "";    // PROXY経由なら proxy.hogehoge.jp:8080 のように指定
        private string ProxyId = "";        // 認証PROXYならIDを指定
        private string ProxyPassword = "";  // 認証PROXYならパスワードを指定

        public async Task<(byte[], string)> TextToSpeechAsync(string text)
        {
            try
            {
                // パラメタ設定
                var param = new Dictionary<string, string>
                    {
                        {"text", text},
                        {"speaker", "hikari"},
                        {"format", "wav"},
                    };

                // プロキシ設定
                var ch = new HttpClientHandler() { UseCookies = true };
                if (!string.IsNullOrEmpty(this.ProxyServer))
                {
                    var proxy = new System.Net.WebProxy(this.ProxyServer);
                    if (!string.IsNullOrEmpty(this.ProxyId) && !string.IsNullOrEmpty(this.ProxyPassword))
                    {
                        proxy.Credentials = new System.Net.NetworkCredential(this.ProxyId, this.ProxyPassword);
                    }
                    ch.Proxy = proxy;
                }
                else
                {
                    ch.Proxy = null;
                }

                // Web API呼び出し
                using (var client = new HttpClient(ch))
                {
                    using (var request = new HttpRequestMessage())
                    {
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri(this.Url);
                        request.Content = new FormUrlEncodedContent(param);
                        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(this.APIKey)));
                        var response = await client.SendAsync(request);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return (await response.Content.ReadAsByteArrayAsync(), string.Empty);
                        }
                        else
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            using (var json = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)))
                            {
                                var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(TVoiceTextResult));
                                {
                                    var result = ser.ReadObject(json) as TVoiceTextResult;
                                    return (null, result.error.message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        [DataContract]
        public class TVoiceTextResult
        {
            [DataMember]
            public TError error { get; set; }
        }

        [DataContract]
        public class TError
        {
            [DataMember]
            public string message { get; set; }
        }
    }
}
