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
using System.Runtime.InteropServices;
using System.Diagnostics.Eventing.Reader;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace CategoryProducts
{
    enum CurrentState
    {
        Existed,
        New,
        Changed,
        ChangedAgain,
        Deleted
    }
    public partial class BackendProgram : Form
    {
        private readonly CheckUser user;
        Database database = new Database();
        int selectedrow;
        public BackendProgram(CheckUser user) 
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.user = user;
        }
        public BackendProgram()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;  
        }

        private void BackendProgram_Load(object sender, EventArgs e)
        {
            textboxCheckPrivacy.Text = $"{user.login} Права доступа: {user.GetStatuseMethod()}";
            isAdmin();
            isManager();
            CreateColumns();
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9);
            RefreshDataGridView(dataGridView1);
        }
        private void isAdmin()
        {
            buttonModeration.Enabled = user.IsAdmin;
            buttonSalaryStatement.Enabled = user.IsAdmin;
        }
        private void isManager()
        { 
            buttonRemoveLessThanY.Enabled = user.IsManager;
            button2.Enabled = user.IsManager;
            buttonDevisionXPercent.Enabled = user.IsManager;
            
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("surname_of", "Фамилия сборщика");
            dataGridView1.Columns.Add("nameTSEX_of", "Название цеха");
            dataGridView1.Columns.Add("namecategory_of", "Категория");
            dataGridView1.Columns.Add("nameproduct_of", "Изделие");
            dataGridView1.Columns.Add("count_of", "Количество");
            dataGridView1.Columns.Add("IsNew", "Состояние записи");
            dataGridView1.Columns["surname_of"].Width = 125;
            dataGridView1.Columns["nameTSEX_of"].Width = 120;
            dataGridView1.Columns["namecategory_of"].Width = 200;
            dataGridView1.Columns["nameproduct_of"].Width = 140;

        }
        private void ClearAll()
        {
            textBox1_ID.Text = "";
            textBox2_Surname.Text = "";
            textBox3_Tsex.Text = "";
            textBox4_product.Text = "";
            textBox5_Category.Text = "";
            textBox6_count.Text = "";

        }
        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4), record.GetInt32(5), CurrentState.ChangedAgain);
        }
        private void RefreshDataGridView(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from products_zv";
            SqlCommand command = new SqlCommand(queryString, database.GetConnection());
            database.OpenConnection();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedrow = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedrow];
                textBox1_ID.Text = row.Cells[0].Value.ToString();
                textBox2_Surname.Text = row.Cells[1].Value.ToString();
                textBox3_Tsex.Text = row.Cells[2].Value.ToString();
                textBox4_product.Text = row.Cells[4].Value.ToString();
                textBox5_Category.Text = row.Cells[3].Value.ToString();
                textBox6_count.Text = row.Cells[5].Value.ToString();

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            RefreshDataGridView(dataGridView1);
            ClearAll();
        }

        private void Addbutton_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
             Application.Run(new Add_form(user))
            );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            this.Close();
        }

        private void Removebutton_Click(object sender, EventArgs e)
        {
            deleteRow();
            UpdateChanges();
            RefreshDataGridView(dataGridView1);
            ClearAll();
        }
        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[index].Visible = false;
            if (dataGridView1.Rows[index].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[index].Cells[6].Value = CurrentState.Deleted;
            }
        }
        private void UpdateChanges()
        {
            database.OpenConnection();
            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowstate = (CurrentState)dataGridView1.Rows[index].Cells[6].Value;
                if (rowstate == CurrentState.Existed)
                {
                    continue;
                }
                if (rowstate == CurrentState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from products_zv where id = {id}";
                    var command = new SqlCommand(deleteQuery, database.GetConnection());
                    command.ExecuteNonQuery();
                }
                if (rowstate == CurrentState.Changed)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var surname = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var nametsex = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var namecategory = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var nameproduct = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var count = dataGridView1.Rows[index].Cells[5].Value.ToString();

                    var changeQuery = $"update products_zv set surname_of = '{surname}', nameTSEX_of = '{nametsex}', namecategory_of = '{namecategory}', nameproduct_of = '{nameproduct}', count_of = '{count}' where id = '{id}'";

                    var command = new SqlCommand(changeQuery, database.GetConnection());
                    command.ExecuteNonQuery();
                }

            }
            database.CloseConnection();
        }
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string searchstring = $"select * from products_zv where concat (surname_of,nameTSEX_of,namecategory_of,nameproduct_of,count_of) like '%" + textBox_search.Text + "%'";
            SqlCommand cmd = new SqlCommand(searchstring, database.GetConnection());
            database.OpenConnection();
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            while (sqlDataReader.Read())
            {
                ReadSingleRow(dgw, sqlDataReader);
            }
            sqlDataReader.Close();

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void Changebutton_Click(object sender, EventArgs e)
        {
            Change();
            UpdateChanges();
            ClearAll();
        }
        private void Change()
        {
            var selecteRowIndex = dataGridView1.CurrentCell.RowIndex;
            var id = textBox1_ID.Text;
            var surname = textBox2_Surname.Text;
            var nameTsex = textBox3_Tsex.Text;
            var nameCategory = textBox5_Category.Text;
            var nameProduct = textBox4_product.Text;
            int CountProduct;
            if (dataGridView1.Rows[selecteRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBox6_count.Text, out CountProduct))
                {
                    dataGridView1.Rows[selecteRowIndex].SetValues(id, surname, nameTsex, nameCategory, nameProduct, CountProduct);
                    dataGridView1.Rows[selecteRowIndex].Cells[6].Value = CurrentState.Changed;
                }
                else
                {
                    MessageBox.Show("Данные введены неверно!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void pictureBox4_Click(object sender, EventArgs e)
        {
            ClearAll();
        }
        private long CalculationAllProducts()
        {
            long totalCount = 0;
           
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    var value = row.Cells["count_of"].Value;

                    if (value != null)
                    {

                        totalCount += Convert.ToInt64(value);
                    }
                }
            }
           
            return totalCount;
        }
        private long CalculationSelectedTsexProducts()
        {
            long totalCount = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string nametsexof = (string)row.Cells["nameTSEX_of"].Value;
                    if (nametsexof.Equals(textBox_selectedTSEX?.Text))
                    {
                        var value = row.Cells["count_of"].Value;
                        if (value != null)
                        {
                            totalCount += Convert.ToInt64(value);
                        }
                    }
                }
            }
            return totalCount;
        }
        private void CalculateDevision()
        {
            double temp = 0;
            int percentage;

           
            if (int.TryParse(textBox_Percent.Text, out percentage))
            {
                MessageBox.Show($"Процент успешно установлен: {percentage}%");
            }
            else
            {
                MessageBox.Show("Ошибка: Пожалуйста, введите корректное целое число.");
            }
            database.OpenConnection();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string namecategory = (string)row.Cells["namecategory_of"].Value;
                    if (namecategory.Equals(textBox_SelectedCategory.Text))
                    {
                        int id = (int)row.Cells["id"].Value;
                        int value = (int)row.Cells["count_of"].Value;
                        if (value != 0)
                            temp = Math.Round(value * (1 - ((double)percentage / 100)), 0);
                        string newquery = $"UPDATE products_zv SET count_of = {temp} WHERE id = {id}";
                        SqlCommand command = new SqlCommand(newquery, database.GetConnection());
                        command.ExecuteNonQuery();
                     
                    }
                }

            }
            database.CloseConnection();

        }
        private void RemoveObjectLessThanY()
        {
            int number;

            
            if (int.TryParse(textBox_SelectedCount.Text, out number))
            {
                MessageBox.Show($"Изделия менее: {number} будут удалены");
            }
            else
            {
                MessageBox.Show("Ошибка: Пожалуйста, введите корректное целое число.");
            }
            database.OpenConnection();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string namecategory = (string)row.Cells["namecategory_of"].Value;
                    if (namecategory.Equals(textBox_SelectedCategory.Text))
                    {                      
                        int value = (int)row.Cells["count_of"].Value;
                        if (value < number) {
                            string deleteQuery = $"DELETE FROM products_zv WHERE count_of = '{value}'";
                            SqlCommand command = new SqlCommand(deleteQuery, database.GetConnection());
                            command.ExecuteNonQuery();
                        }
                    }
                }

            }
            database.CloseConnection();
        }

        private void CalculateAllbutton_Click(object sender, EventArgs e)
        {
            textBox_CountProducts.Text = "";
            textBox_CountProducts.Text = Convert.ToString(CalculationAllProducts());
        }

        private void button_SelectedTSEX_Click(object sender, EventArgs e)
        {
            if (textBox_selectedTSEX.Text == string.Empty)
            {
                MessageBox.Show("Вы не задали цех для расчёта изделий!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                textBox_CountProducts.Text = "";
                textBox_CountProducts.Text = Convert.ToString(CalculationSelectedTsexProducts());
            }
        }

        private void buttonDevisionXPercent_Click(object sender, EventArgs e)
        {
            if (textBox_Percent.Text == string.Empty || textBox_SelectedCategory.Text == string.Empty)
            {
                MessageBox.Show("Вы не задали категорию или процент для расчёта изделий!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CalculateDevision();
                RefreshDataGridView(dataGridView1);
                MessageBox.Show("Вы уменьшили количество изделий заданной категории на ваш процент!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void buttonRemoveLessThanY_Click(object sender, EventArgs e)
        {
            if (textBox_SelectedCount.Text == string.Empty || textBox_SelectedCategory.Text == string.Empty)
            {
                MessageBox.Show("Вы не задали категорию или число для удаления изделий!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                RemoveObjectLessThanY();
                RefreshDataGridView(dataGridView1);
                MessageBox.Show("Вы удалили изделия заданной категории, которые меньше заданного вами числа!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonSalaryStatement_Click(object sender, EventArgs e)
        {
            
            Thread thread = new Thread(() =>
            Application.Run(new Salary_Statement(user))
            );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            this.Close();
        }

        private void toolStripButtonGoBack_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            Application.Run(new Log_in_form())
            );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            this.Close();
        }

        private void toolStripButtonInfoAbout_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(()=>
             Application.Run(new ProgramInfo())
            );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void buttonModeration_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
             Application.Run(new ControlStatus(user))
            );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            this.Close();
        }

       
    }
}
