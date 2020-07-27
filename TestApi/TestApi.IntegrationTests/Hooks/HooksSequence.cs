namespace TestApi.IntegrationTests.Hooks
{
    internal enum HooksSequence
    {
        ConfigHooks = 1,
        RemoveDataCreatedDuringTest = 2,
        RemoveServer = 3
    }
}
