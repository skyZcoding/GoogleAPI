using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPILibrary
{
    public class GoogleAPIConfig
    {
        private static readonly string _configPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\config.json";
        private static GoogleAPIConfig instance;

        /// <summary>
        /// Dient als Benutzername um auf die API zuzugreifen
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        /// Dient als Kennwort um auf die API zuzugreifen
        /// </summary>
        public string ClientSecret { get; private set; }

        /// <summary>
        /// Such die Mails nach den festgelegten begriefen durch
        /// Sprich man muss zuerst das Stichwort angeben z.B. subject und dann ein : und dann der Suchbegriff
        /// Das ein Bespiel würde wie folgt aussehen subject: test
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// Der Pfad, wo die Dateien aus dem Email gespeichert werden
        /// </summary>
        public string NewFilesPath { get; private set; }

        /// <summary>
        /// Die Email von dem Account, wo die Mails gelesen werden
        /// </summary>
        public string Email { get; private set; }

        private GoogleAPIConfig()
        {
        }

        /// <summary>
        /// Gibt die Singelton Instanz zurück, falls die Instanz noch nicht instanziert wurde, wird dies auch noch gemacht
        /// </summary>
        /// <returns>die Singelton Instanz</returns>
        public static GoogleAPIConfig CreateInstance()
        {
            if (instance == null)
            {
                instance = new GoogleAPIConfig();
            }

            LoadConfig();

            return instance;
        }


        /// <summary>
        /// Deserialisiert die JSON Datei und instanziert damit die Singelton Instanz
        /// </summary>
        private static void LoadConfig()
        {
            try
            {
                string json = File.ReadAllText(_configPath);
                instance = JsonConvert.DeserializeObject<GoogleAPIConfig>(json);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
