using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class ProductosDAI

    {
        //private readonly MasterDBDataContext _objContext = new MasterDBDataContext();

        //public List<ProductoEntitie> GetAllProductos(int intIdCliente)
        //{
        //    try
        //    {
        //        List<ProductoEntitie> producto = (from a in _objContext.sp_IW_PRODUCTOSelectAll(intIdCliente)
        //                                          orderby a.NOMBRE
        //                                          select new ProductoEntitie()
        //                                          {
        //                                              intIdCliente = intIdCliente,
        //                                              strCodigo = a.CODIGO,
        //                                              intEstado = a.ESTADO,
        //                                              decPrecioCol = a.PRECIO_COL,
        //                                              decPrecioDol = a.PRECIO_DOL,
        //                                              strNombre = a.NOMBRE,
        //                                              strNombreExt = a.NOMBRE_EXTENDIDO,
        //                                              strUnidadMedida = a.UNIDAD_MEDIDA,
        //                                              strCodCabys = a.CABYS,
        //                                              strDescripCabys = a.DESCRIPCION_DEL_BIEN_O_SERVICIO
        //                                          }
        //            ).ToList();
        //        return producto;
        //    }
        //    catch (Exception)
        //    {
        //        return new List<ProductoEntitie>();
        //    }
        //}

        
        //public ProductoEntitie GetProductoByCodigo(int IdCliente, string Codigo)
        //{
        //    try
        //    {
        //        ProductoEntitie producto = (from a in _objContext.sp_IW_PRODUCTOSelect(IdCliente, Codigo)
        //                                    orderby a.NOMBRE
        //                                    select new ProductoEntitie()
        //                                    {
        //                                        decPrecioCol = a.PRECIO_COL,
        //                                        intEstado = a.ESTADO,
        //                                        decPrecioDol = a.PRECIO_DOL,
        //                                        strCodigo = Codigo,
        //                                        strNombre = a.NOMBRE,
        //                                        strNombreExt = a.NOMBRE_EXTENDIDO,
        //                                        strUnidadMedida = a.UNIDAD_MEDIDA,
        //                                        strCodImpuesto = a.COD_IMPUESTO,//*
        //                                        strCodTarifa = a.COD_IMPUESTO_BASE,//*
        //                                        strSimboloUM = a.SIMBOLO,
        //                                        TarifaIVA = a.VALORIVA,
        //                                        strCodCabys = a.CABYS
        //                                    }).FirstOrDefault();
        //        return producto;
        //    }
        //    catch (Exception)
        //    {
        //        return new ProductoEntitie();
        //    }
        //}

        //public bool UpdateProducto(int IdCliente, string Codigo, string Nombre, string NombreExt, string UnidadMedida, decimal PrecioCol
        //, decimal PrecioDol, short Estado, string CodImpuesto, string CodImpuestoBase, string CodCabys)
        //{
        //    try
        //    {
        //        _objContext.sp_IW_PRODUCTOUpdate(IdCliente, Codigo, Nombre, NombreExt, UnidadMedida, PrecioCol, PrecioDol, Estado, CodImpuesto, CodImpuestoBase, CodCabys);
        //        _objContext.SubmitChanges();


        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }

        //    return true;
        //}

        //public bool InsertProducto(int IdCliente, string Codigo, string Nombre, string NombreExt, string UnidadMedida, decimal PrecioCol
        //    , decimal PrecioDol, short Estado, string CodImpuesto, string CodImpuestoBase, string CodCabys)
        //{
        //    try
        //    {
        //        _objContext.sp_IW_PRODUCTOInsert(IdCliente, Codigo, Nombre, NombreExt, UnidadMedida, PrecioCol, PrecioDol, Estado, CodImpuesto, CodImpuestoBase, CodCabys);
        //        _objContext.SubmitChanges();


        //    }
        //    catch (Exception ex)
        //    {
        //        //throw ex;
        //    }
        //    return true;
        //}

        //public bool UpdateCodCABYS(int IdCliente, string Codigo, string CodCabys)
        //{
        //    try
        //    {
        //        _objContext.sp_IW_PRODUCTOUpdateCABYS(IdCliente, Codigo, CodCabys);
        //        _objContext.SubmitChanges();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return true;
        //}

       


        //public int GetCountProductos(int IdCliente)
        //{
        //    try
        //    {
        //        int cliente = (from a in _objContext.sp_IW_PRODUCTOSCount(IdCliente)

        //                       select (int)a.CANTIDAD
        //            ).FirstOrDefault();
        //        return cliente;
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //    }
        //}


    }
}
