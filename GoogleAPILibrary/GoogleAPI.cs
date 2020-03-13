using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleAPILibrary
{
    public class GoogleAPI
    {
        private GoogleAPIConfig config;
        private static GmailService service;
        private static string[] Scopes = { GmailService.Scope.GmailReadonly };


        /// <summary>
        /// Holt den Singelton und initialisiert die Variable von der Klasse <see cref="GmailService"/>
        /// </summary>
        public GoogleAPI()
        {
            config = GoogleAPIConfig.CreateInstance();
            SetUpGmailService();
        }

        /// <summary>
        /// Initialisiert die Variable von der Klasse <see cref="GmailService"/>
        /// </summary>
        private void SetUpGmailService()
        {
            UserCredential credential;
            string credPath = "token.json";

            ClientSecrets secrets = new ClientSecrets   // erstellt ein Objekt mit den "Logindaten"
            {
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret
            };

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                Scopes,
                config.Email,
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;  //Autorisiert den Account

            Console.WriteLine("Credential Datei gespeichert an dem Ort: " + credPath);

            service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Test",
            });
        }


        /// <summary>
        /// Geht durch alle Mails von dem User
        /// Die Mails werden zuerst gefilter und dann so lange auseinandergenommen bis man die Datei hat
        /// </summary>
        /// <returns>gibt eine Rückmeldung zur Methode zurück also ob es Fehler gab oder nicht</returns>
        public string GetAttachments()
        {
            try
            {
                UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List(config.Email);

                request.Q = config.Query; // Such die Messages/Mails nach den festgelegten Parameter durch

                ListMessagesResponse messagesResponse = request.Execute();

                List<Message> messages = messagesResponse.Messages.ToList();

                foreach (string messageId in messages.Select(m => m.Id))
                {
                    Message message = service.Users.Messages.Get(config.Email, messageId).Execute();

                    SafeFiles(GetAllFilesFromMessage(message));
                }

                return "Die Datei wurden erfolgreich in dem Pfad " + config.NewFilesPath + " abgespeichert.";
            }
            catch (Exception e)
            {
                return "Es trat folgende Fehlermeldung auf: " + e.Message;
            }
        }


        /// <summary>
        /// Holte alle Datei aus der "Message" raus
        /// </summary>
        /// <param name="message">Das Mail</param>
        /// <returns>List von allen Dateien in der Message</returns>
        private List<MailFile> GetAllFilesFromMessage(Message message)
        {
            List<MailFile> files = new List<MailFile>();

            foreach (MessagePart part in message.Payload.Parts)
            {
                if (!string.IsNullOrEmpty(part.Filename))
                {
                    string attId = part.Body.AttachmentId;
                    MessagePartBody attachPart = service.Users.Messages.Attachments.Get(config.Email, message.Id, attId).Execute();

                    string attachData = attachPart.Data.Replace('-', '+');  // wandelt die Datei von string zu einem Byte Array um
                    attachData = attachData.Replace('_', '/');

                    byte[] data = Convert.FromBase64String(attachData);
                    files.Add(new MailFile { Filename = part.Filename, Data = data});
                }
            }

            return files;
        }

        /// <summary>
        /// Speicher die Datein in dem gewünschten Pfad
        /// </summary>
        /// <param name="files">Alle Datein as der Mail</param>
        private void SafeFiles(List<MailFile> files)
        {
            foreach (MailFile file in files)
            {
                File.WriteAllBytes(Path.Combine(config.NewFilesPath, file.Filename.Replace("/", "")), file.Data);
            }
        }
    }
}
