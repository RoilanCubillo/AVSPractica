using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class MonitoreoDatosController : Controller
    {
        public ActionResult Inicio()
        {
            return View(BuildModel());
        }

        private static MonitoreoDatosViewModel BuildModel()
        {
            var tiendas = new List<MonitoreoTiendaViewModel>
            {
                CreateStore("1", "SJ-CTR", "San Jose Centro"),
                CreateStore("2", "HER-PLZ", "Heredia Plaza"),
                CreateStore("3", "CRT-EST", "Cartago Este"),
                CreateStore("4", "ESC-GAM", "Escazu"),
                CreateStore("5", "LBR-OCC", "Liberia")
            };

            return new MonitoreoDatosViewModel
            {
                Tiendas = tiendas,
                DocumentosMH = new List<MonitoreoDocumentoViewModel>
                {
                    CreateDocument(1, "MH", "1", "San Jose Centro", "00100001010000000531", "50613052600310123456700100001010000000531123456789", "Factura electronica", "Distribuidora La Sabana", new DateTime(2026, 5, 13, 9, 15, 0), new DateTime(2026, 5, 13, 9, 16, 0), 1854500m, 1, "Aceptado por Ministerio de Hacienda."),
                    CreateDocument(2, "MH", "4", "Escazu", "00100001010000000532", "50613052600310123456700100001010000000532123456789", "Nota credito", "Cafe Costa Rica S.A.", new DateTime(2026, 5, 13, 10, 40, 0), new DateTime(2026, 5, 13, 10, 42, 0), 124980m, 1, "Documento aceptado."),
                    CreateDocument(3, "MH", "2", "Heredia Plaza", "00100001010000000533", "50613052600310123456700100001010000000533123456789", "Factura electronica", "Central de Carnes CR", new DateTime(2026, 5, 12, 15, 5, 0), new DateTime(2026, 5, 12, 15, 9, 0), 879200m, 2, "Rechazado por detalle de impuesto."),
                    CreateDocument(4, "MH", "3", "Cartago Este", "00100001010000000534", "50613052600310123456700100001010000000534123456789", "Tiquete electronico", "Cliente mostrador", new DateTime(2026, 5, 12, 18, 22, 0), null, 35460m, 3, "Pendiente de respuesta."),
                    CreateDocument(5, "MH", "5", "Liberia", "00100001010000000535", "50613052600310123456700100001010000000535123456789", "Factura electronica", "Hotel Guanacaste Verde", new DateTime(2026, 5, 11, 11, 8, 0), new DateTime(2026, 5, 11, 11, 10, 0), 642300m, 1, "Aceptado por Ministerio de Hacienda."),
                    CreateDocument(6, "MH", "1", "San Jose Centro", "00100001010000000536", "50613052600310123456700100001010000000536123456789", "Nota debito", "Proveedor Nacional", new DateTime(2026, 5, 10, 8, 55, 0), new DateTime(2026, 5, 10, 9, 1, 0), 74500m, 2, "Rechazado por clave duplicada.")
                },
                DocumentosERP = new List<MonitoreoDocumentoViewModel>
                {
                    CreateDocument(101, "ERP", "1", "San Jose Centro", "ERP-0009821", "ERP-SJ-0009821", "Factura electronica", "Distribuidora La Sabana", new DateTime(2026, 5, 13, 9, 18, 0), new DateTime(2026, 5, 13, 9, 19, 0), 1854500m, 1, "Sincronizado contra ERP."),
                    CreateDocument(102, "ERP", "4", "Escazu", "ERP-0009822", "ERP-ESC-0009822", "Nota credito", "Cafe Costa Rica S.A.", new DateTime(2026, 5, 13, 10, 45, 0), new DateTime(2026, 5, 13, 10, 45, 0), 124980m, 1, "Sincronizado contra ERP."),
                    CreateDocument(103, "ERP", "2", "Heredia Plaza", "ERP-0009823", "ERP-HER-0009823", "Factura electronica", "Central de Carnes CR", new DateTime(2026, 5, 12, 15, 12, 0), null, 879200m, 2, "No se encontro documento aceptado en Hacienda."),
                    CreateDocument(104, "ERP", "3", "Cartago Este", "ERP-0009824", "ERP-CRT-0009824", "Tiquete electronico", "Cliente mostrador", new DateTime(2026, 5, 12, 18, 25, 0), null, 35460m, 3, "Pendiente de sincronizacion."),
                    CreateDocument(105, "ERP", "5", "Liberia", "ERP-0009825", "ERP-LBR-0009825", "Factura electronica", "Hotel Guanacaste Verde", new DateTime(2026, 5, 11, 11, 15, 0), new DateTime(2026, 5, 11, 11, 16, 0), 642300m, 1, "Sincronizado contra ERP.")
                },
                AsientosERP = new List<MonitoreoAsientoViewModel>
                {
                    CreateAccounting(301, "1", "San Jose Centro", "AS-2026-0513-001", "ERP-0009821", "Venta diaria", new DateTime(2026, 5, 13), new DateTime(2026, 5, 13, 23, 10, 0), 1854500m, 1854500m, 1, "Asiento enviado correctamente."),
                    CreateAccounting(302, "4", "Escazu", "AS-2026-0513-002", "ERP-0009822", "Nota credito", new DateTime(2026, 5, 13), new DateTime(2026, 5, 13, 23, 12, 0), 124980m, 124980m, 1, "Asiento enviado correctamente."),
                    CreateAccounting(303, "2", "Heredia Plaza", "AS-2026-0512-004", "ERP-0009823", "Venta diaria", new DateTime(2026, 5, 12), null, 879200m, 879200m, 2, "Diferencia entre debito y credito por impuesto."),
                    CreateAccounting(304, "3", "Cartago Este", "AS-2026-0512-005", "ERP-0009824", "Cierre de caja", new DateTime(2026, 5, 12), null, 35460m, 35460m, 0, "Pendiente de envio."),
                    CreateAccounting(305, "5", "Liberia", "AS-2026-0511-002", "ERP-0009825", "Venta diaria", new DateTime(2026, 5, 11), new DateTime(2026, 5, 11, 23, 5, 0), 642300m, 642300m, 1, "Asiento enviado correctamente."),
                    CreateAccounting(306, "1", "San Jose Centro", "AS-2026-0510-001", "ERP-0009818", "Ajuste contable", new DateTime(2026, 5, 10), null, 74500m, 74200m, 2, "Diferencia de 300 colones.")
                }
            };
        }

        private static MonitoreoTiendaViewModel CreateStore(string id, string code, string name)
        {
            return new MonitoreoTiendaViewModel { ID = id, Codigo = code, Nombre = name };
        }

        private static MonitoreoDocumentoViewModel CreateDocument(int id, string origen, string tiendaID, string tienda, string consecutivo, string clave, string tipo, string cliente, DateTime fecha, DateTime? fechaSync, decimal total, int estadoID, string mensaje)
        {
            return new MonitoreoDocumentoViewModel
            {
                ID = id,
                Origen = origen,
                TiendaID = tiendaID,
                Tienda = tienda,
                Consecutivo = consecutivo,
                Clave = clave,
                ComprobanteTipo = tipo,
                Cliente = cliente,
                Fecha = fecha,
                FechaSincronizacion = fechaSync,
                Total = total,
                EstadoID = estadoID,
                Mensaje = mensaje
            };
        }

        private static MonitoreoAsientoViewModel CreateAccounting(int id, string tiendaID, string tienda, string numero, string referencia, string tipo, DateTime fecha, DateTime? fechaSync, decimal debito, decimal credito, int estadoID, string mensaje)
        {
            return new MonitoreoAsientoViewModel
            {
                ID = id,
                TiendaID = tiendaID,
                Tienda = tienda,
                NumeroAsiento = numero,
                Referencia = referencia,
                Tipo = tipo,
                FechaAsiento = fecha,
                FechaSincronizacion = fechaSync,
                Debito = debito,
                Credito = credito,
                EstadoID = estadoID,
                Mensaje = mensaje
            };
        }
    }
}
