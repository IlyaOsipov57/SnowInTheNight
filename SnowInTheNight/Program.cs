using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowInTheNight
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GameForm());
            }
            catch (Exception e)
            {
                ErrorLog(e);
            }
        }
        public static void ErrorLog (Exception e)
        {
            using (var sw = File.AppendText("errorLog"))
            {
                sw.WriteLine(e.GetType().ToString());
                sw.WriteLine(e.Message);
                sw.WriteLine(e.StackTrace);
                sw.WriteLine("");
            }
        }
    }
}
