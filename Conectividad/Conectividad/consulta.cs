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
    public partial class consulta : Form
    {
        private BaseDeDatos bd;
        private string servidor;
        private string baseDeDatos;
        private string usuario;
        private string contraseña;
        public consulta(string servidor, string baseDeDatos, string usuario, string contraseña)
        {
            InitializeComponent();
            this.servidor = servidor;
            this.baseDeDatos = baseDeDatos;
            this.usuario = usuario;
            this.contraseña = contraseña;
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
            txtPrecio.KeyPress += txtPrecio_KeyPress;
          

        }


        private void consulta_Load(object sender, EventArgs e)
        {
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
            dataGridViewConsulta.DataSource = bd.Innerjoin(); ;
            cmbFamiliaID.DropDownStyle = ComboBoxStyle.DropDownList;
          
            using (SqlConnection connection = bd.GetConnection())
            {
                string query = "SELECT famnombre from familias";
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Establecer el origen de datos y los campos a mostrar en el ComboBox
                    cmbFamiliaID.DataSource = dt;
                    cmbFamiliaID.DisplayMember = "famnombre";
                    cmbFamiliaID.SelectedIndex = -1;
                }
                catch (Exception ex)
                {

                }
            }
        
       


        }
        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verificar si la tecla presionada no es un número ni una tecla de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
            {
                // Mostrar un mensaje de advertencia
                MessageBox.Show("Solo se permiten números", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Cancelar la entrada
                e.Handled = true;
            }
        }
        private void Refresh()
        {
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
            dataGridViewConsulta.DataSource = bd.Innerjoin();
            txtArtID.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
            cmbFamiliaID.Text = "";
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);

            string filtroArtID = txtArtID.Text;
            string filtroArtnombre = txtNombre.Text;
            string filtroPrecio = txtPrecio.Text;
            string filtroDescripcion = txtDescripcion.Text;
            string filtroFamiliaID = cmbFamiliaID.Text;

            string query = "SELECT artid, artnombre, artdescripcion, artprecio, famnombre FROM articulos a inner join familias f on f.famid = a.famid WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(filtroArtID))
            {
                query += " AND artid = @artid";
            }

            if (!string.IsNullOrWhiteSpace(filtroArtnombre))
            {
                query += " AND artnombre LIKE '%' + @artnombre + '%'";
            }

            if (!string.IsNullOrWhiteSpace(filtroPrecio))
            {
                query += " AND artprecio LIKE '%' + @artprecio + '%'";
            }

            if (!string.IsNullOrWhiteSpace(filtroDescripcion))
            {
                query += " AND artdescripcion LIKE '%' + @artdescripcion + '%'";
            }

            if (!string.IsNullOrWhiteSpace(filtroFamiliaID))
            {
                query += " AND famnombre LIKE '%' + @familia + '%'";
            }

            using (SqlConnection connection = bd.GetConnection())
            {
                try
                {
                    connection.Open();
                    DataTable dataTable = new DataTable();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(filtroArtID))
                        {
                            command.Parameters.AddWithValue("@artid", filtroArtID);
                        }

                        if (!string.IsNullOrWhiteSpace(filtroArtnombre))
                        {
                            command.Parameters.AddWithValue("@artnombre", filtroArtnombre);
                        }

                        if (!string.IsNullOrWhiteSpace(filtroPrecio))
                        {
                            command.Parameters.AddWithValue("@artprecio", filtroPrecio);
                        }

                        if (!string.IsNullOrWhiteSpace(filtroDescripcion))
                        {
                            command.Parameters.AddWithValue("@artdescripcion", filtroDescripcion);
                        }

                        if (!string.IsNullOrWhiteSpace(filtroFamiliaID))
                        {
                            command.Parameters.AddWithValue("@familia", filtroFamiliaID);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }

                    dataGridViewConsulta.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontraron resultados para la búsqueda.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al ejecutar la búsqueda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }





        private void dataGridViewConsulta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtArtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbFamiliaID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
