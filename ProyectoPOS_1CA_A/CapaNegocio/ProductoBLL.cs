using ProyectoPOS_1CA_A.CapaDatos;
using ProyectoPOS_1CA_A.CapaEntidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPOS_1CA_A.CapaNegocio
{
    public class ProductoBLL
        {
            ProductoDAL dal = new ProductoDAL();

            // LISTAR
            public DataTable Listar()
            {
                return ProductoDAL.Listar();
            }

            // GUARDAR (Insertar/Actualizar)
            public int Guardar(Producto p)
            {
                // Si el ID es 0 → INSERTAR
                if (p.Id == 0)
                {
                    ValidarProducto(p, esEdicion: false);
                    return dal.Insertar(p);
                }
                else // Si el ID es distinto de 0 → ACTUALIZAR
                {
                    ValidarProducto(p, esEdicion: true);

                    bool actualizado = dal.Actualizar(p);

                    if (!actualizado)
                        throw new Exception("No se pudo actualizar el producto. Es posible que el ID no exista.");

                    return p.Id; // Devuelve el mismo ID
                }
            }

            // ELIMINAR
            public bool Eliminar(int id)
            {
                // Validación de FK ANTES de intentar eliminar (Ej: está en detalles de venta)
                if (dal.TieneDetallesDeVentaAsociados(id))
                    throw new Exception("No se puede eliminar este producto porque está asociado a ventas realizadas.");

                // Si no tiene dependencias, eliminar
                return dal.Eliminar(id);
            }

            private void ValidarProducto(Producto p, bool esEdicion = false)
            {
                // 1. Campos obligatorios
                if (string.IsNullOrWhiteSpace(p.Nombre))
                    throw new Exception("El nombre del producto es obligatorio.");

                if (p.Id_Categoria <= 0)
                    throw new Exception("El producto debe estar asociado a una categoría válida.");

                // 2. Valores numéricos
                if (p.Precio <= 0)
                    throw new Exception("El precio del producto debe ser un valor positivo.");

                if (p.Stock < 0)
                    throw new Exception("El stock del producto no puede ser negativo.");

                // 3. Longitud
                if (p.Nombre.Length > 100)
                    throw new Exception("El nombre no debe superar los 100 caracteres.");
                // Nota: Se asume que la descripción no existe en Producto o no tiene validación de longitud.

                // 4. Validación de duplicados (asumiendo que se valida el nombre del producto)
                if (!esEdicion) // Caso: INSERTAR
                {
                    if (dal.ExisteNombre(p.Nombre))
                        throw new Exception("Ya existe un producto con ese nombre.");
                }
                else // Caso: EDITAR
                {
                    if (dal.ExisteNombreEnOtroProducto(p.Nombre, p.Id))
                        throw new Exception("Ya existe otro producto con ese nombre.");
                }
            }
        }
    }

