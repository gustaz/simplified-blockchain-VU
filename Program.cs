using simplified_blockchain_VU.Objects;
using simplified_blockchain_VU.RandomGen;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using hash_algorithm.Logic;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Dynamic;

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

            // merkle hash implementation demo
            const string blockchainApi = "https://blockchain.info/rawblock/000000000021a5a985c8b3e1abd2c4e36e1e0b85170426ffa89f28088add6520";
            dynamic blockData = FetchBlockData(blockchainApi);
            List<string> transactionHashes = new List<string>();

            foreach (dynamic transaction in blockData.tx)
            {
                transactionHashes.Add(SwapAndReverse(transaction.hash));
            }

            string obtainedMarkle = SwapAndReverse(BuildMerkle(transactionHashes)).ToLower();
            Console.WriteLine(obtainedMarkle == blockData.mrkl_root ? "Hash'as atitiko" : "Hash'as neatitiko");

            double max = 0;
            double min = Int32.MaxValue;
            int minIndex = -1;
            int maxIndex = -1;

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].GetBalance() > max)
                {
                    max = users[i].GetBalance();
                    maxIndex = i;
                }
                if (users[i].GetBalance() < min)
                {
                    min = users[i].GetBalance();
                    minIndex = i;
                }
            }

            double sum = 0;
            foreach (User useris in users)
            {
                sum += useris.GetBalance();
            }

            Console.WriteLine("Total: {0}", sum);
            Console.WriteLine("Max: {0}", users[maxIndex].GetBalance());
            Console.WriteLine("Min: {0}", users[minIndex].GetBalance());
            Console.WriteLine("Start mining.");

            while (transactions.Count > 0)
            {
                int ctr = 1;
                List<Block> blockCandidates = new List<Block>();
                Block newBlock = new Block();

                for(int i = 0; i < 4; i++)
                {
                    string prevHash;
                    if (blocks.Count == 0) prevHash = "0000000000000000000000000000000000000000000000000000000000000000";
                    else prevHash = blocks[^1].Hash;

                    int getTransactions;
                    if (transactions.Count >= 1000)
                        getTransactions = gen.RandomNumber(1000) + 1;
                    else getTransactions = transactions.Count;
                    Block candidate = new Block(prevHash, DateTime.Now, "1", 1, transactions.GetRange(0, getTransactions));

                    blockCandidates.Add(candidate);
                }

                Parallel.ForEach(blockCandidates, (blockCandidate, state) =>
                {
                    blockCandidate.Mine();

                    foreach (Block candidate in blockCandidates)
                        if (candidate.Mined)
                        {
                            newBlock = candidate;
                            state.Break();
                            break;
                        }
                });

                if(newBlock.Mined)
                {
                    List<string> generalIds = new List<string>();
                    List<string> invalidIds = new List<string>();

                    ctr = 1;
                    foreach (Transaction transaction in newBlock.Transactions)
                    {
                        int from = users.FindIndex(x => x.PublicKey == transaction.From);
                        int to = users.FindIndex(x => x.PublicKey == transaction.To);
                        
                        if (hashGenerator.ToHash(transaction.From + transaction.To + transaction.UTXO.Sum()) == transaction.Id && users[from].GetBalance() >= transaction.UTXO.Sum())
                        {
                            transaction.UTXO.Sort();
                            users[from].UTXO.Sort();
                            bool transact = false;

                            double summed = 0;

                            for(int i = 0; i < users[from].UTXO.Count; i++)
                            {
                                if (summed >= transaction.UTXO.Sum())
                                {
                                    transact = true;
                                    if (summed > transaction.UTXO.Sum())
                                        users[from].UTXO.Add(summed - transaction.UTXO.Sum());

                                    users[to].UTXO.Add(transaction.UTXO.Sum()); 
                                    users[from].UTXO.RemoveRange(0, i);
                                    break;
                                }
                                else summed += users[from].UTXO[i];
                            }

                            if (transact)
                            {
                                Console.WriteLine("#{0}: From: {1}, To: {2}, Sending: {3}", ctr, users[from].PublicKey, users[to].PublicKey, Math.Round(summed, 2));
                                ctr++;
                            }
                        }
                        else invalidIds.Add(transaction.Id);
                        generalIds.Add(transaction.Id);
                    }
                    transactions.RemoveAll(x => generalIds.Contains(x.Id));
                    newBlock.Transactions.RemoveAll(x => invalidIds.Contains(x.Id));
                    if(ctr != 1) newBlock.Transacted = true;
                }

                if (ctr != 1)
                {
                    ctr--;
                    blocks.Add(newBlock);
                    Console.WriteLine(
                    "\nHash: {0}" +
                    "\nPrevious hash: {1}" +
                    "\nTimeStamp: {2}" +
                    "\nVersion: {3}" +
                    "\nDifficulty rating: {4}" +
                    "\nNonce: {5}" +
                    "\nHeight: {6}\n",
                    newBlock.Hash, newBlock.PrevHash, newBlock.TimeStamp, newBlock.Version, newBlock.Difficulty, newBlock.Nonce, blocks.Count);
                    Console.WriteLine("Total transactions: {0}\n", ctr);
                }
            }

            max = 0;
            min = Int32.MaxValue;
            maxIndex = -1;
            minIndex = -1;

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].GetBalance() > max)
                {
                    max = users[i].GetBalance();
                    maxIndex = i;
                }
                if (users[i].GetBalance() < min)
                {
                    min = users[i].GetBalance();
                    minIndex = i;
                }
            }

            sum = 0;
            foreach (User useris in users)
            {
                sum += useris.GetBalance();
            }

            Console.WriteLine("Total: {0}", sum);
            Console.WriteLine("Max: {0}", users[maxIndex].GetBalance());
            Console.WriteLine("Min: {0}", users[minIndex].GetBalance());

        }
        private static string BuildMerkle(List<string> transacHashes)
        {
            if (transacHashes == null || !transacHashes.Any())

                return string.Empty;

            if (transacHashes.Count() == 1)
            {
                return transacHashes.First();
            }

            if (transacHashes.Count() % 2 > 0)
            {
                transacHashes.Add(transacHashes.Last());
            }

            List<string> merkleHashes = new List<string>();

            for (int i = 0; i < transacHashes.Count(); i += 2)
            {
                string leafPair = string.Concat(transacHashes[i], transacHashes[i + 1]);
                merkleHashes.Add(Hash(Hash(leafPair)));
            }
            return BuildMerkle(merkleHashes);
        }

        private static string Hash(string data)
        {
            using SHA256 sha256 = SHA256.Create();
            return ComputeHash(sha256, HexToByteArray(data));

        }

        private static string ComputeHash(HashAlgorithm hashAlgorithm, byte[] input)
        {
            byte[] data = hashAlgorithm.ComputeHash(input);
            return ByteArrayToHex(data);
        }

        private static string ByteArrayToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        private static byte[] HexToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        private static string SwapAndReverse(string input)
        {
            string newString = string.Empty; ;
            for (int i = 0; i < input.Count(); i += 2)
            {
                newString += string.Concat(input[i + 1], input[i]);
            }
            return ReverseString(newString);
        }

        private static string ReverseString(string original)
        {
            return new string(original.Reverse().ToArray());
        }

        private static dynamic FetchBlockData(string path)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<ExpandoObject>(json);
                }
            }
            return null;
        }
    }
}
