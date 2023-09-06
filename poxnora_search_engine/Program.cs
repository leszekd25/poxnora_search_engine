using poxnora_search_engine.Pox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poxnora_search_engine
{
    static public class Program
    {
        static public Database database = new Database();
        static public ImageCache image_cache = new ImageCache();

        static public MainForm main_form;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.Info(Log.LogSource.Main, "Program.Main() started, application version: " + Utility.APP_VERSION);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //try
            //{
                main_form = new MainForm();
                Log.Info(Log.LogSource.Main, "Program.Main(): form created");
                Application.Run(main_form);

                main_form = null;
            /*}
            catch (Exception e)
            {
                Log.Error(Log.LogSource.Main, "Program.Main(): Uncaught exception! Exception info:\r\n" + e.Message);
            }
            finally
            {
                Log.Info(Log.LogSource.Main, "Program.Main(): Saving log");
                Log.SaveLog("UserLog.txt");
            }*/
        }
    }
}
