using Imposter.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Hosting;

[assembly: GenerateImposter(typeof(IMediator))]
[assembly: GenerateImposter(typeof(IWebHostEnvironment))]
