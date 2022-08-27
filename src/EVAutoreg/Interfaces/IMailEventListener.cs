using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace EVAutoreg.Interfaces;

public interface IMailEventListener
{
    Task ProcessEvent(EmailMessage email);
}