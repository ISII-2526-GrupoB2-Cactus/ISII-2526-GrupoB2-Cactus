-- ======================================
--          ALQUILAR Y RESEÑAR DISPOSITIVOS
-- ======================================

-- BORRAR TODO AL PRINCIPIO (orden inverso por FKs)
DELETE FROM REVIEWITEM;
DELETE FROM REVIEW;
DELETE FROM RentDevice;
DELETE FROM Rental;
DELETE FROM Device;
DELETE FROM Model;

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

-- DISPOSITIVOS 
INSERT INTO Device 
(Brand, Color, Name, PriceForPurchase, PriceForRent, Year, Quality, 
 QuantityForPurchase, QuantityForRent, ModelId, Description)
VALUES
('Apple', 'Negro', 'iPhone 15', 1200.00, 50.50, 2023, 0, 10, 5, 1, 'Smartphone Apple iPhone 15'),
('Samsung', 'Gris', 'Galaxy S23', 999.00, 89.99, 2023, 0, 8, 4, 2, 'Smartphone Samsung Galaxy S23'),
('Sony', 'Blanco', 'PlayStation 5', 550.00, 120.75, 2023, 0, 12, 6, 3, 'Consola PlayStation 5'),
('Microsoft', 'Azul', 'Surface Pro 9', 1500.00, 90.85, 2022, 0, 7, 3, 4, 'Tablet Surface Pro 9'),
('Apple', 'Plateado', 'MacBook Air', 1400.00, 75.80, 2022, 0, 5, 2, 5, 'Portátil MacBook Air');







-- ======================================
--          ALQUILAR DISPOSITIVOS
-- ======================================

-- RENTAL (3 ALQUILERES - UNO POR USUARIO)
INSERT INTO Rental 
(DeliveryAddress, NameCustomer, SurnameCustomer, TotalPrice, RentalDate, 
 RentalDateFrom, RentalDateTo, PaymentMethod, ApplicationUserId)
VALUES
-- Laura Gonzalez Rico
('Avda España 33, Albacete', 'Laura', 'Gonzalez Rico', 0, GETDATE(), 
 DATEADD(DAY, 1, GETDATE()), DATEADD(DAY, 8, GETDATE()), 0, (SELECT Id FROM AspNetUsers WHERE UserName = 'laura@alu.uclm.es')),
-- Elena Organero Maroto
('Calle Mayor 12, Toledo', 'Elena', 'Organero Maroto', 0, GETDATE(), 
 DATEADD(DAY, 2, GETDATE()), DATEADD(DAY, 9, GETDATE()), 1, (SELECT Id FROM AspNetUsers WHERE UserName = 'elena@alu.uclm.es')),
-- Maria Martinez Gonzalez
('Calle Libertad 9, Ciudad Real', 'Maria', 'Martinez Gonzalez', 0, GETDATE(), 
 DATEADD(DAY, 3, GETDATE()), DATEADD(DAY, 10, GETDATE()), 0, (SELECT Id FROM AspNetUsers WHERE UserName = 'maria@alu.uclm.es'));

-- RENTDEVICE (DETALLES DE ALQUILERES)
INSERT INTO RentDevice (RentId, DeviceId, Price, Quantity, RentalId) VALUES
-- Laura: iPhone 15 + MacBook Air (7 días)
(1, 1, 50.50, 1, 1),
(1, 5, 75.80, 1, 1),
-- Elena: Galaxy S23 + Surface Pro 9 (7 días)
(2, 2, 89.99, 1, 2),
(2, 4, 90.85, 1, 2),
(2, 1, 50.50, 1, 2),
-- Maria: PlayStation 5 (7 días)
(3, 3, 120.75, 1, 3),
(3, 2, 89.99, 1, 3);

-- ACTUALIZAR PRECIOS TOTALES BASADOS EN LOS DISPOSITIVOS ALQUILADOS
UPDATE Rental 
SET TotalPrice = (
    SELECT SUM(rd.Price * rd.Quantity * 7) -- 7 días para todos
    FROM RentDevice rd 
    WHERE rd.RentalId = Rental.Id
)
WHERE Id IN (1, 2, 3);







-- ======================================
--          RESEÑAR DISPOSITIVOS
-- ======================================

-- REVIEW
INSERT INTO REVIEW (CustomerId, DateOfReview, OverallRating, ReviewTitle, ApplicationUserId)
VALUES 
((SELECT Id FROM AspNetUsers WHERE UserName = 'laura@alu.uclm.es'), GETDATE(), 5, 'Excelente experiencia de compra', (SELECT Id FROM AspNetUsers WHERE UserName = 'laura@alu.uclm.es')),
((SELECT Id FROM AspNetUsers WHERE UserName = 'elena@alu.uclm.es'), DATEADD(DAY, -1, GETDATE()), 4, 'Muy buena atención al cliente', (SELECT Id FROM AspNetUsers WHERE UserName = 'elena@alu.uclm.es')),
((SELECT Id FROM AspNetUsers WHERE UserName = 'maria@alu.uclm.es'), DATEADD(DAY, -2, GETDATE()), 5, 'Servicio rápido y eficiente', (SELECT Id FROM AspNetUsers WHERE UserName = 'maria@alu.uclm.es')),
((SELECT Id FROM AspNetUsers WHERE UserName = 'laura@alu.uclm.es'), DATEADD(DAY, -3, GETDATE()), 3, 'Producto bueno con entrega regular', (SELECT Id FROM AspNetUsers WHERE UserName = 'laura@alu.uclm.es')),
((SELECT Id FROM AspNetUsers WHERE UserName = 'elena@alu.uclm.es'), DATEADD(DAY, -4, GETDATE()), 5, 'Calidad premium garantizada', (SELECT Id FROM AspNetUsers WHERE UserName = 'elena@alu.uclm.es'));

-- REVIEWITEM (COMENTARIOS CON 50 CARACTERES MÁXIMO)
INSERT INTO REVIEWITEM (ReviewId, DeviceId, Comments, Rating) VALUES
(1, 1, 'iPhone 15: cámara increíble y batería duradera.', 5),
(1, 3, 'PlayStation 5 funciona perfectamente para gaming.', 5),
(2, 5, 'MacBook Air rápido pero precio algo elevado.', 4),
(2, 2, 'Buen smartphone, relación calidad-precio.', 4),
(3, 4, 'Excelente tablet para trabajo y entretenimiento.', 5),
(4, 2, 'Envío tardó pero el producto es bueno.', 3),
(5, 1, 'Calidad premium, totalmente recomendado.', 5);







-- ======================================
-- VERIFICACIÓN FINAL
-- ======================================

SELECT '=== TABLAS PRINCIPALES ===' as Info;
SELECT 
    'Model' as Tabla, COUNT(*) as Cantidad FROM Model
UNION ALL
SELECT 'Device', COUNT(*) FROM Device
UNION ALL
SELECT 'Rental', COUNT(*) FROM Rental
UNION ALL
SELECT 'RentDevice', COUNT(*) FROM RentDevice
UNION ALL
SELECT 'Review', COUNT(*) FROM Review
UNION ALL
SELECT 'ReviewItem', COUNT(*) FROM ReviewItem;

SELECT '=== ALQUILERES ===' as Info;
SELECT Id, NameCustomer, SurnameCustomer, TotalPrice, RentalDateFrom, RentalDateTo FROM Rental;

SELECT '=== DISPOSITIVOS ALQUILADOS ===' as Info;
SELECT RentId, DeviceId, RentalId, Price, Quantity FROM RentDevice;

SELECT '=== RESEÑAS ===' as Info;
SELECT ReviewId, CustomerId, OverallRating, ReviewTitle, DateOfReview FROM Review;

SELECT '=== ITEMS DE RESEÑAS ===' as Info;
SELECT ReviewId, DeviceId, Comments, Rating FROM ReviewItem;