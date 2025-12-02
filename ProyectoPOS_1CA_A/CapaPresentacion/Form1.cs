using ProyectoPOS_1CA_A.CapaEntidades;
using ProyectoPOS_1CA_A.CapaPresentacion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoPOS_1CA_A
{
    public partial class FrmMenuPrincipal : Form
    {
        public FrmMenuPrincipal()
        {
            InitializeComponent();
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            //creo una instancia del formulario FrmProductos
            FrmProductos frm = new FrmProductos();
            //muestro el formulario
            frm.ShowDialog();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            Clientes formClientes = new Clientes();
            formClientes.Show();
        }

        private void FrmMenuPrincipal_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = $"Usuario: {SesionActual.NombreUsuario} - Rol: {SesionActual.Rol}";

            /// Control básico por rol
//Con este codigo deshabilitamos un botón de prueba para el usuario cajero, por ejemplo que no pueda Registrar Cliente(ojo esto es solo prueba)
            switch (SesionActual.Rol)
            {
                case "Admin":
                    // todo habilitado
                    break;
                case "Cajero":
                    btnClientes.Enabled = false;
                    btnUsuarios.Enabled = false;
                    break;
                default:
                    btnClientes.Enabled = false;
                    btnUsuarios.Enabled = false;
                    break;

            }



        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmUsuario frm = new FrmUsuario();
            frm.ShowDialog();

        }

        private void cambiarClaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCambiarClave frm = new FrmCambiarClave();
            frm.ShowDialog();


        }
    }
}
