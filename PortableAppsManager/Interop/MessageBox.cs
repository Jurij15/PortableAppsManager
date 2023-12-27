using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PortableAppsManager.Interop
{
    public class MessageBox
    {
        public static void Show(string Content, string Title = null)
        {
            [DllImport("user32.dll")]
            static extern int MessageBox(IntPtr hWind, String text, String caption, int options);
            MessageBox(IntPtr.Zero, Content, Title, 0);
        }
    }
}
