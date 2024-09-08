using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace OnlineStore
{
    internal static class MessageManager
    {
        public static void SendMessage (string message)
        {
            WriteLine(message);
        }
    }
}
