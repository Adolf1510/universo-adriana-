IF DB_ID('BoticaPOS') IS NULL
BEGIN
    CREATE DATABASE BoticaPOS;
END
GO
USE BoticaPOS;
GO

CREATE TABLE Role (
    IdRole INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Usuario (
    IdUsuario INT IDENTITY PRIMARY KEY,
    IdRole INT NOT NULL,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(300) NULL,
    RequirePasswordChange BIT NOT NULL CONSTRAINT DF_Usuario_RequirePasswordChange DEFAULT(1),
    Estado BIT NOT NULL CONSTRAINT DF_Usuario_Estado DEFAULT(1),
    FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Usuario_Fecha DEFAULT(SYSUTCDATETIME()),
    CONSTRAINT FK_Usuario_Role FOREIGN KEY (IdRole) REFERENCES Role(IdRole)
);

CREATE TABLE BoticaConfig (
    IdBoticaConfig INT IDENTITY PRIMARY KEY,
    NombreBotica NVARCHAR(200) NOT NULL,
    RUC NVARCHAR(11) NOT NULL,
    Direccion NVARCHAR(300) NOT NULL,
    Telefono NVARCHAR(30) NULL,
    LogoPath NVARCHAR(300) NULL,
    SerieTicket NVARCHAR(4) NOT NULL,
    SerieBoleta NVARCHAR(4) NOT NULL,
    SerieFactura NVARCHAR(4) NOT NULL,
    ExpirationAlertDays INT NOT NULL CONSTRAINT DF_BoticaConfig_ExpirationAlertDays DEFAULT(30)
);

CREATE TABLE Medicamento (
    IdMedicamento INT IDENTITY PRIMARY KEY,
    NombreComercial NVARCHAR(200) NOT NULL,
    PrincipioActivo NVARCHAR(150) NULL,
    Laboratorio NVARCHAR(120) NULL,
    Presentacion NVARCHAR(120) NULL,
    Concentracion NVARCHAR(120) NULL,
    CodigoBarras NVARCHAR(100) NULL,
    PrecioVenta DECIMAL(18,2) NOT NULL,
    CostoCompra DECIMAL(18,2) NULL,
    RequiereReceta BIT NOT NULL,
    StockMinimo INT NOT NULL CONSTRAINT DF_Medicamento_StockMinimo DEFAULT(0),
    Estado BIT NOT NULL CONSTRAINT DF_Medicamento_Estado DEFAULT(1)
);

CREATE TABLE Lote (
    IdLote INT IDENTITY PRIMARY KEY,
    IdMedicamento INT NOT NULL,
    NumeroLote NVARCHAR(60) NOT NULL,
    FechaVencimiento DATE NOT NULL,
    CantidadDisponible INT NOT NULL,
    FechaIngreso DATETIME2 NOT NULL CONSTRAINT DF_Lote_FechaIngreso DEFAULT(SYSUTCDATETIME()),
    CONSTRAINT FK_Lote_Medicamento FOREIGN KEY (IdMedicamento) REFERENCES Medicamento(IdMedicamento)
);

CREATE TABLE Proveedor (
    IdProveedor INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    RUC NVARCHAR(11) NULL,
    Telefono NVARCHAR(50) NULL,
    Direccion NVARCHAR(250) NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Proveedor_Estado DEFAULT(1)
);

CREATE TABLE Compra (
    IdCompra INT IDENTITY PRIMARY KEY,
    IdProveedor INT NOT NULL,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_Compra_Fecha DEFAULT(SYSUTCDATETIME()),
    Total DECIMAL(18,2) NOT NULL,
    Estado NVARCHAR(20) NOT NULL,
    CONSTRAINT FK_Compra_Proveedor FOREIGN KEY (IdProveedor) REFERENCES Proveedor(IdProveedor)
);

CREATE TABLE CompraDetalle (
    IdCompraDetalle INT IDENTITY PRIMARY KEY,
    IdCompra INT NOT NULL,
    IdMedicamento INT NOT NULL,
    NumeroLote NVARCHAR(60) NOT NULL,
    FechaVencimiento DATE NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    SubTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_CompraDetalle_Compra FOREIGN KEY (IdCompra) REFERENCES Compra(IdCompra),
    CONSTRAINT FK_CompraDetalle_Medicamento FOREIGN KEY (IdMedicamento) REFERENCES Medicamento(IdMedicamento)
);

CREATE TABLE Venta (
    IdVenta INT IDENTITY PRIMARY KEY,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_Venta_Fecha DEFAULT(SYSUTCDATETIME()),
    IdUsuario INT NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    Estado NVARCHAR(20) NOT NULL,
    MotivoAnulacion NVARCHAR(300) NULL,
    CONSTRAINT FK_Venta_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE VentaDetalle (
    IdVentaDetalle INT IDENTITY PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdMedicamento INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    SubTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_VentaDetalle_Venta FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta),
    CONSTRAINT FK_VentaDetalle_Medicamento FOREIGN KEY (IdMedicamento) REFERENCES Medicamento(IdMedicamento)
);

CREATE TABLE VentaLoteAllocation (
    IdVentaLoteAllocation INT IDENTITY PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdMedicamento INT NOT NULL,
    IdLote INT NOT NULL,
    Cantidad INT NOT NULL,
    CONSTRAINT FK_VLA_Venta FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta),
    CONSTRAINT FK_VLA_Medicamento FOREIGN KEY (IdMedicamento) REFERENCES Medicamento(IdMedicamento),
    CONSTRAINT FK_VLA_Lote FOREIGN KEY (IdLote) REFERENCES Lote(IdLote)
);

CREATE TABLE Pago (
    IdPago INT IDENTITY PRIMARY KEY,
    IdVenta INT NOT NULL,
    MetodoPago NVARCHAR(20) NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_Pago_Fecha DEFAULT(SYSUTCDATETIME()),
    CONSTRAINT FK_Pago_Venta FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta)
);

CREATE TABLE Comprobante (
    IdComprobante INT IDENTITY PRIMARY KEY,
    IdVenta INT NOT NULL,
    Tipo NVARCHAR(20) NOT NULL,
    Serie NVARCHAR(4) NOT NULL,
    Correlativo INT NOT NULL,
    Estado NVARCHAR(30) NOT NULL,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_Comprobante_Fecha DEFAULT(SYSUTCDATETIME()),
    CONSTRAINT FK_Comprobante_Venta FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta)
);

CREATE TABLE ComprobanteEnvioAttempt (
    IdAttempt INT IDENTITY PRIMARY KEY,
    IdComprobante INT NOT NULL,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_Attempt_Fecha DEFAULT(SYSUTCDATETIME()),
    Exito BIT NOT NULL,
    Mensaje NVARCHAR(400) NULL,
    CONSTRAINT FK_Attempt_Comprobante FOREIGN KEY (IdComprobante) REFERENCES Comprobante(IdComprobante)
);

CREATE TABLE InventoryMovement (
    IdMovement INT IDENTITY PRIMARY KEY,
    IdMedicamento INT NOT NULL,
    IdLote INT NULL,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_InventoryMovement_Fecha DEFAULT(SYSUTCDATETIME()),
    Tipo NVARCHAR(20) NOT NULL,
    Cantidad INT NOT NULL,
    ReferenciaId INT NULL,
    ReferenciaTabla NVARCHAR(40) NULL,
    CONSTRAINT FK_InventoryMovement_Medicamento FOREIGN KEY (IdMedicamento) REFERENCES Medicamento(IdMedicamento),
    CONSTRAINT FK_InventoryMovement_Lote FOREIGN KEY (IdLote) REFERENCES Lote(IdLote)
);

CREATE TABLE Caja (
    IdCaja INT IDENTITY PRIMARY KEY,
    FechaApertura DATETIME2 NOT NULL,
    FechaCierre DATETIME2 NULL,
    MontoInicial DECIMAL(18,2) NOT NULL,
    MontoCierre DECIMAL(18,2) NULL,
    Estado NVARCHAR(20) NOT NULL,
    IdUsuarioApertura INT NOT NULL,
    IdUsuarioCierre INT NULL,
    CONSTRAINT FK_Caja_UsuarioApertura FOREIGN KEY (IdUsuarioApertura) REFERENCES Usuario(IdUsuario),
    CONSTRAINT FK_Caja_UsuarioCierre FOREIGN KEY (IdUsuarioCierre) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE MovimientoCaja (
    IdMovimientoCaja INT IDENTITY PRIMARY KEY,
    IdCaja INT NOT NULL,
    Tipo NVARCHAR(10) NOT NULL,
    Motivo NVARCHAR(200) NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_MovCaja_Fecha DEFAULT(SYSUTCDATETIME()),
    IdUsuario INT NOT NULL,
    CONSTRAINT FK_MovCaja_Caja FOREIGN KEY (IdCaja) REFERENCES Caja(IdCaja),
    CONSTRAINT FK_MovCaja_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE AuditLog (
    IdAuditLog INT IDENTITY PRIMARY KEY,
    Fecha DATETIME2 NOT NULL CONSTRAINT DF_Audit_Fecha DEFAULT(SYSUTCDATETIME()),
    Accion NVARCHAR(100) NOT NULL,
    Detalle NVARCHAR(500) NOT NULL,
    IdUsuario INT NULL,
    CONSTRAINT FK_Audit_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE DocumentSequence (
    IdSequence INT IDENTITY PRIMARY KEY,
    Tipo NVARCHAR(20) NOT NULL,
    Serie NVARCHAR(4) NOT NULL,
    CorrelativoActual INT NOT NULL,
    CONSTRAINT UQ_DocSequence UNIQUE(Tipo, Serie)
);
GO

CREATE UNIQUE INDEX UX_Medicamento_CodigoBarras_NotNull ON Medicamento(CodigoBarras) WHERE CodigoBarras IS NOT NULL;
CREATE INDEX IX_Lote_IdMedicamento_FechaVencimiento ON Lote(IdMedicamento, FechaVencimiento);
CREATE INDEX IX_Venta_Fecha ON Venta(Fecha);
CREATE UNIQUE INDEX UX_Comprobante_Tipo_Serie_Correlativo ON Comprobante(Tipo, Serie, Correlativo);
CREATE INDEX IX_InventoryMovement_Medicamento_Fecha ON InventoryMovement(IdMedicamento, Fecha);
GO

IF NOT EXISTS (SELECT 1 FROM Role)
BEGIN
    INSERT INTO Role(Nombre) VALUES ('Admin'),('Cajero'),('Almacen');
END

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE Username='admin')
BEGIN
    INSERT INTO Usuario(IdRole, Username, PasswordHash, RequirePasswordChange, Estado)
    SELECT IdRole, 'admin', NULL, 1, 1 FROM Role WHERE Nombre='Admin';
END

IF NOT EXISTS (SELECT 1 FROM BoticaConfig)
BEGIN
    INSERT INTO BoticaConfig(NombreBotica,RUC,Direccion,Telefono,SerieTicket,SerieBoleta,SerieFactura,ExpirationAlertDays)
    VALUES('BoticaPOS Demo','20123456789','Av. Principal 123','999888777','T001','B001','F001',30);
END

IF NOT EXISTS (SELECT 1 FROM DocumentSequence)
BEGIN
    INSERT INTO DocumentSequence(Tipo,Serie,CorrelativoActual)
    VALUES('Ticket','T001',0),('Boleta','B001',0),('Factura','F001',0);
END

IF NOT EXISTS (SELECT 1 FROM Medicamento)
BEGIN
    INSERT INTO Medicamento(NombreComercial,PrincipioActivo,Laboratorio,Presentacion,Concentracion,CodigoBarras,PrecioVenta,CostoCompra,RequiereReceta,StockMinimo,Estado)
    VALUES
    ('Paracetamol 500mg','Paracetamol','Genfar','Tableta','500mg','775123000001',1.50,0.80,0,20,1),
    ('Ibuprofeno 400mg','Ibuprofeno','MK','Tableta','400mg','775123000002',2.00,1.10,0,15,1),
    ('Amoxicilina 500mg','Amoxicilina','AC Farma','Capsula','500mg','775123000003',3.40,2.00,1,10,1);
END
GO
