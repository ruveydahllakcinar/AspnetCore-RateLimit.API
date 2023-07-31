

using Microsoft.AspNetCore.RateLimiting;
using RateLimit.API.Controllers;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Fixed Window
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Fixed", _options =>
    {
        _options.Window = TimeSpan.FromSeconds(12); //Her 12 saniyede bir yazacağımız politika geçerli olacak.
        _options.PermitLimit = 4; //Hedr 12 saniyede bir 4 tane request gönderme hakkımız bulunmakta.
        _options.QueueLimit = 2; // 12  saniye içerisinde 4 tane request al. Bu 4'ten fazlası gelirse kuyruğa al. 
        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Kuyruğa aldıklarımı eskiden yeniye doğru sıralayarak işlemeye başlıyorum.
    });
});

#endregion 
#region Sliding Window
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("Sliding", _options =>
    {
        _options.Window = TimeSpan.FromSeconds(12);
        _options.PermitLimit = 4;
        _options.QueueLimit = 2;
        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        _options.SegmentsPerWindow = 2; // Her bir periyodun kendisinden önceki sürece ne kadarlık kota harcayacağını ifade etmektedir. Bir sonraki periyodun en fazla 2 tane hakkını kullanabilecektir.                                                                                                             

    });
});
#endregion

#region Token Bucket

builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("Token", _options =>
    {
        _options.TokenLimit = 4;
        _options.TokensPerPeriod = 4;
        _options.QueueLimit = 2;
        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        _options.ReplenishmentPeriod = TimeSpan.FromSeconds(12); //Periyotları ne kadar süreye böleceğimi belirtiyorum.
    });
});
#endregion

#region Concurrency
builder.Services.AddRateLimiter(options =>
{
    options.AddConcurrencyLimiter("Concurrency", _options =>
    {
      
        _options.PermitLimit = 4; 
        _options.QueueLimit = 2; 
        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; 
    });
});

#endregion

#region OnRejected Property
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Fixed", _options =>
    {
        _options.Window = TimeSpan.FromSeconds(12); //Her 12 saniyede bir yazacağımız politika geçerli olacak.
        _options.PermitLimit = 4; //Hedr 12 saniyede bir 4 tane request gönderme hakkımız bulunmakta.
        _options.QueueLimit = 2; // 12  saniye içerisinde 4 tane request al. Bu 4'ten fazlası gelirse kuyruğa al. 
        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Kuyruğa aldıklarımı eskiden yeniye doğru sıralayarak işlemeye başlıyorum.
    });

    options.OnRejected = (context, cancellationToken) =>
    {
        return new();
    };
});
#endregion

#region Creating a Customized Rate Limit Policy
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy<string, CustomRateLimitPolicy>("CustomPolicy");
});
#endregion

builder.Services.AddMemoryCache();


var app = builder.Build();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();