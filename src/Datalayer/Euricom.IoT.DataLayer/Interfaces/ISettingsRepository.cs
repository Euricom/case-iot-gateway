using Euricom.IoT.Models;

namespace Euricom.IoT.DataLayer.Interfaces
{
    public interface ISettingsRepository
    {
        Settings Get();
        void Update(Settings settings);
        void Seed();
    }
}