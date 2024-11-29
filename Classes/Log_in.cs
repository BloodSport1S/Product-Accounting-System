using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CategoryProducts
{
    public partial class Log_in_form : Form
    {
        Database database = new Database();
       
        public Log_in_form()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void log_in_form(object sender, EventArgs e)
        {
            textBox_pass.PasswordChar = '*';
            pictureBox1.Visible = false;    
            textBox_log.MaxLength = 30;
            textBox_pass.MaxLength = 30;
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            var loginUser = textBox_log.Text;
            var passwordUser = textBox_pass.Text;
            SqlDataAdapter adapter = new SqlDataAdapter();  
            DataTable table = new DataTable();
            string querystring = $"select id_user, login_user, password_user, is_admin, is_manager from register where login_user = '{loginUser}' and password_user = '{passwordUser}'";
            SqlCommand command = new SqlCommand(querystring, database.GetConnection());
            adapter.SelectCommand = command;
            adapter.Fill(table);
            if (table.Rows.Count == 1)
            {
                var user = new CheckUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3].ToString()), Convert.ToBoolean(table.Rows[0].ItemArray[4].ToString()));
                MessageBox.Show("Вы успешо вошли!","Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Thread thread = new Thread(() =>
                {
                   
                    Application.Run(new BackendProgram(user));
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                this.Close();
            }
            else
            {
                MessageBox.Show("Такого аккаунт не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
            Thread thread = new Thread(() =>
            {
               
                Application.Run(new Sign_up());
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            this.Close();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox_pass.UseSystemPasswordChar = true;
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            textBox_pass.UseSystemPasswordChar = false;
            pictureBox1.Visible = false;
            pictureBox2.Visible = true;
        }

       
    }
}
