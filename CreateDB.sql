INSERT INTO ProductCategories(CategoryName) 
VALUES 
('Телевизоры'), ('Холодильники');

INSERT INTO Products(ProductName, CategoryId, Company, ProductCount, Price, IsDiscounted) 
VALUES 
('Indesit', (SELECT Id FROM ProductCategories WHERE CategoryName='Холодильники'), 'SD 128', 2, 30000, false),
('Samsung', (SELECT Id FROM ProductCategories WHERE CategoryName='Телевизоры'),'AS 32', 3, 35000, false);

INSERT INTO Users(FirstName, LastName, UserPassword, Email, Phone, Address, Age) 
VALUES ('Святослав', 'Москаленко', '1234567', '1@mail.ru', '88888888888', '1 ул.', 20);

ALTER TABLE Purchases
DROP COLUMN Quantity;

INSERT INTO Purchases(UsersId, OrderDate) 
VALUES 
(
	(SELECT Id FROM Users WHERE Phone='88888888888'),
	'2024-08-20'
);

ALTER TABLE PurchaseProduct
ADD COLUMN QuantityProduct INT DEFAULT 0 CHECK(QuantityProduct>=0) NOT NULL;

INSERT INTO PurchaseProduct(PurchaseId, ProductId)
SELECT Purchases.Id, Products.Id FROM Purchases CROSS JOIN Products;

UPDATE PurchaseProduct
SET QuantityProduct = (SELECT ProductCount FROM Products WHERE Products.Id = ProductId);

UPDATE Purchases
SET Price = (SELECT SUM(Pr * QP) 
FROM (SELECT Products.Price AS Pr, PurchaseProduct.QuantityProduct AS QP
FROM PurchaseProduct, Products
WHERE PurchaseProduct.ProductId = Products.Id));