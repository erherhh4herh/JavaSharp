//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2014 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the logic to simulate Java rectangular arrays, which are jagged
//	arrays with inner arrays of the same length. A size of -1 indicates unknown length.
//----------------------------------------------------------------------------------------
internal static partial class RectangularArrays
{
    internal static int[][] ReturnRectangularIntArray(int Size1, int Size2)
    {
        int[][] Array;
        if (Size1 > -1)
        {
            Array = new int[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new int[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static double[][] ReturnRectangularDoubleArray(int Size1, int Size2)
    {
        double[][] Array;
        if (Size1 > -1)
        {
            Array = new double[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new double[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static float[][] ReturnRectangularFloatArray(int Size1, int Size2)
    {
        float[][] Array;
        if (Size1 > -1)
        {
            Array = new float[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new float[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static sbyte[][] ReturnRectangularSbyteArray(int Size1, int Size2)
    {
        sbyte[][] Array;
        if (Size1 > -1)
        {
            Array = new sbyte[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new sbyte[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static short[][] ReturnRectangularShortArray(int Size1, int Size2)
    {
        short[][] Array;
        if (Size1 > -1)
        {
            Array = new short[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new short[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static Name[][] ReturnRectangularNameArray(int Size1, int Size2)
    {
        Name[][] Array;
        if (Size1 > -1)
        {
            Array = new Name[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new Name[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static Annotation[][] ReturnRectangularAnnotationArray(int Size1, int Size2)
    {
        Annotation[][] Array;
        if (Size1 > -1)
        {
            Array = new Annotation[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new Annotation[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }
}