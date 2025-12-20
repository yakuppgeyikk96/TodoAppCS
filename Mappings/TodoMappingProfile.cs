
using AutoMapper;
using FirstWebApi.DTOs;
using FirstWebApi.Models;

namespace FirstWebApi.Mappings;

public class TodoMappingProfile : Profile
{
  public TodoMappingProfile()
  {
    CreateMap<Todo, TodoDto>();

    CreateMap<CreateTodoDto, Todo>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.UserId, opt => opt.Ignore())
      .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))
      .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.User, opt => opt.Ignore());

    CreateMap<UpdateTodoDto, Todo>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.UserId, opt => opt.Ignore())
      .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
      .ForMember(dest => dest.User, opt => opt.Ignore());
  }
}
