using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPILibrary
{
    public class MailFile
    {
        /// <summary>
        /// Der Name von der Datei
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Die Datei in einem Byte Array
        /// </summary>
        public byte[] Data { get; set; }
    }
}
