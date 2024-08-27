using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace OnlineStore
{
    public static class DataTransmission
    {
        private static List<Purchase> _orders = new List<Purchase>();
        private static List<Product> _products = new List<Product>();
        private static List<User> _users = new List<User>();
        private static List<Purchaseproduct> _purchaseproducts = new List<Purchaseproduct>();
        public static void XMLToDBXmlReader()
        {
            using (XmlReader reader = XmlReader.Create("XMLFiles\\DATA.xml"))
            {
                if (!reader.ReadToFollowing("orders"))
                {
                    Console.WriteLine("Ошибка");
                    return;
                }
                Purchase? order = null;
                Product? product = null;
                User? user = null;
                Purchaseproduct? purchaseproduct = null;
                string element = "orders";
                string text = string.Empty;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        element = reader.Name;
                        if (reader.Name == "order")
                        {
                            order = new Purchase();
                            _orders.Add(order);
                        }
                        else if (reader.Name == "product")
                        {
                            purchaseproduct = new Purchaseproduct();
                            product = new Product();
                            purchaseproduct.Product = product;
                            purchaseproduct.Purchase = order;
                            purchaseproduct.Purchaseid = order.Id;
                            //purchaseproduct.Productid = product.Id;
                            _products.Add(product);
                            _purchaseproducts.Add(purchaseproduct);
                        }
                        else if (reader.Name == "user")
                        {
                            user = new User();
                            _users.Add(user);
                            //order_element.Usersid = user.Id;
                            order.Users = user;
                            user.Purchases.Add(order);
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Text)
                    {
                        if (element == "no")
                            order.Id = int.Parse(reader.Value);
                        else if (element == "reg_date")
                            order.Orderdate = DateOnly.Parse(reader.Value);
                        else if (element == "sum")
                            order.Price = decimal.Parse(reader.Value, CultureInfo.InvariantCulture);
                        else if (element == "quantity")
                        {
                            purchaseproduct.Quantityproduct = int.Parse(reader.Value);
                            product.Productcount = purchaseproduct.Quantityproduct;
                        }
                        else if (element == "name")
                            product.Productname = reader.Value;
                        else if (element == "price")
                            product.Price = decimal.Parse(reader.Value, CultureInfo.InvariantCulture);
                        else if (element == "fio")
                        {
                            string[] subs = reader.Value.Split(null, 2);
                            user.Firstname = subs[0];
                            user.Lastname = subs[1];
                        }
                        else if (element == "email")
                            user.Email = reader.Value;
                    }
                }
            }
            BDAdd();
        }
        public static void XMLToDBXDocument()
        {
            XDocument xdoc = XDocument.Load("XMLFiles\\DATA.xml");

            XElement? orders = xdoc.Element("orders");
            if (orders is not null)
            {
                Purchase order;
                Product product;
                User user;
                Purchaseproduct purchaseproduct;
                foreach (XElement order_element in orders.Elements("order"))
                {
                    order = new Purchase(int.Parse(order_element.Element("no").Value), 
                        DateOnly.Parse(order_element.Element("reg_date").Value), 
                        decimal.Parse(order_element.Element("sum").Value, CultureInfo.InvariantCulture));
                    order_element.Element("user");

                    foreach (XElement product_element in order_element.Elements("product"))
                    {
                        product = new Product(product_element.Element("name").Value, 
                            int.Parse(product_element.Element("quantity").Value), 
                            decimal.Parse(product_element.Element("price").Value, CultureInfo.InvariantCulture));

                        purchaseproduct = new Purchaseproduct(int.Parse(product_element.Element("quantity").Value),
                            product, 
                            order);

                        product.Purchaseproducts.Add(purchaseproduct);
                        order.Purchaseproducts.Add(purchaseproduct);

                        _products.Add(product);
                        _purchaseproducts.Add(purchaseproduct);
                    }

                    string[] subs = order_element.Element("user").Element("fio").Value.Split(null, 2);
                    user = new User(subs[0], subs[1], order_element.Element("user").Element("email").Value, [order]);
                    order.Users = user;
                    _users.Add(user);
                    _orders.Add(order);
                }
                BDAdd();
            }
        }
        private static void BDAdd()
        {
            Console.WriteLine("Введите строку для подключения к базе данных в формате:\n \"Host=localhost;Port=NumberPort;Database=DatabaseName;Username=postgres;Password=password");
            string connect = Console.ReadLine();
            try
            {
                using (OnlineStoreContext onlineStoreContext = new OnlineStoreContext(connect))
                {
                    if (_orders.Count > 0)
                        foreach (Purchase order in _orders)
                            onlineStoreContext.Purchases.Add(order);
                    if (_products.Count > 0)
                        foreach (Product product in _products)
                            onlineStoreContext.Products.Add(product);
                    if (_users.Count > 0)
                        foreach (User user in _users)
                            onlineStoreContext.Users.Add(user);
                    if (_purchaseproducts.Count > 0)
                        foreach (Purchaseproduct product in _purchaseproducts)
                            onlineStoreContext.Purchaseproducts.Add(product);
                    onlineStoreContext.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ошибка подключения: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown error");
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"Метод: {ex.TargetSite}");
                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                Console.WriteLine();
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _orders.Clear();
                _products.Clear();
                _users.Clear();
                _purchaseproducts.Clear();
            }

        }
    }
}
