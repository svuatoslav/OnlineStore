using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using OnlineStore.BDeditor;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace OnlineStore
{
    public class DataTransmission
    {
        public List<Purchase> _orders = new List<Purchase>();
        public List<Product> _products = new List<Product>();
        public List<User> _users = new List<User>();
        public List<Purchaseproduct> _purchaseproducts = new List<Purchaseproduct>();
        private List<int> _ordersId;

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
            if (!CheckDbConnect(connect))
                return;

            _ordersId = BDLoad.GetIdOrders(connect);

            if (dataTransmission._xmlReader is null)
            {
                XDocument xdoc = XDocument.Load(pathXml);
                _xmlDocument?.ParseXmlDocument(dataTransmission, xdoc, _ordersId);
            }
            else
                _xmlReader?.ParseXmlReader(dataTransmission, pathXml, _ordersId);


            SaveDataToDatabase(dataTransmission, connect);
        }
        private void SaveDataToDatabase(DataTransmission dataTransmission, string connect)
        {
            _writeBD.BDWrite(dataTransmission, connect);
            _clearing.Clear(dataTransmission);
        }
        private bool CheckDbConnect(string connect)
        {
            try
            {
                using (var context = new OnlineStoreContext(connect))
                {
                    context.Database.OpenConnection();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подключении к базе данных: {ex.Message}");
                return false;
            }
            return true;
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
