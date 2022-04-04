using System.Collections.Generic;

/// <summary>Checks if the store meets validation requirements</summary>
public abstract class Condition<T>
{
    public static Condition<T> True => new AlwaysTrue();

    private class AlwaysTrue : Condition<T> { public override bool IsSatisfiedBy(T s) => true; }
    
    public static Condition<T> False => new AlwaysFalse();
    
    private class AlwaysFalse : Condition<T> { public override bool IsSatisfiedBy(T s) => false; }

    public abstract bool IsSatisfiedBy(T item);

    public override string ToString()
    {
        return GetType().Name;
    }

    public Condition<T> Not() => new NotCondition(this);
    
    public Condition<T> Not(Condition<T> condition) => new NotCondition(condition);
    
    private class NotCondition : Condition<T>
    {
        private Condition<T> _condition;
        public NotCondition(Condition<T> condition)
        {
            _condition = condition;
        }
        public override bool IsSatisfiedBy(T s)
        {
            return !_condition.IsSatisfiedBy(s);
        }
        
        public override string ToString()
        {
            return $"!({_condition})";
        }
    }

    public Condition<T> And(Condition<T> other) => new AndCondition(this, other);
    
    public Condition<T> And(params Condition<T>[] conditions) => new AndCondition(conditions);
    
    private class AndCondition : Condition<T>
    {
        private Condition<T>[] _conditions;
        public AndCondition(params Condition<T>[] conditions)
        {
            _conditions = conditions;
        }
        public override bool IsSatisfiedBy(T s)
        {
            foreach (var c in _conditions)
                if (!c.IsSatisfiedBy(s))
                    return false;
            return true;
        }
        
        public override string ToString()
        {
            return $"({string.Join<Condition<T>>(" && ",_conditions)})";
        }
    }

    public Condition<T> Or(Condition<T> other) => new OrCondition(this, other);
    
    public static Condition<T> Or(params Condition<T>[] conditions) => new OrCondition(conditions);

    private class OrCondition : Condition<T>
    {
        private Condition<T>[] _conditions;
        public OrCondition(params Condition<T>[] conditions)
        {
            _conditions = conditions;
        }
        public override bool IsSatisfiedBy(T s)
        {
            foreach (var c in _conditions)
                if (c.IsSatisfiedBy(s))
                    return true;
            return false;
        }

        public override string ToString()
        {
            return $"({string.Join<Condition<T>>(" || ",_conditions)})";
        }
    }
}

/// <summary>Applies an update to the  store meets validation requirements</summary>
public abstract class Processor<T>
{
    private Condition<T> condition;

    public Condition<T> Condition
    {
        get => condition ??= Condition<T>.True;
        set => condition = value;
    }

    public void ProcessAll(IEnumerable<T> ss)
    {
        foreach (var s in ss)
            Process(s);
    }
    public void Process(T s)
    {
        if (Condition?.IsSatisfiedBy(s) ?? true)
        {
            ProcessCore(s);
        }
    }
    protected abstract void ProcessCore(T s);

    public override string ToString()
    {
        return $"{GetType().Name} Condition: {Condition}";
    }
}


public class Collector<T> : Processor<T>
{
    public List<T> Items { get; } = new List<T>();

    protected override void ProcessCore(T item)
    {
        Items.Add(item);
    }
}
