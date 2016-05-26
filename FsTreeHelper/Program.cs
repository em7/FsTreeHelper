using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FsTreeHelperLib;

namespace FsTreeHelper
{
    class Program
    {
        private class Node
        {
            public string Name;
            public Node LChild;
            public Node RChild;
        }

        static void Main(string[] args)
        {

            var tree = new Node()
            {
                Name = "Root",
                LChild = new Node()
                {
                    Name = "L"
                },
                RChild = new Node()
                {
                    Name = "R"
                }
            };

            var rootCluster = DendrogramTraversal.Cluster<Node>.Create(tree, n => n.LChild, n => n.RChild);
        }
    }
}
