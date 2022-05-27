using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yazLab
{
    public partial class Form1 : Form
    {
        DataTable tablo = new DataTable();
        int count = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tablo.Columns.Add("Sıra");
            tablo.Columns.Add("Etiket");
            tablo.Columns.Add("Enlem");
            tablo.Columns.Add("Boylam");
            dataGridView1.DataSource = tablo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty || textBox2.Text == string.Empty || textBox3.Text == string.Empty)
            {
                MessageBox.Show("Enlem - Boylam ve Etiket değerlerini girin.", "Hata !", MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            try
            {
                string lat = string.Empty;
                string lon = string.Empty;
                count++;

                StringBuilder queryAddress = new StringBuilder();
                queryAddress.Append("http://maps.google.com/maps?q=");

                if (textBox1.Text != string.Empty)
                {
                    lat = textBox1.Text;
                    queryAddress.Append(lat + "%2C");
                }

                if (textBox2.Text != string.Empty)
                {
                    lon = textBox2.Text;
                    queryAddress.Append(lon);
                }
                tablo.Rows.Add(count, textBox3.Text, textBox1.Text, textBox2.Text);
                dataGridView1.DataSource = tablo;
                //listBox1.Items.Add(txtLat.Text);
                webBrowser1.Navigate(queryAddress.ToString());

                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Hata !");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
            else
            {
                MessageBox.Show("Lüffen silinecek satırı seçin.","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }
    }
}
