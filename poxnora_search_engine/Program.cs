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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            main_form = new MainForm();
            Application.Run(main_form);

            main_form = null;
        }
    }
}
