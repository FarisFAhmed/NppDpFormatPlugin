using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppFormatPlugin.DpTools;

namespace Kbg.NppPluginNET
{
    class Main
    {
        #region Members

        internal const string PluginName = "DP Formatter";
        static string iniFilePath = null;
        static bool someSetting = false;

        // Id of the format menu item
        protected static int menuId_Format = 0;

        // Id of the seperator menu item
        protected static int menuId_MenuSeparator = 1;

        // Id of the About menu item
        protected static int menuId_ShowAboutWindow = 2;

        // Holds the reference to the current Scintilla
        protected static ScintillaGateway scintillaGateway = new ScintillaGateway(PluginBase.GetCurrentScintilla());

        // Holds the reference of the DpTools class that hat the formatting functionality
        protected static DpTools m_DpFormatter = null;

        #endregion
        
        public static void OnNotification(ScNotification notification)
        {
            // This method is invoked whenever something is happening in notepad++
            // use eg. as
            // if (notification.Header.Code == (uint)NppMsg.NPPN_xxx)
            // { ... }
            // or
            //
            // if (notification.Header.Code == (uint)SciMsg.SCNxxx)
            // { ... }
        }

        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            // Here we build the menu of our plugin
            PluginBase.SetCommand(menuId_Format, "Format", FormatMessages, new ShortcutKey(true, false, false, Keys.F12));
            PluginBase.SetCommand(menuId_MenuSeparator, "-", null);
            PluginBase.SetCommand(menuId_ShowAboutWindow, "About", ShowAboutWindow);
        }

        /// <summary>
        /// Provides the icons in the Notepad++ toolbar
        /// </summary>
        internal static void SetToolBarIcon()
        {
            // Icon for the print button
            toolbarIcons tbIcons = new toolbarIcons()
            {
                // Get the handle from the Format resource object
                hToolbarBmp = NppFormatPlugin.Properties.Resources.Format.GetHbitmap()
            };

            // Allocate the resources in the none .NET world of Notepad++
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);

            // Send a system message to Npp to set add a toolbar icon.
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[menuId_Format]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }

        /// <summary>
        /// Returns the text content of the selected tab
        /// </summary>
        /// <returns>Content of the selected tab</returns>
        static string GetCurrentTabText()
        {
            // Handle zum Scintilla was der word editor control in Notepad++ ist
            IntPtr hCurrentEditView = PluginBase.GetCurrentScintilla();

            // Laenge des Texts in den current edit view lesen
            int currentTabTextLength = (int)Win32.SendMessage(hCurrentEditView, SciMsg.SCI_GETLENGTH, 0, 0) + 1;

            Debug.WriteLine("CurrentEditView groesse = " + currentTabTextLength.ToString());

            IntPtr currentTextPtr = IntPtr.Zero;
            string contentString;

            // speicher reservieren fuer den Text
            try
            {
                currentTextPtr = Marshal.AllocHGlobal(currentTabTextLength);

                // Text aus den current Tab lesen
                Win32.SendMessage(hCurrentEditView, SciMsg.SCI_GETTEXT, currentTabTextLength, currentTextPtr);

                // von Umanaged zu managed casten
                contentString = Marshal.PtrToStringAnsi(currentTextPtr);
            }
            finally
            {
                // jetzt kann der unmanged text freigegeben werden
                Marshal.FreeHGlobal(currentTextPtr);
            }

            return contentString;
        }

        internal static void FormatMessages()
        {
            try
            {
                string currentTabContent = GetCurrentTabText();

                if (currentTabContent.Length < 4)
                {
                    // The shortest DP message is <a:> four characters long
                    return;
                }

                if (m_DpFormatter == null)
                {
                    m_DpFormatter = new DpTools(currentTabContent);
                }
                else
                {
                    m_DpFormatter.Load(currentTabContent);
                }

                string formattedString = m_DpFormatter.Format();

                // Text im current Tab setzen
                scintillaGateway.SetText(formattedString);
            }
            catch (Exception ex)
            {
                // Make sure that Notepad++ does not go down in case of exceptions.
            }
        }

        internal static void ShowAboutWindow()
        {
            try
            {
                NppFormatPlugin.Forms.frmAboutDPDHL aboutForm = new NppFormatPlugin.Forms.frmAboutDPDHL();
                aboutForm.ShowDialog();
            }
            catch (Exception ex)
            {
                // Make sure that Notepad++ does not go down in case of exceptions.
            }
        }
    }
}
