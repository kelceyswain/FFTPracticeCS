using System.Numerics;

// A simple Hann window
double[] HannWindow(double[] inputArray)
{
    double[] outputArray = new double[inputArray.Length];
    for (int i = 0; i < inputArray.Length; i++)
    {
        double hannFactor = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (inputArray.Length - 1)));
        outputArray[i] = hannFactor * inputArray[i];
    }
    return outputArray;
}

double CompareSignals(double[] signalInput, double[] signalOutput)
{
    if (signalInput.Length != signalOutput.Length)
    {
        throw new ArgumentException("Arrays must be equally shaped");
    }
    double error = 0.0;
    for (int i = 0; i < signalInput.Length; i++)
    {
        error += Math.Abs(signalInput[i] - signalOutput[i]);
    }
    // Normalise it to get average error
    return error/(double)signalInput.Length;
}


double AnalyzeFreqs(Complex[] freqDomain, int sampleRate, double[] freqArray, int fftSize)
{
    // Lets make some FFT bins with frequency centres
    double[] bins = new double[(fftSize / 2) + 1];
    double[] amps = new double[bins.Length];
    int nyquist = sampleRate / 2;
    double ampTarget = 0.0;
    double ampTotal = 0.0;
    for (int i = 0; i < bins.Length; i++)
    {
        bins[i] = i * sampleRate / (double)fftSize;
        amps[i] =  Math.Sqrt(Math.Pow(freqDomain[i].Real, 2) + Math.Pow(freqDomain[i].Imaginary, 2));
        foreach(double f in freqArray)
        {
            // If the bin is close to the desired frequency add its amplitude  to ampTarget
            if (Math.Abs(f - bins[i]) < (sampleRate / (2.0 * fftSize)))
            {
                ampTarget += amps[i];
            }
        }
        // Add up all the amplitudes
        ampTotal += amps[i];
    }
    return ampTarget / ampTotal;
}

Console.WriteLine("A Simple test to perform the Cooley-Tukey DFT algorithm.");
WavFile w = new WavFile(@"C:\5sec.wav");

// Lets hunt for c-sharps, of course!
int[] notes = {37, 49, 61, 73, 85, 97 };
double[] freqs = new double[notes.Length];
// Convert them to frequencies
for (int i= 0; i < notes.Length; i++)
{
    double f = 440.0 * Math.Pow(2, (notes[i] - 69) / 12.0);
    freqs[i] = f;
}

// Create two 2**n sample buffer
int fftSize = 1024;
double[] inputSamples = new double[fftSize];

DFT dft = new DFT(fftSize);

// Move half a fftSize window forward each time, skip the last few for now
for (int k = 0; k < (w.NumSamples-fftSize); k += fftSize/2)
{
    Array.Copy(w.SampleBuffer, k, inputSamples, 0, fftSize);
    // Window the samples
    inputSamples = HannWindow(inputSamples);
    // Perform the DFT
    dft.realValues = inputSamples;
    dft.ForwardDFT();
    // TODO:
    // Sort out a better detection than threshold
    // Detect onsets not continuants
    double threshold = 0.25;
    if (AnalyzeFreqs(dft.complexValues, w.SampleRate, freqs, fftSize) > threshold ) {
        Console.WriteLine($"Found a C-sharp at {k/(double)w.SampleRate} seconds");
    }
}


