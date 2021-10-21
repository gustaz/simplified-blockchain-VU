using hash_algorithm.Logic;
using simplified_blockchain_VU.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace simplified_blockchain_VU.RandomGen
{
    public class User
    {
        public string Name { get; set; }
        public string PublicKey { get; set; }
        public List<double> UTXO { get; set; }

        public User() { }
        public User(string name, string publicKey, List<double> utxo)
        {
            Name = name;
            PublicKey = publicKey;
            UTXO = utxo;
        }

        public double GetBalance()
        {
            return UTXO.Sum();
        }
    }
}
