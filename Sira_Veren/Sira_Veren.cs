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
using System.Runtime.InteropServices;
using System.Drawing.Printing;

namespace Sira_Veren
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            //this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
        }


        DataTable dt_bolum = new DataTable();
        static string conString = "Server=10.42.2.10\\SQLEXPRESS;Initial Catalog=siramatik;MultipleActiveResultSets=true;User Id=konyaism;Password=1234qwER;";
        SqlConnection baglanti = new SqlConnection(conString);
        int x;

        private void Form1_Load(object sender, EventArgs e)
        {

            string bolum_tablo = "SELECT bolum_id,bolum_adi From Bolumler";
            SqlDataAdapter adp_bolum = new SqlDataAdapter(bolum_tablo, baglanti);
            adp_bolum.Fill(dt_bolum);

            comboBox1.DataSource = dt_bolum;
            comboBox1.DisplayMember = "bolum_adi";
            comboBox1.ValueMember = "bolum_id";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }
                string deger = "SELECT TOP 1 sira_no FROM Sira_Numara Where bolum_id = @p1 and sira_no < 800 ORDER BY id DESC";
                SqlCommand sonveri = new SqlCommand(deger, baglanti);
                sonveri.Parameters.AddWithValue("@p1", comboBox1.SelectedValue);
                SqlDataReader dr = sonveri.ExecuteReader();

                if (dr.Read() == true)
                {
                    string isim = dr["sira_no"].ToString();
                    x = Convert.ToInt32(isim);
                    x++;
                }
                else
                {
                    x = 100;
                }
                dr.Close();
                baglanti.Close();

                baglanti.Open();
                string kayit = "insert into Sira_Numara(bolum_id,sira_no,sira_control) values (@bolum_id,@sira_no,@sira_control)";
                SqlCommand komut = new SqlCommand(kayit, baglanti);
                komut.Parameters.AddWithValue("@bolum_id", comboBox1.SelectedValue);
                komut.Parameters.AddWithValue("@sira_control", "0");
                komut.Parameters.AddWithValue("@sira_no", x);
                komut.ExecuteNonQuery();
                baglanti.Close();
                BarkodBas(x.ToString(), comboBox1.Text);
                x++;
            }
            catch (Exception)
            {
                MessageBox.Show("Hata");
            }

        }
        public void BarkodBas(string numara,string bolum)
        {
            PrintDocument yazdir = new PrintDocument();
            yazdir.PrintPage += delegate (object sender1, PrintPageEventArgs s)
            {
                RectangleF myRectangleF = new RectangleF(5, 0, 200, 300);

                StringFormat format2 = new StringFormat(StringFormatFlags.NoClip);
                format2.LineAlignment = StringAlignment.Center;
                format2.Alignment = StringAlignment.Center;

                StringFormat format1 = new StringFormat(StringFormatFlags.NoClip);
                format1.LineAlignment = StringAlignment.Near;
                format1.Alignment = StringAlignment.Center;

                s.Graphics.DrawString("TC.\n SAĞLIK BAKANLIĞI", new Font("Arial", 7), new SolidBrush(Color.Black), myRectangleF, format1);
                s.Graphics.DrawString(Environment.NewLine + "\nNUMUNE HASTANESİ", new Font("Arial", 9), new SolidBrush(Color.Black), myRectangleF, format1);
                s.Graphics.DrawString(Environment.NewLine + Environment.NewLine +"\n"+bolum, new Font("Arial", 10), new SolidBrush(Color.Black), myRectangleF, format1);
                s.Graphics.DrawString("\n"+numara, new Font("Arial", 42), new SolidBrush(Color.Black), myRectangleF, format1);
                s.Graphics.DrawString(DateTime.Now.ToString(), new Font("Arial", 7), new SolidBrush(Color.Black), myRectangleF, format2);
            };
            try
            {
                yazdir.DefaultPageSettings.PaperSize = new PaperSize("Barkod", 200, 200);
                yazdir.PrinterSettings.PrinterName = "CUSTOM TG2460-H";
                yazdir.Print();
               
            }
            catch (Exception ex)
            {

                throw new Exception("Yazdırma Sırasında Bir Sorun Oluştu.", ex);

            }
        }

        private void Button2_MouseEnter(object sender, EventArgs e)
        {
            this.button2.BackColor = Color.LightGray;
        }

        private void Button2_MouseLeave(object sender, EventArgs e)
        {
            this.button2.BackColor = Color.Gray;
        }
    }
}
