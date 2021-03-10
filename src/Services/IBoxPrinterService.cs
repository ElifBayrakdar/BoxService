using System.Threading.Tasks;
using BoxApi;

namespace BoxService.Services
{
    public interface IBoxPrinterService
    {
        Task PrintBox(BoxCreated boxCreated);
    }
}