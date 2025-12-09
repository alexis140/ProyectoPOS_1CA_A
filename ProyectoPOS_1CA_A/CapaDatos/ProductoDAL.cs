using ProyectoPOS_1CA_A.CapaEntidades;
using ProyectoPOS_1CA_A.CapaNegocio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuestPDF.Helpers.Colors;

namespace ProyectoPOS_1CA_A.CapaDatos
{
    public  class ProductoDAL
    {
            // --------------------------------------------------------------------------------

            // Método INSERTAR
            // Retorna el ID del nuevo producto insertado
            public int Insertar(Producto p)
            {
                int idGenerado = 0;
                string query = @"
                INSERT INTO Producto (Nombre, Precio, Stock, Estado, IdCategoria) 
                VALUES (@Nombre, @Precio, @Stock, @Estado, @IdCategoria); 
                SELECT SCOPE_IDENTITY();";

                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                        cmd.Parameters.AddWithValue("@Precio", p.Precio);
                        cmd.Parameters.AddWithValue("@Stock", p.Stock);
                        cmd.Parameters.AddWithValue("@Estado", p.Estado);
                        cmd.Parameters.AddWithValue("@IdCategoria", p.Id_Categoria);

                        con.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            idGenerado = Convert.ToInt32(result);
                        }
                    }
                }
                return idGenerado;
            }

            // Método ACTUALIZAR
            // Retorna true si se actualizó una fila
            public bool Actualizar(Producto p)
            {
                string query = @"
                UPDATE Producto 
                SET Nombre = @Nombre, Precio = @Precio, Stock = @Stock, Estado = @Estado, IdCategoria = @IdCategoria
                WHERE Id = @Id";

                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                        cmd.Parameters.AddWithValue("@Precio", p.Precio);
                        cmd.Parameters.AddWithValue("@Stock", p.Stock);
                        cmd.Parameters.AddWithValue("@Estado", p.Estado);
                        cmd.Parameters.AddWithValue("@IdCategoria", p.Id_Categoria);
                        cmd.Parameters.AddWithValue("@Id", p.Id);

                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }

            // Método ELIMINAR (Desactivación lógica)
            // Se asume que la eliminación es un cambio de estado a 0 (false)
            public bool Eliminar(int id)
            {
                string query = "UPDATE Producto SET Estado = 0 WHERE Id = @Id";

                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        con.Open();
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }


            // Método BUSCAR
            public DataTable Buscar(string filtro)
            {
                DataTable tabla = new DataTable();
                string query = @"
                SELECT p.Id, p.Nombre, p.Precio, p.Stock, p.Estado, c.Nombre AS Categoria
                FROM Producto p
                INNER JOIN Categoria c ON p.Id_Categoria = c.Id
                WHERE p.Estado = 1 AND (p.Nombre LIKE @Filtro OR c.Nombre LIKE @Filtro);";

                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Filtro", "%" + filtro + "%");
                        con.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            tabla.Load(dr);
                        }
                    }
                }
                return tabla;
            }

            // Comprueba si ya existe un producto con el mismo nombre (para insertar)
            public bool ExisteNombre(string nombre)
            {
                string query = "SELECT COUNT(*) FROM Producto WHERE Nombre = @Nombre AND Estado = 1";

                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }

            // Comprueba si ya existe un producto con el mismo nombre PERO con un ID diferente (para editar)
            public bool ExisteNombreEnOtroProducto(string nombre, int idActual)
            {
                string query = "SELECT COUNT(*) FROM Producto WHERE Nombre = @Nombre AND Id <> @IdActual AND Estado = 1";

                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@IdActual", idActual);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }

            // Comprueba si el producto está asociado a detalles de venta (o cualquier otra tabla FK que impida el borrado)
            public bool TieneDetallesDeVentaAsociados(int idProducto)
            {
                // Se asume que existe una tabla 'DetalleVenta' con una FK 'Id_Producto'
                string query = "SELECT COUNT(*) FROM DetalleVenta WHERE Id_Producto = @IdProducto";

                using (SqlConnection con = new SqlConnection(Conexion.Cadena))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            public static int ObtenerStock(int idProducto)
        {
            int stock = 0;

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                string sql = "SELECT Stock FROM Producto WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", idProducto);

                    con.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                        stock = Convert.ToInt32(result);
                }
            }

            return stock;
        }
        public static DataTable Listar()
        {
            DataTable tabla = new DataTable();

            using (SqlConnection con = new SqlConnection(Conexion.Cadena))
            {
                string sql = @"
                    SELECT p.Id, p.Nombre, p.Precio, p.Stock, p.Estado, c.Nombre AS Categoria
                    FROM Producto p
                    INNER JOIN Categoria c ON p.IdCategoria = c.Id
                    WHERE p.Estado = 1;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        tabla.Load(dr);
                    }
                }
            }

            return tabla;
        }
    }


}

