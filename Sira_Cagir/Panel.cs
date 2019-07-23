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
using System.Net;
using System.Net.Sockets;

namespace Sira_Cagir
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static string BaglantiAdresi = "Server=**.**.*.**\\SQLEXPRESS;Initial Catalog=siramatik;MultipleActiveResultSets=true;User Id=****;Password=****;";
        SqlConnection baglanti = new SqlConnection(BaglantiAdresi);
        Form2 asda = new Form2();

        DataTable dt_list = new DataTable();
        //Form1 formkapa = new Form1();
        //formkapa.Close();
        //Form2 form = new Form2();
        //form.Show();
        //this.Hide();
        
        public string kullaniciid { get; set; }
        public string kullaniciisim { get; set; }
        public string kullanicisoyisim { get; set; }
        private void Btn_cagir_Click_1(object sender, EventArgs e)
        {

            string kayit = "SELECT bolum_id,sira_no,id,sira_control FROM Sira_Numara Where sira_control=0 AND bolum_id="+comboBox1.SelectedValue+"ORDER BY id ASC";
            string kayit1 = "SELECT bolum_id,sira_no,id,sira_control FROM Sira_Numara Where sira_control=0 AND sira_no >= 800 AND bolum_id=" + comboBox1.SelectedValue + " ORDER BY id ASC";
            baglanti.Open();
            try
            {
                SqlCommand komutozel = new SqlCommand(kayit1, baglanti);
                SqlDataReader drozel = komutozel.ExecuteReader();
                if (drozel.Read())
                {
                    if (Convert.ToInt32(drozel["sira_control"]) == 0)
                    {
                        ExecuteClient(drozel["sira_no"].ToString());
                        //asda.lbl_sira.Text = drozel["sira_no"].ToString();
                        string c = drozel["id"].ToString();
                        drozel.Close();
                        string update = "UPDATE Sira_Numara SET sira_control=1 WHERE id=" + c;
                        SqlCommand komutozelupdate = new SqlCommand(update, baglanti);
                        SqlDataReader drozelupdate = komutozelupdate.ExecuteReader();
                        drozelupdate.Close();
                    }
                }
                else
                {
                    drozel.Close();
                    SqlCommand komut = new SqlCommand(kayit, baglanti);
                    SqlDataReader dr = komut.ExecuteReader();
                    if (dr.Read())
                    {
                        if (Convert.ToInt32(dr["sira_control"]) == 0)
                        {
                            ExecuteClient(dr["sira_no"].ToString());
                            //asda.lbl_sira.Text = dr["sira_no"].ToString();
                            string y = dr["id"].ToString();
                            dr.Close();
                            string update = "UPDATE Sira_Numara SET sira_control=1 WHERE id=" + y;
                            SqlCommand komutupdate = new SqlCommand(update, baglanti);
                            SqlDataReader dr2 = komutupdate.ExecuteReader();
                        }
                    }
                    else
                        MessageBox.Show("Sırada Kayıt Bulunmamaktadır");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            baglanti.Close();

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string grln = txt_Girilen.Text;
            string girilen = "SELECT bolum_id,sira_no,sira_control FROM Sira_Numara WHERE sira_no=" + grln +"AND bolum_id="+comboBox1.SelectedValue;
            string girilenozel = "SELECT bolum_id,sira_no,sira_control FROM Sira_Numara WHERE sira_no=" + grln +"AND bolum_id=" + comboBox1.SelectedValue;
            if (grln == "")
            {
                MessageBox.Show("Lütfen Sıra Numarası Giriniz");
            }
            else
            {
                baglanti.Open();
                try
                {
                    SqlCommand komut = new SqlCommand(girilen, baglanti);
                    SqlDataReader dr = komut.ExecuteReader();
                    if (dr.Read())
                    {
                        if (Convert.ToInt32(dr["sira_control"]) == 1)
                        {
                            ExecuteClient(dr["sira_no"].ToString());
                            //asda.lbl_sira.Text = dr["sira_no"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Sıra Numaranız Henüz Gelmemiştir");
                        }
                    }
                    else
                    {
                        dr.Close();
                        SqlCommand komutgozel = new SqlCommand(girilenozel, baglanti);
                        SqlDataReader drgozel = komutgozel.ExecuteReader();
                        if (drgozel.Read())
                        {
                            if (Convert.ToInt32(drgozel["sira_control"]) == 1)
                            {
                                ExecuteClient(drgozel["sira_no"].ToString());
                                //asda.lbl_sira.Text = drgozel["sira_no"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Sıra Numaranız Henüz Gelmemiştir");
                            }

                        }
                        else
                            MessageBox.Show("Girdiğiniz Kayıt Bulunmamaktadır");
                    }
                    txt_Girilen.Clear();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                baglanti.Close();
            }
        }

        private void Txt_Girilen_KeyPress_1(object sender, KeyPressEventArgs e)
        {

            if ((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57)
            {
                e.Handled = false;//eğer rakamsa  yazdır.
            }

            else if ((int)e.KeyChar == 8)
            {
                e.Handled = false;//eğer basılan tuş backspace ise yazdır.
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            baglanti.ConnectionString = BaglantiAdresi;
            lbl_isim.Text = kullaniciisim;
            lbl_soyisim.Text = kullanicisoyisim;
            string kayit = "SELECT Bolumler.bolum_adi,Bolumler.bolum_id from Kullanicilar join Kullanici_Bolum on Kullanicilar.id = Kullanici_Bolum.kullanici_id join Bolumler on Bolumler.bolum_id = Kullanici_Bolum.bolum_id Where Kullanicilar.id ="+kullaniciid;
            baglanti.Open();
            SqlDataAdapter adp_list = new SqlDataAdapter(kayit, baglanti);
            adp_list.Fill(dt_list);

            comboBox1.DataSource = dt_list;
            comboBox1.DisplayMember = "bolum_adi";
            comboBox1.ValueMember = "bolum_id";
            baglanti.Close();
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            Form2  form2 = new Form2();
            form1.Close();
            form2.Show();
            this.Hide();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbl_bolum.Text = comboBox1.Text;
        }
        int x;
        private void Btn_ozel_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }
                string deger = "SELECT TOP 1 sira_no FROM Sira_Numara Where bolum_id="+comboBox1.SelectedValue+" AND sira_no >= 800 ORDER BY id DESC";
                SqlCommand sonveri = new SqlCommand(deger, baglanti);
                SqlDataReader dr = sonveri.ExecuteReader();

                if (dr.Read() == true)
                {
                    string isim = dr["sira_no"].ToString();
                    x = Convert.ToInt32(isim);
                    x++;
                }
                else
                {
                    x = 800;
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

                //BARKODA SIRA  NUMARASI BASIALACAK YER
                MessageBox.Show("SIRA NUMARANIZ\n" + x.ToString());
                x++;
            }
            catch (Exception)
            {
                MessageBox.Show("Hata");
            }
        }
        static void ExecuteClient(string numara)
        {

            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
                MessageBox.Show(localEndPoint.ToString());
                Socket sender = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(localEndPoint);
                    //Console.WriteLine("Socket connected to -> {0} ",
                    //              sender.RemoteEndPoint.ToString());

                    byte[] messageSent = Encoding.ASCII.GetBytes(numara+"<EOF>");
                    int byteSent = sender.Send(messageSent);

                    byte[] messageReceived = new byte[1024];

                    int byteRecv = sender.Receive(messageReceived);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                catch (ArgumentNullException ane)
                {

                    MessageBox.Show("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    MessageBox.Show("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    MessageBox.Show("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
        }

        private void Btn_sifirla_Click(object sender, EventArgs e)
        {
            string sifirla = "DELETE FROM Sira_Numara WHERE bolum_id=" + comboBox1.SelectedValue;
            if (MessageBox.Show("SIFIRLAMAK İSTEDİĞİNİZE EMİN MİSİNİZ?", "SIFIRLA", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                baglanti.Open();
                try
                {
                    SqlCommand komut = new SqlCommand(sifirla, baglanti);
                    SqlDataReader dr = komut.ExecuteReader();
                    MessageBox.Show("BAŞARIYLA SIFIRLANDI");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                baglanti.Close();

            }
            else
            {
                MessageBox.Show("İşleminiz iptal edildi");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
