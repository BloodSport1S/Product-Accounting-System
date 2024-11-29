using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CategoryProducts
{
    public partial class ControlStatus : Form
    {
        private readonly CheckUser user;
        Database database = new Database();
        public ControlStatus(CheckUser user)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.user = user;
        }

        private void ControlStatus_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGridView();
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_user", "ID");
            dataGridView1.Columns.Add("login_user", "Логин пользователя");
            dataGridView1.Columns.Add("password_user", "Пароль пользователя");
            dataGridView1.Columns["id_user"].Width = 80;
            dataGridView1.Columns["login_user"].Width = 150;
            dataGridView1.Columns["login_user"].Width = 150;
            var checkcolumns = new DataGridViewCheckBoxColumn();
            var checkcolumns2 = new DataGridViewCheckBoxColumn();
            checkcolumns.HeaderText = "Назначить Админом";
            checkcolumns2.HeaderText = "Назначить Менеджером";
            dataGridView1.Columns.Add(checkcolumns);
            dataGridView1.Columns.Add(checkcolumns2);
           
        }
        private void ReadSingleRow(IDataRecord record)
        {
            dataGridView1.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetBoolean(3), record.GetBoolean(4));
        }
        private void RefreshDataGridView()
        {
            dataGridView1.Rows.Clear();
            string queryString = $"SELECT * FROM register";
            
            SqlCommand cmd = new SqlCommand(queryString,database.GetConnection());
          
            database.OpenConnection();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ReadSingleRow(reader);
            }
            reader.Close();
            database.CloseConnection();

        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
             Application.Run(new BackendProgram(user))
            );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            this.Close();
        }

        private void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            database.OpenConnection();
            for (int i=0; i < dataGridView1.Rows.Count; i++)
            {
                var id = dataGridView1.Rows[i].Cells[0].Value.ToString();
                var admin = dataGridView1.Rows[i].Cells[3].Value.ToString();
                var manager = dataGridView1.Rows[i].Cells[4].Value.ToString();
                var changeQuery = $"UPDATE register SET is_admin = '{admin}' , is_manager ='{manager}' WHERE id_user = '{id}'";
                SqlCommand command = new SqlCommand(changeQuery,database.GetConnection());
                command.ExecuteNonQuery();
            }
            database.CloseConnection(); 
        }

        private void buttonDeleteUser_Click(object sender, EventArgs e)
        {
            var selectedRowindex = dataGridView1.CurrentCell.RowIndex;
            var id = Convert.ToInt32(dataGridView1.Rows[selectedRowindex].Cells[0].Value);
            database.OpenConnection();
            var deletequery = $"DELETE FROM register WHERE id_user = '{id}'";
            SqlCommand command = new SqlCommand(deletequery,database.GetConnection());
            command.ExecuteNonQuery();
            database.CloseConnection();
            RefreshDataGridView();  
        }
    }
}
