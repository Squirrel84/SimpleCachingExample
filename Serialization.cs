public static class Serialization
{
    private static BinaryFormatter binaryFormatter = new BinaryFormatter();

    public static byte[] ToByteArray(this object obj)
    {
        if (obj == null)
        {
            return null;
        }

        using (MemoryStream memoryStream = new MemoryStream())
        {
            binaryFormatter.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }
    }
    public static T FromByteArray<T>(this byte[] byteArray)
    {
        if (byteArray == null)
        {
            return default(T);
        }

        using (MemoryStream memoryStream = new MemoryStream(byteArray))
        {
            return (T)binaryFormatter.Deserialize(memoryStream);
        }
    }

}
