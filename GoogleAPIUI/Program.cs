using GoogleAPILibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPIUI
{
    class Program
    {
        private static GoogleAPI googleAPI;
        static void Main(string[] args)
        {
            PrintIntroduction();
            googleAPI = new GoogleAPI();
            PrintGetAttachementsResults();
        }

        private static void PrintIntroduction()
        {
            Console.WriteLine("Das ist ein Projekt um zu veranschaulichen, wie die Google Gmail API funktioniert.");
            Console.WriteLine();
            Console.WriteLine("Bitte legen Sie ihre Konfiguration zuerst in der config.json Datei fest.");
            Console.WriteLine("Falls Sie das gemacht haben, können Sie nun eine beliebige Taste drücken.");
            Console.ReadKey();
        }

        private static void PrintGetAttachementsResults()
        {
            Console.WriteLine(googleAPI.GetAttachments());
            Console.ReadKey();
        }
    }
}
