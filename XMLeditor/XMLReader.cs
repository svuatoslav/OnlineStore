using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OnlineStore.XMLeditor
{
    class XMLReader : IXmlReader
    {
        public void ParseXmlReader(DataTransmission dataTransmission, string pathXml, List<int> idOrders)
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
        private IEnumerable<XElement> CheckId(IEnumerable<XElement> id, List<int> idOrders)
        {
            //IEnumerable<string> no = id.Elements("no").Select(x => x.Value);
            //id = no.Intersect(idOrders.Select(x => x.ToString()));
            id = id.Where(e =>
            {
                int value;
                return !idOrders.Contains(e.Element("no").Value != null && int.TryParse(e.Element("no").Value, out value) ? value : -1);
            });
            return id;
        }
    }
}
