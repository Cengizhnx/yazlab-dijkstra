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
        Dictionary<string, string> visited = new Dictionary<string, string>();
        double[,] distances = new double[0, 0];

        double[] visitedPoints = new double[0];
        double[] lastVisitiedPoint = new double[2];

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
            distances = new double[points.Count(), points.Count()];

            CreateVisitedPointsArray();

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
                    else dgwRow[col + 1] = 0.0;

                    col++;
                }
                tablo2.Rows.Add(dgwRow);
                row++;
            }
        }

        private void CreateVisitedPointsArray()
        {
            visitedPoints = new double[points.Count()];
            for (int i = 0; i < points.Count(); i++)
            {
                visitedPoints[i] = 0;
            }
        }

        private void DrawDijkstraGraph()
        {
            
        }

        private void findShortestPath()
        {
            int counter = 0;


            foreach (var firstPoint in points)
            {
                double weight = 0;
                for (int i = 0; i < distances.Length; i++)
                {
                    if (counter == i)
                    {

                        double leastWeight = 9999999; 

                        for (int j = 0; j < distances.Length; j++)
                        {
                            if (distances[i, j] < leastWeight && visitedPoints[j] == 0)
                            {
                                visitedPoints[j] = 1;
                                double shortestWeightToSecondPoint = distances[i, j];
                            }
                        }
                    }
                }

                counter = counter + 1;
            }
        }

        private double findShortestNextMove(string startPointName)
        {
            int indexNumber = 0, shortestPathLeftNumber = 0, shortestPathRightNumber = 0;
            double shortestWeightToSecondPoint = 9999999, leastWeight;

            visitedPoints[findPathNameToIndex(startPointName)] = 1;

            if (isAllPointsVisited())
            {
                shortestPathLeftNumber = points.Count();
                shortestPathRightNumber = 0;
                shortestWeightToSecondPoint = distances[points.Count() - 1, 0];
            }
            else{
                foreach (var point in points)
                {
                    if (startPointName == point.Key)
                    {   
                        leastWeight = 9999999;

                        for (int j = 0; j < points.Count(); j++)
                        {
                            if (distances[indexNumber, j] < shortestWeightToSecondPoint && visitedPoints[j] == 0 && distances[indexNumber, j] != 0)
                            {
                                shortestPathLeftNumber = indexNumber;
                                shortestPathRightNumber = j;
                                shortestWeightToSecondPoint = distances[indexNumber, j];
                            }
                        }
                    }
                    indexNumber++;
                }
            }

            visited.Add(startPointName, shortestWeightToSecondPoint.ToString());

            if (!isAllPointsVisited()) shortestWeightToSecondPoint += findShortestNextMove(findRightNumberToPathName(shortestPathRightNumber));
            return shortestWeightToSecondPoint;
        }

        private string findRightNumberToPathName(int indexNumber)
        {
            int counter = 0;
            foreach (var point in points)
            {
                if (counter == indexNumber)
                {
                    return point.Key;
                }

                counter++;
            }
            return "-1";
        }

        private int findPathNameToIndex(string pathName)
        {
            int counter = 0;
            foreach (var point in points)
            {
                if (pathName == point.Key) return counter;

                counter++;
            }
            return -1;
        }

        private bool isAllPointsVisited()
        {
            for (int i = 0; i < points.Count(); i++)
            {
                if (visitedPoints[i] == 0) return false;
            }

            return true;
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

            visited = new Dictionary<string, string>();

            DrawDijkstraGraph();
            CreateVisitedPointsArray();
            double shortestWeight = findShortestNextMove(points.First().Key);
        }
    }
}
