using UnityEngine;
public static class Helpers
{
    public static string UID()
    {
        return System.Guid.NewGuid().ToString("N");
    }
}
