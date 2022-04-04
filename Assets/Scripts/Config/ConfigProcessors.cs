using System.Collections.Generic;

public class StoreBoundsInvalid : Condition<Store>
{
    public override bool IsSatisfiedBy(Store s)
    {
        var b = s.PhysicalBounds;
        return b.Width < 100 && b.Height < 100 && b.Length < 100;
    }
}

public class HasStoreType : Condition<Store>
{
    public ICollection<string> Types { get; } = new HashSet<string>();

    public override bool IsSatisfiedBy(Store s)
    {
        return Types.Contains(s.Type);
    }

    public override string ToString()
    {
        return $"{GetType().Name} {string.Join(",", Types)}";
    }
}

public class HasStoreId : Condition<Store>
{
    public ICollection<string> Ids { get; set; } = new HashSet<string>();

    public override bool IsSatisfiedBy(Store s)
    {
        return Ids.Contains(s.Id);
    }

    public override string ToString()
    {
        return $"{GetType().Name} {string.Join(",", Ids)}";
    }
}

public class HasMachineNumber : Condition<Machine>
{
    public ICollection<string> MachineNumbers { get; } = new HashSet<string>();
    public override bool IsSatisfiedBy(Machine s)
    {
        return MachineNumbers.Contains(s.MachineNumber);
    }

    public override string ToString()
    {
        return $"{GetType().Name} {string.Join(",", MachineNumbers)}";
    }
}

public class StoreBenchSizeInvalid : Condition<Store>
{
    public override bool IsSatisfiedBy(Store s)
    {
        var b = s.BenchSize;
        return b.X < 100 && b.Y < 100 && b.Z < 100;
    }
}

public class BenchSizeUpdater : Processor<Store>
{
    public Vec3 BenchSize { get; set; } = new Vec3 { X = 2800, Y = 1250, Z = 100 };
    protected override void ProcessCore(Store s)
    {
        s.BenchSize = BenchSize.Clone();
    }
}

public class StorePhysicalBoundUpdater : Processor<Store>
{
    public Vec3 BenchSize { get; set; } = new Vec3 { X = 2800, Y = 1250, Z = 100 };
    protected override void ProcessCore(Store s)
    {
        s.PhysicalBounds = new PhysicalBounds
        {
            X = s.Position.X * BenchSize.X,
            Y = s.Position.Y * BenchSize.Y,
            Z = s.Position.Z * BenchSize.Z,
            Length = s.Dimension.X * BenchSize.X,
            Width = s.Dimension.Y * BenchSize.Y,
            Height = s.Dimension.Z * BenchSize.Z
        };
    }
}

