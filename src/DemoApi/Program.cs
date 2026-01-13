var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/sum", (int a, int b) => Results.Ok(new { a, b, sum = a + b }));

app.Run();