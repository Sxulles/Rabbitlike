namespace Rabbitlike.Utils
{
    public class Result<T>
    {
        public string? Message { get; set; }
        public bool Succeeded { get; set; }
        public T? Data { get; set; } = default;

        public Result()
        {
            
        }

        public Result(T data)
        {
            Data = data;
            if (data is not null)
                Succeeded = true;
            else 
                Message = new ArgumentNullException().Message;
        }

        public Result(string message)
        {
            Succeeded = false;
            Message = message;
        }
    }
}
