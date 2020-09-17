using System;
using System.Collections.Generic;

public static class FigureTypes 
{ 

    public static List<int[,]> types = new List<int[,]>();
   
    public static void Init()
    {
        types.Add
            (
                new int[,]
                    {
                        { 0, 4, 0},
                        { 0, 3, 0},
                        { 1, 2, 0}
                    }
            );
        types.Add
            (
                new int[,]
                    {
                        {0, 4, 0},
                        {0, 3, 0},
                        {0, 2, 1}
                    }
            );
        types.Add
            (
                new int[,]
                    {
                        {0, 0, 0},
                        {4, 3, 0},
                        {0, 2, 1}
                    }
            );
        types.Add
            (
                new int[,]
                    {
                        {0, 0, 0},
                        {0, 3, 4},
                        {1, 2, 0}
                    }
            );
        types.Add
            (
                new int[,]
                    {
                        {0, 0, 0},
                        {1, 2, 3},
                        {0, 4, 0}
                    }
            );
        types.Add
            (
                new int[,]
                    {
                        {1, 2},
                        {3, 4}
                    }
            );
        types.Add
            (
                new int[,]
                    {
                        {0, 0, 0, 0},
                        {0, 0, 0, 0},
                        {1, 2, 3, 4},
                        {0, 0, 0, 0},
                    }
            );
    }

    public static int[,] GetRangdom()
    {
        Random random = new Random();
        int index = random.Next(types.Count);
        return types[index];
    }
}