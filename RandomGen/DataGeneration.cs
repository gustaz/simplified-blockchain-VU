using hash_algorithm.Logic;
using simplified_blockchain_VU.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace simplified_blockchain_VU.RandomGen
{
    public class DataGeneration
    {
        private readonly Random _random = new Random();
        private readonly CustomHashAlgorithm _hashAlgorithm = new CustomHashAlgorithm();

        public Tuple<List<User>, List<Transaction>> GenerateData(int no)
        {
            List<User> users = new List<User>();
            List<Transaction> transactions = new List<Transaction>();

            for (int i = 0; i < no; i++)
            {
                List<double> UTXOs = new List<double>();
                for (int j = 0; j < RandomNumber(10) + 1; j++)
                    UTXOs.Add(Math.Round(RandomFloat(10000), 2));

                User user = new User(RandomString(RandomNumber(32)), _hashAlgorithm.ToHash(RandomString(512)), UTXOs);
                users.Add(user);
            }

            for (int i = 0; i < no * 100; i++)
            {
                string from = users[RandomNumber(no)].PublicKey;
                string to = users[RandomNumber(no)].PublicKey;

                List<double> UTXOs = new List<double>();
                for (int j = 0; j < RandomNumber(10) + 1; j++)
                    UTXOs.Add(Math.Round(RandomFloat(10000), 2));

                Transaction transaction = new Transaction(from, to, UTXOs);
                transactions.Add(transaction);
            }

            Tuple<List<User>, List<Transaction>> result = new Tuple<List<User>, List<Transaction>>(users, transactions);
            return result;
        }

        public int RandomNumber(int max)
        {
            return _random.Next(max);
        }

        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public double RandomFloat(int max)
        {
            return RandomNumber(max) + Math.Round(_random.NextDouble(), 2);
        }

        public string RandomString(int size)
        {

            var builder = new StringBuilder(size);

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next('a', 'z');
                builder.Append(@char);
            }

            return builder.ToString();
        }
    }
}
