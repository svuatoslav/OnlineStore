using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OnlineStore
{
    public interface IXmlDocument
    {
        void ParseXmlDocument(DataTransmission dataTransmission, XDocument xdoc, List<int> idOrders);
    }
    public interface IXmlReader
    {
        void ParseXmlReader(DataTransmission dataTransmission, string pathXml, List<int> idOrders);
    }
    public interface IWriteBD
    {
        void BDWrite(DataTransmission dataTransmission, string connect);
    }
    public interface IClear
    {
        void Clear(DataTransmission dataTransmission);
    }
}
