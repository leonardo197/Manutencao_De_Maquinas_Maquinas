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

using System.Diagnostics;

namespace Manutenção_De_Maquinas_Maquinas
{
    public partial class Form3 : Form
    {
        string sql_string;
        //SqlCommand cmd;
        SqlDataAdapter da_tab;
        DataTable dat_tab_tab;
        //public static Double id_Login { get; set; }
        public static Double id_permicao { get; set; }
        #region ip's
        //SqlConnection cnn = new SqlConnection("Data Source=10.1.20.39970,1433; Network Library=DBMSSOCN;Initial Catalog=Manutenção_De_Maquinas; User ID=leo;Password=leo");//escola
        //SqlConnection cnn = new SqlConnection("Data Source=192.168.1.117,1433; Network Library=DBMSSOCN;Initial Catalog=Manutenção_De_Maquinas; User ID=admin;Password=caixilour");//casa
        SqlConnection cnn = new SqlConnection(@"Data Source=DESKTOP-HJJN6UL\SQL_LCS197;Initial Catalog=Manutencao_De_Maquinas;Persist Security Info=True;User ID=admin;Password=admin1");
        //SqlConnection cnn = new SqlConnection("Data Source=192.168.2.39,1433; Network Library=DBMSSOCN;Initial Catalog=Manutencao_De_Maquinas; User ID=admin;Password=caixilour1");  //coneção                                                                                                                                                                        //string sql_string_pdf;        
        #endregion
        private void pes_utilizador()
        {
            id_permicao = 10;

            if (NOME_Utelirador.text != "" && PASSE_Utelirador.text != "")//verifica se o login e valuido
            {
                sql_string = "select count(*) from Logins where Nome_Logins='" + NOME_Utelirador.text + "'and Pass_Logins='" + PASSE_Utelirador.text + "'";//

                ////ligar tab
                cnn.Open();
                da_tab = new SqlDataAdapter(sql_string, cnn);
                dat_tab_tab = new System.Data.DataTable();
                da_tab.Fill(dat_tab_tab);

                if ("1" == Convert.ToString(dat_tab_tab.Rows[0][0]))//verifica se o login esiste
                {
                    sql_string = "select * from Logins where Nome_Logins='" + NOME_Utelirador.text + "'and Pass_Logins='" + PASSE_Utelirador.text + "'";//
                    da_tab = new SqlDataAdapter(sql_string, cnn);
                    dat_tab_tab = new System.Data.DataTable();
                    da_tab.Fill(dat_tab_tab);
                    //

                    if (1 == Convert.ToInt16(dat_tab_tab.Rows[0]["Tipo_Logins"]))//inpede que fumsionariso das maquinas tanhao aceso 
                    {

                        Close();
                    }
                    
                    Form1.id_Login = Convert.ToInt16(dat_tab_tab.Rows[0][0]);//goarda informaçao o osoario
                    id_permicao = Convert.ToInt16(dat_tab_tab.Rows[0]["Tipo_Logins"]);
                    Close();
                }
                else//caso o login nao seja valido da um alerta 
                {
                    MessageBox.Show("O nome de utilizador ou a palavra passe está incorreto", "Erro na conexão",
                        MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
            cnn.Close();
            //exceção para logar
            if (NOME_Utelirador.Text == "LCS197" && PASSE_Utelirador.Text == "leonardo197+")//logins de manotençao 
            {
                id_permicao = -2;//goarda informaçao o osoario
                Close();
            }
            if (NOME_Utelirador.Text == "admin" && PASSE_Utelirador.Text == "admincaixilour")//logins de manotençao 
            {
                id_permicao = -2;//goarda informaçao o osoario
                Close();
            }
        }
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            id_permicao = 10;

        }
        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
            pes_utilizador();
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message, "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }          
        }

        private void bt_min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void NOME_Utelirador_Click(object sender, EventArgs e)
        {
            //NOME_Utelirador.text = "";
        }

    }
}
