using RealEstateApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateApp.Services
{
    public interface IRepository
    {
        List<Agent> GetAgents();
        List<Property> GetProperties();
        void SaveProperty(Property property);
        Task<LoginResult> LoginAsync(string username, string password);
        Task<LoginResult> GetLoginStorage();
    }
}
