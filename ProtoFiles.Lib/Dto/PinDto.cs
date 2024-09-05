namespace ProtoFiles.Lib.Dto;

public class PinDto
{
    public string? Username { get; set; }
    public int NewPin { get; set; }
    public int OldPin { get; set; }
    public bool PinUnlock { get; set; }
}