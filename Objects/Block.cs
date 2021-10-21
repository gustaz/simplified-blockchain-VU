using hash_algorithm.Logic;
using simplified_blockchain_VU.RandomGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplified_blockchain_VU.Objects
{
    public class Block
    {
        private readonly CustomHashAlgorithm _hashAlgorithm = new CustomHashAlgorithm();

        public string Hash { get; }
        public string PrevHash { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Version { get; set; }
        public string MerkleHash { get; }
        public int Nonce { get; set;  } = 0;
        public int Difficulty { get; set; }
        public bool Mined { get; set; } = false;
        public bool Transacted { get; set; } = false;
        public List<Transaction> Transactions { get; set; }

        public Block() { }
        public Block(string prevHash, DateTime timeStamp, string version, int difficulty, List<Transaction> transactions)
        {
            PrevHash = prevHash;
            TimeStamp = timeStamp;
            Version = version;
            Difficulty = difficulty;
            Transactions = transactions;

            List<string> transactionHashes = new List<string>();
            foreach(Transaction transaction in Transactions)
            {
                transactionHashes.Add(_hashAlgorithm.ToHash(transaction.Id));
            }

            MerkleHash = BuildMerkle(transactionHashes);

            Hash = _hashAlgorithm.ToHash(_hashAlgorithm.ToHash(PrevHash + timeStamp + MerkleHash) + Nonce);
        }

        private string BuildMerkle(IList<string> hashes)
        {
            if (hashes == null || !hashes.Any())
                return string.Empty;

            if (hashes.Count == 1)
                return hashes.First();

            if (hashes.Count % 2 > 0)
                hashes.Add(hashes.Last());

            List<string> merkle = new List<string>();

            for (int i = 0; i < hashes.Count; i += 2)
            {
                string leafPair = string.Concat(hashes[i], hashes[i + 1]);
                merkle.Add(_hashAlgorithm.ToHash(_hashAlgorithm.ToHash(leafPair)));
            }
            return BuildMerkle(merkle);
        }

        public void Mine()
        {
            int randomNonce = 0;
            string guessHash = _hashAlgorithm.ToHash(_hashAlgorithm.ToHash(PrevHash + TimeStamp + Version + MerkleHash) + randomNonce);

            StringBuilder adjustedHashBuilder = new StringBuilder(Hash);
            for (int i = 0; i < Difficulty; i++)
                adjustedHashBuilder[i] = '0';
            string adjustedHash = adjustedHashBuilder.ToString();

            while (!Mined)
            {
                if (guessHash.CompareTo(adjustedHash) > 0)
                {
                    randomNonce++;
                    guessHash = _hashAlgorithm.ToHash(_hashAlgorithm.ToHash(PrevHash + TimeStamp + Version + MerkleHash) + randomNonce);
                } 
                else
                {
                    Mined = true;
                    Nonce = randomNonce;
                }
            }
        }
    }
}
