using AutoMapper;
using Core.Entity;
using Data.Models;

namespace DekstopApp.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductModel>().ReverseMap();
    }
}