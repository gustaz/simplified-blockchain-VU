# VU Simplified Blockchain 

## What it is

A simulation of a blockchain written in C#.

## How it works

### Data generation

The Program initially generates a set of 1 000 users, each with a total of up to 10 UTXOs each. Afterwards, a total of 10 000 transactions between the users are generated. Everything is done using a pseudo-random number generator. 

### Sanity-check data #1

After data generation is done, sanity-check data in the form of three outputs is shown to the user:
```
Total: ###
Max: ###
Min: ###
```
Where Total shows the total amount of coins in circulation, max shows the balance of the richest user and min shows the balance of the poorest user.

### Transactions

#### Candidate block

After the sanity-check output, five candidate blocks are generated. Each one has transactions added to them (which are able to overlap) and then they are all mined in parallel. The desired result is for one of the five blocks to be mined, after which point the mined block is considered a candidate.

#### Making the transactions happen

Once the candidate block is obtained, UTXO summation operations and hash checking is performed in order to determine if a given transaction is valid. If it is not valid, no operations are performed and the given transaction is dismissed. Otherwise, UTXO operations are performed and the desired amount is sent to the receiver, with UTXOs being produced for the sender depending on their balance. This constitutes a valid transaction. All transactions in the block are checked this way. Afterwards, all valid and invalid transactions are dropped from the main transactions list and the block is set as transacted. If a block is both mined and transacted, then it is successfully added to the blockchain.

### Sanity-check data #2

After adding all blocks to the block-chain, sanity-check data in the form of three outputs is shown to the user:
```
Total: ###
Max: ###
Min: ###
```
Where Total shows the total amount of coins in circulation, max shows the balance of the richest user and min shows the balance of the poorest user. This is done to ensure no currency appeared out of nowhere and to check what amounts the richest and poorest hold respectively.

### Merkle check

As an added bonus, the program performs calculation of Merkle root hashes, which fully conforms to the actual merkle root generated in bitcoin blocks.
