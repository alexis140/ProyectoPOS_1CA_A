using ProyectoPOS_1CA_A.CapaDatos;
using ProyectoPOS_1CA_A.CapaEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoPOS_1CA_A.CapaPresentacion
{
    public partial class FrmProductos : Form
    {
        private readonly ProductoDAL bll = new ProductoDAL();
        private List<Producto> listaProductos = new List<Producto>();
        public FrmProductos()
        {
            InitializeComponent();
        }
        //Creacion de una lista estatica que simulara la DB
        private void DesabilitarBotones()
        {
            btnLimpiar.Enabled = false;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnNuevo.Enabled = true;
        }

        private void HabilitarBotones()
        {
            btnLimpiar.Enabled = true;
            btnEditar.Enabled = true;
            btnEliminar.Enabled = true;
            btnNuevo.Enabled = false;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void FrmProductos_Load(object sender, EventArgs e)
        {
            CargarProductos(string.Empty);
            DesabilitarBotones();
        }
            private void CargarProductos(string filtro)
        {
            // Obtenemos la lista desde DAL
            var tabla = ProductoDAL.Listar(); // ya lo creamos en Paso 2

            // Actualizar listaProductos con los datos actuales
            listaProductos = tabla.AsEnumerable()
                .Select(row => new Producto
                {
                    Id = row.Field<int>("Id"),
                    Nombre = row.Field<string>("Nombre"),
                    Precio = row.Field<decimal>("Precio"),
                    Stock = row.Field<int>("Stock"),
                    Estado = row.Field<bool>("Estado"),
                    Id_Categoria = row.Table.Columns.Contains("Id_Categoria") ? row.Field<int>("Id_Categoria") : 1
                })
                .ToList();

            // Filtrar si hay texto
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var dv = tabla.DefaultView;
                dv.RowFilter = $"Nombre LIKE '%{filtro}%'";
                dgvProductos.DataSource = dv;
            }
            else
            {
                dgvProductos.DataSource = tabla;
            }

            dgvProductos.Columns["Id"].Visible = false;
        }

        //asignar la lista como DataSOurce al datagridview
        private void RefrescarGrid()
        {
            CargarProductos(string.Empty);
        }
        //BOTON GUARDAR
        private void btnNuevo_Click(object sender, EventArgs e)
        {

            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("Debe ingresar un nombre.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
                {
                    MessageBox.Show("Precio inválido.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPrecio.Focus();
                    return;
                }
                if (!int.TryParse(txtStock.Text, out int stock))
                {
                    MessageBox.Show("Stock inválido.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStock.Focus();
                    return;
                }

                Producto p = new Producto
                {
                    Id = 0,
                    Nombre = txtNombre.Text.Trim(),
                    Precio = precio,
                    Stock = stock,
                    Estado = chkEstado.Checked,
                    Id_Categoria = 1 
                };

                // AGREGAR:
                int nuevoId = bll.Insertar(p); // Llama al método Insertar de la DAL

                MessageBox.Show("Producto agregado correctamente. ID: " + nuevoId, "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefrescarGrid(); // Llama a CargarProductos(string.Empty) para refrescar la DB
                LimpiarCampos();
                DesabilitarBotones();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LimpiarCampos();
        }
        //Metodo para limpiar los controles
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            chkEstado.Checked = true;
            txtNombre.Focus();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
        //Evento click del datagridview 
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProductos.CurrentRow == null) return;
            //Obtener el producto seleccionado
            txtId.Text = dgvProductos.CurrentRow.Cells["Id"].Value.ToString();
            txtNombre.Text = dgvProductos.CurrentRow.Cells["Nombre"].Value.ToString();
            txtPrecio.Text = dgvProductos.CurrentRow.Cells["Precio"].Value.ToString();
            txtStock.Text = dgvProductos.CurrentRow.Cells["Stock"].Value.ToString();
            chkEstado.Checked = Convert.ToBoolean(dgvProductos.CurrentRow.Cells["Estado"].Value);
            HabilitarBotones();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Seleccione un producto válido para eliminar.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar el producto seleccionado?", "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {

                    bll.Eliminar(id);

                    MessageBox.Show("Producto eliminado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    RefrescarGrid();
                    LimpiarCampos();
                    DesabilitarBotones();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar eliminar:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //Evento para editar un producto
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id)) return;

            Producto p = new Producto
            {
                Id = id, // Importante: Se envía el ID para la actualización
                Nombre = txtNombre.Text.Trim(),
                // Debes parsear los textos, ya validaste que son correctos
                Precio = decimal.Parse(txtPrecio.Text),
                Stock = int.Parse(txtStock.Text),
                Estado = chkEstado.Checked,
                Id_Categoria = 1 // Asumido, si no tienes ComboBox
            };

            try
            {
       
                bool actualizado = bll.Actualizar(p);

                if (actualizado)
                {
                    MessageBox.Show("Producto actualizado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefrescarGrid();
                    LimpiarCampos();
                    DesabilitarBotones();
                }
                else
                {
                    MessageBox.Show("No se encontró el producto para actualizar.", "Advertencia",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefrescarGrid();//refrescar el datagridview
            LimpiarCampos();//limpiar los controles
            DesabilitarBotones();
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            HabilitarBotones();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
