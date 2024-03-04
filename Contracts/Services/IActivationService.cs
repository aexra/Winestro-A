namespace Winestro_A.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
