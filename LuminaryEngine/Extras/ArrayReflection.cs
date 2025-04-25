namespace LuminaryEngine.Extras;

public static class ArrayReflection
{
    /// <summary>
    /// Reflects a square 2D array across the line y = -x (the anti-diagonal).
    /// For a square array of size n, the element at (i, j) in the original array
    /// is mapped to (n-1-j, n-1-i) in the reflected array.
    /// </summary>
    /// <typeparam name="T">Type of the array elements.</typeparam>
    /// <param name="matrix">The square 2D array to reflect.</param>
    /// <returns>A new square 2D array that is the reflection of the input.</returns>
    public static T[,] ReflectOverAntiDiagonal<T>(T[,] matrix)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);
        if (n != m)
            throw new ArgumentException("Matrix must be square for this reflection.");

        T[,] reflected = new T[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                // Swap rows and columns: (i, j) becomes (j, i)
                reflected[j, i] = matrix[i, j];
            }
        }

        return reflected;
    }
}