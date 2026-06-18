namespace eventos_vivos.Tests.TestInfrastructure;

internal static class TestAssert
{
    public static void MessageEquals(string actual, params string[] expectedValues)
    {
        if (expectedValues.Contains(actual, StringComparer.Ordinal))
        {
            return;
        }

        Assert.Fail($"Expected one of [{string.Join(", ", expectedValues)}] but was [{actual}]");
    }
}
