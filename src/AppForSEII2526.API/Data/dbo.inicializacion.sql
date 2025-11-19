-- ======================================
--          ALQUILAR Y RESEÑAR DISPOSITIVOS - CORREGIDO
-- ======================================

-- PRIMERO VERIFICAR SI HAY DATOS Y ELIMINAR EN ORDEN CORRECTO
IF EXISTS (SELECT 1 FROM Reviewitem) DELETE FROM Reviewitem;
IF EXISTS (SELECT 1 FROM Review) DELETE FROM Review;
IF EXISTS (SELECT 1 FROM RentDevice) DELETE FROM RentDevice;
IF EXISTS (SELECT 1 FROM Rental) DELETE FROM Rental;
IF EXISTS (SELECT 1 FROM Device) DELETE FROM Device;
IF EXISTS (SELECT 1 FROM Model) DELETE FROM Model;

-- REINICIAR IDENTITY
DBCC CHECKIDENT ('Model', RESEED, 0);
DBCC CHECKIDENT ('Device', RESEED, 0);
DBCC CHECKIDENT ('Rental', RESEED, 0);
DBCC CHECKIDENT ('Review', RESEED, 0);

-- MODELOS 
INSERT INTO Model (Name) VALUES 
('iPhone 15'),
('Galaxy S23'),
('PlayStation 5'),
('Surface Pro 9'),
('MacBook Air');

-- OBTENER LOS IDs REALES DE LOS MODELOS INSERTADOS
DECLARE @Model1 INT, @Model2 INT, @Model3 INT, @Model4 INT, @Model5 INT;
SELECT @Model1 = Id FROM Model WHERE Name = 'iPhone 15';
SELECT @Model2 = Id FROM Model WHERE Name = 'Galaxy S23';
SELECT @Model3 = Id FROM Model WHERE Name = 'PlayStation 5';
SELECT @Model4 = Id FROM Model WHERE Name = 'Surface Pro 9';
SELECT @Model5 = Id FROM Model WHERE Name = 'MacBook Air';

-- DISPOSITIVOS - USAR LOS IDs REALES DE MODELOS
INSERT INTO Device 
(Brand, Color, Name, PriceForPurchase, PriceForRent, Year, Quality, 
 QuantityForPurchase, QuantityForRent, ModelId, Description)
VALUES
('Apple', 'Negro', 'iPhone 15', 1200.00, 50.50, 2023, 0, 10, 5, @Model1, 'Smartphone Apple iPhone 15'),
('Samsung', 'Gris', 'Galaxy S23', 999.00, 89.99, 2023, 0, 8, 4, @Model2, 'Smartphone Samsung Galaxy S23'),
('Sony', 'Blanco', 'PlayStation 5', 550.00, 120.75, 2023, 0, 12, 6, @Model3, 'Consola PlayStation 5'),
('Microsoft', 'Azul', 'Surface Pro 9', 1500.00, 90.85, 2022, 0, 7, 3, @Model4, 'Tablet Surface Pro 9'),
('Apple', 'Plateado', 'MacBook Air', 1400.00, 75.80, 2022, 0, 5, 2, @Model5, 'Portatil MacBook Air');

-- OBTENER LOS IDs REALES DE LOS DISPOSITIVOS INSERTADOS
DECLARE @Device1 INT, @Device2 INT, @Device3 INT, @Device4 INT, @Device5 INT;
SELECT @Device1 = Id FROM Device WHERE Name = 'iPhone 15';
SELECT @Device2 = Id FROM Device WHERE Name = 'Galaxy S23';
SELECT @Device3 = Id FROM Device WHERE Name = 'PlayStation 5';
SELECT @Device4 = Id FROM Device WHERE Name = 'Surface Pro 9';
SELECT @Device5 = Id FROM Device WHERE Name = 'MacBook Air';

-- ======================================
--          ALQUILAR DISPOSITIVOS - CORREGIDO
-- ======================================

-- PRIMERO INSERTAR RENTALS 
INSERT INTO Rental 
(DeliveryAddress, NameCustomer, SurnameCustomer, TotalPrice, RentalDate, 
 RentalDateFrom, RentalDateTo, PaymentMethod)
VALUES
('Avda Espana 33, Albacete', 'Laura', 'Gonzalez Rico', 883.10, GETDATE(), 
 DATEADD(DAY, 1, GETDATE()), DATEADD(DAY, 8, GETDATE()), 0),
('Calle Mayor 12, Toledo', 'Elena', 'Organero Maroto', 1620.83, GETDATE(), 
 DATEADD(DAY, 2, GETDATE()), DATEADD(DAY, 9, GETDATE()), 1),
('Calle Libertad 9, Ciudad Real', 'Maria', 'Martinez Gonzalez', 1475.18, GETDATE(), 
 DATEADD(DAY, 3, GETDATE()), DATEADD(DAY, 10, GETDATE()), 0);

-- LUEGO INSERTAR RENTDEVICES (USAR LOS IDs REALES DE DISPOSITIVOS)
INSERT INTO RentDevice (RentId, DeviceId, Price, Quantity, RentalId) VALUES
(1, @Device1, 50.50, 1, 1),
(1, @Device5, 75.80, 1, 1),
(2, @Device2, 89.99, 1, 2),
(2, @Device4, 90.85, 1, 2),
(2, @Device1, 50.50, 1, 2),
(3, @Device3, 120.75, 1, 3),
(3, @Device2, 89.99, 1, 3);

-- ======================================
--          RESEÑAR DISPOSITIVOS
-- ======================================

-- INSERTAR REVIEWS
INSERT INTO Review (CustomerId, DateOfReview, OverallRating, ReviewTitle)
VALUES 
('customer1', GETDATE(), 5, 'Excelente experiencia de compra'),
('customer2', DATEADD(DAY, -1, GETDATE()), 4, 'Muy buena atencion al cliente'),
('customer3', DATEADD(DAY, -2, GETDATE()), 5, 'Servicio rapido y eficiente'),
('customer1', DATEADD(DAY, -3, GETDATE()), 3, 'Producto bueno con entrega regular'),
('customer2', DATEADD(DAY, -4, GETDATE()), 5, 'Calidad premium garantizada');

-- INSERTAR REVIEWITEMS (USAR LOS IDs REALES DE DISPOSITIVOS)
INSERT INTO Reviewitem (ReviewId, DeviceId, Comments, Rating) VALUES
(1, @Device1, 'iPhone 15: camara increible y bateria duradera.', 5),
(1, @Device3, 'PlayStation 5 funciona perfectamente para gaming.', 5),
(2, @Device5, 'MacBook Air rapido pero precio algo elevado.', 4),
(2, @Device2, 'Buen smartphone, relacion calidad-precio.', 4),
(3, @Device4, 'Excelente tablet para trabajo y entretenimiento.', 5),
(4, @Device2, 'Envio tardo pero el producto es bueno.', 3),
(5, @Device1, 'Calidad premium, totalmente recomendado.', 5);

-- ======================================
-- VERIFICACION FINAL
-- ======================================

SELECT '=== TABLAS PRINCIPALES ===' as Info;
SELECT 'Model' as Tabla, COUNT(*) as Cantidad FROM Model
UNION ALL SELECT 'Device', COUNT(*) FROM Device
UNION ALL SELECT 'Rental', COUNT(*) FROM Rental
UNION ALL SELECT 'RentDevice', COUNT(*) FROM RentDevice
UNION ALL SELECT 'Review', COUNT(*) FROM Review
UNION ALL SELECT 'Reviewitem', COUNT(*) FROM Reviewitem;

SELECT '=== ALQUILERES ===' as Info;
SELECT Id, NameCustomer, SurnameCustomer, TotalPrice FROM Rental;

SELECT '=== DISPOSITIVOS ALQUILADOS ===' as Info;
SELECT RentId, DeviceId, Price, Quantity, RentalId FROM RentDevice;

SELECT '=== RESEÑAS ===' as Info;
SELECT ReviewId, CustomerId, OverallRating, ReviewTitle FROM Review;

SELECT '=== ITEMS DE RESEÑAS ===' as Info;
SELECT ReviewId, DeviceId, Comments, Rating FROM Reviewitem;