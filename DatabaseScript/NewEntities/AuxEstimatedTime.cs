namespace DatabaseScript.Entities;

public partial class AuxEstimatedTime
{
    public long Sid { get; set; }

    public string? FromBerth { get; set; }

    public string? ToBerth { get; set; }

    public int SumTimeSec { get; set; }

    public int SumMan { get; set; }

    public DateTime LastRegisterTime { get; set; }

    
}
