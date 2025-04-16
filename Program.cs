using System;
using System.Collections.Generic;
using System.IO;

namespace meme {
    // todo
    // 1. 2FA
    // 2. some error handling maybe

    public class Program {
        static vkapi api;

        static void auth_with_creds() {
            Console.Write("enter login: ");
            string login = Console.ReadLine();

            Console.Write("enter password: ");
            string password = Console.ReadLine();

            api.auth(login, password);
            File.WriteAllText("token.owo", api.token);
        }

        static void Main(string[] args) {
            globals.debug = args.Length > 0 && args[0].ToLower() == "-debug";
            try {
                api = new vkapi();
                if (File.Exists("token.owo")) {
                    var t = File.ReadAllText("token.owo");
                    api.auth(t);
                    string l;
                    api.utils_checkLink(out l);
                    if (l == null) {
                        auth_with_creds();
                    }
                }
                else {
                    auth_with_creds();
                }

                Console.WriteLine($"token = {api.token}");
                Console.WriteLine("i hope token above is right");

                Console.Write("enter group_id('-' before id is not needed)(leave it blank if uploading to profile): ");
                string _group_id = Console.ReadLine();
                string group_id = !string.IsNullOrEmpty(_group_id) && !string.IsNullOrWhiteSpace(_group_id) ? _group_id : null;

                Console.Write("schedule post?(y or n): ");
                string _schedule_post = Console.ReadLine();
                bool schedule_post;
                if (_schedule_post.ToLower() == "y")
                    schedule_post = true;
                else
                    schedule_post = false;

                DateTime publish_time;
                long? publish_time_unix = null;
                if (schedule_post) {
                    Console.Write("enter publish datetime(example: 11 september, 2025, 13:37): ");
                    string _schedule_time = Console.ReadLine();
                    if (!DateTime.TryParse(_schedule_time, out publish_time) ||
                        (publish_time_unix = ((DateTimeOffset)publish_time).ToUnixTimeSeconds()) <= DateTimeOffset.Now.ToUnixTimeSeconds()) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("are u retarded?");
                        Console.ReadKey();
                        return;
                    }
                }

                Console.Write("enter message(or leave it blank): ");
                string _message = Console.ReadLine();
                string message = !string.IsNullOrEmpty(_message) && !string.IsNullOrWhiteSpace(_message) ? _message : null;

                Console.Write("upload photos?(y or n): ");
                string _upload_photos = Console.ReadLine();
                bool upload_photos;
                if (_upload_photos.ToLower() == "y")
                    upload_photos = true;
                else
                    upload_photos = false;

                List<string> photos = null;
                if (upload_photos) {
                    Console.Write("enter number of photos(2-10): ");
                    string _number_of_photos = Console.ReadLine();
                    int number_of_photos;
                    if (!int.TryParse(_number_of_photos, out number_of_photos) || number_of_photos < 2 || number_of_photos > 10) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("are u retarded?");
                        Console.ReadKey();
                        return;
                    }

                    photos = new List<string>(number_of_photos);
                    for (int i = 0; i < number_of_photos; i++) {
                        int album_id;
                        string upload_url;
                        int user_id;
                        api.photos_getWallUploadServer(group_id, out album_id, out upload_url, out user_id);

                        Console.WriteLine($"album_id = {album_id}");
                        Console.WriteLine($"upload_url = {upload_url}");
                        Console.WriteLine($"user_id = {user_id}");
                        Console.WriteLine("i hope numbers and links above is right");

                        Console.Write($"enter #{i + 1} photo file path: ");
                        string file_path = Console.ReadLine();

                        int server;
                        string photo;
                        string hash;
                        api.unnamed_uploadPhoto(upload_url, file_path, out server, out photo, out hash);

                        Console.WriteLine($"server = {server}");
                        Console.WriteLine($"hash = {hash}");
                        Console.WriteLine("i hope things above is right");

                        int id;
                        int owner_id;
                        api.photos_saveWallPhoto(group_id, server, photo, hash, out id, out owner_id);

                        Console.WriteLine($"id = {id}");
                        Console.WriteLine($"owner_id = {owner_id}");
                        Console.WriteLine("i hope numbers above is right");

                        photos.Add($"photo{owner_id}_{id}");
                    }
                }

                Console.Write("enter number of audios(1-10): ");
                string _number_of_audios = Console.ReadLine();
                int number_of_audios;
                if (!int.TryParse(_number_of_audios, out number_of_audios) || number_of_audios < 1 || number_of_audios > 10) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("are u retarded?");
                    Console.ReadKey();
                    return;
                }

                List<string> audios = new List<string>(number_of_audios);
                for (int i = 0; i < number_of_audios; i++) {
                    Console.Write($"enter #{i + 1} audio owner id: ");
                    string music_owner_id = Console.ReadLine();
                    Console.Write($"enter #{i + 1} audio id: ");
                    string music_id = Console.ReadLine();
                    audios.Add($"audio{music_owner_id}_{music_id}");
                }

                string attachments = upload_photos ? string.Join(",", photos) + "," : string.Empty;
                //attachments += $"audio{music_owner_id}_{music_id}";
                attachments += string.Join(",", audios);

                api.wall_post(group_id != null ? $"-{group_id}" : null, attachments, publish_time_unix, message);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("done");
                Console.ReadKey();
                return;
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"cringe bro... \n{ex.ToString()}");
                Console.ReadKey();
                return;
            }
        }
    }
}
