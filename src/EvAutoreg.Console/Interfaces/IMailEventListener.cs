using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace EvAutoreg.Console.Interfaces;

public interface IMailEventListener
{
    Task ProcessEvent(EmailMessage email);
}
