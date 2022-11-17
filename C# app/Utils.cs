using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neo4J_Routine_Modelling
{
    public class Utils
    {

        public (int n, int numberOfNodes) CalculateRegularDiamond(int desiredNumberOfNodes, bool simmetrical)
        {
            //calculate min(n) so sum(n^i) >= X from i = 0 to i = n, where X is desired number of nodes

            int i;
            int n = 0;
            int numberOfNodes = 0;
            do
            {
                n++;
                numberOfNodes = 1; //count the initial node since we only want to calculate number of hops from the starting node
                for (i = 1; i <= n; i++)
                {
                    numberOfNodes += (int) Math.Pow(n, i);
                }

                if (simmetrical)
                {
                    i -= 1; //the last check of the for loop increases i. We don't want that
                    numberOfNodes += (numberOfNodes - (int) Math.Pow(n, i));
                }

            } while (numberOfNodes < desiredNumberOfNodes);


            return (n, numberOfNodes);
        }

        public (int n, int numberOfNodes) CalculateIrregularDiamond(int baseNumber, int desiredNumberOfNodes, bool simmetrical)
        {
            int i = -1;
            int numberOfNodes = 0;
            int check = 0;
            // variable to do the check so we don't multiply the "previous" nodes by 2 several times, carrying it over on each iteration.
            do
            {
                i++;
                if (simmetrical)
                {
                    check = numberOfNodes * 2;
                }
                else
                {
                    check = numberOfNodes;
                }

                numberOfNodes += (int)Math.Pow(baseNumber, i);
                check += (int)Math.Pow(baseNumber, i);

            } while (check < desiredNumberOfNodes);

            numberOfNodes = check + (int)Math.Pow(baseNumber, i);

            return (i, numberOfNodes);
        }
    }
}
