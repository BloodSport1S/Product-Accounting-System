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
    public partial class Add_form : Form
    {
        Database database = new Database();
        private readonly CheckUser user;
        public Add_form(CheckUser user)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.user = user;
        }
        public Add_form()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            
        }

        private void Add_form_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            database.OpenConnection();
            var surname = textBox1_SurName.Text;
            var nameTsex = textBox3_Tsex.Text;
            var nameCategory = textBox2_Category.Text;
            var nameProduct = textBox4_Product.Text;
            int CountProduct;
            if (int.TryParse(textBox5_Count.Text, out CountProduct))
            {
                var addQuery = $"Insert into products_zv (surname_of,nameTSEX_of,namecategory_of,nameproduct_of,count_of) values ('{surname}','{nameTsex}','{nameCategory}','{nameProduct}','{CountProduct}')";
                var command = new SqlCommand(addQuery, database.GetConnection());
                command.ExecuteNonQuery();
                MessageBox.Show("Запись успешно создана!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Данные введены неверно!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            database.CloseConnection();
            textBox1_SurName.Text = string.Empty;
            textBox2_Category.Text = string.Empty;  
            textBox4_Product.Text = string.Empty;
            textBox5_Count.Text = string.Empty;
            textBox3_Tsex.Text = string.Empty;  

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
