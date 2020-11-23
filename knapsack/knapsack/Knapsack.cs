using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace knapsack
{
    class Knapsack
    {
        public int id;
        public BigInteger price;
        public BigInteger weight;
        public double k;
        
        public Knapsack(int id, BigInteger price, BigInteger weight, double k)
        {
            this.id = id;
            this.price = price;
            this.weight = weight;
            this.k = k;
        }
    }
}
