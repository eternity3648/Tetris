using System;
using System.Collections.Generic;

public static class FigureTypes 
{ 

    public static List<int[,]> types = new List<int[,]>();
    private static bool initialized = false;
   
    public static void Init()
    {
        if (!initialized)
        {
            types.Add
                (
                    new int[,]
                        {
                        {0, 4, 0},
                        {0, 3, 0},
                        {1, 2, 0}
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

            initialized = true;
        }
    }

    public static int GetRangdomIndex()
    {
        Random random = new Random();
        return random.Next(types.Count);
    }

    public static int[,] GetFigureByIndex(int index)
    {
        return types[index];
    }
}