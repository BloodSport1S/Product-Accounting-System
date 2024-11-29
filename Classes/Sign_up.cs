using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
namespace CategoryProducts
{
    public partial class Sign_up : Form
    {
        Database database = new Database();
        public Sign_up()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        private void Sign_up_Load(object sender, EventArgs e) {}
        public void btnCreate_Click(object sender, EventArgs e)
        {
            var loginUser = textBox_reg.Text;
            var passwordUser = textBox_passcreate.Text;
            string insertQuery = $"INSERT INTO register (login_user, password_user, is_admin, is_manager) VALUES ('{loginUser}', '{passwordUser}', 0, 0)";

            SqlCommand command = new SqlCommand(insertQuery, database.GetConnection());

            try
            {
                database.OpenConnection();
                command.ExecuteNonQuery();
                MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Thread thread = new Thread(() =>
                  Application.Run(new Log_in_form())
                );
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                this.Close();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    MessageBox.Show("Ошибка: Логин уже существует.", "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при регистрации: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                database.CloseConnection();
                command.Connection.Dispose(); 
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
                  Application.Run(new Log_in_form())
                );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            this.Close();
        }

     
    }
}
