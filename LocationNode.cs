using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using DBNamespace;
using System.Drawing;
using  eMap;
using locationSpace;

namespace GraphSpace
{
    class LocationNode
    {
        int V;
        int max=1000000;
        //LinkedList<int>[] list;
        int [,]matrix;
        public LocationNode(int v)
        {
            V = v;
            /*list = new LinkedList<int>[v];
            for (int i = 0; i < V; i++)
                list[i] = new LinkedList<int>();*/
            matrix = new int[V, V];
            for (int i = 0; i < V; i++)
                for (int j = 0; j < V; j++)
                    matrix[i, j] = 0;
        }
        public void addEdge(int source, int dest,Location []locationArray)
        {
            /*list[source].AddLast(dest);
            list[dest].AddLast(source);*/
            //Form1 f=new Form1();

            int weight =(int) Math.Sqrt(Math.Pow(locationArray[source].xcord - locationArray[dest].xcord, 2) + Math.Pow(locationArray[source].ycord - locationArray[dest].ycord, 2));
            ;

            matrix[source, dest] = weight;
            matrix[dest, source] = weight;

        }
        public string  traverse()
        {
            //Console.WriteLine();
          
            string str = " ";
            for (int i = 0; i < V; i++)
            {
                for (int j = 0; j < V; j++)
                    str = str+ " "+ matrix[i, j];
                str = str + "\n";
            }
            return str;
        }
        /*
        public override string ToString()
        {
            string s=" ";
            for (int j = 0; j < V; j++)
            {
                foreach (int i in this.list[j])
                    s = s + " " + i;
                s = s + "\n";
            }
            return s;
        }*/
        public void buildGraph()
        {

            databaseConnectivity db = new databaseConnectivity();
            db.connectToDatabase();
           // db.readEdgeTable(this);
            
        }



 public int minDistance(int []dist, bool []sptSet)
{
   // Initialize min value
   int min = max, min_index=0;

   for (int v = 0; v < V; v++)
       if (sptSet[v] == false && dist[v] <= min)
       {
           min = dist[v];
           min_index = v;
       }

   return min_index;
}


  public void  dijkstra(int []parent,int src)
  {
   
    int[] dist = new int[V];    // The output array.  dist[i] will hold the shortest
                      // distance from src to i

     bool []sptSet=new bool[V]; // sptSet[i] will true if vertex i is included in shortest
                     // path tree or shortest distance from src to i is finalized

     // Initialize all distances as INFINITE and stpSet[] as false
      
     for (int i = 0; i < V; i++)
     {
         dist[i] = max;
         sptSet[i] = false;
     }
     // Distance of source vertex from itself is always 0
     dist[src] = 0;
     parent[0] = -1;
     // Find shortest path for all vertices
     for (int count = 0; count < V-1; count++)
     {
       // Pick the minimum distance vertex from the set of vertices not
       // yet processed. u is always equal to src in first iteration.
       int u = minDistance(dist, sptSet);

       // Mark the picked vertex as processed
       sptSet[u] = true;
       
       // Update dist value of the adjacent vertices of the picked vertex.
       for (int v = 0; v < V; v++)

         // Update dist[v] only if is not in sptSet, there is an edge from 
         // u to v, and total weight of path from src to  v through u is 
         // smaller than current value of dist[v]
           if (!sptSet[v] && matrix[u, v] != 0 && dist[u] != max
                                         && dist[u] + matrix[u, v] < dist[v])
           {
               dist[v] = dist[u] + matrix[u, v];
               parent[v] = u;
           }
     }
}

       
}

    // return 1;
}