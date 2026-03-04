namespace BoticaPOS.Domain.Enums;

public enum TipoComprobante { Ticket=1, Boleta=2, Factura=3 }
public enum EstadoComprobante { Emitido=1, PendienteEnvio=2, Enviado=3, Rechazado=4, Anulado=5 }
public enum TipoMovimientoInventario { CompraIn=1, VentaOut=2, AjusteIn=3, AjusteOut=4, DevolucionIn=5, AnulacionIn=6 }
public enum MetodoPago { Efectivo=1, Tarjeta=2, Yape=3 }
public enum RolSistema { Admin=1, Cajero=2, Almacen=3 }
