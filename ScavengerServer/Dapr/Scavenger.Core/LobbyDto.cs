namespace Scavenger.Core;
public class LobbyDto
{
    public required Guid? ScavengerId { get; set; }
    public required Guid? GuideId { get; set; }
    public required bool IsReady { get; set; }
    public required bool IsWaitingForScavenger { get; set; }
    public required bool IsWaitingForGuide { get; set; }
}
