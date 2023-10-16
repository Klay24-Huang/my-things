namespace unit_test_middleware.Middleware
{
    public class AuthHeaderCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthHeaderCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var auth = context.Request.Headers["auth"];
            //Console.WriteLine($"auth is {string.IsNullOrWhiteSpace(auth)}");
            if (!string.IsNullOrWhiteSpace(auth))
            {
                await _next(context);
            }

            Console.WriteLine("before throw");
            throw new InvalidOperationException("Missing header !!");
        }
    }
}
