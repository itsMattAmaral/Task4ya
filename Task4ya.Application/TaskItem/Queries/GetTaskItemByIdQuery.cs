using MediatR;
using Task4ya.Application.Dtos;

namespace Task4ya.Application.TaskItem.Queries;

public record GetTaskItemByIdQuery(int Id) : IRequest<TaskItemDto?>;