using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._1268._Search_Suggestions_System
{
    public class Solution
    {
        public IList<IList<string>> SuggestedProducts(string[] products, string searchWord)
        {
            Array.Sort(products);
            Node top = new();
            Node currentNode = top;

            foreach (var product in products)
            {
                foreach (var c in product)
                {

                    var isExit = currentNode.Nodes.ContainsKey(c);

                    if (isExit)
                    {
                        currentNode = currentNode.Nodes[c];
                        //continue;
                    }

                    if (!isExit)
                    {
                        var newNode = new Node { Char = c };
                        currentNode.Nodes.Add(c, newNode);
                        currentNode = newNode;
                    }

                    if (currentNode.Words.Count < 3)
                    {
                        currentNode.Words.Add(product);
                    }
                }
                currentNode = top;
            }

            var ans = new List<IList<string>>();
            foreach (var c in searchWord)
            {
                var isExit = currentNode.Nodes.ContainsKey(c);
                if (isExit)
                {
                    currentNode = currentNode.Nodes[c];
                    ans.Add(currentNode.Words);
                    continue;
                }

                if (!isExit)
                {
                    ans.Add(new List<string>());
                }
            }
            return ans;
        }

        public class Node
        {
            public char Char { get; set; }
            public List<string> Words { get; set; } = new List<string>();
            public Dictionary<char, Node> Nodes { get; set; } = new Dictionary<char, Node>();
        }
    }
}
