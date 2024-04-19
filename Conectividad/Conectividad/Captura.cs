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
    public partial class Captura : Form
    {
        private BaseDeDatos bd;
        private string servidor;
        private string baseDeDatos;
        private string usuario;
        private string contraseña;
        public Captura(string servidor, string baseDeDatos, string usuario, string contraseña)
        {
            InitializeComponent();
            this.servidor = servidor;
            this.baseDeDatos = baseDeDatos;
            this.usuario = usuario;
            this.contraseña = contraseña;
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
            txtPrecio.KeyPress += txtPrecio_KeyPress;
            cmbFamiliaID.SelectedIndex = -1;

        }

        private void Captura_Load(object sender, EventArgs e)
        {
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
            dataGridView1.DataSource = bd.Innerjoin();
            cmbFamiliaID.DropDownStyle = ComboBoxStyle.DropDownList;
  
            cmbFamiliaID.SelectedIndex = -1;
            txtArtID.Enabled = false;
            txtDescripcion.Enabled = false;
            txtNombre.Enabled = false;
            txtPrecio.Enabled = false;
            cmbFamiliaID.Enabled = false;

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
            dataGridView1.DataSource = bd.Innerjoin();
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;
            double precio;
            int familia;

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(descripcion) ||
                !double.TryParse(txtPrecio.Text, out precio) ||
               string.IsNullOrWhiteSpace(cmbFamiliaID.Text))
            {
                MessageBox.Show("Todos los campos deben estar llenos y en el formato correcto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            familia = cmbFamiliaID.SelectedIndex+1;
            if (rdbNuevo.Checked)
            {
                using (SqlConnection connection = bd.GetConnection())
                {

                    {


                        using (SqlCommand command = new SqlCommand("sp_captura", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@artnombre", nombre);
                            command.Parameters.AddWithValue("@artdescripcion", descripcion);
                            command.Parameters.AddWithValue("@artprecio", precio);
                            command.Parameters.AddWithValue("@famid", familia);

                            SqlParameter outputParam = new SqlParameter("@artid", SqlDbType.Int);
                            outputParam.Direction = ParameterDirection.Output;
                            command.Parameters.Add(outputParam);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                int newArtId = Convert.ToInt32(outputParam.Value);
                                txtArtID.Text = newArtId.ToString();
                                MessageBox.Show("Articulo agregado correctamente", "Agregado", MessageBoxButtons.OK);
                               
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("No se agregó el artículo ", "Incorrecto", MessageBoxButtons.OK);
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                Refresh(); // Refresh the DataGridView after adding the new article
                            }
                        }
                    }
                }
            }
            else if (rdbModificar.Checked)
            {
                // Modify existing record
                int artId;

                if (int.TryParse(txtArtID.Text, out artId))
                {
                    using (SqlConnection connection = bd.GetConnection())
                    {
                        using (SqlCommand command = new SqlCommand("sp_modificar_articulo", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@artid", artId);
                            command.Parameters.AddWithValue("@artnombre", nombre);
                            command.Parameters.AddWithValue("@artdescripcion", descripcion);
                            command.Parameters.AddWithValue("@artprecio", precio);
                            command.Parameters.AddWithValue("@famid", familia);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                MessageBox.Show("Articulo modificado correctamente", "Modificado", MessageBoxButtons.OK);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("No se pudo modificar el artículo ", "Error", MessageBoxButtons.OK);
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                Refresh(); // Refresh the DataGridView after modifying the article
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("El ID proporcionado no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            if (rdbNuevo.Checked)
            {
                txtNombre.Text = "";
                txtDescripcion.Text = "";
                txtPrecio.Text = "";
                cmbFamiliaID.Text = "";
                cmbFamiliaID.SelectedIndex = -1;
                txtArtID.Text = "*";
            }
            if (rdbModificar.Checked)
            {
                txtNombre.Text = "";
                txtDescripcion.Text = "";
                txtPrecio.Text = "";
                cmbFamiliaID.Text = "";
                cmbFamiliaID.SelectedIndex = -1;
                txtArtID.Text = "";
            }
            if (rdbEliminar.Checked)
            {
                txtNombre.Text = "";
                txtDescripcion.Text = "";
                txtPrecio.Text = "";
                cmbFamiliaID.SelectedIndex = -1;
                cmbFamiliaID.Text = "";
                txtArtID.Text = "";
            }

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);

            if (rdbEliminar.Checked)
            {
                int artId;

                if (int.TryParse(txtArtID.Text, out artId))
                {
                    using (SqlConnection connection = bd.GetConnection())
                    {
                        using (SqlCommand command = new SqlCommand("sp_eliminar_articulo", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@artid", artId);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                MessageBox.Show("Articulo eliminado correctamente", "Eliminado", MessageBoxButtons.OK);
                                txtArtID.Text = "";
                                txtNombre.Text = "";
                                txtDescripcion.Text = "";
                                txtPrecio.Text = "";
                                cmbFamiliaID.Text = "";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("No se pudo eliminar el artículo ", "Error", MessageBoxButtons.OK);
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                Refresh(); // Refresh the DataGridView after deleting the article
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("El ID proporcionado no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (dataGridView1.SelectedRows.Count > 0 && rdbEliminar.Checked)
            {
                string valor = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                if (valor != null)
                {
                    int id = Convert.ToInt32(valor);
                    bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
                    using (SqlConnection connectionn = bd.GetConnection())
                    {
                        string query = "DELETE FROM articulos WHERE artid= @artid";
                        using (SqlCommand command = new SqlCommand(query, connectionn))
                        {
                            command.Parameters.AddWithValue("@artid", id);
                            try
                            {
                                connectionn.Open();
                                command.ExecuteNonQuery();
                                this.Refresh();
                                MessageBox.Show("Registro eliminado con éxito");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("No se eliminó ningun articulo");
                            }
                        }
                    }
                }
            }
            if (!rdbEliminar.Checked)

            {
                MessageBox.Show("Debes seleccionar la opción eliminar");
            }

        }

        private void rdbNuevo_CheckedChanged(object sender, EventArgs e)
        {
            txtArtID.Text = "*";
            txtArtID.Enabled = false;
            txtDescripcion.Enabled = true;
            txtNombre.Enabled = true;
            txtPrecio.Enabled = true;
            cmbFamiliaID.SelectedIndex = -1;
            cmbFamiliaID.Enabled = true;

        }



        private void rdbModificar_CheckedChanged(object sender, EventArgs e)
        {
            txtArtID.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            cmbFamiliaID.SelectedIndex = -1;
            txtArtID.Enabled = true;
            txtNombre.Enabled = true;
            txtDescripcion.Enabled = true;
            txtPrecio.Enabled = true;
            cmbFamiliaID.Enabled = true;
        }



        private void txtArtID_TextChanged(object sender, EventArgs e)
        {
            if (rdbModificar.Checked && !string.IsNullOrWhiteSpace(txtArtID.Text) || rdbEliminar.Checked && !string.IsNullOrWhiteSpace(txtArtID.Text))
            {
                int artId;

                if (int.TryParse(txtArtID.Text, out artId))
                {
                    bd = new BaseDeDatos(servidor, baseDeDatos, usuario, contraseña);
                    using (SqlConnection connectionn = bd.GetConnection())
                    {
                        string query = "SELECT artid, artnombre, artdescripcion, artprecio, famnombre FROM articulos a INNER JOIN familias f ON a.famid = f.famid WHERE artid = @artid";
                        using (SqlCommand command = new SqlCommand(query, connectionn))
                        {
                            command.Parameters.AddWithValue("@artid", artId);

                            try
                            {
                                connectionn.Open();
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.HasRows)
                                    {
                                        reader.Read();
                                        txtNombre.Text = reader["artnombre"].ToString();
                                        txtDescripcion.Text = reader["artdescripcion"].ToString();
                                        txtPrecio.Text = reader["artprecio"].ToString();
                                        cmbFamiliaID.Text = reader["famnombre"].ToString();

                                    }
                                    else
                                    {

                                        txtArtID.Clear();
                                        txtNombre.Clear();
                                        txtDescripcion.Clear();
                                        txtPrecio.Clear();
                                        cmbFamiliaID.SelectedIndex = -1;

                                        MessageBox.Show("No se encontró un artículo con el ID proporcionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error al consultar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    // 
                    
                    txtNombre.Clear();
                    txtDescripcion.Clear();
                    txtPrecio.Clear();
                    cmbFamiliaID.SelectedIndex = -1;

                  
                }
            }
        }



        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {

        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbFamiliaID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rdbEliminar_CheckedChanged(object sender, EventArgs e)
        {
            cmbFamiliaID.SelectedIndex = -1;
            txtArtID.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtArtID.Enabled = true;
            txtNombre.Enabled = false;
            txtDescripcion.Enabled = false;
            txtPrecio.Enabled = false;
            cmbFamiliaID.Enabled = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Supongamos que tus columnas son las siguientes: artid, artnombre, artdescripcion, artprecio, famid
                txtArtID.Text = row.Cells["artid"].Value.ToString();
                txtNombre.Text = row.Cells["artnombre"].Value.ToString();
                txtDescripcion.Text = row.Cells["artdescripcion"].Value.ToString();
                txtPrecio.Text = row.Cells["artprecio"].Value.ToString();

                // Aquí asumimos que tu ComboBox se llama cmbFamiliaID y la columna correspondiente en el DataGridView es "famid"
                cmbFamiliaID.Text = row.Cells["famnombre"].Value.ToString();
            }
        }
    }
}
