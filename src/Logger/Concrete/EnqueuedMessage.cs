using System.Threading.Tasks;

namespace RabbitMQLogger.Concrete   
{
    public class EnqueuedMessage
    {
        //Enables Async Publishing
        public TaskCompletionSource<object> TaskCompletionSource { get; set; }
        public byte[] Body { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }  

    }
}
