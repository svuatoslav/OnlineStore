using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.BDeditor
{
    public class BDSave : IWriteBD
    {
        public void BDWrite(DataTransmission dataTransmission, string connect)
        {
            using var onlineStoreContext = new OnlineStoreContext(connect);

            bool isAvalaible = onlineStoreContext.Database.CanConnect();
            // bool isAvalaible2 = await db.Database.CanConnectAsync();
            if (isAvalaible) Console.WriteLine("База данных доступна");
            else Console.WriteLine("База данных не доступна");

            using var transaction = onlineStoreContext.Database.BeginTransaction();
            try
            {
                SaveData(dataTransmission, onlineStoreContext);
                transaction.Commit();
            }
            catch (DbUpdateException ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Ошибка при записи данных: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
        private static void SaveData<TEntity>(OnlineStoreContext onlineStoreContext, IEnumerable<TEntity> models) where TEntity : class
        {
            foreach (var model in models)
                onlineStoreContext.Entry(model).State = EntityState.Added;
            onlineStoreContext.SaveChanges();
        }
        private static void SaveData(DataTransmission dataTransmission, OnlineStoreContext onlineStoreContext)
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
}
