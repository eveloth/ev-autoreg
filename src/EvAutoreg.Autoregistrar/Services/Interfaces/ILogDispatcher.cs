namespace EvAutoreg.Autoregistrar.Services.Interfaces;

public interface ILogDispatcher<T>
{
    Task Log(string log);
}