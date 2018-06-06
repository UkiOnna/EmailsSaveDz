using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using System.IO;
using Newtonsoft.Json;

namespace EmailSaveDz
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите логин ");
            string login;
            login = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(login))
            {
                Console.Write("Вы ввели неправильный логин");
            }

            Console.Write("Введите пароль ");
            string password;
            password = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(password))
            {
                Console.Write("Вы ввели пароль неверно");
            }

            using (Imap imap = new Imap())
            {
                imap.ConnectSSL("imap.gmail.com");

                imap.UseBestLogin(login, password);
                imap.SelectInbox();

                var uids = imap.GetAll();
                uids.Reverse();

                List<IMail> emails = new List<IMail>();
                foreach (var uid in uids)
                {
                    var email = imap.GetMessageByUID(uid);
                    IMail mail = new MailBuilder().CreateFromEml(email);
                    emails.Add(mail);
                }

                using (FileStream stream = new FileStream("emails.json", FileMode.Create))
                {
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        using (JsonTextWriter writer = new JsonTextWriter(streamWriter))
                        {
                            JsonSerializer jsonSerializer = new JsonSerializer();


                            jsonSerializer.Serialize(writer, emails);
                        }
                    }
                }
                //Вывод всех сообщений
                foreach (var mail in emails)
                {

                    Console.WriteLine($"Subject: {mail.Subject}");

                    foreach (var sender in mail.From)
                    {
                        Console.Write($"Sender: {sender.Address}\t");
                    }
                    Console.WriteLine(mail.Text);
                    if (mail.Date != null && mail.Date.HasValue)
                    {
                        Console.WriteLine(mail.Date.Value);
                    }

                    foreach (var attachment in mail.Attachments)
                    {
                        Console.WriteLine(attachment.FileName);
                    }
                    Console.WriteLine("------------------------------------------------------------------");
                }
                Console.ReadLine();
            }
        }
    }
}
