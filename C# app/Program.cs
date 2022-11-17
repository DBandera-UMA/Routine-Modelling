using Neo4j.Driver;
using Neo4J_Routine_Modelling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    enum testTypes { SEQUENTIAL, COMB, DIAMOND };

    public static async Task Main(string[] args)
    {
  

    var uri = "URI";
    var user = "USER";
    var password = "PASSWORD";

    


        Utils utils = new Utils();

        using (var DBManager = new GraphDBManager(uri, user, password))
        {

            bool onlyConformance = false;
           // int numberOfNodes = 20; //number of nodes to create

            List<int> testValuesToCheck = new List<int>{ 10, 20, 50, 100, 200, 500, 1000, 2000 };
            //List<int> testValuesToCheck = new List<int> { 10, 20, 50, 100, 200, 500 };

            //delete previous data and conformance relationships
            String deleteConformanceQuery = "MATCH (n) - [r:conformsTo] - (m) DELETE r";
            await DBManager.ExecuteQueryWithSummaryResults(deleteConformanceQuery);

            String nodeTag1 = "ScalabilityTest1:second";
            String nodeTag2 = "ScalabilityTest2:second";


            //---Generate test data---

            if (!onlyConformance)
            {
                

                foreach(int numberOfNodes in testValuesToCheck)
                {
                    String deleteTestData = "match (n) where (n:ScalabilityTest1) OR (n:ScalabilityTest2) OR (n:ScalabilityTest3) OR (n:ScalabilityTest4) detach delete n";
                    Console.WriteLine("Deleting previous test data...");
                    await DBManager.ExecuteQueryWithSummaryResults(deleteConformanceQuery);
                    await DBManager.ExecuteQueryWithSummaryResults(deleteTestData);

                    Console.WriteLine($"Testing for {numberOfNodes} nodes:");
                    
                   switch (testTypes.DIAMOND) 
                    { 
                        case testTypes.SEQUENTIAL:

                            //Multiple queries
                            await DBManager.GenerateSyntheticTracedata(numberOfNodes, nodeTag1 + ":Test" + numberOfNodes + "Nodes", "Activity");
                            await DBManager.GenerateSyntheticTracedata(numberOfNodes, nodeTag2 + ":Test" + numberOfNodes + "Nodes", "Activity");

                            //Single query
                            //String query = DBManager.CreateQuerySequential(numberOfNodes, "ScalabilityTest1:Test200", "Activity");
                            //await File.WriteAllTextAsync("GeneratedQuery.txt", query);
                            //Stopwatch singleQueryStopwatch = new Stopwatch();
                            //singleQueryStopwatch.Start();
                            //await DBManager.ExecuteQueryWithSummaryResults(query);
                            //Console.WriteLine($"Created {numberOfNodes} nodes in a single query. Time elapsed: {singleQueryStopwatch.ElapsedMilliseconds} milliseconds");


                            //Second trace

                            //String query2 = DBManager.CreateQuerySequential(numberOfNodes, "ScalabilityTest2:Test200", "Activity");
                            //singleQueryStopwatch.Restart();
                            //await DBManager.ExecuteQueryWithSummaryResults(query2);
                            //Console.WriteLine($"Second query; Created {numberOfNodes} nodes in a single query. Time elapsed: {singleQueryStopwatch.ElapsedMilliseconds} milliseconds");

                            break;

                        case testTypes.COMB:
                            /**Comb*/
                            Stopwatch combQueryStopwatch = new Stopwatch();
                            combQueryStopwatch.Start();
                            await DBManager.CreateCombTestNodes(numberOfNodes, nodeTag1 + ":Test" + numberOfNodes + "Nodes", "Activity");
                            Console.WriteLine($"Created {numberOfNodes} nodes in a comb pattern. Time elapsed: {combQueryStopwatch.ElapsedMilliseconds} milliseconds");

                            await DBManager.CreateCombTestNodes(numberOfNodes, nodeTag2 + ":Test" + numberOfNodes + "Nodes", "Activity");

                            break;

                        case testTypes.DIAMOND:
                            /**Diamond*/
                            //Find the number of iterations needed for the amount of nodes we want to generate
                            var results = utils.CalculateRegularDiamond(numberOfNodes, true);
                            Console.WriteLine($" Calculated regular diamond, simmetrical. Number of iterations (max width): {results.n}. Total number of nodes generated: {results.numberOfNodes}");
                        
                            //create nodes regular diamond no simmetrical
                            //await DBManager.CreateRegularDiamondNodes(results.n, false, "ScalabilityTest1", "Activity");
                            //await DBManager.CreateRegularDiamondNodes(results.n, false, "ScalabilityTest2", "Activity");
                            //await DBManager.CreateRegularDiamondNodes(3, false, "ScalabilityTest1", "Activity");

                            // results = utils.CalculateRegularDiamond(numberOfNodes, true);
                            // Console.WriteLine($" Calculated regular diamond, simmetrical. Number of iterations (max width): {results.n}. Total number of nodes generated: {results.numberOfNodes}");
                            //create nodes regular diamond simmetrical

                            //  results = utils.CalculateIrregularDiamond(2, numberOfNodes, false);
                            //   Console.WriteLine($" Calculated regular diamond, simmetrical. Number of iterations (max width): {results.n}. Total number of nodes generated: {results.numberOfNodes}");
                            //create nodes irregular diamond no simmetrical

                            //  results = utils.CalculateIrregularDiamond(2, numberOfNodes, true);
                            //   Console.WriteLine($" Calculated regular diamond, simmetrical. Number of iterations (max width): {results.n}. Total number of nodes generated: {results.numberOfNodes}");
                            //create nodes irregular diamond simmetrical

                             await DBManager.CreateExactDiamondNodes(results.n, numberOfNodes, nodeTag1 + ":Test" + numberOfNodes + "Nodes", "Activity");
                             await DBManager.CreateExactDiamondNodes(results.n, numberOfNodes, nodeTag2 + ":Test" + numberOfNodes + "Nodes", "Activity");

                            break;
                    }

                    //execute conformance algorithm
                    await DBManager.CheckConformance();
                    Console.WriteLine("");
                }
            }
            else
            {
                await DBManager.CheckConformance();
            }




        }
    }
}