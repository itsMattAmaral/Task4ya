using MediatR;

namespace Task4ya.Application.Common.Commands;

public class InvalidateCacheCommand : IRequest
{
    public string[] Keys { get; set; } = [];
    public string[] Patterns { get; set; } = [];
}