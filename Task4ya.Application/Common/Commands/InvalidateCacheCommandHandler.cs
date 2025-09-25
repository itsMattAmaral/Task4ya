using MediatR;
using Task4ya.Application.Services;

namespace Task4ya.Application.Common.Commands;

public class InvalidateCacheCommandHandler(ICacheService cacheService) : IRequestHandler<InvalidateCacheCommand>
{
	public async Task Handle(InvalidateCacheCommand request, CancellationToken cancellationToken)
	{
		if (request.Patterns.Length > 0)
		{
			foreach (var pattern in request.Patterns)
			{
				await cacheService.RemoveByPatternAsync(pattern);
			}
		}
		if (request.Keys.Length > 0)
		{
			foreach (var key in request.Keys)
			{
				await cacheService.RemoveAsync(key);
			}
		}

	}
}