using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.BDeditor
{
    internal class BDLoad
    {
        public static List<int> GetIdOrders(string connect)
        {
            using var onlineStoreContext = new OnlineStoreContext(connect);
            var data = onlineStoreContext.Purchases.Select(s => s.Id).ToList();
            return data;
        }
        public static IQueryable<TEntity> GetModels<TEntity>(string connect) where TEntity : class
        {
            using var onlineStoreContext = new OnlineStoreContext(connect);
            return onlineStoreContext.Set<TEntity>();
        }
    }
}
