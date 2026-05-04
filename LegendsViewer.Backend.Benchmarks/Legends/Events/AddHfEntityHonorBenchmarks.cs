using System;
using System.Collections.Generic;
using System.Text;
using LegendsViewer.Backend.Legends;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using BenchmarkDotNet.Attributes;

namespace LegendsViewer.Backend.Benchmarks.Legends.Events;

[MemoryDiagnoser]
[ShortRunJob]
public class AddHfEntityHonorBenchmarks
{
    private World _world = null!;
    private Entity _entity = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Honor _honor = null!;
    private List<Property> _properties = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        _world = new World();
        
        // Create test entity
        _entity = new Entity([], _world)
        {
            Id = 1,
            Name = "Test Entity",
            Icon = "civilization"
        };
        
        // Create test historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Baron Urist McHero",
            Icon = "human"
        };
        
        // Create test honor
        _honor = new Honor([], _world, _entity)
        {
            Id = 42,
            Name = "Knight of the Deep",
            RequiredBattles = 5,
            RequiredKills = 10,
            RequiredYears = 3
        };
        _entity.Honors.Add(_honor);
        
        // Properties for the event
        _properties =
        [
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "42" }
        ];
    }

    [Benchmark]
    public AddHfEntityHonor Constructor_Benchmark()
    {
        return new AddHfEntityHonor(_properties, _world);
    }

    [Benchmark]
    public string Print_Benchmark()
    {
        var addHfEntityHonor = new AddHfEntityHonor(_properties, _world);
        return addHfEntityHonor.Print(link: true);
    }

    [Benchmark]
    public string Print_NoLink_Benchmark()
    {
        var addHfEntityHonor = new AddHfEntityHonor(_properties, _world);
        return addHfEntityHonor.Print(link: false);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _world?.Dispose();
    }
}
