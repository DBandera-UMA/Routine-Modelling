using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neo4J_Routine_Modelling
{






    public class TestsManager
    {

     
        public class Node
        {
            private int id;
            private List<String> tags;

            public Node(int id, String tag)
            {
                this.Id = id;
                this.Tags = new List<string>();
                this.Tags.Add(tag);
            }

            public Node(int id, List<String> tags)
            {
                this.Id = id;
                this.Tags = tags;
            }

            public int Id { get => id; set => id = value; }
            public List<string> Tags { get => tags; set => tags = value; }
        }


        public void GenerateSyntheticTracedata(int _depth)
        {
            int depth = _depth;

            //generate initial node

            //generate random children form each node
            for (int i = 0; i < depth; i++)
            {
                //give list of children to next generation
            }


        }

        private List<Node> generateRandomChildren(List<Node> list)
        {


            var rand = new Random();

            

            //create random data

            var query = @"";
            List <Node> newList = new List<Node>();

            int choice = rand.Next(1, 2);
            if (list.Count == 1) {
                //1 node, create 1 or 2 nodes at random
                Node oldNode = list.Last();
                switch (choice)
                {
                    case 1:
                        //1 new node
                        Node newNode = new Node(oldNode.Id + 1, "Node " + oldNode.Id + 1);
                        newList.Add(newNode);
                        query = $"MERGE (P1:Process:ScalabilityTest {{name:\"Node {oldNode.Id}\"}})\n" +
                                $"MERGE (P2:Process:ScalabilityTest {{name:\"Node {newNode.Id}\"}})\n" +
                                $"MERGE (P1) - [:next] - (P2)";
                        break;
                    case 2:
                        // 2 new nodes
                        //1 new node
                        Node newNode1 = new Node(oldNode.Id + 1, "Node " + oldNode.Id + 1);
                        Node newNode2 = new Node(oldNode.Id + 2, "Node " + oldNode.Id + 2);
                        query = $"MERGE (P1:Process:ScalabilityTest {{name:\"Node {oldNode.Id}\"}})\n" +
                                $"MERGE (P2:Process:ScalabilityTest {{name:\"Node {newNode1.Id}\"}})\n" +
                                $"MERGE (P3:Process:ScalabilityTest {{name:\"Node {newNode2.Id}\"}})\n" +
                                $"MERGE (P1) - [:next] - (P2)\n" +
                                $"MERGE (P1) - [:next] - (P3)";
                        
                        newList.Add(newNode1);
                        newList.Add(newNode2);
                        break;
                }
               
            }
            else
            {
                //multiple nodes, close the nodes
                Node oldNode1 = list.First();
                Node oldNode2 = list.Last();
                Node newNode1 = new Node(oldNode2.Id + 1, "Node " + oldNode2.Id + 1);

                query = $"MERGE (P1:Process:ScalabilityTest {{name:\"Node {oldNode1.Id}\"}})\n" +
                               $"MERGE (P2:Process:ScalabilityTest {{name:\"Node {oldNode2.Id}\"}})\n" +
                               $"MERGE (P3:Process:ScalabilityTest {{name:\"Node {newNode1.Id}\"}})\n" +
                               $"MERGE (P1) - [:next] - (P3)\n" +
                               $"MERGE (P2) - [:next] - (P3)";

            }

            //execute queries

            
            return newList;

        }

        /*
        private async Task ExecuteQuery(String query)
        {
            var session = _driver.AsyncSession();
            try
            {
                // Write transactions allow the driver to handle retries and transient error
                var writeResults = await session.WriteTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query, new { node1Name, node2Name, node3Name, node4Name });

                    //return (await result.ToListAsync());
                    return (await result.ConsumeAsync());
                });

                Console.WriteLine($"Relationships created: {writeResults.Counters.RelationshipsCreated}");
                /*
                foreach (var result in writeResults)
                {
                    Console.WriteLine($"Number of keys: {result.Keys.Count}");

                    /*foreach (KeyValuePair<string, string> entry in result.)
                    {
                        Console.WriteLine($"Element found: {entry.Key}, {entry.Value}");
                    }
                    var node1 = result["p1"].As<INode>().Properties["name"];
                    var node2 = result["p2"].As<INode>().Properties["name"];
                    var node3 = result["p3"].As<INode>().Properties["name"];
                    var node4 = result["p4"].As<INode>().Properties["name"];
                    Console.WriteLine($"Created nodes: {node1}, {node2}, {node3}, {node4}");
                }
            *//**/
        /*
            }
            // Capture any errors along with the query and data for traceability
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                await session.CloseAsync();
            }
        }


*/


            

        }


    /*
        private async Task Test1CreateRelations(string nodeLabel, string relationshipLabel, string node1Name, string node2Name, string node3Name, string node4Name)
        {
            // To learn more about the Cypher syntax, see https://neo4j.com/docs/cypher-manual/current/
            // The Reference Card is also a good resource for keywords https://neo4j.com/docs/cypher-refcard/current/
            var query = @"
        MATCH (p1: TestNode { name: $node1Name })
        MATCH (p2: TestNode { name: $node2Name })
        MATCH (p3: TestNode { name: $node3Name })
        MATCH (p4: TestNode { name: $node4Name })
        MERGE (p1) - [r1: testRelationship] - (p2)
        MERGE (p1) - [r2: testRelationship] - (p2)
        MERGE (p1) - [r3: testRelationship] - (p3)
        MERGE (p1) - [r4: testRelationship] - (p4)
        MERGE (p2) - [r5: testRelationship] - (p3)
        RETURN *";

            var session = _driver.AsyncSession();
            try
            {
                // Write transactions allow the driver to handle retries and transient error
                var writeResults = await session.WriteTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query, new { node1Name, node2Name, node3Name, node4Name });

                    //return (await result.ToListAsync());
                    return (await result.ConsumeAsync());
                });

                Console.WriteLine($"Relationships created: {writeResults.Counters.RelationshipsCreated}");
                /*
                foreach (var result in writeResults)
                {
                    Console.WriteLine($"Number of keys: {result.Keys.Count}");

                    /*foreach (KeyValuePair<string, string> entry in result.)
                    {
                        Console.WriteLine($"Element found: {entry.Key}, {entry.Value}");
                    }
                    var node1 = result["p1"].As<INode>().Properties["name"];
                    var node2 = result["p2"].As<INode>().Properties["name"];
                    var node3 = result["p3"].As<INode>().Properties["name"];
                    var node4 = result["p4"].As<INode>().Properties["name"];
                    Console.WriteLine($"Created nodes: {node1}, {node2}, {node3}, {node4}");
                }
            *//*
            }
            // Capture any errors along with the query and data for traceability
            catch (Neo4jException ex)
            {
                Console.WriteLine($"{query} - {ex}");
                throw;
            }
            finally
            {
                await session.CloseAsync();
            }
        }


    }*/
}
