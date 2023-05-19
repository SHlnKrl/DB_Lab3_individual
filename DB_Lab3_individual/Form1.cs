using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DB_Lab3_individual
{
    public partial class Form1 : Form
    {
        //структура приймає загальну інформацію про замовлення
        public struct ordering
        {
            public string code; //код
            public string date; //дата оформлення
            public string term; //термін виконання
            public ordering(string code, string date, string term)
            {
                this.code = code;
                this.date = date;
                this.term = term;
            }
        }
        //список приймає загальну інформацію про всі замовлення 
        List<ordering> allorderings = new List<ordering>();

        //структура приймає детальну інформацію по конкретному замовленню
        public struct orderingdetails
        {
            public string code_or;  //код замовлення
            public string code_as;  //код вузла
            public string name_as;  //назва вузла
            public string price;    //ціна вузла
            public string amount;   //кількість вузлів
            public string sum;      //сумарна вартість
            public orderingdetails(string code_or, string code_as, string name_as, string price, string amount, string sum)
            {
                this.code_or = code_or;
                this.code_as = code_as;
                this.name_as = name_as;
                this.price = price;
                this.amount = amount;
                this.sum = sum;
            }
        }

        //список вміщуватиме інформацію про кожен вузол з конкретного замовлення
        List<orderingdetails> allorderingdetails = new List<orderingdetails>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                MessageBox.Show("Базу даних під'єднано");
                QE(conn);

                dgvAllOrders.ColumnCount = 3;
                dgvAllOrders.Columns[1].Width = 100;
                dgvAllOrders.Columns[2].Width = 100;

                dgvAllOrders.Columns[0].HeaderText = "Номер замовлення";
                dgvAllOrders.Columns[1].HeaderText = "Дата замовлення";
                dgvAllOrders.Columns[2].HeaderText = "Термін виконання";

                dgvAllOrders.RowCount = allorderings.Count();

                for (int i = 0; i < dgvAllOrders.RowCount; i++)
                {
                    dgvAllOrders.Rows[i].Cells[0].Value = allorderings[i].code.ToString();
                    dgvAllOrders.Rows[i].Cells[1].Value = allorderings[i].date.ToString().Substring(0, allorderings[i].date.Length - 8);
                    dgvAllOrders.Rows[i].Cells[2].Value = allorderings[i].term.ToString().Substring(0, allorderings[i].date.Length - 8);
                }
            }
            catch
            {
                MessageBox.Show("Базу даних не знайдено");
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        private void QE(MySqlConnection conn)
        {
            string code, date, term;
            string sql = "SELECT ordering_code, ordering_date, ordering_term from ordering";
            ordering ord;

            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql;

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        code = reader["ordering_code"].ToString();
                        date = reader["ordering_date"].ToString();
                        term = reader["ordering_term"].ToString();
                        ord = new ordering(code, date, term);
                        allorderings.Add(ord);
                    }
                }
            }
        }

        private void QE2(MySqlConnection conn, string str)
        {
            string code_or, code_as, name_as, price, amount, sum;
            string sql = "SELECT \n"
            + "\tordering_ordering_code, \n"
            + "\tassembly_assembly_code, \n"
            + "\tassembly_name, \n"
            + "\tassembly_price, \n"
            + "\torder_amount, \n"
            + "\t(order_amount * assembly_price) AS sum \n"
            + "FROM orders, assembly, assembly_names \n"
            + "WHERE \n"
            + "\tassembly_assembly_code = assembly_code AND \n"
            + "\tassembly_assembly_name_assembly_name_id = assembly_name_id AND \n"
            + "\tordering_ordering_code = " + str + "\n"
            + "UNION \n"
            + "SELECT \'-\',\'-\',\'-\',\'-\',\'-\',\'-\' \n"
            + "UNION \n"
            + "SELECT \'РОЗРАХУНОК\',\'Ціна\',\'ПДВ\',\'ДО СПЛАТИ\', \'-\', \'-\' \n"
            + "UNION \n"
            + "SELECT \'-\', \n"
            + "\tSUM(order_amount * assembly_price), \n"
            + "\tSUM(order_amount * assembly_price) * 0.2, \n"
            + "\tSUM(order_amount * assembly_price) * 1.2, '-','-' \n"
            + "FROM orders, assembly, assembly_names \n"
            + "WHERE \n"
            + "\tassembly_assembly_code = assembly_code AND \n"
            + "\tassembly_assembly_name_assembly_name_id = assembly_name_id AND \n"
            + "\tordering_ordering_code = " + str;

            orderingdetails ordd;
            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql;

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        code_or = reader["ordering_ordering_code"].ToString();
                        code_as = reader["assembly_assembly_code"].ToString();
                        name_as = reader["assembly_name"].ToString();
                        price = reader["assembly_price"].ToString();
                        amount = reader["order_amount"].ToString();
                        sum = reader["sum"].ToString();
                        ordd = new orderingdetails(code_or, code_as, name_as, price, amount, sum);
                        allorderingdetails.Add(ordd);
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            allorderingdetails.Clear();
            try
            {
                conn.Open();
                QE2(conn, textBox1.Text);

                dgvOrder.Columns.Clear();

                dgvOrder.ColumnCount = 6;
                dgvOrder.Columns[0].Width = 110;
                dgvOrder.Columns[2].Width = 190;

                dgvOrder.Columns[0].HeaderText = "Номер замовлення";
                dgvOrder.Columns[1].HeaderText = "Код вузла";
                dgvOrder.Columns[2].HeaderText = "Назва вузла";
                dgvOrder.Columns[3].HeaderText = "Ціна вузла";
                dgvOrder.Columns[4].HeaderText = "Кількість";
                dgvOrder.Columns[5].HeaderText = "Сумарна вартість";

                dgvOrder.RowCount = allorderingdetails.Count();

                for (int i = 0; i < dgvOrder.RowCount; i++)
                {
                    dgvOrder.Rows[i].Cells[0].Value = allorderingdetails[i].code_or.ToString();
                    dgvOrder.Rows[i].Cells[1].Value = allorderingdetails[i].code_as.ToString();
                    dgvOrder.Rows[i].Cells[2].Value = allorderingdetails[i].name_as.ToString();
                    dgvOrder.Rows[i].Cells[3].Value = allorderingdetails[i].price.ToString();
                    dgvOrder.Rows[i].Cells[4].Value = allorderingdetails[i].amount.ToString();
                    dgvOrder.Rows[i].Cells[5].Value = allorderingdetails[i].sum.ToString();
                }
            }
            catch
            {
                dgvOrder.Columns.Clear();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
