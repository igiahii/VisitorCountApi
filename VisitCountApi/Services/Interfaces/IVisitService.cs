namespace VisitCountApi.Services.Interfaces
{
    public interface IVisitService
    {
        public Task<bool> AddVisitorAsync(Guid visitID); 
        public Task<bool> UpdateVisitorAsync(Guid visitID); 
        public Task<bool> UpdateDailyVisitAsync(); 
    }
}
