using hash_algorithm.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace simplified_blockchain_VU.RandomGen
{
    public class User
    {
        public string Name { get; set; }
        public string PublicKey { get; set; }
        public double Balance { get; set; }

        public User() { }
        public User(string name, string publicKey, double balance)
        {
            Name = name;
            PublicKey = publicKey;
            Balance = balance;
        }
    }
}
