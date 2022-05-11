using System;
using System.Collections.Generic;
using System.IO;

public class WavFile
{
    // Reinventing the wheel for personal education purposes.
    // https://docs.fileformat.com/audio/wav/
    public int SampleRate { get; private set; }
    public int Channels { get; private set; }
    public int BitDepth { get; private set; }
    public int NumSamples { get; private set;}
    public string FileName { get; private set; }
    public double[] SampleBuffer { get; private set; }

    public WavFile (string wavFileLocation)
    {
        if (File.Exists(wavFileLocation) == false)
        {
            throw new IOException("File does not exist");
        }

        const int POS_WAVEHEADER = 8;
        const int POS_NUMCHANS = 22;
        const int POS_SR = 24;
        const int POS_BITDEPTH = 34;
        const int POS_DATASIZE = 40;
        const int POS_DATASTART = 44;

        int dataSize;

        byte[] readFile = File.ReadAllBytes(wavFileLocation);

        byte[] fourBytes = new byte[4];
        byte[] twoBytes = new byte[2];

        Array.Copy(readFile, fourBytes, 4);
        string riffHeader = System.Text.Encoding.ASCII.GetString(fourBytes);
        

        Array.Copy(readFile, POS_WAVEHEADER, fourBytes, 0, 4);
        string waveHeader = System.Text.Encoding.ASCII.GetString(fourBytes);

        if (riffHeader != "RIFF" || "WAVE" != "WAVE")
        {
            throw new IOException("Wrong file type");
        }
        // Make a note of the file name for later
        FileName = wavFileLocation;
        // Sample rate
        Array.Copy(readFile, POS_SR, fourBytes, 0, 4);
        SampleRate = BitConverter.ToInt32(fourBytes, 0);
        // Data size
        Array.Copy(readFile, POS_DATASIZE, fourBytes, 0, 4);
        dataSize = BitConverter.ToInt32(fourBytes, 0);
        // Channel count
        Array.Copy(readFile, POS_NUMCHANS, twoBytes, 0, 2);
        Channels = twoBytes[0] | twoBytes[1] << 8;
        // Bit rate
        Array.Copy(readFile, POS_BITDEPTH, twoBytes, 0, 2);
        BitDepth = twoBytes[0] | twoBytes[1] << 8;
        Console.WriteLine("=================================");
        Console.WriteLine($"{FileName}");
        Console.WriteLine("=================================");
        Console.WriteLine($"{riffHeader} - {waveHeader}");
        Console.WriteLine($"Sample rate:\t\t{SampleRate}");
        Console.WriteLine($"Channels:\t\t{Channels}");
        Console.WriteLine($"BitDepth:\t\t{BitDepth}");

        // I only need one channel to analyse so I will just take channel 1
        int bufferSize = (dataSize * 8) / (Channels * BitDepth);
        NumSamples = bufferSize;
        // Lets just work with doubles for now and not worry about 8-bit
        double[] buffer = new double[bufferSize];
        for (int i = 0; i < bufferSize; i++)
        {
            byte[] sample = new byte[BitDepth/8];
            Array.Copy(readFile, POS_DATASTART + (i * Channels * (BitDepth/8)), sample, 0, BitDepth/8);

            if (BitDepth == 16)
            {
                buffer[i] = BitConverter.ToInt16(new byte[] { sample[1], sample[0] }, 0) / 32767.0;
            }
            else if (BitDepth == 24)
            {
                //TODO
            }
            else if (BitDepth == 8)
            {
                //TODO
            }
        }
        SampleBuffer = buffer;
    }
}
