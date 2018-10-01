namespace WebJobInstrumentation.MessageFlow.Pingers

{
    public interface IPinger
    {
        bool Ping(string endpoint);
    }
}