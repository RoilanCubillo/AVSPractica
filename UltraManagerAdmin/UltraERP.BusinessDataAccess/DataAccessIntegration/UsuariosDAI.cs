using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class UsuariosDAI
    {
        //private readonly MasterDBDataContext _objContext = new MasterDBDataContext();



        //public int F_UsuarioAccesosInsertUpdate(string NOMBRE, string LOGIN, string CLAVE, short NIVEL_SEGURIDAD, int PRIVILEGIOS, string EMAIL, short ESTADO, string CEDULA_RAZON_SOCIAL, int TIPOGESTION)
        //{
        //    int resultado = 0;
        //    var data = _objContext.sp_F_USUARIOAccesosInsertUpdate(NOMBRE, LOGIN, CLAVE, NIVEL_SEGURIDAD, PRIVILEGIOS, EMAIL, ESTADO, CEDULA_RAZON_SOCIAL, TIPOGESTION);
        //    resultado = data;

        //    return resultado;
        //}


        //public int F_UsuarioInsertUpdate(string NOMBRE, string LOGIN, string CLAVE, short NIVEL_SEGURIDAD, int PRIVILEGIOS, string EMAIL, short ESTADO)
        //{
        //    int resultado = 0;
        //    var data = _objContext.sp_F_USUARIOInsert(NOMBRE, LOGIN, CLAVE, NIVEL_SEGURIDAD, PRIVILEGIOS, EMAIL, ESTADO);
        //    resultado = data;

        //    return resultado;
        //}

        //public List<UsuarioEntitie> GetUsuarios(string strLogin)
        //{
        //    try
        //    {
        //        List<UsuarioEntitie> usuarios = (from a in _objContext.sp_F_USUARIOSelectByLogin(strLogin)

        //                                         select new UsuarioEntitie()
        //                                         {

        //                                             strCedula = a.CEDULA_RAZON_SOCIAL,
        //                                             strNombre = a.NOMBRE,
        //                                             strCorreo = a.LOGIN,
        //                                             intEstado = a.ESTADO
        //                                         }).ToList();

        //        return usuarios;
        //    }
        //    catch (Exception)
        //    {
        //        return new List<UsuarioEntitie>();
        //    }
        //}


        //public List<UsuarioEntitie> GetUsuariosAccesosAll(string strCedula)
        //{
        //    try
        //    {
        //        List<UsuarioEntitie> usuarios = (from a in _objContext.sp_F_USUARIOS_ACCESOS_ALL(strCedula)

        //                                         select new UsuarioEntitie()
        //                                         {
        //                                             strNombre = a.NOMBRE,
        //                                             strCorreo = a.LOGIN,
        //                                             intEstado = a.ESTADO
        //                                         }).ToList();

        //        return usuarios;
        //    }
        //    catch (Exception)
        //    {
        //        return new List<UsuarioEntitie>();
        //    }
        //}


        //public UsuarioEntitie GetUsuarioAcceso(string strCedula, string strLogin)
        //{
        //    try
        //    {
        //        UsuarioEntitie usuarios = (from a in _objContext.sp_F_USUARIOSelect(strCedula, strLogin)

        //                                   select new UsuarioEntitie()
        //                                   {
        //                                       strNombre = a.NOMBRE,
        //                                       strCorreo = a.LOGIN,
        //                                       intEstado = a.ESTADO
        //                                   }).FirstOrDefault();

        //        return usuarios;
        //    }
        //    catch (Exception)
        //    {
        //        return new UsuarioEntitie();
        //    }
        //}

        //public Dictionary<string, string> GetClienteData(string cedula)
        //{
        //    var myList = _objContext.sp_IW_CLIENTE_DATA(cedula).ToList();
        //    if (myList.Count() > 0)
        //    {
        //        return toDictionatry(myList.First());
        //    }
        //    return null;
        //}

        //private Dictionary<string, string> toDictionatry(Object _object)
        //{
        //    return _object.GetType()
        //        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        //        .ToDictionary(prop => prop.Name, prop => prop.GetValue(_object, null).ToString());
        //}

    }
}
