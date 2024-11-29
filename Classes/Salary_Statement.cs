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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Threading;
namespace CategoryProducts
{
    public partial class Salary_Statement : Form
    {
        Database database = new Database();
        private readonly CheckUser user;
        public Salary_Statement(CheckUser user)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.user = user;
        }

        private void Salary_Statement_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGridView(dataGridView2);
            InsertIdsIntoStatement();
            // Для проверки, что данные действительно загружены
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Данные не загружены.");
            }
        }
        private void CreateColumns()
        {
            dataGridView2.Columns.Add("id", "id");
            dataGridView2.Columns.Add("surname_of", "Фамилия сборщика");
            dataGridView2.Columns.Add("nameTSEX_of", "Наименования цеха");
            dataGridView2.Columns.Add("salary_of", "Зарплата");
            dataGridView2.Columns["surname_of"].Width = 243;
            dataGridView2.Columns["nameTSEX_of"].Width = 245;
            dataGridView2.Columns["id"].Width = 240;
            dataGridView2.Columns["salary_of"].Width = 245;
        }
        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            int id = record.GetInt32(0);
            string surname = record.GetString(1);
            string nameTSEX = record.GetString(2);
            int salary = record.IsDBNull(3) ? 0 : record.GetInt32(3); 

            dgw.Rows.Add(id, surname, nameTSEX, salary);
        }

        private void RefreshDataGridView(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"SELECT p.id, p.surname_of, p.nameTSEX_of, s.salary_of FROM products_zv AS p LEFT JOIN statement AS s ON p.id = s.id_Statement;";
            SqlCommand command = new SqlCommand(queryString, database.GetConnection());
            database.OpenConnection();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }
        private void InsertIdsIntoStatement()
        {
            database.OpenConnection();
            try
            {
                var insertQuery = "INSERT INTO statement (id_Statement, salary_of) SELECT id, 0 FROM products_zv WHERE NOT EXISTS (SELECT 1 FROM statement  WHERE statement.id_Statement = products_zv.id);";

                SqlCommand command = new SqlCommand(insertQuery, database.GetConnection());
                int rowsAffected = command.ExecuteNonQuery();   
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при вставке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                database.CloseConnection(); 
            }
        }

        private void buttonSelectSalary_Click(object sender, EventArgs e)
        {      
            database.OpenConnection();
            if (textBox_SelectSalary.Text != string.Empty && textBox_IdUser.Text != string.Empty)
            {
                try
                {
                    int id;
                    if (int.TryParse(textBox_IdUser.Text, out id))
                    {
                        int salary;

                        if (int.TryParse(textBox_SelectSalary.Text, out salary))
                        {
                            MessageBox.Show($"Зарплата успешно установлена: {salary}");

                            // Подготовка команды обновления
                            var changeQuery = $"UPDATE statement SET salary_of = '{salary}' WHERE id_Statement IN (SELECT p.id FROM products_zv AS p WHERE p.id = '{id}');";
                            var command = new SqlCommand(changeQuery, database.GetConnection());   
                            command.ExecuteNonQuery(); 
                            
                        }
                        else
                        {
                            MessageBox.Show("Ошибка: Пожалуйста, введите корректное целое число З/П.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: Пожалуйста, введите корректное ID.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Вы не ввели Ид или З/п для установки!","Ошибка!",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            database.CloseConnection();
            RefreshDataGridView(dataGridView2);
        }

        private void buttonAverageSalary_Click(object sender, EventArgs e)
        {
            double totalSalary = 0; 
            int count = 0; 

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!row.IsNewRow)
                {
                    string nametsexof = (string)row.Cells["nameTSEX_of"].Value;

                    if (nametsexof.Equals(textBox_AverageSalary?.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = row.Cells["salary_of"].Value;

                        
                        if (value != null && int.TryParse(value.ToString(), out int salary))
                        {
                            totalSalary += salary; 
                            count += 1; 
                        }
                    }
                }
            }

         
            if (count > 0)
            {
                double averageSalary = totalSalary / count; 
                textBox_SawSalary.Text = Math.Round(averageSalary, 0).ToString(); 
            }
            else
            {
                textBox_SawSalary.Text = "0"; 
                MessageBox.Show("Нет записей для расчета средней зарплаты.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void вернутьсяНазадToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            Thread thread = new Thread(() =>
            {
                Application.Run(new BackendProgram(user));
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            this.Close();
            
        }
      
      
       
    }
}
