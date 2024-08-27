WITH Order_Date AS 
(
	SELECT ProductId, SUM(QuantityProduct) AS Quantity
	FROM PurchaseProduct
	WHERE PurchaseProduct.PurchaseId = (SELECT Id FROM Purchases WHERE OrderDate = '2024-08-20')
	GROUP BY ProductId
)

SELECT Products.ProductName, Order_Date.Quantity, Order_Date.Quantity * Products.Price AS Amount
FROM Products
JOIN Order_Date
ON Products.Id = Order_Date.ProductId 



