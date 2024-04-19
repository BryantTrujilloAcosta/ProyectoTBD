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


namespace Conectividad
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection;
        private BaseDeDatos bd;
        private string servidor;
        private string baseDeDatos;
        private string usuario;
        private string contraseña;

        public Form1()
        {
            InitializeComponent();
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
        }
        private void txtServidor_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBaseDeDatos_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContraseña_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnConectar_Click(object sender, EventArgs e)
        {

            servidor = txtServidor.Text;
            baseDeDatos = txtBaseDeDatos.Text;
            usuario = txtUsuario.Text;
            contraseña = txtContraseña.Text;

            // Validar que todos los campos estén llenos
            if (string.IsNullOrWhiteSpace(servidor))
            {
                MessageBox.Show("Ingrese el nombre del servidor.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(baseDeDatos))
            {
                MessageBox.Show("Ingrese el nombre de la base de datos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(usuario))
            {
                MessageBox.Show("Ingrese el nombre de usuario.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show("Ingrese la contraseña.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            BaseDeDatos bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);

            bool valor = bd.TestConnection();

            if (valor == true)
            {
                MessageBox.Show("Conexión exitosa");
                lblResultado.Text = "Conexión exitosa";
                lblResultado.ForeColor = System.Drawing.Color.Lime;
                // Habilitar los botones de captura y consulta
                btnCaptura.Visible = true;
                btnConsulta.Visible = true;
            }
            else
            {
                // Mostrar mensaje de error en caso de falla en la conexión
                MessageBox.Show("error de conexión");
                lblResultado.Text = "error de conexión";
                lblResultado.ForeColor = System.Drawing.Color.Red;
                btnCaptura.Visible = false;
                btnConsulta.Visible = false;
            }
            // Cerrar la conexión después de usarla
            if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        private void btnCaptura_Click(object sender, EventArgs e)
        {
            Captura captura = new Captura(servidor, baseDeDatos, usuario, contraseña);
            captura.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnConsulta_Click(object sender, EventArgs e)
        {
            consulta cs = new consulta(servidor, baseDeDatos, usuario, contraseña);
            cs.ShowDialog();
        }
    }
}
