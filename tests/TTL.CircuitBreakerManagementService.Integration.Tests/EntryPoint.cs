namespace Ttl.CircuitBreakerManagementService.IntegrationTests
{
    internal static class EntryPoint
    {
        public static int Main(string[] args)
        {
            var test = new CircuitBreakerManagementServiceTest();
            test.SetUp();
            test.Teardown();
            return 0;
        }
    }
}