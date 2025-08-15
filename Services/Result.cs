namespace WishAWish.Services
{
    public class Result
    {
        public bool Ok { get; set; }
        public string Message { get; set; }
        public static Result Success(string m = "") => new Result { Ok = true, Message = m };
        public static Result Fail(string m) => new Result { Ok = false, Message = m };
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }
        public static Result<T> Success(T v, string m = "") => new Result<T> { Ok = true, Value = v, Message = m };
        public static Result<T> FailT(string m) => new Result<T> { Ok = false, Message = m };
    }
}
