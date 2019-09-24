using MediatR;

namespace Publisher
{
    public class Pinged: INotification
    {
        public string Value { get; }
        public int DelayInMs { get; }

        public Pinged(string value, int delayInMs)
        {
            Value = value;
            DelayInMs= delayInMs;
        }
    }
}