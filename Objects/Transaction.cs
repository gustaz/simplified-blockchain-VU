using hash_algorithm.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplified_blockchain_VU.Objects
{
    public class Transaction
    {
        private readonly CustomHashAlgorithm _hashAlgorithm = new CustomHashAlgorithm();
        public string Id { get; }
        public string From { get; set; }
        public string To { get; set; }
        public List<double> UTXO { get; set; }

        public Transaction() { }
        public Transaction(string from, string to, List<double> utxo)
        {
            From = from;
            To = to;
            UTXO = utxo;
            Id = _hashAlgorithm.ToHash(from + to + utxo.Sum());
        }

        public int GetClosestIndex(double toCompare)
        {
            return UTXO.IndexOf(UTXO.Aggregate((x, y) => Math.Abs(x - toCompare) < Math.Abs(y - toCompare) ? x : y));
        }
    }
}
