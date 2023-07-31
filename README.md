# What is Rate Limit?

A rate limit is a mechanism that limits the number of requests a user makes to a service or API in a given period of time. This limit is applied to prevent overuse and maintain the stability of the service and server. Users can usually make a certain number of requests within a certain time frame

> For example; You have an endpoint that is completely open to the world and you want to set a rule that "at most 100 requests can be made in 1 hour". After this situation, the 101st request reaches the client but may return an error message.

###  IP Rate Limit: 
* If your web application has a fixed ID, you can specify it.
### Client ID Rate Limit:
* If your application does not have a fixed Id, you can use client Id.

We can examine it under two headings.

### If we don't want to use a library, you have to write Middleware. What will this layer do?

When a request comes to your API application, it will receive and check the API, Header information, request information of this request, if it is the API address you specified, it will continue if not, it will return. This will cause you to write long code.

## How can I import .Net Rate Limit?
First you have to open the Startup.cs files.

    builder.Services.AddOptions();
    
 Adds a service to save configuration settings using the Options pattern. Options are used to share configuration data between different components of the application.

    builder.Services.AddMemoryCache();

Enables memory caching in the application. This code specifies that a memory-based cache will be used to track the number of client requests and enforce rate limiting rules.

    builder.Services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

Configures the IpRateLimitOptions class and binds the relevant section ("IpRateLimiting") from the application configuration file ("appsettings.json") to this class. This ensures that IP-based rate limiting settings are retrieved from the configuration file.

    builder.Services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

Configures the IpRateLimitPolicies class and binds the relevant section ("IpRateLimitPolicies") from the application configuration file ("appsettings.json") to this class. This allows IP-based rate limiting policies to be retrieved from the configuration file. Rate limiting policies can be used to specify different limiting rules for different IP addresses or clients.

    builder.Services.AddSingleton<IpPolicyStore, MemoryCacheIpPolicyStore>();

Maps the IpPolicyStore interface to the MemoryCacheIpPolicyStore class in the application. This provides a cache for storing IP-based rate limiting policies in memory. That is, it allows policies to be kept directly in memory without writing them to the database.

    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

Maps the IRateLimitCounterStore interface to the MemoryCacheRateLimitCounterStore class in the application. This provides a cache to store the rate limiting counters in memory. That is, it allows the counters to be kept directly in memory without being written to the database.

    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

Maps the IHttpContextAccessor interface to the HttpContextAccessor class in the application. ASP.NET | Open-source web framework for .NET Core can be used to resolve dependencies associated with HTTP requests. For example, this interface is provided to use the properties of HttpContext to access rate limiting services.

## Asp.net Core. Net7
Rate Limit works with policy logic. First you create the policy and then you write the Controller.

### What are Rate Limit Algorithms?

#### *Fixed Window*

An algorithm that limits requests using a fixed time interval. 

    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("Fixed", _options =>
        {
            _options.Window = TimeSpan.FromSeconds(12); //The policy we will write every 12 seconds will be valid.
            _options.PermitLimit = 4; // We have the right to send 4 requests every 12 seconds.
            _options.QueueLimit = 2; // Receive 4 requests within 12 seconds. If more than these 4 arrive, queue them. 
            _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // I start processing the queued requests in order from oldest to newest.
        }); 
    });
    
#### *Sliding Window*

It is similar to Fixed Window's feature, but after half of the period, it meets the requests in such a way that it consumes the request quota of the next period. 

    builder.Services.AddRateLimiter(options =>
    {
        options.AddSlidingWindowLimiter("Sliding", _options =>
        {
            _options.Window = TimeSpan.FromSeconds(12);
            _options.PermitLimit = 4;
            _options.QueueLimit = 2;
            _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            _options.SegmentsPerWindow = 2; // Indicates how much quota each period will spend on the process before it. The next period will be able to use a maximum of 2 rights.                                                                                                             
    
        });
    });

#### *Token Bucket*

In each period, as many tokens are generated as the number of requests to be processed. If these tokens are used, they can be borrowed from the next period. However, each period will generate as many tokens as the amount of token generation and the rate limit will be applied in this way. 

 **

> Attention: The maximum token limit of each period will be the given
> fixed number.

** 

    builder.Services.AddRateLimiter(options =>
    {
        options.AddTokenBucketLimiter("Token", _options =>
        {
            _options.TokenLimit = 4;
            _options.TokensPerPeriod = 4;
            _options.QueueLimit = 2;
            _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            _options.ReplenishmentPeriod = TimeSpan.FromSeconds(12); /I specify how long to divide the periods.
        });
    });

#### *Concurrency*

It is an algorithm used to limit asynchronous requests. Each request decreases the Concurrency by one and if they are finished, they increase this limit by one. Compared to other algorithms, it only limits asynchronous requests.

### Rate Limit Attribute

`EnableRateLimitting :` Allows you to enable the Rate limiting in a policy you want at the Controller or Action level.

`DisableRateLimiting:` It is an attribute that enables a Rate limit policy enabled at the controller level to be disabled at the action level.

### OnRejected Property

Rate Limit is a property in event logic that we use to perform operations such as logging etc. in cases where there are requests that are wasted due to the limit in the operations applied.


