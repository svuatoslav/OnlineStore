using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace OnlineStore
{
    interface IXmlDocument
    {
        void ParseXmlDocument(DataTransmission dataTransmission, XDocument xdoc);
    }
    interface IXmlReader
    {
        void ParseXmlReader(DataTransmission dataTransmission, string pathXml);
    }
    interface IWriteBD
    {
        void BDWrite(DataTransmission dataTransmission, string connect);
    }
    interface IClear
    {
        void Clear(DataTransmission dataTransmission);
    }
    class DataTransmission
    {
        internal List<Purchase> _orders = new List<Purchase>();
        internal List<Product> _products = new List<Product>();
        internal List<User> _users = new List<User>();
        internal List<Purchaseproduct> _purchaseproducts = new List<Purchaseproduct>();

        private readonly IXmlReader? _xmlReader = null;
        private readonly IXmlDocument? _xmlDocument = null;
        private readonly IWriteBD _writeBD;
        private readonly IClear _clearing;
        public DataTransmission(IXmlDocument xmlDocument, IWriteBD writeBD, IClear clear)
        {
            _xmlDocument = xmlDocument;
            _writeBD = writeBD;
            _clearing = clear;
        }
        public DataTransmission(IXmlReader xmlReader, IWriteBD writeBD, IClear clear)
        {
            _xmlReader = xmlReader;
            _writeBD = writeBD;
            _clearing = clear;
        }
        public void ParseXmlToStoreData(DataTransmission dataTransmission, string connect, string pathXml)
        {
            if (dataTransmission._xmlReader is null)
            {
                XDocument xdoc = XDocument.Load(pathXml);
                _xmlDocument?.ParseXmlDocument(dataTransmission, xdoc);
            }
            else
                _xmlReader?.ParseXmlReader(dataTransmission, pathXml);
            SaveDataToDatabase(dataTransmission, connect);
        }
        private void SaveDataToDatabase(DataTransmission dataTransmission, string connect)
        {
            try { _writeBD.BDWrite(dataTransmission, connect); }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ошибка подключения: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown error");
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine(ex.ToString());
            }
            finally { _clearing.Clear(dataTransmission); }
        }
    }
    class XMLReader : IXmlReader
    {
        public void ParseXmlReader(DataTransmission dataTransmission, string pathXml)
        {
            using (XmlReader reader = XmlReader.Create(pathXml))
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
                            dataTransmission._orders.Add(order);
                        }
                        else if (reader.Name == "product")
                        {
                            purchaseproduct = new Purchaseproduct();
                            product = new Product();
                            purchaseproduct.Product = product;
                            purchaseproduct.Purchase = order;
                            purchaseproduct.Purchaseid = order.Id;
                            //purchaseproduct.Productid = product.Id;
                            dataTransmission._products.Add(product);
                            dataTransmission._purchaseproducts.Add(purchaseproduct);
                        }
                        else if (reader.Name == "user")
                        {
                            user = new User();
                            dataTransmission._users.Add(user);
                            //order_element.Usersid = user.Id;
                            order.Users = user;
                            user.Purchases.Add(order);
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Text)
                    {
                        if (element == "no" && int.TryParse(reader.Value, out int no))
                            order.Id = no;
                        else if (element == "reg_date" && DateOnly.TryParse(reader.Value, out DateOnly reg_date))
                            order.Orderdate = reg_date;
                        else if (element == "sum" && decimal.TryParse(reader.Value, CultureInfo.InvariantCulture, out decimal sum))
                            order.Price = sum;
                        else if (element == "quantity" && int.TryParse(reader.Value, out int quantity))
                        {
                            purchaseproduct.Quantityproduct = quantity;
                            product.Productcount = quantity;
                        }
                        else if (element == "name")
                            product.Productname = reader.Value;
                        else if (element == "price" && decimal.TryParse(reader.Value, CultureInfo.InvariantCulture, out decimal price))
                            product.Price = price;
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
        }
    }
    class XMLXDocument : IXmlDocument
    {
        public void ParseXmlDocument(DataTransmission dataTransmission, XDocument xdoc)
        {
            XElement? orders = xdoc.Element("orders");

            if (orders is not null)
            {
                Purchase order;
                Product product;
                User user;
                Purchaseproduct purchaseproduct;

                foreach (XElement order_element in orders.Elements("order"))
                {
                    if (int.TryParse(order_element.Element("no")?.Value, out int no) && DateOnly.TryParse(order_element.Element("reg_date")?.Value, out DateOnly reg_date) && decimal.TryParse(order_element.Element("sum")?.Value, CultureInfo.InvariantCulture, out decimal sum))
                    {
                        order = new Purchase(no, reg_date, sum);
                        dataTransmission._orders.Add(order);

                        foreach (XElement product_element in order_element.Elements("product"))
                        {
                            if (product_element.Element("name") is not null && int.TryParse(order_element.Element("quantity")?.Value, out int quantity) && decimal.TryParse(order_element.Element("price")?.Value, CultureInfo.InvariantCulture, out decimal price))
                            {
                                product = new Product(product_element.Element("name").Value, quantity, price);
                                purchaseproduct = new Purchaseproduct(quantity, product, order);

                                product.Purchaseproducts.Add(purchaseproduct);
                                order.Purchaseproducts.Add(purchaseproduct);

                                dataTransmission._products.Add(product);
                                dataTransmission._purchaseproducts.Add(purchaseproduct);
                            }
                            else
                                Console.WriteLine("Неполные данные");
                        }

                        XElement? fio = order_element.Element("user")?.Element("fio");
                        if (fio is not null)
                        {
                            string[] subs = fio.Value.Split(null, 2);
                            user = new User(subs[0], subs[1], fio.Element("email")?.Value, [order]);
                            order.Users = user;
                            dataTransmission._users.Add(user);
                        }
                        else
                            Console.WriteLine("Неполные данные");
                    }
                    else
                        Console.WriteLine("Неполные данные");
                }
            }
        }
    }
    class BDSave : IWriteBD
    {
        public void BDWrite(DataTransmission dataTransmission, string connect)
        {
            using var onlineStoreContext = new OnlineStoreContext(connect);
            using var transaction = onlineStoreContext.Database.BeginTransaction();
            try
            {
                SaveDataToDatabase(dataTransmission, onlineStoreContext);
                transaction.Commit();
            }
            catch (DbUpdateException ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Ошибка при записи данных: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
        private static void SaveDataToDatabase(DataTransmission dataTransmission, OnlineStoreContext onlineStoreContext)
        {
            SaveOrders(dataTransmission, onlineStoreContext);
            SaveProducts(dataTransmission, onlineStoreContext);
            SaveUsers(dataTransmission, onlineStoreContext);
            SavePurchaseProducts(dataTransmission, onlineStoreContext);
            onlineStoreContext.SaveChanges();
        }
        private static void SavePurchaseProducts(DataTransmission dataTransmission, OnlineStoreContext onlineStoreContext)
        {
            if (dataTransmission._purchaseproducts.Count > 0)
                foreach (Purchaseproduct product in dataTransmission._purchaseproducts)
                    onlineStoreContext.Purchaseproducts.Add(product);
        }
        private static void SaveUsers(DataTransmission dataTransmission, OnlineStoreContext onlineStoreContext)
        {
            if (dataTransmission._users.Count > 0)
                foreach (User user in dataTransmission._users)
                    onlineStoreContext.Users.Add(user);
        }
        private static void SaveProducts(DataTransmission dataTransmission, OnlineStoreContext onlineStoreContext)
        {
            if (dataTransmission._products.Count > 0)
                foreach (Product product in dataTransmission._products)
                    onlineStoreContext.Products.Add(product);
        }
        private static void SaveOrders(DataTransmission dataTransmission, OnlineStoreContext onlineStoreContext)
        {
            if (dataTransmission._orders.Count > 0)
                foreach (Purchase order in dataTransmission._orders)
                    onlineStoreContext.Purchases.Add(order);
        }
    }
    class DataClear : IClear
    {
        public void Clear(DataTransmission dataTransmission)
        {
            dataTransmission._orders.Clear();
            dataTransmission._products.Clear();
            dataTransmission._users.Clear();
            dataTransmission._purchaseproducts.Clear();
        }
    }
}
