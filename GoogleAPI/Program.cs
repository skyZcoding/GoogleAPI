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
using static Google.Apis.Gmail.v1.UsersResource.SettingsResource.DelegatesResource;

namespace GoogleAPI
{
    class Program
    {
        private static GmailService service;
        private static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static void Main(string[] args)
        {
            SetUpGmailService();
            GetAttachments(service, "jan.walker.jw1@roche.com");
        }

        private static void SetUpGmailService()
        {
            UserCredential credential;

            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            string credPath = "token.json";
            ClientSecrets clientSecrets = new ClientSecrets();
            clientSecrets.ClientId = "656237379861-flv7j4fj0keqeduhtr1foa984ntscnsu.apps.googleusercontent.com";
            clientSecrets.ClientSecret = "6o3Gn7a4zsNXXeP6djhIX6Re";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                Scopes,
                "jan.walker.jw1@roche.com",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
            Console.WriteLine("Credential file saved to: " + credPath);

            // Create Gmail API service.
            service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Test",
            });
        }

        /// <summary>
        /// Get and store attachment from Message with given ID.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="messageId">ID of Message containing attachment.</param>
        /// <param name="outputDir">Directory used to store attachments.</param>
        public static void GetAttachments(GmailService service, string userId)
        {
            try
            {
                UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List(userId);

                request.Q = "has:attachment subject:Mit einem Xerox-Multifunktionsgerät gescannt"; // Such die Messages nach den festgelegten Parameter durch

                ListMessagesResponse messagesResponse = request.Execute();

                List<Message> messages = new List<Message>();
                messages.AddRange(messagesResponse.Messages);

                foreach (string messageId in messages.Select(m => m.Id))
                {
                    Message message = service.Users.Messages.Get(userId, messageId).Execute();
                    IList<MessagePart> parts = message.Payload.Parts;

                    foreach (MessagePart part in parts)
                    {
                        if (!string.IsNullOrEmpty(part.Filename))
                        {
                            string attId = part.Body.AttachmentId;
                            MessagePartBody attachPart = service.Users.Messages.Attachments.Get(userId, message.Id, attId).Execute();

                            // Converting from RFC 4648 base64 to base64url encoding
                            // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                            string attachData = attachPart.Data.Replace('-', '+');
                            attachData = attachData.Replace('_', '/');

                            byte[] data = Convert.FromBase64String(attachData);
                            File.WriteAllBytes(Path.Combine("C:\\Users\\walkej34\\Desktop\\Test", part.Filename), data);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }
    }
}
