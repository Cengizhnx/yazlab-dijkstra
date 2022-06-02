using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yazLab
{
    public partial class Form1 : Form
    {
        DataTable tablo = new DataTable();
        DataTable tablo2 = new DataTable();
        int count = 0;
        const double PI = Math.PI;

        Dictionary<string, string> points = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
        }

        private double rad2Deg(double radians)
        {
            return radians * 180 / PI; 
        }

        private double deg2Rad(double degress)
        {
            return degress * PI / 180;
        }
        private double GetDistanceBetweenTwoPoint(double latitude1, double longitude1, double latitude2, double longitude2, string unit = "kilometers")
        {
            double theta = longitude1 - longitude2;
            double distance = 60 * 1.1515 * rad2Deg(Math.Acos(((Math.Sin(deg2Rad(latitude1))) * Math.Sin(deg2Rad(latitude2))) + (Math.Cos(deg2Rad(latitude1)) * (Math.Cos(deg2Rad(latitude2)) * (Math.Cos(deg2Rad(theta)))))));

            return Math.Round(distance * 1.609344, 2);
        }

        private void DrawGridViewHead(DataTable table)
        {
            table.Columns.Add("-");
            foreach (var point in points)
            {
                table.Columns.Add(point.Key);
            }
        }

        private void DrawDistancesGridview()
        {

            tablo2 = new DataTable();
            DrawGridViewHead(tablo2);
            
            dataGridView2.DataSource = tablo2;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            CalcMeasurePoints();
        }

        private void CalcMeasurePoints()
        {
            double[,] distances = new double[points.Count(), points.Count()];
            int row = 0, col = 0;

            foreach (var firstPoint in points)
            {
                col = 0;

                double firstPointLongLeft = Double.Parse(firstPoint.Value.Split('-')[0].Split('.')[0]);
                double firstPointLongRight = Double.Parse(firstPoint.Value.Split('-')[0].Split('.')[1]) / Math.Pow(10, firstPoint.Value.Split('-')[0].Split('.')[1].Length);
                double firstPointLong = firstPointLongLeft + firstPointLongRight;

                double firstPointLatLeft = Double.Parse(firstPoint.Value.Split('-')[1].Split('.')[0]);
                double firstPointLatRight = Double.Parse(firstPoint.Value.Split('-')[1].Split('.')[1]) / Math.Pow(10, firstPoint.Value.Split('-')[1].Split('.')[1].Length);
                double firstPointLat = firstPointLatLeft + firstPointLatRight;

                Object[] dgwRow = new object[points.Count() + 1];

                dgwRow[0] = firstPoint.Key;


                foreach (var secondPoint in points)
                {
                    

                    if (firstPoint.Key != secondPoint.Key)
                    {
                        double secondPointLongLeft = Double.Parse(secondPoint.Value.Split('-')[0].Split('.')[0]);
                        double secondPointLongRight = Double.Parse(secondPoint.Value.Split('-')[0].Split('.')[1]) / Math.Pow(10, secondPoint.Value.Split('-')[0].Split('.')[1].Length);
                        double secondPointLong = secondPointLongLeft + secondPointLongRight;

                        double secondPointLatLeft = Double.Parse(secondPoint.Value.Split('-')[1].Split('.')[0]);
                        double secondPointLatRight = Double.Parse(secondPoint.Value.Split('-')[1].Split('.')[1]) / Math.Pow(10, secondPoint.Value.Split('-')[1].Split('.')[1].Length);
                        double secondPointLat = secondPointLatLeft + secondPointLatRight;


                        double distance = GetDistanceBetweenTwoPoint(firstPointLat, firstPointLong, secondPointLat, secondPointLong);
                        distances[row, col] = distance;
                        dgwRow[col + 1] = distance;
                    }
                    else dgwRow[col + 1] = '-';

                    col++;
                }
                tablo2.Rows.Add(dgwRow);
                row++;
            }
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
                
                points.Add(textBox3.Text, textBox1.Text + "-" + textBox2.Text);

                if (points.Count() > 1) DrawDistancesGridview();

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

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
