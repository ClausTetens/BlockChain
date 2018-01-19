using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;


//  inspired by https://medium.freecodecamp.org/how-does-blockchain-really-work-i-built-an-app-to-show-you-6b70cd4caf7d

namespace BlockChain {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            generateChain();
        }


        void generateChain() {
            var blockChain = new BlockChain();
            blockChain.addBlock("Her er det ny blok");
            blockChain.addBlock("Så skal vi have flere data i en blok");
            //blockChain.hærværk();
            textBlock.Text = blockChain.ToString() +"\n isValid: "  + blockChain.isValid();
        }

        class BlockChain {
            List<BlockChainNode> blockChainNodes = new List<BlockChainNode>();

            public void hærværk() {
                //blockChainNodes.Last().index = 37;
                //blockChainNodes.Last().hash = "0010" + blockChainNodes.Last().hash.Substring(4);
                //blockChainNodes.Last().nonce--;
                //blockChainNodes.RemoveAt(1);
            }
            override public string ToString() {
                string s="";
                foreach(BlockChainNode blockChainNode in blockChainNodes) {
                    s += blockChainNode.ToString()+"\n";
                }
                return s;
            }
            ulong getLastIndex() {
                return blockChainNodes.Last().index;
            }
            string getLastHash() {
                return blockChainNodes.Last().hash;
            }
            public void addBlock(string data) {
                blockChainNodes.Add(new BlockChainNode().createBlock(data, getLastIndex()+1, getLastHash()));
            }

            public bool isValid() {
                string previousHash = "";
                ulong index = 0;
                foreach(BlockChainNode blockChainNode in blockChainNodes) {
                    if(blockChainNode.isValid(previousHash) == false)
                        return false;
                    if(blockChainNode.index != index)
                        return false;
                    index++;
                    previousHash = blockChainNode.hash;
                }
                return true;
            }

            public BlockChain() {
                blockChainNodes.Add(new BlockChainNode().createFirstBlock());
            }

        }

        class BlockChainNode {
            const int minimumLeadingZeros = 3;
            public ulong index { get; set; }
            public string previousHash { get; set; }
            public string hash { get; set; }
            public DateTime createdTime { get; set; }
            public int nonce { get; set; }
            public string data { get; set; }

            override public string ToString() {
                return "[" + index + "] previousHash " + previousHash + ", hash " + hash + ", createdTime " + createdTime + ", nonce " + nonce + ", data " + data;
            }
            public BlockChainNode() {
                createFirstBlock();
            }

            public BlockChainNode createBlock(string data, ulong index, string previousHash) {
                this.index = index;
                this.previousHash = previousHash;
                createdTime = DateTime.Now;
                this.data = data;
                createHash();
                return this;
            }
            public BlockChainNode createFirstBlock() {
                index = 0;
                previousHash = "";
                createdTime = DateTime.Now;
                data = "The very first block";
                createHash();
                return this;
            }

            int numberOfLeadingZeroes(string hash) {
                int i = 0;
                while(hash[i] == '0' && i < hash.Length)
                    ++i;
                return i;
            }


            string getHashSha256(string stringToBeHashed) {
                StringBuilder hashedString = new StringBuilder();
                foreach(byte hashedByte in new SHA256Managed().ComputeHash(Encoding.Unicode.GetBytes(stringToBeHashed))) {
                    hashedString.Append(String.Format("{0:x2}", hashedByte));
                }
                return hashedString.ToString();
            }

            public bool isValid(string previousHash) {
                if(this.previousHash != previousHash)
                    return false;
                if(numberOfLeadingZeroes(this.hash) < minimumLeadingZeros)
                    return false;
                if(this.hash != getHashSha256(this.index + this.previousHash + this.createdTime + this.data + this.nonce))
                    return false;
                return true;
            }

            void createHash() {
                nonce = -1;
                do {
                    nonce++;
                    hash = getHashSha256(index + previousHash + createdTime + data + nonce);
                } while(numberOfLeadingZeroes(hash) < minimumLeadingZeros);
            }
        }
    }
}
