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

Console.WriteLine("A Simple test to perform the Cooley-Tukey DFT algorithm.");
// Instantiate Random
Random rand = new Random();
// Create two 2**n sample buffer
int sampleCount = 1024;
double[] inputSamples = new double[sampleCount];

for (int i = 0; i < inputSamples.Length; i++)
{
    inputSamples[i] = rand.NextDouble();
}

// Window the samples
inputSamples = HannWindow(inputSamples);

Console.WriteLine(String.Join("\t", inputSamples));

DFT dft = new DFT(inputSamples);

// Perform the DFT
dft.ForwardDFT();
Console.WriteLine("======================");
Console.WriteLine(String.Join("\t", dft.complexValues));

// Perform the iDFT
dft.InverseDFT();
Console.WriteLine("======================");
Console.WriteLine(String.Join("\t", dft.outValues));

// Add up the errors and see how we did
double e = CompareSignals(inputSamples, dft.outValues);
Console.WriteLine(e);

WavFile w = new WavFile(@"C:\bathtub.wav");
