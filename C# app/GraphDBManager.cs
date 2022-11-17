using Neo4j.Driver;
using Neo4J_Routine_Modelling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Neo4J_Routine_Modelling
{
    public class GraphDBManager : IDisposable
    {
        private bool _disposed = false;
        private readonly IDriver _driver;

        private String queryToFile = "";

        public string QueryToFile { get => queryToFile; set => queryToFile = value; }

        ~GraphDBManager() => Dispose(false);

        public GraphDBManager(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

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


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _driver?.Dispose();
            }

            _disposed = true;
        }


        public async Task<IResultSummary> ExecuteQueryWithSummaryResults(string query)
        {

            var session = _driver.AsyncSession();
            try
            {
                // Write transactions allow the driver to handle retries and transient error
                var writeResults = await session.WriteTransactionAsync(async tx =>
                {
                    var result = await tx.RunAsync(query);
                    return (await result.ConsumeAsync());
                });


                //Console.WriteLine($"Relationships created: {writeResults.Counters.RelationshipsCreated}");
                //Console.WriteLine($"The full list of results is as follows:");

                return writeResults;
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


        public async Task CheckConformance()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var clause1 = @"MATCH (P1:Process:Finish)
                            MATCH (Q1:Process:Finish)
                            MERGE (P1) - [:conformsTo] -> (Q1) ";

            var clause2 = @"MATCH (P1:Process) - [r1:next|fork|join]-> (P2:Process), (P1) - [:startsWith] -> (a1:Activity)//, (P2) - [:startsWith] -> (a3:Activity)
MATCH (Q1:Process) - [r2:next|fork|join] -> (Q2:Process), (Q1) - [:startsWith] -> (a2:Activity)//, (Q2) - [:startsWith] -> (a4:Activity)
WHERE a1.name=a2.name AND id(P1)<>id(Q1)
MATCH (P2) - [:conformsTo] -> (Q2) 
WHERE   size((P1) -[:next]->()) <= size((Q1) -[:next]->()) AND 
        size((P1) -[:fork]->()) >= size((Q1) -[:fork]->()) AND 
        size((P1) -[:join]->()) >= size((Q1) -[:join]->()) 
MERGE (P1) - [:conformsTo] -> (Q1)";



            // var clause3 = "";
            // var clause4 = "";
            int relationshipsCreated = 0;


            //Execute clause 1
            var writeResults = ExecuteQueryWithSummaryResults(clause1);

            relationshipsCreated = writeResults.Result.Counters.RelationshipsCreated;
            //Console.WriteLine($"Relationships created on clause 1: {relationshipsCreated}");

            int count = 0;
            //Execute loop for clauses 1 2 and 3
            while (relationshipsCreated > 0)
            {
                count++;
                IResultSummary loopResults;
                relationshipsCreated = 0;
                //execute clauses 1 2 and 3 until none of them create any relationships

                loopResults = await ExecuteQueryWithSummaryResults(clause2);
                relationshipsCreated += loopResults.Counters.RelationshipsCreated;

                // Console.WriteLine("Executed second Query. Number of executions: " + count);
                // Console.WriteLine("Relationships created on this loop: " + relationshipsCreated +"\n");
            }
            Console.WriteLine($"----Conformance finished. Elapsed time: {sw.ElapsedMilliseconds} milliseconds----");
            

        }



        public async Task GenerateSyntheticTracedata(int _depth, string tag, string activityName)
        {
            int depth = _depth;
            Stopwatch sw;
            sw = Stopwatch.StartNew();
            //generate initial node
            var query = "";
            Node firstNode = new Node(0, "Node 0");
            query = $"MERGE (Pinit:Process:{tag} {{name:\"Node {firstNode.Id}\"}})\n" +
                    $"MERGE (Ainit:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                    $"MERGE(Pinit) - [:startsWith] - (Ainit)\n";

            //QueryToFile += query;

            //execute Query
            var results = await ExecuteQueryWithSummaryResults(query);
            List<Node> nodeList = new List<Node>();
            nodeList.Add(firstNode);
            //generate random children form each node
            for (int i = 0; i < depth; i++)
            {
                //give list of children to next generation
                List<Node> newList = await GenerateRandomChildren(nodeList, tag, activityName);
                //Console.WriteLine($"Nodes generated. Step {i} of {depth}");
                nodeList = newList;
            }

            //close the trace
            //make sure we only have 1 node at the bottom of the tree/graph
            if (nodeList.Count > 1)
            {
                List<Node> newList = await GenerateRandomChildren(nodeList, tag, activityName);
                nodeList = newList;
            }

            query = $"MERGE (P{nodeList.Last().Id}:Process:{tag} {{name:\"Node {nodeList.Last().Id}\"}})\n" +
                    $"MERGE (Pfinish:Finish:Process:{tag})\n" +
                    $"MERGE (P{nodeList.Last().Id}) - [:next] - (Pfinish)\n";
            //QueryToFile += query;
            results = await ExecuteQueryWithSummaryResults(query);
            //Console.WriteLine("Finished!");
            //Console.WriteLine($"Execution time for {_depth} nodes: {sw.ElapsedMilliseconds} milliseconds");
            Console.WriteLine($"Created sequential graph. Execution time for {_depth} nodes: {sw.ElapsedMilliseconds} milliseconds");

        }

        private async Task<List<Node>> GenerateRandomChildren(List<Node> list, String tag, String activityName)
        {
            var rand = new Random();
            //create random data

            var query = @"";
            List<Node> newList = new List<Node>();

            int choice = rand.Next(1, 2);
            if (list.Count == 1)
            {
                //1 node, create 1 or 2 nodes at random
                Node oldNode = list.Last();
                switch (choice)
                {
                    case 1:
                        //1 new node
                        Node newNode = new Node(oldNode.Id + 1, "Node " + oldNode.Id + 1);
                        newList.Add(newNode);
                        query = $"MERGE (P{oldNode.Id}:Process:{tag} {{name:\"Node {oldNode.Id}\"}})\n" +
                                $"MERGE (P{newNode.Id}:Process:{tag} {{name:\"Node {newNode.Id}\"}})\n" +
                                $"MERGE (P{oldNode.Id}) - [:next] - (P{newNode.Id})\n" +
                                $"MERGE (A{oldNode.Id}:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                                $"MERGE(P{newNode.Id}) - [:startsWith] - (A{oldNode.Id})\n";
                        break;
                    case 2:
                        //2 new nodes
                        Node newNode1 = new Node(oldNode.Id + 1, "Node " + oldNode.Id + 1);
                        Node newNode2 = new Node(oldNode.Id + 2, "Node " + oldNode.Id + 2);
                        query = $"MERGE (P{oldNode.Id}:Process:{tag} {{name:\"Node {oldNode.Id}\"}})\n" +
                                $"MERGE (P{newNode1.Id}:Process:{tag} {{name:\"Node {newNode1.Id}\"}})\n" +
                                $"MERGE (P{newNode2.Id}:Process:{tag} {{name:\"Node {newNode2.Id}\"}})\n" +
                                $"MERGE (P{oldNode.Id}) - [:next] - (P{newNode1.Id})\n" +
                                $"MERGE (P{oldNode.Id}) - [:next] - (P{newNode2.Id})\n" +
                                $"MERGE (A1:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                                $"MERGE (A{newNode2.Id}:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                                $"MERGE(P{newNode1.Id}) - [:startsWith] - (A{newNode1.Id})\n" +
                                $"MERGE(P{newNode2.Id}) - [:startsWith] - (A{newNode2.Id})\n"; ;

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

                query = $"MERGE (P{oldNode1.Id}:Process:{tag} {{name:\"Node {oldNode1.Id}\"}})\n" +
                               $"MERGE (P{oldNode2.Id}:Process:{tag} {{name:\"Node {oldNode2.Id}\"}})\n" +
                               $"MERGE (P{newNode1.Id}:Process:{tag} {{name:\"Node {newNode1.Id}\"}})\n" +
                               $"MERGE (P{oldNode1.Id}) - [:next] - (P{newNode1.Id})\n" +
                               $"MERGE (P{oldNode2.Id}) - [:next] - (P{newNode1.Id})\n" +
                               $"MERGE (A{newNode1.Id}:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                               $"MERGE(P{newNode1.Id}) - [:startsWith] - (A{newNode1.Id})\n"; ;
                newList.Add(newNode1);
            }

            //execute queries
            //QueryToFile += (query);
            var results = await ExecuteQueryWithSummaryResults(query);
            //Console.WriteLine("Results of query; Nodes created:  " + results.Counters.NodesCreated);
            return newList;

        }

        public String CreateQuerySequential(int depth, String tag, String activityName)
        {
            string query = "";
            //create first node

            query += $"MERGE (P0:Process:{tag} {{name:\"Node 0\"}})\n" +
                      $"MERGE (A0:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                      $"MERGE(P0) - [:startsWith] -> (A0)\n";
            int i;
            for (i = 0; i < depth; i++)
            {
                //create nodes

                query += $"MERGE (P{i + 1}:Process:{tag} {{name:\"Node {i + 1}\"}})\n" +
                         $"MERGE (P{i}) - [:next] -> (P{i + 1})\n" +
                         $"MERGE (A{i + 1}:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                         $"MERGE (P{i + 1}) - [:startsWith] -> (A{i + 1})\n";
            }

            //create final node
            query += $"MERGE (Pfinish:Finish:Process:{tag})\n" +
                     $"MERGE (P{i}) - [:next] -> (Pfinish)\n";

            return query;
        }

        public async Task CreateCombTestNodes(int width, String tag, String activityName)
        {
            //initial node
            string query = $"MERGE (P0:Process:{tag} {{name:\"Node 0\"}})\n" +
                 $"MERGE (A0:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                 $"MERGE(P0) - [:startsWith] -> (A0)\n";

            var results = await ExecuteQueryWithSummaryResults(query);

            //generate comb
            int i;
            for (i = 0; i < width; i++)
            {
                query = $"MERGE (P0:Process:{tag} {{name:\"Node 0\"}})\n" +
                         $"MERGE (P{i + 1}:Process:{tag} {{name:\"Node {i + 1}\"}})\n" +
                         //$"MERGE (P0) - [:fork] -> (P{i + 1})\n" +
                         $"MERGE (P0) - [:next] -> (P{i + 1})\n" +
                         $"MERGE (A{i + 1}:Activity:{tag} {{name:\"{activityName}{i + 1}\"}})\n" +
                         //$"MERGE (A{i + 1}:Activity:{tag} {{name:\"{activityName}\"}})\n" +
                         $"MERGE (P{i + 1}) - [:startsWith] -> (A{i + 1})\n" +
                         $"MERGE (Pfinish:Finish:Process:{tag})\n" +
                         //$"MERGE (P{i + 1}) - [:join] - (Pfinish)\n";
                         $"MERGE (P{i + 1}) - [:next] - (Pfinish)\n";
                results = await ExecuteQueryWithSummaryResults(query);
            }



        }

        public async Task CreateRegularDiamondNodes(int width, bool simmetrical, String tag, String activityName)
        {
            String query;
            List<Node> nodeList = new List<Node>();
            List<Node> lastRow = new List<Node>();
            List<Node> newRow = new List<Node>();

            Console.WriteLine($"Creating regular diamond pattern with tag {tag}");
            //Initial node
            Node firstNode = new Node(0, "Node 0");
            query = $"MERGE (Pinit:Process:{tag} {{name:\"Node {firstNode.Id}\"}})\n" +
                    $"MERGE (Ainit:Activity:{tag} {{name:\"{activityName} {firstNode.Id}\"}})\n" +
                    $"MERGE(Pinit) - [:startsWith] - (Ainit)\n";

            nodeList.Add(firstNode);
            lastRow.Add(firstNode);

            var results = await ExecuteQueryWithSummaryResults(query);

            //diamond pattern
            for (int n = 0; n < width; n++) 
            { 
                foreach (Node currentParentNode in lastRow)
                {
                    //for each node in the previous row, create "width" amount of nodes
                
                    for (int i = 1; i<=width; i++)
                    {
                        Node newNode = new Node(nodeList.Last().Id+1, tag);
                        nodeList.Add(newNode);
                        newRow.Add(newNode);

                        query = $"MERGE (Pparent:Process:{tag} {{name:\"Node {currentParentNode.Id}\"}})\n" +
                                $"MERGE (Pnew:Process:{tag} {{name:\"Node {newNode.Id}\"}})\n" +
                            $"MERGE (Pparent) - [:next] -> (Pnew)\n" +
                            
                            $"MERGE (Anew:Activity:{tag} {{name:\"{activityName} {newNode.Id}\"}})\n" +
                            $"MERGE (Pnew) - [:startsWith] -> (Anew)\n";

                        results = await ExecuteQueryWithSummaryResults(query);
                    }

                }

                lastRow = new List<Node>(newRow);
                newRow.Clear();
                Console.WriteLine($"Iteration {n}; Created {nodeList.Count} nodes so far...");
            }

            if (simmetrical)
            {
                //close the diamond in a simmetrical way
            }
            else
            {
                //close the diamond immediately
                foreach(Node node in lastRow)
                {
                   query = $"MERGE (P1:Process:{tag} {{name:\"Node {node.Id}\"}})\n" +
                           $"MERGE (Pfinish:Finish:Process:{tag})\n" +
                           $"MERGE (P1) - [:next] - (Pfinish)\n";

                    results = await ExecuteQueryWithSummaryResults(query);
                }

                Console.WriteLine($"Finished creating assymetrical diamond pattern. Total nodes created: {nodeList.Count}");
            }
        }

        public async Task CreateIrregularDiamondNodes(int baseNumber, bool simmetrical)
        {

        }

        public async Task CreateExactDiamondNodes(int width, int objectiveNumberOfNodes, String tag, String activityName)
        {
            bool createNodes = true;
            String query;
            List<Node> nodeList = new List<Node>();
            List<Node> lastRow = new List<Node>();
            List<Node> newRow = new List<Node>();
            int maxDepth = 0;
            int maxWidth = 0;


            Console.WriteLine($"Creating exact diamond pattern with tag {tag} and {objectiveNumberOfNodes} nodes");
            //Initial node
            Node firstNode = new Node(0, "Node 0");
            query = $"MERGE (Pinit:Process:{tag} {{name:\"Node {firstNode.Id}\"}})\n" +
                    $"MERGE (Ainit:Activity:{tag} {{name:\"{activityName} {firstNode.Id}\"}})\n" +
                    $"MERGE(Pinit) - [:startsWith] - (Ainit)\n";

            nodeList.Add(firstNode);
            lastRow.Add(firstNode);

            if (createNodes) { var results = await ExecuteQueryWithSummaryResults(query); }

            //diamond pattern
            for (int n = 0; n < width; n++)
            {
                
                
                //Check if we will go over half of the objective nodes in the next iteration of node creation.
                //We have created nodeList.Count nodes already. To check how many nodes we will create in this loop, multiply
                //number of nodes in this row by the amount of child nodes each node will have (width)
                if ((nodeList.Count + (lastRow.Count * width)) *2 <= objectiveNumberOfNodes) 
                {
                    maxWidth = maxWidth > lastRow.Count ? maxWidth : lastRow.Count;
                    //if we don't go over it, continue normally
                    foreach (Node currentParentNode in lastRow)
                    {
                        //for each node in the previous row, create "width" amount of nodes

                        for (int i = 1; i <= width; i++)
                        {
                            Node newNode = new Node(nodeList.Last().Id + 1, tag);
                            nodeList.Add(newNode);
                            newRow.Add(newNode);

                            query = $"MERGE (Pparent:Process:{tag} {{name:\"Node {currentParentNode.Id}\"}})\n" +
                                    $"MERGE (Pnew:Process:{tag} {{name:\"Node {newNode.Id}\"}})\n" +
                                $"MERGE (Pparent) - [:next] -> (Pnew)\n" +
                                //$"MERGE (Pparent) - [:fork] -> (Pnew)\n" +

                                $"MERGE (Anew:Activity:{tag} {{name:\"{activityName} {newNode.Id}\"}})\n" +
                                $"MERGE (Pnew) - [:startsWith] -> (Anew)\n";

                            if (createNodes) {var results = await ExecuteQueryWithSummaryResults(query); }
                        }

                    }

                    lastRow = new List<Node>(newRow);
                    newRow.Clear();
                    Console.WriteLine($"Iteration {n}; Created {nodeList.Count} nodes so far...");
                    maxDepth++;


                }
                else
                {
                    //If we go over it, break the loop and start second part
                    break;
                }
            }

            //second part, finishing the remaining of nodes
            int remainingNodesToCreate = (int)(objectiveNumberOfNodes / 2) - (nodeList.Count); //we get quotient and ignore remainder 

            List<Node> tempList = new List<Node>(lastRow); //temp list so we can modify lastRow without breaking the for each

            foreach (Node currentParentNode in tempList)
            {
            //for each node in the previous row, create "width" amount of nodes
            lastRow.Remove(currentParentNode);
            for (int i = 1; i <= width; i++)
                {
                    Node newNode = new Node(nodeList.Last().Id + 1, tag);
                    nodeList.Add(newNode);
                    newRow.Add(newNode);

                    query = $"MERGE (Pparent:Process:{tag} {{name:\"Node {currentParentNode.Id}\"}})\n" +
                            $"MERGE (Pnew:Process:{tag} {{name:\"Node {newNode.Id}\"}})\n" +
                        $"MERGE (Pparent) - [:next] -> (Pnew)\n" +
                       // $"MERGE (Pparent) - [:fork] -> (Pnew)\n" +
                        $"MERGE (Anew:Activity:{tag} {{name:\"{activityName} {newNode.Id}\"}})\n" +
                        $"MERGE (Pnew) - [:startsWith] -> (Anew)\n";

                    if (createNodes) {var results = await ExecuteQueryWithSummaryResults(query); }
                    
                    remainingNodesToCreate--;
                    if(remainingNodesToCreate == 0)
                    {
                        break;
                    }
                }

                if (remainingNodesToCreate == 0)
                {
                    //carry over the leftover nodes to the next row

                    newRow.AddRange(lastRow);
                    maxWidth = maxWidth > newRow.Count ? maxWidth : newRow.Count;
                    lastRow = new List<Node>(newRow);
                    newRow.Clear();
                    maxDepth++;
                    break;
                }

        }

            //duplicating the nodes, including leftover nodes 1 to 1
            foreach (Node parentNode in lastRow)
            {
                Node newNode = new Node(nodeList.Last().Id + 1, tag);
                nodeList.Add(newNode);
                newRow.Add(newNode);

                query = $"MERGE (Pparent:Process:{tag} {{name:\"Node {parentNode.Id}\"}})\n" +
                        $"MERGE (Pnew:Process:{tag} {{name:\"Node {newNode.Id}\"}})\n" +
                        $"MERGE (Pparent) - [:next] -> (Pnew)\n" +
                        $"MERGE (Anew:Activity:{tag} {{name:\"{activityName} {newNode.Id}\"}})\n" +
                        $"MERGE (Pnew) - [:startsWith] -> (Anew)\n";

                if (createNodes) { var results = await ExecuteQueryWithSummaryResults(query); }
            }
            lastRow = new List<Node>(newRow);
            newRow.Clear();
            maxDepth++;



            //closing the diamond in groups of width nodes

            while (lastRow.Count >= width) 
            {
                //grab the nodes in groups of width, and the leftover is carried to the next row, until the diamond is completely closed
                List<Node> closingList = new List<Node>();
                for(int i=0; i<width; i++)
                {
                    closingList.Add(lastRow.ElementAt(i));
                }

                Node closingNode = new Node(nodeList.Last().Id + 1, tag);
                nodeList.Add(closingNode);
                newRow.Add(closingNode);
                foreach(Node parentNode in closingList)
                {
                    query = $"MERGE (Pparent:Process:{tag} {{name:\"Node {parentNode.Id}\"}})\n" +
                           $"MERGE (Pnew:Process:{tag} {{name:\"Node {closingNode.Id}\"}})\n" +
                           $"MERGE (Pparent) - [:next] -> (Pnew)\n" +
                           //$"MERGE (Pparent) - [:join] -> (Pnew)\n" +
                           $"MERGE (Anew:Activity:{tag} {{name:\"{activityName} {closingNode.Id}\"}})\n" +
                           $"MERGE (Pnew) - [:startsWith] -> (Anew)\n";

                    if (createNodes) { var results = await ExecuteQueryWithSummaryResults(query); }
                    lastRow.Remove(parentNode);
                }

                if(lastRow.Count < width)
                {
                    newRow.AddRange(lastRow);
                    lastRow = new List<Node>(newRow);
                    newRow.Clear();
                    maxDepth++;

                }

            }

            //close remainigs with a finish
            foreach (Node parentNode in lastRow)
                {
                    query = $"MERGE (P1:Process:{tag} {{name:\"Node {parentNode.Id}\"}})\n" +
                           $"MERGE (Pfinish:Finish:Process:{tag})\n" +
                           $"MERGE (P1) - [:next] - (Pfinish)\n";
                            //$"MERGE (P1) - [:join] - (Pfinish)\n";  

                if (createNodes) { var results = await ExecuteQueryWithSummaryResults(query); }
                }


            Console.WriteLine($"Finished creating exact diamond pattern. Total nodes created: {nodeList.Count}");
            Console.WriteLine($"Depth of graph: {maxDepth}; Width of graph: {maxWidth}");

        }


    }
}
