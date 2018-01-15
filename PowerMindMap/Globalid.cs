using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindNoderPort
{
    public class Globalid
    {
        public static int id = 0;

        public static void IncrementId()
        {
            id += 1;
        }
    }
}
