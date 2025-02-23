using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriceChecker
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            bool isNewInstance;
            using (Mutex mutex = new Mutex(true, "PriceCheckerAppMutex", out isNewInstance))
            {
                if (!isNewInstance)
                {
                    MessageBox.Show("Приложение уже запущено.");
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
