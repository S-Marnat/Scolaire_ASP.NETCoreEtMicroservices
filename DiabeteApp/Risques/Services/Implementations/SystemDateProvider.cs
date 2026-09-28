using Risques.Services.Interfaces;

namespace Risques.Services.Implementations
{
    public class SystemDateProvider : IDateProvider
    {
        public DateTime Today => DateTime.Today;
    }
}
