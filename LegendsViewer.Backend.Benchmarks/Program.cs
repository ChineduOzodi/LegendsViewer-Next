using System;
using System.Collections.Generic;
using LegendsViewer.Backend.Benchmarks;
using LegendsViewer.Backend.Benchmarks.Legends.Events;
using BenchmarkDotNet.Running;

namespace LegendsViewer.Backend.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        // Register code page provider for legacy encodings (e.g., CP437)
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        
        // Run only AddHfEntityHonorBenchmarks
        BenchmarkRunner.Run<AddHfEntityHonorBenchmarks>();
    }
}
