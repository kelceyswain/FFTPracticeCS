using System;
using System.Numerics;

public class DFT
{
	public double[] realValues;
	public Complex[] complexValues;
    public double[] outValues;
    private double twopi = Math.PI * 2.0;

	public DFT(double[] inputArray)
	{
		realValues = inputArray;
        complexValues = new Complex[realValues.Length];
        outValues = new double[realValues.Length];
	}

	public void ForwardDFT()
    {        
        int N = realValues.Length;
        // temporary buffer
        Complex[] X = new Complex[N];

        if (N <= 0)
        {
            throw new IndexOutOfRangeException("Invalid signal length.");
        }
        // Is the array size a power of two?
        if ((N & (N - 1)) != 0)
        {
            throw new ArgumentException("Signal size must be a power of 2, or zero padded.");
        }
        // We really only need (N/2)+1 because it is a 1d array, and therefore reflects
        for (int i=0; i<N; i++)
        {
            X[i] = new Complex(0, 0);
            for (int n=0; n<N; n++)
            {
                double realPart = realValues[n] * Math.Cos(i*n*twopi/N);
                double imagPart = -1 * realValues[n] * Math.Sin(i*n*twopi/N);
                X[i] += new Complex(realPart, imagPart);
            }
            X[i] = X[i] / N;
        }
		complexValues = X;
    }

	public void InverseDFT()
    {
        // Must have made the complex values first!
        int N = realValues.Length;
        // Temporary buffer
        double[] workingOut = new double[N];
        
        for (int i=0; i<N; i++)
        {
            workingOut[i] = 0.0;
            for (int n=0; n<N; n++)
            {
                double realPart = complexValues[n].Real * Math.Cos(i*n*twopi/N);
                double imagPart = complexValues[n].Imaginary * Math.Sin(i*n*twopi/N);
                workingOut[i] += realPart - imagPart;
            }
        }
        // And set the final values;
        outValues = workingOut;
    }

}
