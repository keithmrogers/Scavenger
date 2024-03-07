
namespace Scavenger.XForms.Services
{
    public interface ICapabilityService
    {
        bool CanMakeCalls { get; }
        bool CanSendMessages { get; }
        bool CanSendEmail { get; }
    }
}

