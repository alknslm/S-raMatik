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

namespace Sira_Cagir
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        static string BaglantiAdresi = "Server=**.**.*.**\\SQLEXPRESS;Initial Catalog=siramatik;MultipleActiveResultSets=true;User Id=****;Password=****;";
        SqlConnection baglanti = new SqlConnection(BaglantiAdresi);
        private void Btn_Giris_Click(object sender, EventArgs e)
        {

            string sifre = txtSifre.Text;
            string kimlik = txtKimlik.Text;
            string girilen = "SELECT id,isim,soyisim FROM Kullanicilar WHERE kimlik=@p1 AND sifre=@p2";
            baglanti.Open();
            try
            {
                SqlCommand komut = new SqlCommand(girilen, baglanti);
                komut.Parameters.AddWithValue("@p1", txtKimlik.Text);
                komut.Parameters.AddWithValue("@p2", txtSifre.Text);
                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    MessageBox.Show("Giriş Başarılı");
                    Form1 frm = new Form1();
                    frm.kullaniciid = dr["id"].ToString();
                    frm.kullaniciisim = dr["isim"].ToString();
                    frm.kullanicisoyisim = dr["soyisim"].ToString();
                    Form2 formkapa = new Form2();
                    formkapa.Close();
                    frm.Show();
                    this.Hide();

                }
                else
                {
                    MessageBox.Show("Kullanıcı Adı ya da Şifre Hatalı!");
                    dr.Close();
                    
                }
                
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            baglanti.Close();
        }
    }
}
