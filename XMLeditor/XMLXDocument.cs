using OnlineStore.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OnlineStore.XMLeditor
{
    class XMLXDocument : IXmlDocument
    {
        public void ParseXmlDocument(DataTransmission dataTransmission, XDocument xdoc, List<int> idOrders)
        {
            XElement? orders = xdoc.Element("orders");

            if (orders is not null)
            {
                Purchase order;
                Product product;
                User user;
                Purchaseproduct purchaseproduct;
                var t = orders.Elements("order");

                t = CheckId(orders.Elements("order"), idOrders);

                foreach (XElement order_element in orders.Elements("order"))
                {
                    if (int.TryParse(order_element.Element("no")?.Value, out int no) && DateOnly.TryParse(order_element.Element("reg_date")?.Value, out DateOnly reg_date) && decimal.TryParse(order_element.Element("sum")?.Value, CultureInfo.InvariantCulture, out decimal sum))
                    {
                        order = new Purchase(no, reg_date, sum);
                        dataTransmission._orders.Add(order);

                        foreach (XElement product_element in order_element.Elements("product"))
                        {
                            if (product_element.Element("name") is not null && int.TryParse(product_element.Element("quantity")?.Value, out int quantity) && decimal.TryParse(product_element.Element("price")?.Value, CultureInfo.InvariantCulture, out decimal price))
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
