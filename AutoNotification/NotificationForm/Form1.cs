using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoNotification;
using System.Data.SqlClient;


namespace NotificationForm
{
    public partial class Form1 : Form
    {
        Notification notification = null;

        public Form1()
        {
            InitializeComponent();
            notification = new Notification();
            bt_prisijungti.Enabled = notification.CanRequestNotifications();
        }

        void NotificationHandler(object sender, SqlNotificationEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new OnChangeEventHandler(NotificationHandler), new object[]{sender, e});
                return;
            }
            
            var sqlDep = sender as SqlDependency;
            sqlDep.OnChange -= new OnChangeEventHandler(NotificationHandler);

            dataGridView.DataSource = notification.SomeMethod(NotificationHandler);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (notification == null)
            {
                notification = new Notification();
            }

            notification.Initialization();

            try
            {
                dataGridView.DataSource = notification.SomeMethod(NotificationHandler);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (notification != null)
            {
                notification.Termination();    
            }
        }
    }
}