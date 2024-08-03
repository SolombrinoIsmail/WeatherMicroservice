using AutoMapper;
using WeatherMicroservice.Core.Models;
using WeatherMicroservice.Api.Dtos;

namespace WeatherMicroservice.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Measurement, MeasurementDto>().ReverseMap();
        }
    }
}
