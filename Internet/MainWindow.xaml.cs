using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Internet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Internet_Carts internet_carts = new Internet_Carts();
        private string path7 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Password.mdb";
        private OleDbConnection con = new OleDbConnection();
        
        public MainWindow()
        {
            InitializeComponent();
            if (!Serial())
            {
                while (!Serial())
                {
                    InputDialog input_dialog = new InputDialog("Enter Serial Number ", "");
                    if (input_dialog.ShowDialog() == true)
                    {
                        if (insert_serial(input_dialog.Answer) == true)
                        {
                            MessageBox.Show("Success");
                            break;
                        }
                        else
                            MessageBox.Show("Wrong Serial Number");
                    }
                    else
                        Environment.Exit(0);
                }
            }
            string procName = Process.GetCurrentProcess().ProcessName;

            // get the list of all processes by the "procName"       
            Process[] processes = Process.GetProcessesByName(procName);
            if (processes.Length > 1)
            {
                MessageBox.Show("😡البرنامج مفتوح اعم بتفتحو تانى ليه");
                Environment.Exit(0);
            }
            Host.Text=Properties.Settings.Default.ip;
            User.Text=Properties.Settings.Default.admin;
            Pass.Password=Properties.Settings.Default.password;
            
        }
        public Boolean Serial()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path7 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select [Serial] from [Serial]";
            OleDbDataReader reader = c.ExecuteReader();
            String temp = "error";
            while (reader.Read())
            {
                temp = reader.GetValue(0).ToString();
                break;
            }
            con.Close();
            if (temp != "error")
            {
                if (temp == cpuInfo)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public Boolean insert_serial(String serial)
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }
            if (serial == cpuInfo)
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path7 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "delete from Serial";
                c.ExecuteNonQuery();
                c.CommandText = "INSERT INTO Serial ([Serial]) VALUES ('" + serial + "')";
                c.ExecuteNonQuery();
                con.Close();
                return true;
            }
            else
                return false;
        }

        

        private void Login_Microtik_Login(object sender, RoutedEventArgs e)
        {
            if (Host.Text != "" && User.Text != "")
            {
                bool tt = internet_carts.write_connection(Host.Text, User.Text, Pass.Password);

                if (tt)
                {
                    if (internet_carts.test_login())
                    {
                        Properties.Settings.Default.ip = Host.Text;
                        Properties.Settings.Default.admin = User.Text;
                        Properties.Settings.Default.password = Pass.Password;
                        Properties.Settings.Default.Save();                        
                        internet_cart_menu h = new internet_cart_menu();
                        h.ResizeMode = ResizeMode.NoResize;
                        h.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        this.Close();
                        h.ShowDialog();

                        /*this.Visibility = Visibility.Hidden;
                        if (h.ShowDialog() == false)
                            this.Visibility = Visibility.Visible;*/


                    }
                }                    
            }
            else
                MessageBox.Show("Fill all fields First");            
        }

        private void Save_Microtik_Login_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
