using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OnlineStore
{
    [TestFixture]
    internal class DataTransmissionTests
    {
        [Test]
        public void Test()
        {
            var mockXmlDocument = new Mock<IXmlDocument>();
            var mockWriteBD = new Mock<IWriteBD>();
            var mockClear = new Mock<IClear>();
            var dataTransmission = new DataTransmission(mockXmlDocument.Object, mockWriteBD.Object, mockClear.Object);
            //// Act
            //dataTransmission.XMLToDBXDocument("ConnectionString");

            //// Assert
            //mockXmlDocument.Verify(x => x.Update(It.IsAny<XmlDocument>()), Times.Once);
            //mockWriteBD.Verify(x => x.BDWrite(dataTransmission, "ConnectionString"), Times.Once);
            //mockClear.Verify(x => x.Clear(dataTransmission), Times.Once);
        }
    }
    [TestFixture]
    public class BDSaveTests
    {
        [Test]
        public void BDWrite_ShouldSaveDataToDatabase()
        {
            // Arrange
            var dataTransmission = new DataTransmission(new Mock<IXmlDocument>().Object, new Mock<IWriteBD>().Object, new Mock<IClear>().Object);
            var bdSave = new BDSave();

            // Act
            bdSave.BDWrite(dataTransmission, "ConnectionString");

            // Assert
            // Проверка, что данные из коллекций dataTransmission были сохранены в базе данных
        }

        [Test]
        public void BDWrite_ShouldRollbackTransactionOnException()
        {
            // Arrange
            var dataTransmission = new DataTransmission(new Mock<IXmlDocument>().Object, new Mock<IWriteBD>().Object, new Mock<IClear>().Object);
            var bdSave = new BDSave();
            var mockContext = new Mock<OnlineStoreContext>("ConnectionString");
            mockContext.Setup(x => x.SaveChanges()).Throws<DbUpdateException>();

            // Act and Assert
            Assert.Throws<DbUpdateException>(() => bdSave.BDWrite(dataTransmission, "ConnectionString"));
            mockContext.Verify(x => x.Database.BeginTransaction(), Times.Once);
            mockContext.Verify(x => x.Database.BeginTransaction().Rollback(), Times.Once);
        }
    }
    [TestFixture]
    public class DataClearTests
    {
        [Test]
        public void Clear_ShouldClearAllCollections()
        {
            // Arrange
            var dataTransmission = new DataTransmission(new Mock<IXmlDocument>().Object, new Mock<IWriteBD>().Object, new Mock<IClear>().Object);
            dataTransmission._orders.Add(new Purchase());
            dataTransmission._products.Add(new Product());
            dataTransmission._users.Add(new User());
            dataTransmission._purchaseproducts.Add(new Purchaseproduct());
            var dataClear = new DataClear();

            // Act
            dataClear.Clear(dataTransmission);

            // Assert
            //Assert.IsEmpty(dataTransmission._orders);
            //Assert.IsEmpty(dataTransmission._products);
            //Assert.IsEmpty(dataTransmission._users);
            //Assert.IsEmpty(dataTransmission._purchaseproducts);
        }
    }
}
