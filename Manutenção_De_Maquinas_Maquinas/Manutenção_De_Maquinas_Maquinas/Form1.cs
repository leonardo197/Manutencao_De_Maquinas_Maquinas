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
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Configuration;
using System.Collections;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System.Diagnostics;
using Bunifu.Framework.UI;

namespace Manutenção_De_Maquinas_Maquinas
{
    public partial class Form1 : Form
    {
        Form2 form2 = new Form2();
        Form3 form3 = new Form3();
        int maxrows;
        int maxrows1;
        int maxrows_Consumiveis;
        int id;
        string data;
        Boolean nr = true;
        //Double id;
        string Mensagem;
        string sql_string;
        string st = "Consumiveis";

        SqlCommand cmd;
        SqlDataAdapter da_tab;
        PictureBox[] pb_array = new PictureBox[1];
        Label[] lb_array = new Label[1];

        Label[] lb_Maquinas_Hoje_array = new Label[1];
        Label[] lb_data_Hoje_array = new Label[1];
        Label[] lb_tarefa_Hoje_array = new Label[1];
        PictureBox[] pb_Hojearray = new PictureBox[1];
        ListBox[] lbx_Consumivel_Hoje_array = new ListBox[1];
        PictureBox[] bt_Cert_tarefa_Hoje_array = new PictureBox[1];
        PictureBox[] bt_Errado_Hoje_array = new PictureBox[1];
        Panel[] Panel_Hoje_array = new Panel[1];
   
        Double x1;
        public static Double id_Login { get; set; }
        #region ip's
        //SqlConnection cnn = new SqlConnection("Data Source=10.1.20.39970,1433; Network Library=DBMSSOCN;Initial Catalog=Manutenção_De_Maquinas; User ID=leo;Password=leo");//escola
        //SqlConnection cnn = new SqlConnection("Data Source=192.168.1.117,1433; Network Library=DBMSSOCN;Initial Catalog=Manutenção_De_Maquinas; User ID=admin;Password=caixilour");//casa
        SqlConnection cnn = new SqlConnection(@"Data Source=DESKTOP-HJJN6UL\SQL_LCS197;Initial Catalog=Manutencao_De_Maquinas;Persist Security Info=True;User ID=admin;Password=admin1");
        //SqlConnection cnn = new SqlConnection("Data Source=192.168.2.39,1433; Network Library=DBMSSOCN;Initial Catalog=Manutencao_De_Maquinas; User ID=admin;Password=caixilour1");  //coneção                                                                                                                                                                        //string sql_string_pdf;        
        #endregion
        #region DataTable
        public static DataTable dat_tab_tab { get; set; }
        DataTable dat_tab_Tarefas;
        DataTable dat_tab_Outros;
        DataTable dat_tab_Consumiveis;
        DataTable dat_tab_Login;
        DataTable dat_tab_Historico;
        DataTable dat_tab_Fornecedores;
        #endregion
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            tb_Data_De_Inicio.CustomFormat = "yyyy/MM/dd";
            tb_Data_De_Fin.CustomFormat = "yyyy/MM/dd";
            st = "Tarefa_Agendadas";
            tabControl.SelectedTab = panel_tarrefas_de_hoje;
            form2.ShowDialog();
            form3.ShowDialog();
            Utilisador();
            pes_tb_Tarefas_Hoje();
        }
        public void Utilisador()
        {
            if (Form3.id_permicao == 10)
            {
                Close();
            }
            if (Form3.id_permicao == 2)
            {
                MessageBox.Show("Premiação insuficiente para acessar", "Premiação insuficiente",
                MessageBoxButtons.OK, MessageBoxIcon.Question);
                Close();
            }
            if (Form3.id_permicao == 0 || Form3.id_permicao == 1)
            {
                sql_string = "select * from Logins where ID_Logins=" + id_Login;
                cnn.Open();
                da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_tab = new System.Data.DataTable();
                da_tab.Fill(dat_tab_tab);
                maxrows = dat_tab_tab.Rows.Count;
                cnn.Close();
                // comverte byte ei img
                Byte[] fotos = (byte[])dat_tab_tab.Rows[0]["Foto_Logins"];
                MemoryStream ms = new MemoryStream(fotos);
                pb_logo.BackgroundImage = System.Drawing.Image.FromStream(ms);
                pb_logo_2.BackgroundImage = pb_logo.BackgroundImage;
                //cria pb no panel_menu

                N_Login.Text = Convert.ToString(dat_tab_tab.Rows[0]["Nome_Logins"]);
            }
        }
        #region gauardar
        public void gauardar_Avaria()
        {
            id = Convert.ToInt16( tb_Nome_ID.Text) ;

            sql_string = "select * from Tarefas where ID_Tarefas = '" + id + "'";//vai boscar nome das tarefas
            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_Tarefas = new System.Data.DataTable();
            da_tab.Fill(dat_tab_Tarefas);
            cnn.Close();

            cnn.Open();

            cmd = new SqlCommand();
            cmd.Connection = cnn;
            //requecitos para guardar
            cmd.Parameters.Add(new SqlParameter("@Nome_Tarefas", Convert.ToString(tb_nome_avaria.Text)));
            cmd.Parameters.Add(new SqlParameter("@Tipo_Historico", Convert.ToString("Avaria")));
            cmd.Parameters.Add(new SqlParameter("@Relatorio_Historico", Convert.ToString(tb_Relatorio.Text)));
            cmd.Parameters.Add(new SqlParameter("@Data_Inicio_Historico", Convert.ToDateTime(tb_Data_De_Inicio.Value).ToString("yyyy/MM/dd")));
            cmd.Parameters.Add(new SqlParameter("@Data_Fin_Historico", Convert.ToDateTime(tb_Data_De_Fin.Value).ToString("yyyy/MM/dd")));

            //diz o que para gravar e onde
            cmd.CommandText = " INSERT INTO Historico (Nome_Tarefas, Tipo_Historico, Relatorio_Historico, Data_Inicio_Historico, Data_Fin_Historico) VALUES (@Nome_Tarefas, @Tipo_Historico, @Relatorio_Historico, @Data_Inicio_Historico, @Data_Fin_Historico)";
            cmd.ExecuteNonQuery();//grava
            cnn.Close(); //fexa cnn

             cnn.Open();                                //Data_Prevista_Tarefas= ' " + Convert.ToDateTime(dat_tab_tab.Rows[i]["Data_Inicial_Previsto_Tarefa_Agendadas"]).ToString("yyyy/MM/dd") 
            sql_string = "select * from Historico ";// where Nome_Tarefas='" + tb_nome_avaria.Text + " ' and Relatorio_Historico= ' " + tb_Relatorio.Text + "'";// "' and Data_Inicio_Historico= ' "+ Convert.ToDateTime(tb_Data_De_Inicio.Value).ToString("yyyy/MM/dd") + " '";//vai pesquisar na rr onde as tarefas ligao com as maquinas
             da_tab = new SqlDataAdapter(sql_string, cnn);
             dat_tab_Historico = new System.Data.DataTable();
             da_tab.Fill(dat_tab_Historico);
             maxrows = dat_tab_Historico.Rows.Count;
             cnn.Close();


            cnn.Open();
                cmd = new SqlCommand();
                cmd.Connection = cnn;
                //requecitos para guardar
                cmd.Parameters.Add(new SqlParameter("@ID_Logins", Convert.ToInt32(id_Login)));
                cmd.Parameters.Add(new SqlParameter("@ID_Historico", dat_tab_Historico.Rows[maxrows-1]["ID_Historico"]));
                cmd.CommandText = " INSERT INTO H_L (ID_Logins, ID_Historico) VALUES (@ID_Logins, @ID_Historico)";
                //diz o que para gravar e onde
                cmd.ExecuteNonQuery();//grava
            
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                cmd = new SqlCommand();
                cmd.Connection = cnn;
                //requecitos para guardar
                cmd.Parameters.Add(new SqlParameter("@ID_Consumiveis", Convert.ToInt32(listBox4.Items[i])));
                cmd.Parameters.Add(new SqlParameter("@ID_Historico", dat_tab_Historico.Rows[maxrows - 1]["ID_Historico"]));
                cmd.CommandText = " INSERT INTO H_C (ID_Consumiveis, ID_Historico) VALUES (@ID_Consumiveis, @ID_Historico)";
                //diz o que para gravar e onde
                cmd.ExecuteNonQuery();//grava
            }

              cmd = new SqlCommand();
              cmd.Connection = cnn;
              //requecitos para guardar
              cmd.Parameters.Add(new SqlParameter("@ID_Maquinas", Convert.ToInt32(tb_Nome_ID.Text)));
              cmd.Parameters.Add(new SqlParameter("@ID_Historico", dat_tab_Historico.Rows[maxrows - 1]["ID_Historico"]));
            cmd.CommandText = " INSERT INTO H_M (ID_Maquinas, ID_Historico) VALUES (@ID_Maquinas, @ID_Historico)";
              //diz o que para gravar e onde
              cmd.ExecuteNonQuery();//grava
            cnn.Close();


        }
        #endregion 
        #region pes_tb's
        public void pes_tb()
        {
            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_tab = new System.Data.DataTable();
            da_tab.Fill(dat_tab_tab);
            maxrows = dat_tab_tab.Rows.Count;
            cnn.Close();
            //maxrows = 30;
            PictureBox[] pb_array = new PictureBox[maxrows];//array de ing
            Label[] lb_array = new Label[maxrows];//array de Label

            int n;
            int i = 0;
            int x = 0;

            panel_menu.Controls.Clear();

            for (n = 0; n <= maxrows - 1; n++)
            {
                // comverte byte ei img
                Byte[] fotos = (byte[])dat_tab_tab.Rows[n]["Foto_" + st];
                MemoryStream ms = new MemoryStream(fotos);
                System.Drawing.Image fotos_s = System.Drawing.Image.FromStream(ms);
                // cria pb no panel_menu
                x++;
                pb_array[n] = new PictureBox();
                pb_array[n].Location = new Point(22 + ((x - 1) * 160), 5 + (190 * i));
                pb_array[n].Size = new Size(133, 138);
                pb_array[n].SizeMode = PictureBoxSizeMode.Zoom;
                pb_array[n].Image = fotos_s;
                pb_array[n].Name = Convert.ToString(n);
                this.Controls.Add(tabControl);
                if (st == "Maquinas")
                {
                    pb_array[n].Click += new EventHandler(this.click_fotos_Maquinas_Click);
                    panel_menu.Controls.Add(pb_array[n]);
                }
                else
                {
                    pb_array[n].Click += new EventHandler(this.click_fotos_Consumiveis_Click);
                    panel_menu.Controls.Add(pb_array[n]);
                }
                // pb_array[n].Click += new EventHandler(this.click_fotos_Click);
                panel_menu.Controls.Add(pb_array[n]);
                // cria Label no panel_menu
                lb_array[n] = new Label();
                lb_array[n].Location = new Point(18 + ((x - 1) * 160), 146 + (190 * i));
                lb_array[n].Size = new Size(133, 140);
                lb_array[n].Text = Convert.ToString(dat_tab_tab.Rows[n]["Nome_" + st]);
                lb_array[n].Name = Convert.ToString(n);
                lb_array[n].Size = new Size(150, 60);
                lb_array[n].Font = new System.Drawing.Font(lb_array[n].Font.Name, 12, FontStyle.Bold);
                this.Controls.Add(tabControl);
                panel_menu.Controls.Add(lb_array[n]);
                x1 = Convert.ToDouble(panel_menu.Size.Width) / 160;
                Math.Floor(x1);

                if (x == Math.Floor(x1))
                {
                    x = 0;
                    i = i + 1;
                }
            }
        }
        public void pes_tb_Tarefas_Agendadas()
        {

            sql_string = "select * from T_L where ID_Logins='" + id_Login + "'";//vai buscar todas as tarfas do utilisador
            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_Tarefas = new System.Data.DataTable();
            da_tab.Fill(dat_tab_Tarefas);
            maxrows = dat_tab_Tarefas.Rows.Count;
            cnn.Close();

            PictureBox[] pb_array = new PictureBox[maxrows];//array de ing
            Label[] lb_array = new Label[maxrows];//array de Label

            int n;
            int i = 0;
            int x = 0;

            panel_menu.Controls.Clear();

            for (n = 0; n <= maxrows - 1; n++)
            {
                cnn.Open();
                sql_string = "select * from Tarefas where ID_Tarefas='" + Convert.ToString(dat_tab_Tarefas.Rows[n]["ID_Tarefas"]) + "'" ;
                da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_tab = new System.Data.DataTable();
                da_tab.Fill(dat_tab_tab);
                cnn.Close();
                // comverte byte ei img
                Byte[] fotos = (byte[])dat_tab_tab.Rows[0]["Foto_Tarefas"];
                MemoryStream ms = new MemoryStream(fotos);
                System.Drawing.Image fotos_s = System.Drawing.Image.FromStream(ms);
                //cria pb no panel_menu
                x++;
                pb_array[n] = new PictureBox();
                pb_array[n].Location = new Point(22 + ((x - 1) * 160), 5 + (190 * i));
                pb_array[n].Size = new Size(133, 138);
                pb_array[n].SizeMode = PictureBoxSizeMode.Zoom;
                pb_array[n].Image = fotos_s;
                pb_array[n].Name = Convert.ToString(n);
                //this.Controls.Add(tabControl);
                // pb_array[0].Click += new EventHandler(this.click_fotos_Click);
                panel_menu.Controls.Add(pb_array[n]);
                //cria Label no panel_menu
                lb_array[n] = new Label();
                lb_array[n].Location = new Point(18 + ((x - 1) * 160), 146 + (190 * i));
                //lb_array[n].Size = new Size(133, 140);
                lb_array[n].Text = Convert.ToString(dat_tab_tab.Rows[0]["Nome_Tarefas"]);
                lb_array[n].Font = new System.Drawing.Font(lb_array[n].Font.Name, 12, FontStyle.Bold);
                lb_array[n].Size = new Size(150, 60);
                lb_array[n].Name = Convert.ToString(n);
                //this.Controls.Add(tabControl);
                //lb_array[0].Click += new EventHandler(this.click_fotos_Click);
                panel_menu.Controls.Add(lb_array[n]);
                x1 = Convert.ToDouble(tabControl.Size.Width) / 160;
                if (x == Math.Floor(x1))
                {
                    x = 0;
                    i = i + 1;
                }
            }
        }
        public void pes_tb_Tarefas_Hoje()
        {
            //id_Login
            sql_string = "select * from T_L where ID_Logins='" + id_Login + "'";//vai buscar todas as tarfas do utilisador
            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_Login = new System.Data.DataTable();
            da_tab.Fill(dat_tab_Login);
            maxrows = dat_tab_Login.Rows.Count;
            cnn.Close();

            PictureBox[] pb_array = new PictureBox[maxrows];//array de ing
            Label[] lb_array = new Label[maxrows];//array de Label

            PictureBox[] pb_Hojearray = new PictureBox[maxrows];//array de ing
            Label[] lb_Maquinas_Hoje_array = new Label[maxrows];//array de Labe
            Label[] lb_data_Hoje_array = new Label[maxrows];//array de Labe
            Label[] lb_tarefa_Hoje_array = new Label[maxrows];//array de Labe
            ListBox[] lbx_Consumivel_Hoje_array = new ListBox[maxrows];//array de ListBox
            PictureBox[] bt_Cert_tarefa_Hoje_array = new PictureBox[maxrows];//array de Button
            PictureBox[] bt_Errado_Hoje_array = new PictureBox[maxrows];//array de Button
            Panel[] Panel_Hoje_array = new Panel[maxrows];//array de Panel

            int n;
            int nt;
            int i = 0;
            int x = 0;
            int N_ID = 0;

            panel_tarrefas_de_hoje.Controls.Clear();

            for (n = 0; n <= maxrows - 1; n++)
            {
                cnn.Close();
                sql_string = "select * from " + st+ " where ID_Tarefas = '" + Convert.ToString(dat_tab_Login.Rows[n]["ID_Tarefas"]) + "' and Data_Tarefa_Agendadas BETWEEN '2000-01-01' and '" + Convert.ToDateTime( DateTime.Today).ToString("yyyy/MM/dd") + "'";//vai boscar todas  as tarefas agendadas para o utilisador
             cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_Tarefas = new System.Data.DataTable();
            da_tab.Fill(dat_tab_Tarefas);
                maxrows1 = dat_tab_Tarefas.Rows.Count;
                cnn.Close();
              if (maxrows1 != 0)
              {
                    if (maxrows>=n)
                    {                
                cnn.Open();
                sql_string = "select * from Tarefas where ID_Tarefas='" + Convert.ToString(dat_tab_Tarefas.Rows[0]["ID_Tarefas"]) + "'";//vai buscar todas as tarfas agendadas 
                da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_tab = new System.Data.DataTable();
                da_tab.Fill(dat_tab_tab);
                cnn.Close();
                        // comverte byte ei img
                        x++;
                        //cria Panel no panel_menu(painel)
                        Panel_Hoje_array[n] = new Panel();
                Panel_Hoje_array[n].Location = new Point(0 + ((x - 1) * 900), 0 + (170 * i));          
                Panel_Hoje_array[n].Size = new Size(900, 165 ); 
                Panel_Hoje_array[n].Name = Convert.ToString(n);
                Panel_Hoje_array[n].BackColor = Color.White;
                Panel_Hoje_array[n].BorderStyle = BorderStyle.FixedSingle;
                panel_tarrefas_de_hoje.Controls.Add(Panel_Hoje_array[n]);

                //cria lb_nome_Hoje_array no Panel_Hoje_array(nome da maquina a ser alvo da tarefa)
                cnn.Open();
                sql_string = "select * from T_M where ID_Tarefas='" + Convert.ToString(dat_tab_tab.Rows[0]["ID_Tarefas"]) + "'";//vai pesquisar na rr onde as tarefas ligao com as maquinas
                da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_Outros = new System.Data.DataTable();
                da_tab.Fill(dat_tab_Outros);

                sql_string = "select * from Maquinas where ID_Maquinas='" + Convert.ToString(dat_tab_Outros.Rows[0]["ID_Maquinas"]) + "'";//vai descobrir a relaçao entre o id da maquina na rr e o nome dela
                da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_Outros = new System.Data.DataTable();
                da_tab.Fill(dat_tab_Outros);
                cnn.Close();

                lb_Maquinas_Hoje_array[n] = new Label();
                lb_Maquinas_Hoje_array[n].Location = new Point(170,10);
                lb_Maquinas_Hoje_array[n].Text = Convert.ToString(dat_tab_Outros.Rows[0]["Nome_Maquinas"]);
                lb_Maquinas_Hoje_array[n].Name = Convert.ToString(n);
                lb_Maquinas_Hoje_array[n].Size = new Size(150, 60);
                lb_Maquinas_Hoje_array[n].Font = new System.Drawing.Font(lb_Maquinas_Hoje_array[n].Font.Name, 12, FontStyle.Bold);
                Panel_Hoje_array[n].Controls.Add(lb_Maquinas_Hoje_array[n]);

                //cria lb_data_Hoje_array no Panel_Hoje_array(data da tarefa)
                lb_data_Hoje_array[n] = new Label();
                lb_data_Hoje_array[n].Location = new Point(25, 135);
                lb_data_Hoje_array[n].Text = Convert.ToString(dat_tab_Tarefas.Rows[0]["Data_Tarefa_Agendadas"]);
                lb_data_Hoje_array[n].Name = Convert.ToString(n);
                lb_data_Hoje_array[n].Font = new System.Drawing.Font(lb_data_Hoje_array[n].Font.Name, 12, FontStyle.Bold);
                Panel_Hoje_array[n].Controls.Add(lb_data_Hoje_array[n]);

                //cria lb_tarefa_Hoje_array no Panel_Hoje_array(nome da tarefa)
                lb_tarefa_Hoje_array[n] = new Label();
                lb_tarefa_Hoje_array[n].Location = new Point(170, 100);
                lb_tarefa_Hoje_array[n].Text = Convert.ToString(dat_tab_tab.Rows[0]["Nome_Tarefas"]);
                lb_tarefa_Hoje_array[n].Name = Convert.ToString(n);
                lb_tarefa_Hoje_array[n].Size= new Size(150, 60);
                lb_tarefa_Hoje_array[n].Font = new System.Drawing.Font(lb_tarefa_Hoje_array[n].Font.Name, 12, FontStyle.Bold);
                Panel_Hoje_array[n].Controls.Add(lb_tarefa_Hoje_array[n]);

                //cria lbx_Consumivel_Hoje_array no Panel_Hoje_array(consumiveis da tarefa)
                cnn.Open();
                sql_string = "select * from T_C where ID_Tarefas='" + Convert.ToString(dat_tab_tab.Rows[0]["ID_Tarefas"]) + "'";//vai pesquisar na rr onde as tarefas ligao com as _Consumivel
                da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_Outros = new System.Data.DataTable();
                da_tab.Fill(dat_tab_Outros);
                maxrows_Consumiveis = dat_tab_Outros.Rows.Count;

                     lbx_Consumivel_Hoje_array[n] = new ListBox();//fica de fora para o add funsionar

                   for (nt = 0; nt <= maxrows_Consumiveis-1; )
                   {
                sql_string = "select * from Consumiveis where ID_Consumiveis='" + Convert.ToString(dat_tab_Outros.Rows[nt]["ID_Consumiveis"]) + "'";//vai descobrir a relaçao entre o id da _Consumivel na rr e o nome dela
                    da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_Consumiveis = new System.Data.DataTable();
                da_tab.Fill(dat_tab_Consumiveis);
                cnn.Close();
                     lbx_Consumivel_Hoje_array[n].Location = new Point(400, 0);
                     lbx_Consumivel_Hoje_array[n].Text = "";
                     lbx_Consumivel_Hoje_array[n].Name = Convert.ToString(n);
                     lbx_Consumivel_Hoje_array[n].Size = new Size(170, 180);
                     lbx_Consumivel_Hoje_array[n].Items.Add(Convert.ToString(dat_tab_Consumiveis.Rows[0]["Nome_Consumiveis"]));
                     lbx_Consumivel_Hoje_array[n].Font = new System.Drawing.Font(lbx_Consumivel_Hoje_array[n].Font.Name, 12, FontStyle.Bold);
                     Panel_Hoje_array[n].Controls.Add(lbx_Consumivel_Hoje_array[n]);
                            nt++;
                   }
                //cria bt_Cert_tarefa_Hoje_array no Panel_Hoje_array
                bt_Cert_tarefa_Hoje_array[n] = new PictureBox();
                bt_Cert_tarefa_Hoje_array[n].Location = new Point(600, 0);
                
                bt_Cert_tarefa_Hoje_array[n].Name = Convert.ToString(N_ID);//N_ID
                        bt_Cert_tarefa_Hoje_array[n].Size = new Size(140, 160);
                bt_Cert_tarefa_Hoje_array[n].SizeMode = PictureBoxSizeMode.Zoom;
                bt_Cert_tarefa_Hoje_array[n].Image = pb_certo.Image;
                bt_Cert_tarefa_Hoje_array[n].Click += new EventHandler(this.click_bt_c_Click);
                //bt_Cert_tarefa_Hoje_array[n].BackColor = Color.Green;
                Panel_Hoje_array[n].Controls.Add(bt_Cert_tarefa_Hoje_array[n]);

                //cria bt_Errado_Hoje_array no Panel_Hoje_array
                bt_Errado_Hoje_array[n] = new PictureBox();
                bt_Errado_Hoje_array[n].Location = new Point(750, 0);
                bt_Errado_Hoje_array[n].Name = Convert.ToString(N_ID);//N_ID
                        bt_Errado_Hoje_array[n].Size = new Size(140, 160);
                bt_Errado_Hoje_array[n].SizeMode = PictureBoxSizeMode.Zoom;
                bt_Errado_Hoje_array[n].Image = pb_errado.Image;
                bt_Errado_Hoje_array[n].Click += new EventHandler(this.click_bt_r_Click);
                //bt_Errado_Hoje_array[n].BackColor = Color.Red;
                Panel_Hoje_array[n].Controls.Add(bt_Errado_Hoje_array[n]);

                //cria pb_Hojearray no Panel_Hoje_array
                Byte[] fotos = (byte[])dat_tab_tab.Rows[0]["Foto_Tarefas"];//voltar a por n
                MemoryStream ms = new MemoryStream(fotos);
                System.Drawing.Image fotos_s = System.Drawing.Image.FromStream(ms);
                pb_Hojearray[n] = new PictureBox();
                pb_Hojearray[n].Location = new Point(5,5);
                pb_Hojearray[n].Size = new Size(125, 160);
                pb_Hojearray[n].SizeMode = PictureBoxSizeMode.Zoom;
                pb_Hojearray[n].Image = fotos_s;
                pb_Hojearray[n].Name = Convert.ToString(n);
                Panel_Hoje_array[n].Controls.Add(pb_Hojearray[n]);
                        N_ID++;

                        x1 = Convert.ToDouble(tabControl.Size.Width) /900;
                Math.Floor(x1);
                if (x == Math.Floor(x1))
                {
                    x = 0;
                    i = i + 1;
                }
              }
             }
            }
        }
        #endregion
        #region GRAFICA
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void bt_menu_Click(object sender, EventArgs e)
        {
            if (menu.Width == 55)
            {
                pb_logo_2.Visible = false;
                menu.Visible = false;
                menu.Width = 220;
                tb_Pesquisa.Width = 220;

                
                tabControl.Location = new Point(220, 38);
                panel5.Location = new Point(220, 38);
                tabControl.Width -= 165;
                panel.ShowSync(menu);
                logo.ShowSync(pb_logo);
            }
            else
            {
                menu.Visible = false;
                pb_logo_2.Visible = true;
                pb_logo.Visible = false;
                menu.Width = 55;
                tb_Pesquisa.Width = 55;

                tabControl.Location = new Point(55, 38);
                panel5.Location = new Point(55, 38);
                tabControl.Width += 165;
               panel.ShowSync(menu); 
            }
        }
        private void menu_Paint(object sender, PaintEventArgs e)
        {

        }
        private void pb_logo_Click(object sender, EventArgs e)
        {

        }
        private void bt_min_Click(object sender, EventArgs e)
        {
            
           this.WindowState = FormWindowState.Minimized;
        }
        private void bt_max_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
             this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
           // pes_tb();
        }
        #endregion
        #region bt
        public void click_bt_r_Click(object sender, EventArgs e)
        {
            try
            {
                //id_Login
                sql_string = "select * from T_L where ID_Logins='" + id_Login + "'";//vai buscar todas as tarfas do utilisador
            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_Login = new System.Data.DataTable();
            da_tab.Fill(dat_tab_Login);
            maxrows = dat_tab_Login.Rows.Count;
            cnn.Close();
            for (int n = 0; n <= maxrows - 1; n++)
            {

            
            sql_string = "select * from " + st + " where ID_Tarefas = '" + Convert.ToString(dat_tab_Login.Rows[n]["ID_Tarefas"]) + "' and Data_Tarefa_Agendadas BETWEEN '2000-01-01' and '" + Convert.ToDateTime(DateTime.Today).ToString("yyyy/MM/dd") + "'";//vai boscar todas  as tarefas agendadas para o utilisador

            // sql_string = "select * from " + st;
            nr = false;
           
            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_tab = new System.Data.DataTable();
            da_tab.Fill(dat_tab_tab);
            maxrows = dat_tab_tab.Rows.Count;
            cnn.Close();
            var Pictur = sender as PictureBox;
            for (int i = 0; i < maxrows; i++)
            {
                if (Pictur != null && Pictur.Name == Convert.ToString(i))
                {
                  id = Convert.ToInt16(dat_tab_tab.Rows[i]["ID_" + st]);

                    int vn;
                    vn = Convert.ToInt32(dat_tab_tab.Rows[i]["Vezes_Adiada_Tarefa_Agendadas"]);
                    vn = vn + 1;
                    DateTime dt;
                    dt = DateTime.Today;
                    dt = dt.AddDays(1);

                    cnn.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    //requecitos para guardar
                    cmd.Parameters.Add(new SqlParameter("@Vezes_Adiada_Tarefa_Agendadas", Convert.ToInt32(vn)));
                    cmd.Parameters.Add(new SqlParameter("@ID_Tarefas", dat_tab_tab.Rows[i]["ID_Tarefas"]));
                    cmd.Parameters.Add(new SqlParameter("@Data_Inicial_Previsto_Tarefa_Agendadas", dat_tab_tab.Rows[i]["Data_Inicial_Previsto_Tarefa_Agendadas"]));
                    cmd.Parameters.Add(new SqlParameter("@Data_Tarefa_Agendadas", Convert.ToDateTime(dt)));


                        //diz o que para gravar e onde
                        //cmd.CommandText = " UPDATE " + st + " SET Vezes_Adiada_Tarefa_Agendadas=@Vezes_Adiada_Tarefa_Agendadas, ID_Tarefas= @ID_Tarefas, Data_Tarefa_Agendadas=@Data_Tarefa_Agendadas, Data_Inicial_Previsto_Tarefa_Agendadas=@Data_Inicial_Previsto_Tarefa_Agendadas  WHERE ID_Tarefa_Agendadas=" + id;
                        cmd.CommandText = " INSERT INTO " + st + " (Vezes_Adiada_Tarefa_Agendadas, ID_Tarefas,Data_Inicial_Previsto_Tarefa_Agendadas,Data_Tarefa_Agendadas) VALUES (@Vezes_Adiada_Tarefa_Agendadas, @ID_Tarefas, @Data_Inicial_Previsto_Tarefa_Agendadas, @Data_Tarefa_Agendadas)";

                        cmd.ExecuteNonQuery();//grava
                        cnn.Close(); //fexa cnn

                        cmd = new SqlCommand();
                        cmd.Connection = cnn;
                        //diz o que e onde apagar
                        cmd.CommandText = " DELETE FROM  Tarefa_Agendadas WHERE ID_Tarefa_Agendadas = " + id;
                        cnn.Open();
                        cmd.ExecuteNonQuery();//apagar
                        cnn.Close(); //fexa cnn

                        

                    }
             
            }
            pes_tb_Tarefas_Hoje();
            }
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void click_bt_c_Click(object sender, EventArgs e)
        {
            try
            {
                sql_string = "select * from T_L where ID_Logins='" + id_Login + "'";//vai buscar todas as tarfas do utilisador
                cnn.Open();
                da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_Login = new System.Data.DataTable();
                da_tab.Fill(dat_tab_Login);
                maxrows = dat_tab_Login.Rows.Count;
                cnn.Close();
                for (int n = 0; n <= maxrows - 1; n++)
                {


                    sql_string = "select * from " + st + " where ID_Tarefas = '" + Convert.ToString(dat_tab_Login.Rows[n]["ID_Tarefas"]) + "' and Data_Tarefa_Agendadas BETWEEN '2000-01-01' and '" + Convert.ToDateTime(DateTime.Today).ToString("yyyy/MM/dd") + "'";//vai boscar todas  as tarefas agendadas para o utilisador
                  //sql_string = "select * from " + st;
                    nr = false;

            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_tab = new System.Data.DataTable();
            da_tab.Fill(dat_tab_tab);
            maxrows = dat_tab_tab.Rows.Count;
            cnn.Close();
            var Pictur = sender as PictureBox;
            for (int i = 0; i < maxrows; i++)
            {
                if (Pictur != null && Pictur.Name == Convert.ToString(i))
                {
                     id = Convert.ToInt16(dat_tab_tab.Rows[i]["ID_" + st]);

                    sql_string = "select * from Tarefas where ID_Tarefas = '" + Convert.ToInt16(dat_tab_tab.Rows[i]["ID_Tarefas"]) + "'";//vai boscar nome das tarefas
                    cnn.Open();
                    da_tab = new SqlDataAdapter(sql_string, cnn);
                    dat_tab_Tarefas = new System.Data.DataTable();
                    da_tab.Fill(dat_tab_Tarefas);
                    cnn.Close();

                    int vn;
                    vn = Convert.ToInt32(dat_tab_tab.Rows[i]["Vezes_Adiada_Tarefa_Agendadas"]);
                    DateTime dt;
                    dt = Convert.ToDateTime(dat_tab_tab.Rows[i]["Data_Tarefa_Agendadas"]);
                    cnn.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    //requecitos para guardar
                    cmd.Parameters.Add(new SqlParameter("@Vezes_Adiada", Convert.ToInt32(vn)));//
                    cmd.Parameters.Add(new SqlParameter("@Nome_Tarefas", dat_tab_Tarefas.Rows[0]["Nome_Tarefas"]));
                    cmd.Parameters.Add(new SqlParameter("@ID_Tarefas", dat_tab_tab.Rows[i]["ID_Tarefas"]));//

                    cmd.Parameters.Add(new SqlParameter("@Tipo_Historico", Convert.ToString("Manutenção")));
                    cmd.Parameters.Add(new SqlParameter("@Relatorio_Historico", Convert.ToString("Manutenção")));

                    cmd.Parameters.Add(new SqlParameter("@Data_Prevista_Tarefas", dat_tab_tab.Rows[i]["Data_Inicial_Previsto_Tarefa_Agendadas"]));//
                    cmd.Parameters.Add(new SqlParameter("@Data_Inicio_Historico", Convert.ToDateTime(dt)));
                    cmd.Parameters.Add(new SqlParameter("@Data_Fin_Historico", Convert.ToDateTime(dt)));

                    //diz o que para gravar e onde
                    cmd.CommandText = " INSERT INTO Historico (Vezes_Adiada, Nome_Tarefas, ID_Tarefas, Tipo_Historico, Relatorio_Historico, Data_Prevista_Tarefas, Data_Inicio_Historico, Data_Fin_Historico) VALUES (@Vezes_Adiada, @Nome_Tarefas, @ID_Tarefas, @Tipo_Historico, @Relatorio_Historico, @Data_Prevista_Tarefas, @Data_Inicio_Historico, @Data_Fin_Historico)";
                    cmd.ExecuteNonQuery();//grava
                    cnn.Close(); //fexa cnn

                    cnn.Open();
                    sql_string = "select * from Historico where ID_Tarefas='" + Convert.ToString(dat_tab_tab.Rows[0]["ID_Tarefas"]) + " ' and Data_Prevista_Tarefas= ' " + Convert.ToDateTime(dat_tab_tab.Rows[i]["Data_Inicial_Previsto_Tarefa_Agendadas"]).ToString("yyyy/MM/dd") + " ' and Vezes_Adiada= ' "+vn+" '";//vai pesquisar na rr onde as tarefas ligao com as maquinas
                    da_tab = new SqlDataAdapter(sql_string, cnn);
                    dat_tab_Historico = new System.Data.DataTable();
                    da_tab.Fill(dat_tab_Historico);
                    cnn.Close();
                    //--------------------------------------------------------------------------------------------vai pesquisar na rr onde as tarefas ligao com as maquinas

                    cnn.Open();
                    sql_string = "select * from T_C where ID_Tarefas='" + Convert.ToString(dat_tab_tab.Rows[i]["ID_Tarefas"]) + "'";
                    da_tab = new SqlDataAdapter(sql_string, cnn);
                    dat_tab_Consumiveis = new System.Data.DataTable();
                    da_tab.Fill(dat_tab_Consumiveis);
                    cnn.Close();

                    cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    //requecitos para guardar
                    cmd.Parameters.Add(new SqlParameter("@ID_Consumiveis", Convert.ToInt32(dat_tab_Consumiveis.Rows[i]["ID_Consumiveis"])));
                    cmd.Parameters.Add(new SqlParameter("@ID_Historico", Convert.ToInt32(dat_tab_Historico.Rows[i]["ID_Historico"])));
                    cmd.CommandText = " INSERT INTO H_C (ID_Consumiveis, ID_Historico) VALUES (@ID_Consumiveis, @ID_Historico)";
                    //diz o que para gravar e onde
                    cnn.Open();
                    cmd.ExecuteNonQuery();//grava
                    cnn.Close();
                    //--------------------------------------------------------------------------------------------vai pesquisar na rr onde as tarefas ligao com as maquinas

                    cnn.Open();
                    sql_string = "select * from T_L where ID_Tarefas='" + Convert.ToString(dat_tab_tab.Rows[i]["ID_Tarefas"]) + "'";
                    da_tab = new SqlDataAdapter(sql_string, cnn);
                    dat_tab_Login = new System.Data.DataTable();
                    da_tab.Fill(dat_tab_Login);
                    cnn.Close();

                    cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    //requecitos para guardar
                    cmd.Parameters.Add(new SqlParameter("@ID_Logins", Convert.ToInt32(dat_tab_Login.Rows[i]["ID_Logins"])));
                    cmd.Parameters.Add(new SqlParameter("@ID_Historico", Convert.ToInt32(dat_tab_Historico.Rows[i]["ID_Historico"])));
                    cmd.CommandText = " INSERT INTO H_L (ID_Logins, ID_Historico) VALUES (@ID_Logins, @ID_Historico)";
                    //diz o que para gravar e onde
                    cnn.Open();
                    cmd.ExecuteNonQuery();//grava
                    cnn.Close();
                    //-------------------------------------------------------------------------------------------vai pesquisar na rr onde as tarefas ligao com as maquinas

                    cnn.Open();
                    sql_string = "select * from T_M where ID_Tarefas='" + Convert.ToString(dat_tab_tab.Rows[i]["ID_Tarefas"]) + "'";
                    da_tab = new SqlDataAdapter(sql_string, cnn);
                    dat_tab_Outros = new System.Data.DataTable();
                    da_tab.Fill(dat_tab_Outros);
                    cnn.Close();

                    cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    //requecitos para guardar
                    cmd.Parameters.Add(new SqlParameter("@ID_Maquinas", Convert.ToInt32(dat_tab_Outros.Rows[i]["ID_Maquinas"])));
                    cmd.Parameters.Add(new SqlParameter("@ID_Historico", Convert.ToInt32(dat_tab_Historico.Rows[i]["ID_Historico"])));
                    cmd.CommandText = " INSERT INTO H_M (ID_Maquinas, ID_Historico) VALUES (@ID_Maquinas, @ID_Historico)";
                    //diz o que para gravar e onde
                    cnn.Open();
                    cmd.ExecuteNonQuery();//grava
                    cnn.Close();

                    //-------------------------------------------------------------------------------------------remarca tarefa
                   

                    int dt1= Convert.ToInt16 ( dat_tab_Tarefas.Rows[0]["Periodo_Tarefas"]);
                    dt = DateTime.Today;
                    dt = dt.AddDays(dt1);
                    cnn.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    //requecitos para guardar
                    cmd.Parameters.Add(new SqlParameter("@Vezes_Adiada_Tarefa_Agendadas", Convert.ToInt32(0)));
                    cmd.Parameters.Add(new SqlParameter("@ID_Tarefas", dat_tab_tab.Rows[i]["ID_Tarefas"]));
                    cmd.Parameters.Add(new SqlParameter("@Data_Inicial_Previsto_Tarefa_Agendadas", Convert.ToDateTime(dt)));
                    cmd.Parameters.Add(new SqlParameter("@Data_Tarefa_Agendadas", Convert.ToDateTime(dt)));

                    //diz o que para gravar e onde
                    //cmd.CommandText = " UPDATE " + st + " SET Vezes_Adiada_Tarefa_Agendadas=@Vezes_Adiada_Tarefa_Agendadas, ID_Tarefas= @ID_Tarefas, Data_Tarefa_Agendadas=@Data_Tarefa_Agendadas, Data_Inicial_Previsto_Tarefa_Agendadas=@Data_Inicial_Previsto_Tarefa_Agendadas  WHERE ID_Tarefa_Agendadas=" + id;
                    cmd.CommandText = " INSERT INTO " + st + " (Vezes_Adiada_Tarefa_Agendadas, ID_Tarefas,Data_Inicial_Previsto_Tarefa_Agendadas,Data_Tarefa_Agendadas) VALUES (@Vezes_Adiada_Tarefa_Agendadas, @ID_Tarefas, @Data_Inicial_Previsto_Tarefa_Agendadas, @Data_Tarefa_Agendadas)";

                        cmd.ExecuteNonQuery();//grava
                    cnn.Close(); //fexa cnn

                        cmd = new SqlCommand();
                        cmd.Connection = cnn;
                        //diz o que e onde apagar
                        cmd.CommandText = " DELETE FROM  Tarefa_Agendadas WHERE ID_Tarefa_Agendadas = " + id;
                        cnn.Open();
                        cmd.ExecuteNonQuery();//apagar
                        cnn.Close(); //fexa cnn



                        //--------------------------------------------------------------------dar baixa de comsomiveis na bd
                        int id_Consumiveis= Convert.ToInt16(dat_tab_Consumiveis.Rows[i]["ID_Consumiveis"]);
                    cnn.Open();
                    sql_string = "select * from Consumiveis where ID_Consumiveis='" + Convert.ToString(dat_tab_Consumiveis.Rows[i]["ID_Consumiveis"]) + "'";
                    da_tab = new SqlDataAdapter(sql_string, cnn);
                    dat_tab_Consumiveis = new System.Data.DataTable();
                    da_tab.Fill(dat_tab_Consumiveis);
                    cnn.Close();

                    //faz desconto nos consomiveis
                    int Quantidade_Consumiveis = Convert.ToInt16(dat_tab_Consumiveis.Rows[i]["Quantidade_Consumiveis"]);
                    Quantidade_Consumiveis = Quantidade_Consumiveis - 1;

                    cnn.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    //requecitos para guardar
                    cmd.Parameters.Add(new SqlParameter("@Nome_Consumiveis" , Convert.ToString(dat_tab_Consumiveis.Rows[i]["Nome_Consumiveis"])));
                    cmd.Parameters.Add(new SqlParameter("@Quantidade_Consumiveis", Convert.ToDecimal(Quantidade_Consumiveis)));
                    cmd.Parameters.Add(new SqlParameter("@Max_Consumiveis" , Convert.ToDecimal(dat_tab_Consumiveis.Rows[i]["Max_Consumiveis"]))); 
                    cmd.Parameters.Add(new SqlParameter("@Min_Consumiveis" , Convert.ToDecimal(dat_tab_Consumiveis.Rows[i]["Min_Consumiveis"])));
                    cmd.Parameters.Add(new SqlParameter("@Encomenda_Quantidade_Consumiveis", Convert.ToDecimal(dat_tab_Consumiveis.Rows[i]["Encomenda_Quantidade_Consumiveis"])));

                    //diz o que para gravar e onde
                    cmd.CommandText = " UPDATE Consumiveis SET Nome_Consumiveis=@Nome_Consumiveis, Quantidade_Consumiveis= @Quantidade_Consumiveis, Max_Consumiveis=@Max_Consumiveis, Min_Consumiveis=@Min_Consumiveis, Encomenda_Quantidade_Consumiveis=@Encomenda_Quantidade_Consumiveis  WHERE ID_Consumiveis=" + id_Consumiveis;

                    cmd.ExecuteNonQuery();//grava
                    cnn.Close(); //fexa cnn


                    //--------------------------------------------------------------ve se os Consumiveis tao na coatia indicada
                    //---------------------------------------------------------------enviar email.
                    if (Quantidade_Consumiveis<= Convert.ToDecimal(dat_tab_Consumiveis.Rows[i]["Encomenda_Quantidade_Consumiveis"]))
                    {
                        cnn.Open();
                        sql_string = "select * from C_F where ID_Consumiveis='" + id_Consumiveis + "'";
                        da_tab = new SqlDataAdapter(sql_string, cnn);
                        dat_tab_Outros = new System.Data.DataTable();
                        da_tab.Fill(dat_tab_Outros);//dat_tab_Fornecedores
                        cnn.Close();
                        maxrows = dat_tab_Outros.Rows.Count;
                      
                            for (int ii = 0; ii < maxrows; ii++)
                            {
                            cnn.Open();
                            sql_string = "select * from Fornecedores where ID_Fornecedores='" + Convert.ToString(dat_tab_Outros.Rows[ii]["ID_Fornecedores"]) + "'";
                            da_tab = new SqlDataAdapter(sql_string, cnn);
                            dat_tab_Fornecedores = new System.Data.DataTable();
                            da_tab.Fill(dat_tab_Fornecedores);
                            cnn.Close();

                            //mail

                            Mensagem ="Bom dia "+ Convert.ToString(dat_tab_Fornecedores.Rows[ii]["Nome_Fornecedores"]) + "" +
                            "\n Gostávamos de saber a disponibilidade e preço do seguinte produto: " +
                            "\n \n \n " + dat_tab_Consumiveis.Rows[i]["Nome_Consumiveis"]+ "Quantidade      "+ dat_tab_Consumiveis.Rows[i]["Encomenda_Quantidade_Consumiveis"] +
                            "\n \n \n Muito obrigado, \n Ficamos a espera de resposta o mais breve possível. " +
                            "\n CAIXILOUR";

                              MailMessage mensagemEmail = new MailMessage("manutencaomaquinas197@gmail.com", "leonardocosta197@gmail.com", "Pedido De Requisição De Material Para Caixilour", Mensagem);
                              //MailMessage mensagemEmail = new MailMessage(Remetente, Destinatario, Assunto, enviaMensagem);
                              SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                              client.EnableSsl = true;
                              NetworkCredential cred = new NetworkCredential("manutencaomaquinas197@gmail.com", "nhixrxvrghayjbwe");
                              client.Credentials = cred;
                               // envia a mensagem
                              client.Send(mensagemEmail);


                            }
                    }
                }
                    }
                    pes_tb_Tarefas_Hoje();
            }
            pes_tb_Tarefas_Hoje();
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void click_fotos_Maquinas_Click(object sender, EventArgs e)
        {
            nr = false;

            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_tab = new System.Data.DataTable();
            da_tab.Fill(dat_tab_tab);
            maxrows = dat_tab_tab.Rows.Count;
            cnn.Close();
            var Pictur = sender as PictureBox;
            for (int i = 0; i < maxrows; i++)
            {
                if (Pictur != null && Pictur.Name == Convert.ToString(i))
                {
                    lb_nome_maquina.Text = Convert.ToString(dat_tab_tab.Rows[i]["Nome_"+st]);
                    Byte[] fotos = (byte[])dat_tab_tab.Rows[i]["Foto_" + st];
                    MemoryStream ms = new MemoryStream(fotos);
                    System.Drawing.Image fotos_s = System.Drawing.Image.FromStream(ms);
                    //cria pb no panel_menu
                    pb_foto_maquinas.BackgroundImage = fotos_s;
                    tb_Nome_ID.Text= Convert.ToString(dat_tab_tab.Rows[i]["ID_" + st]);
                }
             tabControl.SelectedTab = panel_Avarias;
            }
        }
        public void click_fotos_Consumiveis_Click(object sender, EventArgs e)
        {
            nr = false;

            cnn.Open();
            da_tab = new SqlDataAdapter(sql_string, cnn);
            dat_tab_tab = new System.Data.DataTable();
            da_tab.Fill(dat_tab_tab);
            maxrows = dat_tab_tab.Rows.Count;
            cnn.Close();
            var Pictur = sender as PictureBox;
            for (int i = 0; i < maxrows; i++)
            {
                if (Pictur != null && Pictur.Name == Convert.ToString(i))
                {
                    listBox5.Items.Add(Convert.ToString(dat_tab_tab.Rows[i]["Nome_" + st]));
                    listBox4.Items.Add(Convert.ToString(dat_tab_tab.Rows[i]["ID_" + st]));
                }
                tabControl.SelectedTab = panel_Avarias;
            }
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            st = "Consumiveis";
            //pes_tb();
        }
        private void bt_TARREFAS_DE_HOJE_Click(object sender, EventArgs e)
        {
            try
            {
            st = "Tarefa_Agendadas";
            tabControl.SelectedTab = panel_tarrefas_de_hoje;
            pes_tb_Tarefas_Hoje();
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
                // cria uma mensagem
                
                MailMessage mensagemEmail = new MailMessage( "manutencaomaquinas197@gmail.com", "leonardocosta197@gmail.com", "teste", "texttt \n oi");
                //MailMessage mensagemEmail = new MailMessage(Remetente, Destinatario, Assunto, enviaMensagem);
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                NetworkCredential cred = new NetworkCredential("manutencaomaquinas197@gmail.com", "nhixrxvrghayjbwe");
                client.Credentials = cred;
                // envia a mensagem
                client.Send(mensagemEmail);
        }
        private void BT_Consumiveis_Click(object sender, EventArgs e)
        {
            try
            {
            sql_string = "select * from Consumiveis";
            st = "Consumiveis";
            pes_tb();
            tabControl.SelectedTab = panel_menu;
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BT_Maquinas_Click(object sender, EventArgs e)
        {
            try
            {
                sql_string = "select * from Maquinas";
                st = "Maquinas";
                pes_tb();
                tabControl.SelectedTab = panel_menu;
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void bunifuFlatButton4_Click(object sender, EventArgs e)
        {        
            try
            {
            pes_tb_Tarefas_Agendadas();
            tabControl.SelectedTab = panel_menu;
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void bunifuFlatButton5_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = panel_Avarias;
        }
        #endregion
        private void bunifuImageButton8_Click(object sender, EventArgs e)
        {
            try
            {
            if (tb_Relatorio.Text!="")
            {
                if (tb_Nome_ID.Text!="")
                {
                    if (tb_nome_avaria.Text!="")
                    {
                        if (listBox4.Items.Count!=0)
                        {
                         gauardar_Avaria();
                         listBox5.Items.Clear();
                         listBox4.Items.Clear();
                         tb_Relatorio.Text = "";
                         tb_Nome_ID.Text = "";
                         tb_nome_avaria.Text = ""; 
                        }
                    }
                }
            } 
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message , "Erro",
    MessageBoxButtons.OK, MessageBoxIcon.Error);
  
            }

            
        }
        private void tb_Pesquisa_OnTextChange(object sender, EventArgs e)
        {

        }
        private void bunifuImageButton7_Click(object sender, EventArgs e)
        {
            listBox5.Items.Clear();
            listBox4.Items.Clear();
            tb_Relatorio.Text = "";
            tb_Nome_ID.Text = "";
            tb_nome_avaria.Text = "";
        }

        private void panel_menu_Click(object sender, EventArgs e)
        {

        }
    }
}
