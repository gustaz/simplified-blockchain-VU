using simplified_blockchain_VU.Objects;
using simplified_blockchain_VU.RandomGen;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using hash_algorithm.Logic;

namespace simplified_blockchain_VU
{
    class Program
    {
        static void Main(string[] args)
        {
            DataGeneration gen = new DataGeneration();
            CustomHashAlgorithm hashGenerator = new CustomHashAlgorithm();

            Tuple<List<User>, List<Transaction>> data = gen.GenerateData(1000);
            List<User> users = data.Item1;
            List<Transaction> transactions = data.Item2;

            List<Block> blocks = new List<Block>();
            while(transactions.Count > 0)
            {
                string prevHash;
                if (blocks.Count == 0) prevHash = "0";
                else prevHash = blocks[^1].Hash;

                int getTransactions;
                if (transactions.Count > 100)
                    getTransactions = gen.RandomNumber(99) + 1;
                else getTransactions = transactions.Count;
                Block newBlock = new Block(prevHash, DateTime.Now, "1", 2, transactions.GetRange(0, getTransactions));
                newBlock.Mine();

                if(newBlock.Mined)
                {

                    Console.WriteLine("New block mined!" +
                        "\nHash: {0}" +
                        "\nTimeStamp: {1}" +
                        "\nVersion: {2}" +
                        "\nDifficulty rating: {3}" +
                        "\nNonce: {4}" +
                        "\nHeight: {5}\n",
                        newBlock.Hash, newBlock.TimeStamp, newBlock.Version, newBlock.Difficulty, newBlock.Nonce, blocks.Count);

                    List<string> generalIds = new List<string>();
                    List<string> invalidIds = new List<string>();

                    int ctr = 1;
                    foreach (Transaction transaction in newBlock.Transactions)
                    {
                        int from = users.FindIndex(x => x.PublicKey == transaction.From);
                        int to = users.FindIndex(x => x.PublicKey == transaction.To);
                        
                        if (users[from].Balance >= transaction.Value && hashGenerator.ToHash(transaction.From + transaction.To + transaction.Value) == transaction.Id)
                        {
                            users[from].Balance -= transaction.Value;
                            users[to].Balance += transaction.Value;

                            Console.WriteLine("#{0}: From: {1}, To: {2}, Sending: {3}", ctr, users[from].PublicKey, users[to].PublicKey, transaction.Value);
                            ctr++;
                        }
                        else invalidIds.Add(transaction.Id);
                        generalIds.Add(transaction.Id);
                    }
                    transactions.RemoveAll(x => generalIds.Contains(x.Id));
                    newBlock.Transactions.RemoveAll(x => invalidIds.Contains(x.Id));
                    newBlock.Transacted = true;

                }
                blocks.Add(newBlock);
                Console.WriteLine("Total transactions: {0}\n", newBlock.Transactions.Count);
            }
            
            
        }
    }
}
