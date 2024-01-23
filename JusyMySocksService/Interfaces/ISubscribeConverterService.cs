namespace JustMySocksService.Interfaces
{
    public interface ISubscribeConverterService
    {
        public Task<string> ConvertSubscribeAsync(string url);
    }
}
