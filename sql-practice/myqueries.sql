/*

Categories(CategoryId, Name)
Products(ProductId, Name, Price, CategoryId)
Customers(CustomerId, Name, City)
Orders(OrderId, CustomerId, OrderDate)
OrderItems(OrderItemId, OrderId, ProductId, Quantity, UnitPrice)

-- to run the query use this 
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ShopPractice -E -W -s"|" -i sql-practice\myqueries.sql

*/

--challenge 9 - List all pairs of products within the same category.
select c.name, p1.name as Product1, p2.name as Product2 from Products p1
inner join Products p2 on p2.CategoryId = p1.CategoryId
inner join Categories c on c.CategoryId = p1.CategoryId
where p1.ProductId < p2.ProductId;

/*
-- Challenge 8 - Calculate the revenue change for each customer's orders.
WITH orderRevenue AS (
    SELECT o.OrderId, o.OrderDate, o.CustomerId, SUM(oi.Quantity*oi.UnitPrice) AS Revenue
    FROM Orders o JOIN OrderItems oi ON oi.OrderId = o.OrderId
    GROUP BY o.OrderId, o.OrderDate, o.CustomerId
),
prev AS (
    SELECT CustomerId, OrderDate, Revenue,
           LAG(Revenue) OVER (PARTITION BY CustomerId ORDER BY OrderDate) AS PrevRevenue
    FROM orderRevenue
)
SELECT c.Name, p.OrderDate, p.Revenue,
       p.Revenue - p.PrevRevenue AS RevenueChange,
       p.PrevRevenue             AS PreviousOrderRevenue
FROM prev p JOIN Customers c ON c.CustomerId = p.CustomerId
ORDER BY c.Name, p.OrderDate;



-- Challenge 7 - Calculate the total revenue for each customer, ordered by the total revenue in descending order.
with orderRevenue as (
    select o.OrderId, o.OrderDate, o.CustomerId, sum(oi.Quantity * oi.UnitPrice) as Revenue
    from Orders o
    join OrderItems oi on oi.OrderId = o.OrderId
    group by o.OrderId, o.OrderDate, o.CustomerId
)
SELECT c.Name, orr.OrderDate, orr.Revenue,
 sum(orr.Revenue) over (partition by c.CustomerId order by orr.OrderDate) as TotalRevenue
from customers c
join orderRevenue orr on orr.CustomerId = c.CustomerId
order by TotalRevenue desc

--Challenge 6 - For each customer, show their name, their total spent on Electronics, and their total spent on Books — as two separate columns. (Customers who bought neither can be left out.)
select c.name,
sum(case when c2.name = 'Electronics' then oi.Quantity * oi.UnitPrice else 0 end) as ElectronicsSpending,
sum(case when c2.name = 'Books' then oi.Quantity * oi.UnitPrice else 0 end) as BooksSpending
from customers c
inner join Orders o on o.CustomerId = c.CustomerId
inner join OrderItems oi on oi.OrderId = o.OrderId
inner join Products p on p.ProductId = oi.ProductId
inner join Categories c2 on c2.CategoryId = p.CategoryId
group by c.customerid, c.name




-- Challenge 5 - Return the names of products that have never been ordered.
select p.name from
Products p
left join orderitems oi on oi.ProductId = p.ProductId
where oi.OrderItemId is null;


-- Challenge 4 - Return the total spending of each customer who has spent more than $500, ordered by spending descending.
select C.name, Sum(oi.Quantity * oi.UnitPrice) as Spending
from Customers c
join Orders o on o.CustomerId = c.CustomerId
join orderitems oi on oi.OrderId = o.OrderId
group by c.CustomerId, c.Name
having Sum(oi.Quantity * oi.UnitPrice) > 500
order by Spending desc;


-- Challenge 3 - For each category, find its single most expensive product — show the category name, the product name, and the price.
with ranked AS (
    select c.name as category, p.name as product, p.price,
    row_number() over(partition by p.categoryid order by p.price desc) as rn
    from Categories c
    join Products p on p.CategoryId = c.CategoryId
)
select category, product, price
from ranked
where rn = 1
order by price desc;


-- Challenge 2 - Return the revenue for each category, including categories with no sales.
SELECT c.CategoryId, c.Name, COALESCE(SUM(oi.Quantity * oi.UnitPrice), 0) AS Revenue
FROM Categories c
LEFT JOIN Products   p  ON p.CategoryId = c.CategoryId
LEFT JOIN OrderItems oi ON oi.ProductId = p.ProductId
GROUP BY c.CategoryId, c.Name
ORDER BY Revenue DESC;



-- Challenge 1 - Return the name and price of every product that costs more than the average price of all products — most expensive first.
SELECT name, Price
FROM Products
WHERE Price > (SELECT AVG(Price) FROM Products)
ORDER BY Price DESC;

*/
