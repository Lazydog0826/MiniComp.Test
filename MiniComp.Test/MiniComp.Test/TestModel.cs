namespace MiniComp.Test;

public class TestModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int TestModel2Id { get; set; }
}

public class TestModel2
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
