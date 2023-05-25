namespace Testing_System.Services.Kdf
{
    public interface IKdfService
    {
        String GetDerivedKey(String password, String salt);
    }
}
