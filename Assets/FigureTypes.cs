using System;
using System.Collections.Generic;
using UnityEngine;

public static class FigureTypes 
{ 

    public static List<int[,]> types = new List<int[,]>();
    public static List<Vector3> positionShifts = new List<Vector3>();

    private static bool initialized = false;
    private static System.Random random = new System.Random();
    private static Color[] figureColors =
{
        new Color(15, 212, 65, 255),
        new Color(15, 187, 212, 255),
        new Color(71, 15, 212, 255),
        new Color(209, 15, 212, 255),
        new Color(212, 15, 83, 255),
        new Color(212, 137, 15, 255),
        new Color(208, 212, 15, 255)
    };

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
            positionShifts.Add(new Vector3(46, -58));
            types.Add
                (
                    new int[,]
                        {
                        {0, 4, 0},
                        {0, 3, 0},
                        {0, 2, 1}
                        }
                );
            positionShifts.Add(new Vector3(35, -62));
            types.Add
                (
                    new int[,]
                        {
                        {0, 0, 0},
                        {4, 3, 0},
                        {0, 2, 1}
                        }
                );
            positionShifts.Add(new Vector3(56, -60));
            types.Add
                (
                    new int[,]
                        {
                        {0, 0, 0},
                        {0, 3, 4},
                        {1, 2, 0}
                        }
                );
            positionShifts.Add(new Vector3(57, -60));
            types.Add
                (
                    new int[,]
                        {
                        {0, 0, 0},
                        {1, 2, 3},
                        {0, 4, 0}
                        }
                );
            positionShifts.Add(new Vector3(51, -59));
            types.Add
                (
                    new int[,]
                        {
                        {1, 2},
                        {3, 4}
                        }
                );
            positionShifts.Add(new Vector3(37, -48));
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
            positionShifts.Add(new Vector3(74, -72));

            initialized = true;
        }
    }

    public static int GetRangdomIndex()
    {
        return random.Next(types.Count);
    }

    public static int[,] GetFigureByIndex(int index)
    {
        return types[index];
    }

    public static Color GetСolor(int index)
    {
        Color color = figureColors[index];
        Color dividedColor = new Color(color.r / 255, color.g / 255, color.b / 255, color.a / 255);
        return dividedColor;
    }

    public static Vector3 GetPositionShiftByIndex(int index)
    {
        return positionShifts[index];
    }
}