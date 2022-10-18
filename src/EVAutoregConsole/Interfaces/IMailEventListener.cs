using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace EVAutoregConsole.Interfaces;

public interface IMailEventListener
{
    Task ProcessEvent(EmailMessage email);
}