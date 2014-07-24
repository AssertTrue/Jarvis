using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Jarvis.alias;
using Jarvis.command;
using WpfAutomation;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Jarvis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HwndSource hWndSource;
        private short atom;
        private NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            this.viewModel = new ViewModel();
            this.DataContext = this.viewModel;

            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Icon = Properties.Resources.Jarvis;
            this.notifyIcon.Visible = true;

            MenuItem exitMenuItem = new MenuItem("Exit");
            exitMenuItem.Index = 0;
            exitMenuItem.Click += (s, a) => this.shutdown();
            this.notifyIcon.ContextMenu = new ContextMenu(new[]{exitMenuItem});
            
            this.viewModel.CommandAlias.add("quit", (aArguments) => { this.shutdown(); return true; });
        }

        private void shutdown()
        {
            this.Close();
        }

        private void onWindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wih = new WindowInteropHelper(this);
            hWndSource = HwndSource.FromHwnd(wih.Handle);
            hWndSource.AddHook(MainWindowProc);

            // create an atom for registering the hotkey
            atom = Win32.GlobalAddAtom("WpfAutomation");

            if (atom == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            uint modWin = Win32.MOD_NONE;
            #if (DEBUG)
            modWin = Win32.MOD_WIN;
            #endif
            // register the Win+C hotkey
            if (!Win32.RegisterHotKey(wih.Handle, atom, modWin, Win32.HOTKEY_HEX))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private void onWindowClosed(object sender, EventArgs e)
        {
            if (atom != 0)
            {
                // unregister the hotkey
                Win32.UnregisterHotKey(hWndSource.Handle, atom);
            }
        }

        private IntPtr MainWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_HOTKEY:
                    this.commandBox.Focus();
                    this.Show();
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }

        private void onCommandKeyUp(object sender, KeyEventArgs e)
        {
            this.viewModel.onCommandKeyUp(e);
        }

        private ViewModel viewModel;

        private void onShown(object aSender, EventArgs aE)
        {
            this.Hide();
        }

        private void onClosing(object sender, CancelEventArgs e)
        {
            this.notifyIcon.Visible = false;
        }
    }
}
