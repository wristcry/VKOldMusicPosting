using System;
using System.IO;
using System.Net;
using System.Net.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace meme {
    public class vkapi {
        public const string VERSION = "5.101";

        bool _authorized;
        public bool authorized { 
            get {
                return _authorized;
            }
            private set {
                _authorized = value;
            }
        }
        string _token;
        public string token {
            get {
                return _token;
            }
            private set {
                _token = value;
            }
        }

        public void auth(string login, string password) {
            using (WebClient wc = new WebClient()) {
                string response = wc.DownloadString($"https://oauth.vk.com/token?grant_type=password&client_id=2274003&client_secret=hHbZxrka2uZ6jB1inYsH&username={login}&password={password}&scope=photos,audio,video,docs,notes,pages,status,offers,questions,wall,groups,messages,email,notifications,stats,ads,offline,docs,pages,stats,notifications");
                if (globals.debug)
                    Console.WriteLine(response);
                token = Convert.ToString(JsonConvert.DeserializeObject<JObject>(response)["access_token"]);
                authorized = true;
            }
        }

        // ehh
        public void auth(string token) {
            this.token = token;
            authorized = true;
        }

        public void utils_checkLink(out string link) {
            if (!authorized) {
                throw new Exception("must perform authorization before calling api methods");
            }

            using (HttpClient client = new HttpClient()) {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/utils.checkLink")) {
                    using (MultipartFormDataContent content = new MultipartFormDataContent()) {
                        content.Add(new StringContent(token), "access_token");
                        content.Add(new StringContent(VERSION), "v");
                        content.Add(new StringContent("https://google.com/"), "url");
                        request.Content = content;
                        using (HttpResponseMessage response = client.SendAsync(request).Result) {
                            response.EnsureSuccessStatusCode();
                            string json_response = response.Content.ReadAsStringAsync().Result;
                            if (globals.debug)
                                Console.WriteLine(json_response);
                            if (!json_response.Contains("error"))
                                link = Convert.ToString(JsonConvert.DeserializeObject<JObject>(json_response)["link"]);
                            else
                                link = null;
                        }
                    }
                }
            }
        }
        // ---

        public void photos_getWallUploadServer(string group_id, out int album_id, out string upload_url, out int user_id) {
            if (!authorized) {
                throw new Exception("must perform authorization before calling api methods");
            }

            using (HttpClient client = new HttpClient()) {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/photos.getWallUploadServer")) {
                    using (MultipartFormDataContent content = new MultipartFormDataContent()) {
                        content.Add(new StringContent(token), "access_token");
                        content.Add(new StringContent(VERSION), "v");
                        if (group_id != null) {
                            content.Add(new StringContent(group_id), "group_id");
                        }
                        request.Content = content;
                        using (HttpResponseMessage response = client.SendAsync(request).Result) {
                            response.EnsureSuccessStatusCode();
                            string json_response = response.Content.ReadAsStringAsync().Result;
                            if (globals.debug)
                                Console.WriteLine(json_response);
                            album_id = Convert.ToInt32(JsonConvert.DeserializeObject<JObject>(json_response)["response"]["album_id"]);
                            upload_url = Convert.ToString(JsonConvert.DeserializeObject<JObject>(json_response)["response"]["upload_url"]);
                            user_id = Convert.ToInt32(JsonConvert.DeserializeObject<JObject>(json_response)["response"]["user_id"]);
                        }
                    }
                }
            }
        }

        public void unnamed_uploadPhoto(string upload_url, string file_path, out int server, out string photo, out string hash) {
            if (!authorized) {
                throw new Exception("must perform authorization before calling api methods");
            }

            if (!File.Exists(file_path)) {
                throw new Exception("file not found");
            }

            using (HttpClient client = new HttpClient()) {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, upload_url)) {
                    using (MultipartFormDataContent content = new MultipartFormDataContent()) {
                        content.Add(new ByteArrayContent(File.ReadAllBytes(file_path)), "photo", Path.GetFileName(file_path));
                        request.Content = content;
                        using (HttpResponseMessage response = client.SendAsync(request).Result) {
                            response.EnsureSuccessStatusCode();
                            string json_response = response.Content.ReadAsStringAsync().Result;
                            if (globals.debug)
                                Console.WriteLine(json_response);
                            server = Convert.ToInt32(JsonConvert.DeserializeObject<JObject>(json_response)["server"]);
                            photo = Convert.ToString(JsonConvert.DeserializeObject<JObject>(json_response)["photo"]);
                            hash = Convert.ToString(JsonConvert.DeserializeObject<JObject>(json_response)["hash"]);
                        }
                    }
                }
            }
        }

        public void photos_saveWallPhoto(string group_id, int server, string photo, string hash, out int id, out int owner_id) {
            if (!authorized) {
                throw new Exception("must perform authorization before calling api methods");
            }

            using (HttpClient client = new HttpClient()) {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/photos.saveWallPhoto")) {
                    using (MultipartFormDataContent content = new MultipartFormDataContent()) {
                        content.Add(new StringContent(token), "access_token");
                        content.Add(new StringContent(photo), "photo");
                        content.Add(new StringContent(server.ToString()), "server");
                        content.Add(new StringContent(hash), "hash");
                        content.Add(new StringContent(VERSION), "v");
                        if (group_id != null) {
                            content.Add(new StringContent(group_id), "group_id");
                        }
                        request.Content = content;
                        using (HttpResponseMessage response = client.SendAsync(request).Result) {
                            response.EnsureSuccessStatusCode();
                            string json_response = response.Content.ReadAsStringAsync().Result;
                            if (globals.debug)
                                Console.WriteLine(json_response);
                            id = Convert.ToInt32(JsonConvert.DeserializeObject<JArray>(JsonConvert.DeserializeObject<JObject>(json_response)["response"].ToString())[0]["id"]);
                            owner_id = Convert.ToInt32(JsonConvert.DeserializeObject<JArray>(JsonConvert.DeserializeObject<JObject>(json_response)["response"].ToString())[0]["owner_id"]);
                        }
                    }
                }
            }
        }

        public void wall_post(string group_id, string attachments, long? publish_date, string message) {
            if (!authorized) {
                throw new Exception("must perform authorization before calling api methods");
            }

            using (HttpClient client = new HttpClient()) {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.vk.com/method/wall.post")) {
                    using (MultipartFormDataContent content = new MultipartFormDataContent()) {
                        content.Add(new StringContent(token), "access_token");
                        content.Add(new StringContent(attachments), "attachments");
                        content.Add(new StringContent(VERSION), "v");
                        if (group_id != null) {
                            content.Add(new StringContent(group_id), "owner_id");
                        }
                        if (publish_date != null) {
                            content.Add(new StringContent(publish_date.Value.ToString()), "publish_date");
                        }
                        if (message != null) {
                            content.Add(new StringContent(message), "message");
                        }
                        request.Content = content;
                        using (HttpResponseMessage response = client.SendAsync(request).Result) {
                            response.EnsureSuccessStatusCode();
                            string json_response = response.Content.ReadAsStringAsync().Result;
                            if (globals.debug)
                                Console.WriteLine(json_response);
                        }
                    }
                }
            }
        }

        public vkapi() {
            authorized = false;
            token = null;
        }
    }
}
