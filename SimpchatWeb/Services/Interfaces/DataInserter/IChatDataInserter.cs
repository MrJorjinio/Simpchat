using System.Threading.Tasks;

namespace SimpchatWeb.Services.Interfaces.DataInserter
{
    public interface IChatDataInserter
    {
        Task InsertSysGroupPermissionsAsync();
    }
}
